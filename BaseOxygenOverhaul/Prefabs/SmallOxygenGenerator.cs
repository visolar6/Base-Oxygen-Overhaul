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
                unlockAtStart: true,
                techTypeOwner: Assembly.GetExecutingAssembly()
            )
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("SmallOxygenGeneratorIcon"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(CreatePrefab);

            prefab
                .SetUnlock(TechType.Titanium)
                .WithCompoundTechsForUnlock(new List<TechType> { TechType.WiringKit, TechType.FiberMesh })
                .WithAnalysisTech(null, null, null)
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
            // Load base prefab
            var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("SmallOxygenGenerator");
            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
            MaterialUtils.ApplySNShaders(prefab, 6);

            // Allow construction
            var model = prefab.transform.Find("default").gameObject;
            // Push the model back slightly so it sits inside of the wall slightly
            model.transform.localPosition += new Vector3(0f, 0f, -0.02f);
            var constructable = PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.Base | ConstructableFlags.Inside | ConstructableFlags.Wall, model);
            constructable.rotationEnabled = false;
            constructable.placeDefaultDistance = 5f;
            constructable.placeMinDistance = 1f;
            constructable.placeMaxDistance = 10f;
            var constructableBounds = prefab.AddComponent<ConstructableBounds>();
            model.TryGetComponent<Collider>(out var collider);
            if (collider == null) Plugin.Log.LogWarning($"SmallOxygenGenerator prefab is missing a collider! This may cause issues with construction.");
            constructableBounds.bounds = collider != null
                ? new OrientedBounds(collider.bounds.center, Quaternion.identity, collider.bounds.size)
                : new OrientedBounds(Vector3.zero, Quaternion.identity, Vector3.one);

            // Add O2 display
            prefab.AddComponent<OxygenGeneratorO2Display>();

            // Add behaviours
            var manager = prefab.EnsureComponent<OxygenGeneratorManager>();
            manager.Size = OxygenGeneratorSize.Small;
            // prefab.EnsureComponent<OxygenGeneratorAudioVisual>();
            prefab.EnsureComponent<OxygenHandTarget>();

            result.Set(prefab);
            yield break;
        }
    }
}