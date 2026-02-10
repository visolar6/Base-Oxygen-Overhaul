using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using UWE;

namespace BaseOxygenOverhaul.Prefabs
{

    public static class LargeOxygenGeneratorFragment
    {
        public static PrefabInfo Info { get; private set; } = PrefabInfo
            .WithTechType(
                classId: "LargeOxygenGeneratorFragment",
                displayName: null,
                description: null,
                techTypeOwner: Assembly.GetExecutingAssembly()
            )
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("LargeOxygenGeneratorFragmentIcon"));

        private static WorldEntityInfo WorldEntityInfo => new WorldEntityInfo()
        {
            cellLevel = LargeWorldEntity.CellLevel.Near,
            classId = Info.ClassID,
            localScale = Vector3.one,
            prefabZUp = false,
            slotType = EntitySlot.Type.Small,
            techType = Info.TechType
        };

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(new CloneTemplate(Info, "3c076458-505e-4683-90c1-34c1f7939a0f") // cyclops engine fragment
            {
                ModifyPrefab = ModifyPrefab
            });

            prefab.SetUnlock(Info.TechType, 3)
                .WithAnalysisTech(null, null, null)
                .WithScannerEntry(
                    blueprint: LargeOxygenGenerator.Info.TechType,
                    scanTime: 5f,
                    isFragment: true,
                    encyKey: "Ency_LargeOxygenGenerator",
                    destroyAfterScan: true
                );

            prefab.SetSpawns(WorldEntityInfo, new LootDistributionData.BiomeData[] {
                new LootDistributionData.BiomeData(){ biome = BiomeType.BloodKelp_TechSite, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.Dunes_TechSite, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.GrandReef_TechSite, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.JellyShroomCaves_AbandonedBase_Outside, count = 2, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.SeaTreaderPath_TechSite, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.ShipInterior_Cargo_Crate, count = 1, probability = 0.05f },
                new LootDistributionData.BiomeData(){ biome = BiomeType.ShipInterior_PowerRoomUnderwater, count = 1, probability = 0.05f },
            });

            prefab.Register();
        }

        private static void ModifyPrefab(GameObject prefab)
        {

        }
    }
}