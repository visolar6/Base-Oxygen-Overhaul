using Nautilus.Options.Attributes;

namespace BaseOxygenOverhaul
{
    [Menu("Base Oxygen Overhaul")]
    public class Options : Nautilus.Json.ConfigFile
    {
        [Slider(LabelLanguageId = "Option_ProductionRateSmallOxygenGenerator", TooltipLanguageId = "OptionTooltip_ProductionRateSmallOxygenGenerator", DefaultValue = 0.5f, Min = 0.1f, Max = 2.5f, Step = 0.01f)]
        public float ProductionRateSmallOxygenGenerator = 0.5f;

        [Slider(LabelLanguageId = "Option_ProductionRateLargeOxygenGenerator", TooltipLanguageId = "OptionTooltip_ProductionRateLargeOxygenGenerator", DefaultValue = 4.0f, Min = 1.0f, Max = 10.0f, Step = 0.1f)]
        public float ProductionRateLargeOxygenGenerator = 4.0f;
    }
}
