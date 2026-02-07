using UnityEngine;

namespace BaseOxygenOverhaul.Utilities
{
    public static class BaseOxygenUtils
    {
        public static float GetOxygenToRemove(float netProductionRate, float oxygenAvailable)
        {
            // Any oxygen rate at or above 0 means the base meets or exceeds player needs, so remove nothing
            // This method shouldn't ever be called with a positive production rate, but this is a safeguard against that and against any weird edge cases where the rate might be slightly positive due to floating point imprecision
            if (netProductionRate >= 0f) return 0f;

            // For negative rates, the max that should ever be removed is 3f (since we're removing oxygen every 3 seconds, that's 1 unit per second)
            // But we also can't remove more oxygen than is actually available, so take the minimum of those two values and the absolute value of the net production rate
            return Mathf.Min(-netProductionRate, oxygenAvailable, 3f);
        }
    }
}