using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHunter.Game.Config
{
    public class CaptureDataConfig
    {
        public static CaptureDataConfig Default { get; } = new CaptureDataConfig();

        public IDictionary<string, float> Thresholds { get; } = new Dictionary<string, float>() {
            // 10%
            { "em001_02", 0.1F }, // Gold Rathian
            { "em002_02", 0.1F }, // Silver Rathalos
            { "em043_01", 0.1F }, // Savage Deviljho

            // 15%
            { "em002_01", 0.15F }, // Azure Rathalos
            { "em007_01", 0.15F }, // Black Diablos
            { "em100_01", 0.15F }, // Anjanath Flugur
            { "em113_01", 0.15F }, // Ebony Odogaron

            // 20%
            { "em111_00", 0.2F }, // Legiana
            { "em113_00", 0.2F }, // Odogaron
            { "em002_00", 0.2F }, // Rathalos
            { "em007_00", 0.2F }, // Diablos
            { "em036_00", 0.2F }, // Lavasioth
            { "em043_00", 0.2F }, // Deviljho
            { "em045_00", 0.2F }, // Uragaan
            { "em102_01", 0.2F }, // Pukei-Pukei Coral
            { "em042_00", 0.2F }, // Barioth
            { "em063_00", 0.2F }, // Brachydios

            // 25%
            { "em100_00", 0.25F }, // Anjanath
            { "em102_00", 0.25F }, // Pukei-Pukei
            { "em108_00", 0.25F }, // Jyuratodus
            { "em109_00", 0.25F }, // Tobi Kadachi
            { "em110_00", 0.25F }, // Paolumu
            { "em001_00", 0.25F }, // Rathian
            { "em001_01", 0.25F }, // Pink Rathian
            { "em044_00", 0.25F }, // Barroth
            { "em122_00", 0.25F }, // Beotodus
            { "em109_01", 0.25F }, // Viper Tobi Kadachi
            { "em110_01", 0.25F }, // Paolumu Nightshade

            // 30%
            { "em101_00", 0.3F }, // Great Jagras
            { "em112_00", 0.3F }, // Great Girros
            { "em116_00", 0.3F }, // Dodogama
            { "em118_00", 0.3F }, // Bazelgeuse
        };
    }
}
