using BepInEx.Configuration;
using CraigsCorner.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

using static CraigsCorner.Utils.RichTextFormating;
using static R2API.RecalculateStatsAPI;

namespace CraigsCorner.Items.Tier1
{
    public class Monocle : ItemBase<Monocle>
    {
        public static ConfigEntry<float> CritDamage { get; set; }

        public override string ItemName => "Monocle";

        public override string ItemLangTokenName => "CRIT_DAMAGE";

        public override string ItemPickupDesc => "Increase critical strike damage.";

        public override string ItemFullDescription => $"{DT("Critical Strikes")} deal an additional {DT($"{CritDamage}% damage")}{ST($"(+{CritDamage}% per stack)")}.";

        public override string ItemLore => "<Insert some lore about monocles.>";

        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };

        public override GameObject ItemModel => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

        public override Sprite ItemIcon => Main.MainAssets.LoadAsset<Sprite>("MonocleIcon.png");

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
            CritDamage = config.Bind<float>(
            $"Item: {ItemName}",
            "Crit Damage",
            15,
            "Additional Percent Damage added to Critical Strikes."
            );
        }

        public override void Hooks()
        {
            GetStatCoefficients += Crit_Damage_Calc;
        }

        private void Crit_Damage_Calc(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender == null || sender.inventory == null) { return; }

            var inventoryCount = GetCount(sender);

            if (inventoryCount > 0) { args.critDamageMultAdd += (CritDamage.Value / 100f) * inventoryCount; }
        }
    }
}

