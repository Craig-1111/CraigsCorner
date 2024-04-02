using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CraigsCorner.Items.Tier1
{
    // Dependency Information
    [BepInPlugin(ModGUID, ModName, ModVer)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]


    public class HikingBoot : BaseUnityPlugin
    {   
        // PluginGUID information
        public const string ModGUID = "com.Craiglet.CraigsCorner";
        public const string ModName = "CraigsCorner";
        public const string ModVer = "0.0.0";

        // We need our item definition to persist through our functions, and therefore make it a class field. 
        public static ItemDef myItemDef;
        public static AssetBundle MainAssets;

        public static PluginInfo pluginInfo { get; private set; }

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            // Initialize pluginInfo before using it
            pluginInfo = Info;

            // Init our logging class so that we can properly log for debugging.
            Log.Init(Logger);

            // First let's define our item
            myItemDef = ScriptableObject.CreateInstance<ItemDef>();

            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            myItemDef.name = "HIKING_BOOT";
            myItemDef.nameToken = "HIKING_BOOT_NAME";
            myItemDef.pickupToken = "HIKING_BOOT_PICKUP";
            myItemDef.descriptionToken = "HIKING_BOOT_DESC";
            myItemDef.loreToken = "HIKING_BOOT_LORE";

            myItemDef.tier = ItemTier.Tier1;

            MainAssets = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(pluginInfo.Location), "hikingboot_assets"));

            myItemDef.pickupIconSprite = MainAssets.LoadAsset<Sprite>("HikingBootIcon.png");
            myItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            // Determines if item can be used in printer or shrine of order. Generally true if item has a tier 
            myItemDef.canRemove = true;

            // Hidden is used for helper items that give invisible buffs/debuffs
            myItemDef.hidden = false;

            // For if we get frisky
            var displayRules = new ItemDisplayRuleDict(null);

            // Then finally add it to R2API
            ItemAPI.Add(new CustomItem(myItemDef, displayRules));

            Hooks();
        }

        public  void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender == null || sender.inventory == null) return;

                var count = sender.inventory.GetItemCount(myItemDef);

                if (count > 0)
                {
                    args.moveSpeedMultAdd += 0.1f * count;
                }
            };

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.rejected)
            {
                return;
            }

            var inventoryCount = GetCount(self.body);

            var healthBefore = self.health;

            if (inventoryCount > 0)
            {
                // Reduce damage by 2 per stack of the item
                var damageReduction = 2 * inventoryCount;

                // Calculate the reduced damage
                var reducedDamage = Mathf.Max(damageInfo.damage - damageReduction, 1f);

                // Update the damage info with the reduced damage
                damageInfo.damage = reducedDamage;
            }

            orig(self, damageInfo);
        }


        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(myItemDef);
        }

    }
}