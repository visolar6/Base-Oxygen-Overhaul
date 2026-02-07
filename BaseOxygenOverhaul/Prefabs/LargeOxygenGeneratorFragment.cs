using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using BaseOxygenOverhaul.Utilities;
using BaseOxygenOverhaul.Mono;
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
            .WithIcon(ResourceHandler.LoadSpriteFromFile("Assets/Sprite/LargeOxygenGeneratorIcon.png"));

        // private static WorldEntityInfo WorldEntityInfo => new WorldEntityInfo()
        // {
        //     cellLevel = LargeWorldEntity.CellLevel.Near,
        //     classId = Info.ClassID,
        //     localScale = Vector3.one,
        //     prefabZUp = false,
        //     slotType = EntitySlot.Type.Small,
        //     techType = Info.TechType
        // };

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(new CloneTemplate(Info, "3c076458-505e-4683-90c1-34c1f7939a0f") // cyclops engine fragment
            {
                ModifyPrefab = ModifyPrefab
            });

            prefab.SetUnlock(Info.TechType, 3).WithAnalysisTech(null, null, null).WithScannerEntry(
                blueprint: LargeOxygenGenerator.Info.TechType,
                scanTime: 5f,
                isFragment: true,
                encyKey: Global.Keys.EncyLargeOxygenGeneratorFragment,
                destroyAfterScan: true
            );

            // TODO: figure out good spawn locations for the fragments - should be mid-to-late game biomes
            // prefab.SetSpawns(WorldEntityInfo,
            // [
            //     new(){ biome = BiomeType.SafeShallows_CaveFloor, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_Grass, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_Plants, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_SandFlat, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_ShellTunnel, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_ShellTunnelHuge, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_CaveSpecial, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.Kelp_CaveWall, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.Kelp_CaveFloor, count = 3, probability = 1f },
            //     new(){ biome = BiomeType.SafeShallows_ShellTunnelHuge, count = 3, probability = 1f },
            // ]);
            // Or, for testing, just spawn a few in the starting biome
            // prefab.SetSpawns([
            //     new SpawnLocation(Vector3.zero, Vector3.zero, Vector3.zero),
            //     new SpawnLocation(Vector3.one, Vector3.zero, Vector3.zero),
            //     new SpawnLocation(new Vector3(20f, 0f, 20f), Vector3.zero, Vector3.zero),
            // ]);

            prefab.Register();
        }

        private static void ModifyPrefab(GameObject prefab)
        {

        }
    }
}