// using System;
// using Nautilus.Utility;
// using UnityEngine;

// namespace BaseOxygenOverhaul.Mono.OxygenGenerator
// {
//     /// <summary>
//     /// Handles the audio and visual effects for oxygen generators.
//     /// </summary>
//     public class OxygenGeneratorAudioVisual : MonoBehaviour
//     {
//         private const float SmallOxygenGeneratorAmbientInterval = 2f;
//         private const float LargeOxygenGeneratorAmbientInterval = 4f;

//         private OxygenGeneratorManager manager;

//         private FMODAsset ambientSound;

//         private float ambientSoundTimer = 0f;

//         private void Start()
//         {
//             manager = GetComponent<OxygenGeneratorManager>();
//             try
//             {
//                 switch (manager.Size)
//                 {
//                     case Types.OxygenGeneratorSize.Small:
//                         ambientSound = AudioUtils.GetFmodAsset(Global.FMODSoundIds.SmallOxygenGeneratorAmbient, Global.FMODSoundIds.SmallOxygenGeneratorAmbient);
//                         break;
//                     case Types.OxygenGeneratorSize.Large:
//                         ambientSound = AudioUtils.GetFmodAsset(Global.FMODSoundIds.LargeOxygenGeneratorAmbient, Global.FMODSoundIds.LargeOxygenGeneratorAmbient);
//                         break;
//                 }
//             }
//             catch (Exception e)
//             {
//                 Plugin.Log.LogError($"Failed to load ambient sound for {manager.Size} oxygen generator: {e}");
//             }
//         }
//     }
// }