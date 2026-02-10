using TMPro;
using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    /// <summary>
    /// Manages the oxygen generator's O2 display, which displays the current oxygen status.
    /// </summary>
    public class OxygenGeneratorO2Display : MonoBehaviour
    {
        private static readonly Color ColorOnline = new Color(4f / 255f, 1f, 13f / 255f, 1f);
        private static readonly Color ColorOffline = new Color(1f, 0f, 0f, 0.5f);
        private static readonly Color ColorFlooded = new Color(1f, 1f, 0f, 1f);

        private OxygenGeneratorManager manager;

        private TextMeshPro o2DisplayLabel;
        private TextMeshPro o2DisplayStatus;

        private float timeSinceLastUpdate = 1f; // Update immediately on start
        private string o2OnlineText;
        private string o2OfflineText;
        private string o2FloodedText;

        private void Start()
        {
            manager = GetComponent<OxygenGeneratorManager>();
            o2OnlineText = Language.main.Get("O2StatusOnline");
            o2OfflineText = Language.main.Get("O2StatusOffline");
            o2FloodedText = Language.main.Get("O2StatusFlooded");

            var o2DisplayLabelObj = transform.Find("default/O2Display/Label");
            o2DisplayLabelObj?.TryGetComponent(out o2DisplayLabel);
            if (o2DisplayLabel == null)
            {
                Plugin.Log.LogError($"OxygenGeneratorO2Display on {gameObject.name} could not find O2 display label TextMeshPro component. O2 display will not function.");
            }

            var o2DisplayStatusObj = transform.Find("default/O2Display/Status");
            o2DisplayStatusObj?.TryGetComponent(out o2DisplayStatus);
            if (o2DisplayStatus == null)
            {
                Plugin.Log.LogError($"OxygenGeneratorO2Display on {gameObject.name} could not find O2 display status TextMeshPro component. O2 display will not function.");
            }
        }

        private void Update()
        {
            if (timeSinceLastUpdate >= 1f)
            {
                UpdateO2Display();
                timeSinceLastUpdate = 0f;
            }
            else
            {
                timeSinceLastUpdate += Time.deltaTime;
            }
        }

        private void UpdateO2Display()
        {
            if (o2DisplayLabel != null)
            {
                o2DisplayLabel.text = Language.main.Get("O2StatusLabel");
            }

            if (o2DisplayStatus != null)
            {
                if (manager.ParentBasePowerRelay != null)
                {
                    if (manager.ParentBasePowerRelay != null && manager.ParentBasePowerRelay.IsPowered())
                    {
                        if (manager.ParentBaseFloodSim != null && manager.ParentBaseFloodSim.IsUnderwater(transform.position))
                        {
                            o2DisplayStatus.text = o2FloodedText;
                            o2DisplayStatus.color = ColorFlooded;
                            o2DisplayStatus.alpha = 1f;
                        }
                        else
                        {
                            o2DisplayStatus.text = o2OnlineText;
                            o2DisplayStatus.color = ColorOnline;
                            o2DisplayStatus.alpha = 1f;
                        }
                    }
                    else
                    {
                        o2DisplayStatus.text = o2OfflineText;
                        o2DisplayStatus.color = ColorOffline;
                        o2DisplayStatus.alpha = 0.5f;
                    }
                }
                else
                {
                    Plugin.Log.LogWarning($"OxygenGeneratorO2Display on {gameObject.name} does not have a parent base power relay assigned. O2 display status will not update correctly.");
                }
            }
            else
            {
                Plugin.Log.LogWarning($"OxygenGeneratorO2Display on {gameObject.name} does not have a TextMeshPro component assigned for the O2 status. O2 display status will not update.");
            }
        }
    }
}