using System.Linq;
using UnityEngine;

namespace BaseOxygenOverhaul.Utilities
{
    public static class PrefabCleaner
    {
        /// <summary>
        /// Cleans the wall locker prefab by removing unnecessary components and child objects, leaving only the essential structure
        /// </summary>
        /// <param name="wallLocker"></param>
        public static void CleanWallLocker(GameObject wallLocker)
        {
            /*
            Component Hierarchy
            
            Before:
            | SmallLocker(Clone) [Transform, Constructable, TechTag, PrefabIdentifier, StorageContainer, SkyApplier, ConstructableBounds]
            |   model [Transform]
            |     submarine_locker_02 [Transform, MeshFilter, MeshRenderer, Animator]
            |       submarine_locker_02_door [Transform, MeshFilter, MeshRenderer]
            |   Label [Transform, BoxCollider, ColoredLabel, ChildObjectIdentifier, VFXSurface]
            |     UI [RectTransform, Canvas, uGUI_GraphicRaycaster, uGUI_SignInput, uGUI_NavigableControlGrid]
            |       InputField [RectTransform, CanvasRenderer, uGUI_InputField]
            |         Text [RectTransform, CanvasRenderer, TextMeshProUGUI, ContentSizeFitter]
            |       ColorSelector [RectTransform, Button, LayoutElement]
            |         Indicator [RectTransform, CanvasRenderer, Image]
            |   StorageRoot [Transform, ChildObjectIdentifier]
            |   Build Trigger [Transform, BoxCollider]
            |   Collider [Transform, BoxCollider]
            |   TriggerCull [Transform, TriggerCull, SphereCollider]

            After:
            | SmallLocker(Clone) [Transform, Constructable, TechTag, PrefabIdentifier, SkyApplier, ConstructableBounds]
            |   model [Transform]
            |     submarine_locker_02 [Transform, MeshFilter, MeshRenderer, Animator]
            |       submarine_locker_02_door [Transform, MeshFilter, MeshRenderer]
            |   Build Trigger [Transform, BoxCollider]
            |   Collider [Transform, BoxCollider]
            |   TriggerCull [Transform, SphereCollider]
            */

            // Remove unnecessary components
            var componentsToRemove = wallLocker.GetComponentsInChildren<Component>(true).ToList();
            foreach (var component in componentsToRemove)
            {
                // Check if component is still valid (not destroyed)
                if (component == null)
                    continue;

                var typeName = component.GetType().Name;

                if (component is StorageContainer)
                {
                    Object.DestroyImmediate(component);
                }
                else if (typeName == "ColoredLabel" || typeName == "TriggerCull")
                {
                    Object.DestroyImmediate(component);
                }
            }

            // Remove StorageRoot GameObject
            var storageRoot = wallLocker.transform.Find("StorageRoot");
            if (storageRoot != null)
            {
                Object.DestroyImmediate(storageRoot.gameObject);
            }

            // Remove Label GameObject
            var label = wallLocker.transform.Find("Label");
            if (label != null)
            {
                Object.DestroyImmediate(label.gameObject);
            }
        }
    }
}