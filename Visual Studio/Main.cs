using BepInEx;
using BepInEx.Configuration;
using CraigsCorner.Items.Tier1;
using CraigsCorner.Utils;
using R2API;
using R2API.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CraigsCorner
{
    // Dependency Information
    [BepInPlugin(ModGUID, ModName, ModVer)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Main : BaseUnityPlugin
    {
        // PluginGUID information
        public const string ModGUID = "com.Craiglet.CraigsCorner";
        public const string ModName = "CraigsCorner";
        public const string ModVer = "0.0.0";

        // PluginInfo so we can grab the location of the mod files
        public static PluginInfo PluginInfo { get; private set; }

        // AssetBundle
        public static AssetBundle MainAssets;

        // Main Config Stuff
        internal static ConfigFile MainConfig;
        public static ConfigEntry<bool> EnableItems { get; set; }
        public List<ItemBase> Items = new List<ItemBase>();

        public void Awake()
        {
            // Init our logging class so that we can properly log for debugging.
            Utils.Log.Init(Logger);

            // Awaken it?? Cause it's fucking sleepy I guess????????
            PluginInfo = Info;
            MainConfig = Config;

            // Loads our Asset Bundle from the Asset Folder in the thunderstore package. NOT A FOLDER??? WHY NO FOLDER WORK?????????
            MainAssets = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(PluginInfo.Location), "craigscorner_assets"));

            Item_Initializer();
        }

        private void Item_Initializer()
        {
            EnableItems = Config.Bind<bool>(
            "Main", // The heading our setting is going to appear under
            "Enable Items", // Name of our setting
            true, // Default value of our setting 
            "Should items from Craig's Corner appear? Change to 'false' to disable all items." // More detailed Description 
            );

            if (EnableItems.Value)
            {
                // Instantiate SteelToeBoot --> WTF???? THATS NOT EVEN A FUCKING WORD???????????????
                SteelToeBoot steelToeBoot = new SteelToeBoot();
                Monocle monocle = new Monocle();

                // Initialize SteelToeBoot
                steelToeBoot.Init(Config);

                monocle.Init(Config);
            }
        }
    }
}

//make the initializer not a piece of shit 