﻿using SmartHunter.Core;
using SmartHunter.Core.Data;
using SmartHunter.Game.Config;
using SmartHunter.Game.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartHunter.Game.Data
{
    public enum MonsterCrown
    {
        None,
        Mini,
        Silver,
        Gold
    }

    public class Monster : TimedVisibility
    {
        public ulong Address { get; private set; }

        string m_Id;
        public string Id
        {
            get { return m_Id; }
            set
            {
                if (SetProperty(ref m_Id, value))
                {
                    NotifyPropertyChanged(nameof(IsVisible));
                    UpdateLocalization();
                }
            }
        }

        public string Name
        {
            get
            {
                return LocalizationHelper.GetMonsterName(Id);
            }
        }

        float m_SizeScale;
        public float SizeScale
        {
            get { return m_SizeScale; }
            set
            {
                if (SetProperty(ref m_SizeScale, value))
                {
                    NotifyPropertyChanged(nameof(ModifiedSizeScale));
                    NotifyPropertyChanged(nameof(Size));
                    NotifyPropertyChanged(nameof(Crown));
                }
            }
        }

        public float ModifiedSizeScale
        {
            get
            {
                float modifiedSizeScale = SizeScale;

                MonsterConfig config = null;
                if (ConfigHelper.MonsterData.Values.Monsters.TryGetValue(Id, out config))
                {
                    modifiedSizeScale /= config.ScaleModifier;
                }

                return modifiedSizeScale;
            }
        }

        public float Size
        {
            get
            {
                float size = 0;

                MonsterConfig config = null;
                if (ConfigHelper.MonsterData.Values.Monsters.TryGetValue(Id, out config))
                {
                    size = config.BaseSize * ModifiedSizeScale;
                }

                return size;
            }
        }

        public MonsterCrown Crown
        {
            get
            {
                MonsterCrown crown = MonsterCrown.None;

                MonsterConfig config = null;
                if (ConfigHelper.MonsterData.Values.Monsters.TryGetValue(Id, out config) && config.Crowns != null)
                {
                    float modifiedSizeScale = ModifiedSizeScale;

                    if (modifiedSizeScale <= config.Crowns.Mini)
                    {
                        crown = MonsterCrown.Mini;
                    }
                    else if (modifiedSizeScale >= config.Crowns.Gold)
                    {
                        crown = MonsterCrown.Gold;
                    }
                    else if (modifiedSizeScale >= config.Crowns.Silver)
                    {
                        crown = MonsterCrown.Silver;
                    }
                }

                return crown;
            }
        }

        public Progress Health { get; private set; }
        public ObservableCollection<MonsterPart> Parts { get; private set; }
        public ObservableCollection<MonsterStatusEffect> StatusEffects { get; private set; }

        public bool IsVisible
        {
            get
            {
                return IsIncluded(Id) && IsTimeVisible(ConfigHelper.Main.Values.Overlay.MonsterWidget.ShowUnchangedMonsters, ConfigHelper.Main.Values.Overlay.MonsterWidget.HideMonstersAfterSeconds);
            }
        }

        static IDictionary<string, float> CapturableThreshold = new Dictionary<string, float>();
        static Monster()
        {
            // 10%
            CapturableThreshold.Add("LOC_MONSTER_GOLD_RATHIAN", 0.1F);
            CapturableThreshold.Add("LOC_MONSTER_SILVER_RATHALOS", 0.1F);

            // 15%
            CapturableThreshold.Add("LOC_MONSTER_AZURE_RATHALOS", 0.15F);
            CapturableThreshold.Add("LOC_MONSTER_BLACK_DIABLOS", 0.15F);
            CapturableThreshold.Add("LOC_MONSTER_ANJANATH_FULGUR", 0.15F);
            CapturableThreshold.Add("LOC_MONSTER_EBONY_ODOGARON", 0.15F);

            // 20%
            CapturableThreshold.Add("LOC_MONSTER_LEGIANA", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_ODOGARON", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_RATHALOS", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_DIABLOS", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_LAVASIOTH", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_DEVILJHO", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_URAGAAN", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_PUKEI_PUKEI_CORAL", 0.2F);
            CapturableThreshold.Add("LOC_MONSTER_BARIOTH", 0.2F);

            // 25%
            CapturableThreshold.Add("LOC_MONSTER_ANJANATH", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_PUKEI_PUKEI", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_JYURATODUS", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_TOBI_KADACHI", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_PAOLUMU", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_RATHIAN", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_PINK_RATHIAN", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_BARROTH", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_BEOTODUS", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_VIPER_TOBI_KADACHI", 0.25F);
            CapturableThreshold.Add("LOC_MONSTER_PAOLUMU_NIGHTSHADE", 0.25F);

            // 30%
            CapturableThreshold.Add("LOC_MONSTER_GREAT_JAGRAS", 0.3F);
            CapturableThreshold.Add("LOC_MONSTER_GREAT_GIRROS", 0.3F);
            CapturableThreshold.Add("LOC_MONSTER_DODOGAMA", 0.3F);
            CapturableThreshold.Add("LOC_MONSTER_BAZELGEUSE", 0.3F);
        }

        public bool IsCapturable
        {
            get
            {
                if (CapturableThreshold.ContainsKey(Id))
                {
                    float threshold = CapturableThreshold[Id];
                    return Health.Fraction <= threshold;
                }

                // Unknown
                return false;
            }
        }

        public Monster(ulong address, string id, float maxHealth, float currentHealth, float sizeScale)
        {
            Address = address;
            m_Id = id;
            Health = new Progress(maxHealth, currentHealth);
            m_SizeScale = sizeScale;

            Parts = new ObservableCollection<MonsterPart>();
            StatusEffects = new ObservableCollection<MonsterStatusEffect>();
        }

        public MonsterPart UpdateAndGetPart(ulong address, bool isRemovable, float maxHealth, float currentHealth, int timesBrokenCount)
        {
            MonsterPart part = Parts.SingleOrDefault(collectionPart => collectionPart.Address == address);
            if (part != null)
            {
                part.IsRemovable = isRemovable;
                part.Health.Max = maxHealth;
                part.Health.Current = currentHealth;
                part.TimesBrokenCount = timesBrokenCount;
            }
            else
            {
                part = new MonsterPart(this, address, isRemovable, maxHealth, currentHealth, timesBrokenCount);
                part.Changed += PartOrStatusEffect_Changed;

                Parts.Add(part);
            }

            part.NotifyPropertyChanged(nameof(MonsterPart.IsVisible));

            return part;
        }

        public MonsterStatusEffect UpdateAndGetStatusEffect(int index, float maxBuildup, float currentBuildup, float maxDuration, float currentDuration, int timesActivatedCount)
        {
            MonsterStatusEffect statusEffect = StatusEffects.SingleOrDefault(collectionStatusEffect => collectionStatusEffect.Index == index);
            if (statusEffect != null)
            {
                statusEffect.Duration.Max = maxDuration;
                statusEffect.Duration.Current = currentDuration;
                statusEffect.Buildup.Max = maxBuildup;
                statusEffect.Buildup.Current = currentBuildup;
                statusEffect.TimesActivatedCount = timesActivatedCount;
            }
            else
            {
                statusEffect = new MonsterStatusEffect(this, index, maxBuildup, currentBuildup, maxDuration, currentDuration, timesActivatedCount);
                statusEffect.Changed += PartOrStatusEffect_Changed;

                StatusEffects.Add(statusEffect);
            }

            statusEffect.NotifyPropertyChanged(nameof(MonsterStatusEffect.IsVisible));

            return statusEffect;
        }

        public void UpdateLocalization()
        {
            NotifyPropertyChanged(nameof(Name));

            foreach (var part in Parts)
            {
                part.NotifyPropertyChanged(nameof(MonsterPart.Name));
            }
            foreach (var statusEffect in StatusEffects)
            {
                statusEffect.NotifyPropertyChanged(nameof(MonsterStatusEffect.Name));
            }
        }

        public static bool IsIncluded(string monsterId)
        {
            return ConfigHelper.Main.Values.Overlay.MonsterWidget.MatchIncludeMonsterIdRegex(monsterId);
        }

        private void PartOrStatusEffect_Changed(object sender, GenericEventArgs<DateTimeOffset> e)
        {
            UpdateLastChangedTime();
        }
    }
}
