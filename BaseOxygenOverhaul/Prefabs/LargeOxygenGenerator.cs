using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Nautilus.Crafting;
using Nautilus.Utility;
using BaseOxygenOverhaul.Types;
using BaseOxygenOverhaul.Mono.OxygenGenerator;

namespace BaseOxygenOverhaul.Prefabs
{
    public static class LargeOxygenGenerator
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType(
                classId: "LargeOxygenGenerator",
                unlockAtStart: false,
                techTypeOwner: Assembly.GetExecutingAssembly()
            )
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("LargeOxygenGeneratorIcon"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(CreatePrefab);

            prefab
                .SetUnlock(TechType.PlasteelIngot)
                .WithCompoundTechsForUnlock(new List<TechType> { TechType.AdvancedWiringKit, TechType.Aerogel, TechType.Lubricant, TechType.FiberMesh })
                .WithAnalysisTech(null, null, null)
                .WithPdaGroupCategoryAfter(
                    TechGroup.InteriorPieces,
                    TechCategory.InteriorPiece,
                    TechType.Bioreactor
                );

            prefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>() {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.Aerogel, 1),
                    new Ingredient(TechType.Lubricant, 1),
                    new Ingredient(TechType.FiberMesh, 2)
                }
            });

            prefab.Register();
        }

        private static IEnumerator CreatePrefab(IOut<GameObject> result)
        {
            // Load base prefab
            var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("LargeOxygenGenerator");
            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
            MaterialUtils.ApplySNShaders(prefab, 6);

            // Allow construction
            var model = prefab.transform.Find("default").gameObject;
            var constructable = PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.Base | ConstructableFlags.Inside | ConstructableFlags.Wall, model);
            constructable.attachedToBase = true;
            constructable.deconstructionAllowed = true;
            constructable.placeDefaultDistance = 5f;
            constructable.placeMinDistance = 1f;
            constructable.placeMaxDistance = 10f;
            constructable.rotationEnabled = false;
            var constructableBounds = prefab.AddComponent<ConstructableBounds>();
            model.TryGetComponent<Collider>(out var collider);
            if (collider == null) Plugin.Log.LogWarning($"LargeOxygenGenerator prefab is missing a collider! This may cause issues with construction.");
            constructableBounds.bounds = collider != null
                ? new OrientedBounds(collider.bounds.center, Quaternion.identity, collider.bounds.size)
                : new OrientedBounds(Vector3.zero, Quaternion.identity, Vector3.one);

            // Add O2 display
            // prefab.AddComponent<OxygenGeneratorO2Display>();

            // Add behaviours
            var manager = prefab.EnsureComponent<OxygenGeneratorManager>();
            manager.Size = OxygenGeneratorSize.Large;
            // prefab.EnsureComponent<OxygenGeneratorAudioVisual>();
            prefab.EnsureComponent<OxygenHandTarget>();

            result.Set(prefab);
            yield break;
        }
    }
}