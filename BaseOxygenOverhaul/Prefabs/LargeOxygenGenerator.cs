using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using BaseOxygenOverhaul.Utilities;
using System.Collections;
using UnityEngine;
using Nautilus.Handlers;
using System.Collections.Generic;
using Nautilus.Crafting;

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
            .WithIcon(ResourceHandler.LoadSpriteFromFile("Assets/Sprite/OxygenGeneratorIcon.png"));

        public static void Register()
        {
            var prefab = new CustomPrefab(Info);

            // Clone the bioreactor prefab and modify it
            prefab.SetGameObject(new CloneTemplate(Info, "83190e0d-d632-4b18-9906-6ad1b91f3315") // bioreactor
            {
                ModifyPrefabAsync = ModifyPrefab
            });

            // Set unlocks and PDA placement
            prefab
                .SetUnlock(TechType.PlasteelIngot)
                .WithCompoundTechsForUnlock(new List<TechType> { TechType.AdvancedWiringKit, TechType.Aerogel, TechType.Lubricant, TechType.FiberMesh })
                .WithPdaGroupCategoryAfter(TechGroup.InteriorPieces, TechCategory.InteriorPiece, TechType.Bioreactor);

            // Set crafting recipe and other properties
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

            KnownTechHandler.SetAnalysisTechEntry(Info.TechType, new List<TechType>(), KnownTechHandler.DefaultUnlockData.BlueprintUnlockMessage, ResourceHandler.LoadSpriteFromFile("Assets/Sprite/OxygenGeneratorPopup.png"));
        }

        private static IEnumerator ModifyPrefab(GameObject prefab)
        {
            // log every component in the prefab for debugging
            foreach (var component in prefab.GetComponentsInChildren<Component>(true))
            {
                // Log all information about the component for debugging
                Plugin.Log.LogInfo($"Component: {component.GetType().Name}, Active: {component.gameObject.activeSelf}, Layer: {component.gameObject.layer}, Tag: {component.gameObject.tag}");
            }
            yield break;
        }
    }
}