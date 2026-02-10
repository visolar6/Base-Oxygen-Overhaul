using BaseOxygenOverhaul.Utilities;
using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    public class OxygenHandTarget : HandTarget, IHandTarget
    {
        private const float CalculationInterval = 1f;
        private float calculationTimer = 0f;
        private string text = string.Empty;

        // Cache in Awake/Start
        private OxygenGeneratorManager manager;
        private string netOxygenText;
        private string o2PerSecondText;

        private void Start()
        {
            manager = GetComponentInParent<OxygenGeneratorManager>();
            netOxygenText = Language.main.Get("NetOxygenProduction");
            o2PerSecondText = Language.main.Get("O2PerSecond");
        }

        public void OnHandClick(GUIHand hand)
        {

        }

        public void OnHandHover(GUIHand hand)
        {
            calculationTimer += Time.deltaTime;
            if (calculationTimer >= CalculationInterval)
            {
                calculationTimer = 0f;
                RecalculateText();
            }

            if (!string.IsNullOrEmpty(text))
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, text, false, GameInput.Button.None);
                HandReticle.main.SetIcon(HandReticle.IconType.Info);
            }
        }

        void RecalculateText()
        {
            if (manager != null)
                switch (manager.GetState())
                {
                    case Types.OxygenGeneratorState.Online:
                        {
                            var netRate = BaseOxygenHandler.GetNetRate(manager.ParentBase);

                            float roundedNetRate = Mathf.Round(netRate * 100f) / 100f;
                            var baseNetRateText = roundedNetRate > 0 ? $"+{roundedNetRate}" : roundedNetRate < 0 ? $"-{Mathf.Abs(roundedNetRate)}" : "0";
                            text = $"{netOxygenText}: {baseNetRateText} {o2PerSecondText}";
                            break;
                        }
                    default:
                        text = string.Empty;
                        break;
                }
        }
    }
}
