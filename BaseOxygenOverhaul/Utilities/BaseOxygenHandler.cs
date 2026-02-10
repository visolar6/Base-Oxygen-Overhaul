using System.Collections.Generic;
using UnityEngine;
using BaseOxygenOverhaul.Mono.OxygenGenerator;
using BaseOxygenOverhaul.Types;

namespace BaseOxygenOverhaul.Utilities
{
    public static class BaseOxygenHandler
    {
        /// <summary>
        /// The interval (in seconds) at which the player's oxygen should be depleted when inside a base that doesn't produce enough oxygen to sustain them.
        /// </summary>
        public const float BaseOxygenDepletionInterval = 3f;

        /// <summary>
        /// The maximum amount of oxygen that can be depleted from the player every interval.
        /// </summary>
        public const float BaseMaxOxygenDepletionRate = 3f;

        /// <summary>
        /// The interval (in seconds) for how often the base reference is updated.
        /// </summary>
        public const float BaseReferenceInterval = 1f;

        /// <summary>
        /// The interval (in seconds) at which the base's net oxygen production rate is checked and updated.
        /// </summary>
        public const float BaseNetRateCheckInterval = 2f;

        /// <summary>
        /// The interval (in seconds) at which habitable cells are checked for being above water.
        /// </summary>
        public const float HabitableCellCheckInterval = 3f;

        /// <summary>
        /// The interval (in seconds) at which warnings about no oxygen generators being present should be logged
        /// </summary>
        public const float SmartWarningsInterval = 30f;

        public static readonly Dictionary<OxygenGeneratorSize, float> GeneratorProductionRates = new Dictionary<OxygenGeneratorSize, float>()
        {
            { OxygenGeneratorSize.Small, Plugin.Options.ProductionRateSmallOxygenGenerator },
            { OxygenGeneratorSize.Large, Plugin.Options.ProductionRateLargeOxygenGenerator }
        };

        public static readonly Dictionary<Base.CellType, float> HabitableCellDepletionRates = new Dictionary<Base.CellType, float>()
        {
            { Base.CellType.Connector, 0.1f },
            { Base.CellType.Corridor, 0.15f },
            { Base.CellType.Observatory, 0.2f },
            { Base.CellType.MapRoom, 0.25f },
            { Base.CellType.MapRoomRotated, 0.25f },
            { Base.CellType.Moonpool, 0.35f },
            { Base.CellType.MoonpoolRotated, 0.35f },
            { Base.CellType.Room, 0.25f },
            { Base.CellType.LargeRoom, 1f },
            { Base.CellType.LargeRoomRotated, 1f }
        };

        // Leave this public so the Player_Patch can set it when the player enters or leaves a base
        public static Base _base;
        // Timer to track base reference update interval
        private static float baseReferenceTimer = 0f;
        // Cached reference to the base's current net oxygen production rate, since this is an expensive calculation to do and we only need to do it every 3 seconds at most
        private static float cachedNetRate = 0f;
        // Timer to track net rate interval
        private static float netRateCheckTimer = 0f;
        // Cached reference to whether the current base has at least one habitable cell above water, since this is an expensive check to do and we only need to do it every 5 seconds at most
        private static bool cachedHasHabitableCellAboveWater = false;
        // Timer to track habitable cells interval
        private static float habitableCellCheckTimer = 0f;
        // Timer to track oxygen depletion interval
        private static float baseOxygenDepleteTimer = 0f;
        // Timer to track no oxygen generator warning interval
        private static float smartWarningsTimer = 0f;

