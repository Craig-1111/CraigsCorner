using BepInEx.Configuration;
using CraigsCorner.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

using static CraigsCorner.Utils.RichTextFormating;
using static R2API.RecalculateStatsAPI;

namespace CraigsCorner.Items.Tier1
{
    public class SteelToeBoot : ItemBase<SteelToeBoot>
    {
        public static ConfigEntry<float> MovementSpeed { get; set; }
        public static ConfigEntry<float> DamageReduction { get; set; }

        public override string ItemName => "Steel Toe Boot";

        public override string ItemLangTokenName => "MOVEMENTSPEED_DAMAGEREDUCTION";

        public override string ItemPickupDesc => "Increase Movement speed and receive flat damage reduction from all attacks.";

        public override string ItemFullDescription => $"Increases {UT("movement speed")} by {UT($"{MovementSpeed.Value}%")} {ST($"(+{MovementSpeed.Value}% per stack)")} and reduce all {DT("incoming damage")} by {DT($"{DamageReduction.Value}")} {ST($"(+{DamageReduction.Value} per stack)")}. {DT("Incoming damage")} cannot be reduced below {DT("1")}.";

        public override string ItemLore => "<Insert some lore about a construction workers boot.>";

        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Utility };

        public override GameObject ItemModel => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

        public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("HikingBootIcon.png");

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        private void CreateConfig(ConfigFile config)
        {
            MovementSpeed = config.Bind<float>(
            $"Item: {ItemName}",
            "Movement Speed",
            10,
            "Percentage Movement Speed Increase applied per stack."
            );

            DamageReduction = config.Bind<float>(
            $"Item: {ItemName}",
            "Damage Reduction",
            2,
            "Flat Damage Reduction applied per stack."
            );
        }

        public override void Hooks()
        {
            GetStatCoefficients += Movement_Speed_Calc;
            On.RoR2.HealthComponent.TakeDamage += Damage_Reduction_Calc;
        }

        private void Damage_Reduction_Calc(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.rejected) return;

            var inventoryCount = GetCount(self.body);

            if (inventoryCount > 0)
            {
                // Reduce damage per stack of the item
                var damageReduction = DamageReduction.Value * inventoryCount;

                // Calculate the reduced damage
                var reducedDamage = Mathf.Max(damageInfo.damage - damageReduction, 1f);

                // Update the damage info with the reduced damage
                damageInfo.damage = reducedDamage;
            }

            orig(self, damageInfo);
        }

        private void Movement_Speed_Calc(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender == null || sender.inventory == null) { return; }

            var inventoryCount = GetCount(sender);

            if (inventoryCount > 0) { args.moveSpeedMultAdd += (MovementSpeed.Value / 100f) * inventoryCount; }
        }
    }
}
