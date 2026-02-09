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

        public static Dictionary<OxygenGeneratorSize, float> GeneratorProductionRates => new Dictionary<OxygenGeneratorSize, float>()
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

        // Timer to track oxygen depletion interval
        private static float baseOxygenDepleteTimer = 0f;

        /// <summary>
        /// Handles player oxygen add logic when inside a base, based on the base's net oxygen production rate.
        /// Returns true if oxygen should be added as normal, or false if oxygen addition should be blocked.
        /// </summary>
        /// <returns>A boolean indicating whether oxygen addition should proceed as normal</returns>
        public static bool OnPlayerOxygenAdd(ref Oxygen oxygen)
        {
            if (oxygen.isPlayer)
            {
                var player = Player.main;
                // Block oxygen if the player is in a base (not Cyclops, not outside)
                if (player != null && player.currentSub != null && player.currentSub.isBase)
                {
                    var _base = player.currentSub.GetComponent<Base>();
                    if (_base == null)
                    {
                        // Reset timer if we can't find the base for some reason
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    if (HasHabitableCellAboveWater(_base))
                    {
                        // If the base has any valid cell above water, it provides infinite oxygen, so allow oxygen to be added as normal
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    var baseOxygenNetRate = GetNetRate(_base);
                    if (baseOxygenNetRate >= 0f)
                    {
                        // As long as net production rate is above 0f, we can allow oxygen to be added as normal (base produces more than it consumes)
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    // For values between 0 and 1, we deplete the player's oxygen at the appropriate rate, but still block oxygen from being added (base produces some oxygen, but not enough to sustain the player)
                    baseOxygenDepleteTimer += Time.deltaTime;
                    if (baseOxygenDepleteTimer >= BaseOxygenDepletionInterval)
                    {
                        oxygen.oxygenAvailable -= GetOxygenToRemove(baseOxygenNetRate, oxygen.oxygenAvailable);
                        baseOxygenDepleteTimer = 0f;
                    }

                    return false; // Discontinue the AddOxygen method to block oxygen from being added, with the side effect of depleting the player's oxygen based on the base's net production rate
                }
                else
                {
                    // Reset timer if not in base
                    baseOxygenDepleteTimer = 0f;
                }
            }

            // Continue with normal oxygen addition if not in a base or if not the player, and reset timer if not in base
            return true;
        }

        public static bool IsHabitableCellType(Base.CellType cellType)
        {
            return HabitableCellDepletionRates.ContainsKey(cellType);
        }

        public static bool HasHabitableCellAboveWater(Base _base)
        {
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
            return baseOxygenProductionRate - baseOxygenDepletionRate;
        }

        /// <summary>
        /// Calculates the base's oxygen production rate based on its constructed oxygen generators
        /// </summary>
        public static float GetProductionRate(Base _base)
        {
            var oxygenGeneratorManagers = _base.gameObject.GetComponentsInChildren<OxygenGeneratorManager>();
            var rate = 0f;
            for (var i = 0; i < oxygenGeneratorManagers.Length; i++)
            {
                var oxygenGeneratorManager = oxygenGeneratorManagers[i];
                if (oxygenGeneratorManager != null && oxygenGeneratorManager.enabled)
                {
                    var constructable = oxygenGeneratorManager.GetComponentInParent<Constructable>();
                    if (constructable != null && constructable.constructed)
                        rate += GeneratorProductionRates[oxygenGeneratorManager.type];
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
            for (var i = 0; i < _base.cells.Length; i++)
            {
                var cell = _base.GetCell(i);
                if (HabitableCellDepletionRates.ContainsKey(cell))
                {
                    if (!cellTypeCounts.ContainsKey(cell))
                        cellTypeCounts[cell] = 0;
                    cellTypeCounts[cell]++;
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
            }
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