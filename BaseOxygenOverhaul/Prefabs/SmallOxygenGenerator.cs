using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BaseOxygenOverhaul.Mono.OxygenGenerator;
using BaseOxygenOverhaul.Utilities;
using BaseOxygenOverhaul.Types;

namespace BaseOxygenOverhaul.Prefabs
{
    public static class SmallOxygenGenerator
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType(
                classId: "SmallOxygenGenerator",
                unlockAtStart: false,
                techTypeOwner: Assembly.GetExecutingAssembly()
            )
            .WithIcon(ResourceHandler.LoadSpriteFromFile("Assets/Sprite/SmallOxygenGeneratorIcon.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            // prefab.SetGameObject(new CloneTemplate(Info, "5fc7744b-5a2c-4572-8e53-eebf990de434") // wall locker
            // {
            //     ModifyPrefab = ModifyPrefab
            // });

            prefab.SetGameObject(CreatePrefab);

            prefab.SetUnlock(TechType.Titanium)
                .WithPdaGroupCategoryAfter(
                    TechGroup.InteriorModules,
                    TechCategory.InteriorModule,
                    TechType.Radio
                );

            prefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.FiberMesh, 1)
                }
            });

            prefab.Register();
        }

        private static IEnumerator CreatePrefab(IOut<GameObject> result)
        {
            var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("SmallOxygenGenerator");

            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

            MaterialUtils.ApplySNShaders(prefab, 4);

            var model = prefab.transform.Find("default").gameObject;

            var constructable = PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.Base | ConstructableFlags.Inside | ConstructableFlags.Wall, model);
            constructable.rotationEnabled = false;
            constructable.placeDefaultDistance = 5f;
            constructable.placeMinDistance = 1f;
            constructable.placeMaxDistance = 10f;

            var bounds = prefab.AddComponent<ConstructableBounds>();
            bounds.bounds = new OrientedBounds(Vector3.up * 0.5f, Quaternion.identity, new Vector3(0.8f, 1f, 0.8f));

            var manager = prefab.EnsureComponent<OxygenGeneratorManager>();
            manager.type = OxygenGeneratorSize.Small;

            result.Set(prefab);
            yield break;
        }

        private static void ModifyPrefab(GameObject wallLocker)
        {
            // Remove all unnecessary components
            PrefabCleaner.CleanWallLocker(wallLocker);

            // Add manager component
            var manager = wallLocker.EnsureComponent<OxygenGeneratorManager>();
            manager.type = OxygenGeneratorSize.Small;

            // Add audio-visual component
            wallLocker.EnsureComponent<OxygenGeneratorAudioVisualSmall>();
        }
    }
}