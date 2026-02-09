using BaseOxygenOverhaul.Utilities;
using UnityEngine;

namespace BaseOxygenOverhaul.Mono.OxygenGenerator
{
    public class OxygenHandTarget : HandTarget, IHandTarget
    {
        private const float CalculationInterval = 1f;
        private float calculationTimer = 0f;
        private float lastHoverTime = 0f;
        private string text = string.Empty;

        public void OnHandClick(GUIHand hand)
        {
            // Do nothing
        }

        public void OnHandHover(GUIHand hand)
        {
            float currentTime = Time.time;
            bool isNewHover = currentTime - lastHoverTime > CalculationInterval;
            lastHoverTime = currentTime;

            // Recalculate immediately on new hover, otherwise use interval
            if (calculationTimer <= 0f || isNewHover)
            {
                var constructable = GetComponentInParent<Constructable>();
                if (constructable?.constructed ?? false == true)
                {
                    var manager = GetComponentInParent<OxygenGeneratorManager>();
                    if (manager != null)
                    {
                        float netRate;
                        var _base = GetComponentInParent<Base>();
                        if (_base != null)
                        {
                            netRate = BaseOxygenHandler.GetNetRate(_base);
                            calculationTimer = CalculationInterval;

                            float roundedNetRate = Mathf.Round(netRate * 100f) / 100f;
                            var baseNetRateText = roundedNetRate > 0 ? $"+{roundedNetRate}" : roundedNetRate < 0 ? $"-{Mathf.Abs(roundedNetRate)}" : "0";
                            var netOxygenProductionText = Language.main.Get("NetOxygenProduction");
                            var unitsPerSecondText = Language.main.Get("UnitsPerSecond");
                            text = $"{netOxygenProductionText}: {baseNetRateText} {unitsPerSecondText}";
                        }
                        else
                        {
                            text = string.Empty;
                        }
                    }
                    else
                    {
                        text = string.Empty;
                    }
                }
                else
                {
                    text = string.Empty;
                }
            }
            else
            {
                calculationTimer -= Time.deltaTime;
            }

            if (!string.IsNullOrEmpty(text))
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, text, false, GameInput.Button.None);
                HandReticle.main.SetIcon(HandReticle.IconType.Info);
            }
        }
    }
}