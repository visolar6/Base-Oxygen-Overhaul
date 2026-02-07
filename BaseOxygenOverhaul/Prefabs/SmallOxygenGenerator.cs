using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using BaseOxygenOverhaul.Mono.OxygenGenerator;
using BaseOxygenOverhaul.Utilities;
using BaseOxygenOverhaul.Types;
using Nautilus.Crafting;
using System.Collections.Generic;

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

            prefab.SetGameObject(new CloneTemplate(Info, "5fc7744b-5a2c-4572-8e53-eebf990de434") // wall locker
            {
                ModifyPrefab = ModifyPrefab
            });

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