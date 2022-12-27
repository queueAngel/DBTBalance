using DBTBalance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DBTBalance.Helpers
{
    public class AccessoryHooks
    {
        public static Dictionary<string, string[]> upgradePaths = new Dictionary<string, string[]>()
        {
            { "ArmCannon", new string[]{ "BattleKit", "FinalShield" } },
            { "WornGloves", new string[]{ "BattleKit", "FinalShield" } },
            { "ScouterT2", new string[]{ "BattleKit", "ScouterT3", "ScouterT4","ScouterT5", "ScouterT6", "FinalShield" } },
            { "ScouterT3", new string[]{ "ScouterT4","ScouterT5", "ScouterT6" } },
            { "ScouterT4", new string[]{ "ScouterT5", "ScouterT6" } },
            { "ScouterT5", new string[]{ "ScouterT6" } },
            { "BattleKit", new string[] { "FinalShield" } },
            { "CrystalliteFlow", new string[] { "CrystalliteAlleviate" }},
            { "CrystalliteControl", new string[] { "CrystalliteAlleviate", "CrystalliteFlow" }},
            { "BaldurEssentia", new string[]{ "BuldariumSigmite" } },
            { "LargeTurtleShell", new string[]{ "BlackDiamondShell", "DiamondStorm" } },
            { "BurningEnergyAmulet", new string[]{ "PureEnergyCirclet" } },
            { "IceTalisman", new string[]{ "PureEnergyCirclet" } },
            { "EarthenSigil", new string[]{ "EarthenArcanium" } },
            { "EarthenScarab", new string[]{ "EarthenArcanium" } },
            { "KiChip", new string[]{ "BuldariumSigmite" } },
            { "SpiritualEmblem", new string[]{ "SpiritCharm" } },
            { "DragonGemNecklace", new string[]{ "SpiritCharm" } },
            { "AmberNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "AmethystNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "DiamondNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "EmeraldNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "RubyNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "SapphireNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "TopazNecklace", new string[]{ "DragonGemNecklace", "SpiritCharm" } },
            { "BloodstainedBandana", new string[] { "FrostburnBandana", "RainbowBandana", "RingOfAll" } },
            { "BlackDiamondShell", new string[] { "DiamondStorm" } }
        };

        public static Dictionary<string, string[]> dbcaUpgradePaths = new Dictionary<string, string[]>()
        {
            { "FreezingBandana", new string[] { "FrostburnBandana", "RainbowBandana", "RingOfAll" } },
            { "BurningBandana", new string[] { "FrostburnBandana", "RainbowBandana", "RingOfAll" } },
            { "RainbowBandana", new string[] { "RingOfAll" } },
            { "ShadowEye", new string[] { "Scheusslich" } },
            { "HemoTouch", new string[] { "Scheusslich" } },
            { "ExtraPair", new string[] { "Scheusslich" } },
            { "PhantomShield", new string[] { "FinalShield" } }
        };

        public static Dictionary<string, float> DamageAdjustment = new Dictionary<string, float>()
        {
            { "BlackDiamondShell", 0.02f }, // 12% > 10%
            { "BloodstainedBandana", 0.10f },// 14% > 4%
            { "BurningEnergyAmulet", 0.05f }, //0%
            { "Earthen Arcanium", 0.10f }, // 0%
            { "EarthenScarab", 0.04f }, // 0%
            { "EarthenSigil", 0.06f }, // 0%
            { "IceTalisman", 0.05f }, // 7% > 2%
            { "LuminousSectum", 0.05f }, // 4%
            { "MetamoranSash", 0.05f }, // 5%
            { "PureEnergyCirclet", 0.07f }, // 5%
            { "RadiantTotem", 0.12f }, // 0%
            { "ZenkaiCharm", 0.08f }  // 0%
        };

        public static Dictionary<string, float> dbcaDamageAdjustment = new Dictionary<string, float>()
        {
            { "AviationBoots", 0.04f }, // 0%
            { "BurningBandana", 0.08f }, // 12% > 4%
            { "FreezingBandana", 0.08f }, // 12% > 4%
            { "DiamondStorm", 0.02f }, // 12% > 10%
            { "ZenkaiOrb", 0.1f  }, // 0%
            { "ZenkaiAmulet", 0.12f }, // 0%
            { "ElysianCrystal", 0.04f }, // 0%
            { "PhantomShield", 0.06f }, // 11% > 5%
            { "ZenkaiRing", 0.16f }, // 0%
            { "ZenkaiCrown", 0.21f }, // 0%
            { "FinalShield", 0.04f } // 19% > 15%
        };

        public static Dictionary<string, string> ChagnedTooltips = new Dictionary<string, string>
        {
            { "BlackDiamondShell","A jeweled turtle shell that gets the attention of many creatures, for some reason it's unbelievably tough.\n10% increased ki damage, 14% increased ki knockback.\n+200 Max Ki.\nGetting hit restores a small amount of Ki." },
            { "BloodstainedBandana", "'Change the future.'\n4% Increased Ki damage.\nThorns effect." },
            { "BuldariumSigmite", "'A fragment of the god of defense's soul.'\nCharging grants a protective barrier that grants massively increased defense\nCharging also grants drastically increased life regen\n+120 ki/sec charge rate" },
            { "BurningEnergyAmulet", "'A glowing amulet that radiates with extreme heat.'\n+200 Max ki\nCharging grants a aura of fire around you." },
            { "DragonGemNecklace", "'Infused with the essence of the dragon.'\n8% increased ki damage.\n9% increased non-ki damage.\n9% increased crit chance.\nMinor life regen.\n9% reduced damage taken.\n+250 max Ki.\n+2 max minons." },
            { "EarthenArcanium", "'A core of the pure energy of the earth.'\n+60 ki/sec charge rate.\nReduced flight ki usage.\n+1 Max Charges.\nIncreased flight speed.\nThe longer you charge the more ki you charge, limits at +500%." },
            { "EarthenScarab", "'The soul of the land seems to give it life.'\nIncreased flight speed.\nThe longer you charge the more ki you charge, limits at +500%." },
            { "EarthenSigil", "'The soul of the land lives within.'\n+60 ki/sec charge rate.\nReduced flight ki usage\n+1 Max Charges." },
            { "IceTalisman", "'A frozen talisman that seems to make even your soul cold.'\n2% Increased Ki damage\n+120 ki/sec charge rate.\nCharging grants a aura of frostburn around you." },
            { "KiChip", "'A piece of a ki fragment.'\n+60 ki/sec charge rate." },
            { "LuminousSectum", "'It radiates with unstable energy.'\n4% increased ki damage.\n+250 max ki.\nHitting enemies has a small chance to fire off homing ki sparks." },
            { "MajinNucleus", "'The pulsing nucleus of a invicible being.'\nMassively increased health regen.\n+360 ki/sec charge rate.\n-1500 max ki." },
            { "MetamoranSash", "'Your own bad energy will be your undoing!'\n5% Increased Ki damage.\n30% Reduced Ki usage.\n15% chance to do double damage." },
            { "PureEnergyCirclet" , "'It radiates a unbelievably pure presence.'\n5% Increased Ki damage.\n+120 ki/sec charge rate.\n+300 Max Ki.\nCharging grants a aura of inferno and frostburn around you." },
            { "RadiantTotem", "'It explodes with radiant energy.'\n+500 Max Ki\n+120 ki/sec charge rate." },
            { "TimeRing", "'The sacred ring of the kais'\nDrastically increased health regen.\n+180 ki/sec charge rate." },
            { "ZenkaiCharm" ,"'A charm that harnesses the true power of a saiyan.'\nTaking fatal damage will instead return you to 50 hp\nand grant x2 damage for a short time.\n2 Minute cooldown" }
        };

        public static Dictionary<string, string> DBCAChagnedToolttips = new Dictionary<string, string>()
        {
            { "AviationBoots", "You're able to fly for a short while.\nFlying creates a damaging trail." },
            { "BurningBandana", "4% Increased Ki damage.\nBurns enemies on hit." },
            { "FreezingBandana", "4% Increased Ki damage.\nFreezes enemies on hit." },
            { "ZenkaiOrb", "'This orb activates in near-death situations'\nZenkai effectivity increased by 10%.\nTaking fatal damage will instead return you to 50 hp\nand grant x2 damage for a short time.\n2 Minute cooldown" },
            { "ZenkaiAmulet", "'This amulet activates in near-death situations.'\nZenkai effectivity increased by 16%.\nTaking fatal damage will instead return you to 50 hp\nand grant x2 damage for a short time.\n2 Minute cooldown" },
            { "DiamondStorm", "+20 max HP\n10% increased ki damage.\n14% increased ki knockback.\n+250 Max Ki.\nGetting hit restores a small amount of Ki and shoots down many arrows.\nThe amount of arrows is increased with the damage taken.\nIf they don't hit an enemy, they go back to you.\nThe arrows give you a damage increasing buff." },
            { "ElysianCrystal", "'Blessed by the Profaned Flame'\nIncreased flight speed.\n+300 ki/sec charge rate.\nReduced flight ki usage by 25%.\n+1500 Max ki" },
            { "PhantomShield", "5% Increased Ki Damage\n4% Increased Ki Crit rate\nGives a chance to dodge attacks on hit\nAfter dodging, you gain increased damage for a short time" },
            { "ZenkaiRing", "'This ring activates in near-death situations'\nZenkai effectivity increased by 24%.\nTaking fatal damage will instead return you to 50 hp\nand grant x2 damage for a short time.\n2 Minute cooldown" },
            { "ZenkaiCrown", "'This crown activates in near-death situations'\n+2500 Max ki.\nZenkai effectivity increased by 32%.\nTaking fatal damage will instead return you to 50 hp\nand grant x2 damage for a short time.\n2 Minute cooldown\nDrastically Increased speed while charging" }
        };

        public static bool HasAccessory(Player player, params string[] types)
        {
            foreach (var type in types)
            {
                int typeIndex = -400;
                if(DBTBalance.DBZMOD.TryFind<ModItem>(type, out ModItem dbtValue))
                {
                    typeIndex = dbtValue.Type;
                }
                if (player.armor.Any(x => x.type == typeIndex))
                    return true;
            }
            if (ModLoader.HasMod("dbzcalamity"))
            {
                foreach (var type in types)
                {
                    int typeIndex = -400;
                    if (DBTBalance.DBCA.TryFind<ModItem>(type, out ModItem cValue))
                    {
                        typeIndex = cValue.Type;
                    }
                    if (player.armor.Any(x => x.type == typeIndex))
                        return true;
                }
            }
            return false;
        }

        public delegate void orig_Update(dynamic self, Player player, bool hideVisual);

        public static void Update_hook(orig_Update orig, dynamic self, Player player, bool hideVisual)
        {
            var itemName = ItemLoader.GetItem((int)self.Type).Name;

            if (upgradePaths.ContainsKey(itemName))
                if (HasAccessory(player, upgradePaths[itemName]))
                    return;

            if(orig != null && player != null && self != null)
                orig(self, player, hideVisual);

            if(DamageAdjustment.TryGetValue(itemName, out var amount))
            {
                dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { player });

                modPlayer.KiDamage -= amount;
            }
            if (ModLoader.HasMod("dbzcalamity"))
            {
                if(dbcaDamageAdjustment.TryGetValue(itemName, out var amount2))
                {
                    dynamic modPlayer = DBTBalance.DBZMOD.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer")).GetMethod("ModPlayer").Invoke(null, new object[] { player });

                    modPlayer.KiDamage -= amount2;
                }
            }
        }

        public delegate void orig_SetStaticDefaults(dynamic self);

        public static void SetStaticDefaults_Hook(orig_SetStaticDefaults orig, dynamic self)
        {
            orig(self);

            if (ChagnedTooltips.TryGetValue(ItemLoader.GetItem((int)self.Type).Name, out string tip))
            {
                self.Tooltip.SetDefault(tip);
            }
            if (ModLoader.HasMod("dbzcalamity"))
            {
                if(DBCAChagnedToolttips.TryGetValue(ItemLoader.GetItem((int)self.Type).Name, out string tip2))
                {
                    self.Tooltip.SetDefault(tip2);
                }
            }
        }
    }
}