        /// <summary>
        /// Handles player oxygen add logic when inside a base, based on the base's net oxygen production rate.
        /// Returns true if oxygen should be added as normal, or false if oxygen addition should be blocked.
        /// </summary>
        /// <returns>A boolean indicating whether oxygen addition should proceed as normal</returns>
        public static bool OnPlayerOxygenAdd(ref Oxygen oxygen)
        {
            if (oxygen.isPlayer)
            {
                // Block oxygen if the player is in a base (not Cyclops, not outside)
                if (Player.main != null && Player.main.currentSub != null && Player.main.currentSub.isBase)
                {
                    IncrementAllTimers();

                    if (baseReferenceTimer >= BaseReferenceInterval)
                    {
                        // Update base reference periodically in case the player has moved to a different base
                        _base = Player.main.currentSub.GetComponent<Base>();
                        baseReferenceTimer = 0f;
                    }

                    if (_base == null)
                    {
                        // Reset oxygen depletion timer if we can't find the base for some reason
                        baseOxygenDepleteTimer = 0f;

                        // Early return true because technically the mod is broken, so restore normal oxygen behavior rather than blocking all oxygen from being added and potentially suffocating
                        // the player when they should be getting oxygen from their base but the mod can't find the base for some reason
                        return true;
                    }

                    // This is the habitable cell check timer which runs every 5 seconds, to avoid doing expensive checks for habitable cells above water every time we want to deplete oxygen
                    if (Plugin.Options.AllowBaseSnorkel)
                    {
                        if (habitableCellCheckTimer >= HabitableCellCheckInterval)
                        {
                            // Update the cached value for whether the base has a habitable cell above water, since this can change as the player builds new modules or floods parts of their base
                            habitableCellCheckTimer = 0f;
                            cachedHasHabitableCellAboveWater = HasHabitableCellAboveWater(_base);
                        }
                    }
                    else
                    {
                        // If snorkel option is disabled, ensure the cache is false
                        cachedHasHabitableCellAboveWater = false;
                    }

                    // If the base has at least one habitable cell above water, we can treat the base as an infinite oxygen source and allow oxygen to be added as normal without depleting the player's oxygen
                    if (cachedHasHabitableCellAboveWater)
                    {
                        // If the base has at least one habitable cell above water, we can treat the base as an infinite oxygen source and allow oxygen to be added as normal without depleting the player's oxygen
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    // This is the net rate check timer which runs every 3 seconds, to avoid doing expensive calculations for the base's net oxygen production rate every time we want to deplete oxygen
                    if (netRateCheckTimer >= BaseNetRateCheckInterval)
                    {
                        // Get the base's net oxygen production rate (production - depletion)
                        cachedNetRate = GetNetRate(_base);
                        netRateCheckTimer = 0f;
                    }

                    // If the base's net oxygen production rate is at or above 0, allow oxygen to be added as normal without depleting the player's oxygen, since the base is producing enough oxygen to sustain the player
                    if (cachedNetRate >= 0f)
                    {
                        // As long as net production rate is at or above 0f, we can allow oxygen to be added as normal (base produces more than it consumes)
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    // This is the oxygen depletion timer which only runs every 3 seconds
                    if (baseOxygenDepleteTimer >= BaseOxygenDepletionInterval)
                    {
                        // If net production rate is below 0f, we need to deplete the player's oxygen (base produces less than it consumes)
                        // Unless the option for partial oxygen loss is enabled, in which case we just remove oxygen proportional to how much the base is failing to meet the player's oxygen needs, with a maximum of 3 seconds of oxygen removed every 3 seconds
                        // For example, a net negative production of -1f would remove 1 second of oxygen every 3 seconds, a net negative production of -2f would remove 2 seconds of oxygen every 3 seconds, and a net negative production of -4f would remove 3 seconds of oxygen every 3 seconds (due to the cap), assuming the player has that much oxygen to be removed
                        float oxygenToRemove;
                        if (Plugin.Options.PartialOxygenLoss) oxygenToRemove = GetOxygenToRemove(cachedNetRate, oxygen.oxygenAvailable);
                        else oxygenToRemove = BaseMaxOxygenDepletionRate;
                        oxygen.oxygenAvailable -= oxygenToRemove;
                        baseOxygenDepleteTimer = 0f;
                    }

                    // Discontinue the AddOxygen method to block oxygen from being added, with the side effect of depleting the player's oxygen based on the base's net production rate
                    return false;
                }
                else
                {
                    // Not in base - reset timers and cached values
                    ResetAllTimers();
                    cachedHasHabitableCellAboveWater = false;
                }
            }

            // Continue with normal oxygen addition for non-player oxygen or if the player is not in a base
            return true;
        }

        public static void IncrementAllTimers()
        {
            baseOxygenDepleteTimer += Time.deltaTime;
            baseReferenceTimer += Time.deltaTime;
            netRateCheckTimer += Time.deltaTime;
            habitableCellCheckTimer += Time.deltaTime;
            smartWarningsTimer += Time.deltaTime;
        }

        public static void ResetAllTimers()
        {
            baseOxygenDepleteTimer = 0f;
            baseReferenceTimer = BaseReferenceInterval;
            netRateCheckTimer = BaseNetRateCheckInterval;
            habitableCellCheckTimer = HabitableCellCheckInterval;
            smartWarningsTimer = SmartWarningsInterval * 0.9f; // Slightly reduce to avoid immediate warning spam when re-entering a base
        }

        public static bool IsHabitableCellType(Base.CellType cellType)
        {
            return HabitableCellDepletionRates.ContainsKey(cellType);
        }

        public static bool HasHabitableCellAboveWater(Base _base)
        {
            Plugin.Log.LogInfo(_base.cells.Length);

            // Check if any valid habitable cell is above water
            foreach (Int3 cellPos in _base.AllCells)
            {
                var cellType = _base.GetCell(cellPos);

                // Check if this is a valid cell type that contributes to oxygen depletion
                bool isDepletionCell = IsHabitableCellType(cellType);
                if (isDepletionCell)
                {
                    Vector3 worldPos = _base.GridToWorld(cellPos);
                    if (worldPos.y > 0f)
                    {
                        // At least one valid cell is above water, so the base acts as an infinite oxygen source
                        return true;
                    }
                }
            }

            return false;
        }

        public static float GetNetRate(Base _base)
        {
            var baseOxygenProductionRate = GetProductionRate(_base);
            var baseOxygenDepletionRate = GetDepletionRate(_base);
            var baseOxygenNetRate = baseOxygenProductionRate - baseOxygenDepletionRate;
            if (smartWarningsTimer > SmartWarningsInterval && baseOxygenNetRate < 0f)
            {
                ErrorMessage.AddMessage(Language.main.Get("NegativeNetOxygenProductionWarning"));
                smartWarningsTimer = 0f;
            }
            return baseOxygenNetRate;
        }

        /// <summary>
        /// Calculates the base's oxygen production rate based on its constructed oxygen generators
        /// </summary>
        public static float GetProductionRate(Base _base)
        {
            var oxygenGeneratorManagers = _base.gameObject.GetComponentsInChildren<OxygenGeneratorManager>();

            // If there are no oxygen generators, log a warning every 60 seconds to encourage the player to build oxygen generators, unless the option for allowing the base to act as an infinite oxygen source if it has at least one habitable cell above water is enabled and the base currently has at least one habitable cell above water, in which case we can allow the player to survive without oxygen generators and avoid spamming them with warnings about it
            if (smartWarningsTimer > SmartWarningsInterval && oxygenGeneratorManagers.Length == 0 && !(Plugin.Options.AllowBaseSnorkel && cachedHasHabitableCellAboveWater))
            {
                ErrorMessage.AddMessage(Language.main.Get("NoOxygenGeneratorWarning"));
                smartWarningsTimer = 0f;
                return 0f;
            }

            var rate = 0f;
            for (var i = 0; i < oxygenGeneratorManagers.Length; i++)
            {
                var oxygenGeneratorManager = oxygenGeneratorManagers[i];
                if (oxygenGeneratorManager != null && oxygenGeneratorManager.enabled)
                {
                    if (oxygenGeneratorManager.Constructable != null && oxygenGeneratorManager.Constructable.constructed)
                        rate += GeneratorProductionRates[oxygenGeneratorManager.Size];
                }
            }
            return rate;
        }

        /// <summary>
        /// Calculates the base's oxygen depletion rate based on its cells
        /// </summary>
        public static float GetDepletionRate(Base _base)
        {
            // Count cells per module type
            var cellTypeCounts = new Dictionary<Base.CellType, int>();
            foreach (Int3 cellPos in _base.AllCells)
            {
                var cell = _base.GetCell(cellPos);
                if (HabitableCellDepletionRates.ContainsKey(cell))
                {
                    if (!cellTypeCounts.ContainsKey(cell))
                        cellTypeCounts[cell] = 0;
                    cellTypeCounts[cell]++;
                    Plugin.Log.LogDebug($"Base Oxygen Depletion - Found cell of type {cell} at position {cellPos}, total count for this cell type is now {cellTypeCounts[cell]}");
                }
                else
                {
                    Plugin.Log.LogDebug($"Base Oxygen Depletion - Found cell of type {cell} at position {cellPos}, which does not contribute to oxygen depletion");
                }
            }

            float totalDepletion = 0f;
            foreach (var kvp in cellTypeCounts)
            {
                var cellType = kvp.Key;
                var cellCount = kvp.Value;
                var moduleDepletion = HabitableCellDepletionRates[cellType];
                float perCellDepletion = moduleDepletion / cellCount;
                for (int i = 0; i < cellCount; i++) totalDepletion += perCellDepletion;
                Plugin.Log.LogDebug($"Base Oxygen Depletion - Cell Type: {cellType}, Cell Count: {cellCount}, Module Depletion: {moduleDepletion}, Per Cell Depletion: {perCellDepletion}, Total Depletion So Far: {totalDepletion}");
            }
            Plugin.Log.LogInfo($"Base Total Oxygen Depletion Rate: {totalDepletion}");
            return totalDepletion;
        }

        public static float GetOxygenToRemove(float netRate, float oxygenAvailable)
        {
            // Any oxygen rate at or above 0 means the base meets or exceeds player needs, so remove nothing
            // This method shouldn't ever be called with a positive production rate, but this is a safeguard against that and against any weird edge cases where the rate might be slightly positive due to floating point imprecision
            if (netRate >= 0f) return 0f;

            // For negative rates, the max that should ever be removed is `BaseMaxOxygenDepletionRate`
            // But we also can't remove more oxygen than is actually available, so take the minimum of those two values and the absolute value of the net production rate
            return Mathf.Min(-netRate, oxygenAvailable, BaseMaxOxygenDepletionRate);
        }
    }
}