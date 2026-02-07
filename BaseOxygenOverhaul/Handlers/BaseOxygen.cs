using System.Collections.Generic;
using UnityEngine;
using BaseOxygenOverhaul.Mono.OxygenGenerator;
using BaseOxygenOverhaul.Types;
using BaseOxygenOverhaul.Utilities;

namespace BaseOxygenOverhaul.Handlers
{
    public static class BaseOxygen
    {
        /// <summary>
        /// Oxygen production rate for Small Oxygen Generator
        /// </summary>
        public const float ProductionRateSmallOxygenGenerator = 0.5f;
        /// <summary>
        /// Oxygen production rate for Large Oxygen Generator
        /// </summary>
        public const float ProductionRateLargeOxygenGenerator = 4.0f;

        /// <summary>
        /// Interval in seconds for caching the base's oxygen depletion rate, to avoid expensive calculations every frame.
        /// The depletion rate is based on the base's cells and doesn't need to be updated every frame, so we can cache it and only recalculate at a set interval (e.g. every 15 seconds) or when the base reference changes.
        /// </summary>
        public const float DepletionRateCacheInterval = 15f;
        /// <summary>
        /// Interval in seconds for caching the base's oxygen production rate, to avoid expensive calculations every frame.
        /// The production rate is based on the base's constructed oxygen generators and doesn't need to be updated every frame, so we can cache it and only recalculate at a set interval (e.g. every 5 seconds) or when the base reference changes.
        /// </summary>
        public const float ProductionRateCacheInterval = 5f;
        /// <summary>
        /// Interval in seconds for caching the base reference, to avoid expensive calculations every frame.
        /// The base reference is used for oxygen calculations and doesn't need to be updated every frame, so we can cache it and only recalculate at a set interval (e.g. every 5 seconds).
        /// </summary>
        public const float BaseReferenceCacheInterval = 5f;

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

                    var baseOxygenProductionRate = GetProductionRate(_base);
                    var baseOxygenDepletionRate = GetDepletionRate(_base);
                    var baseOxygenNetProductionRate = baseOxygenProductionRate - baseOxygenDepletionRate;
                    if (baseOxygenNetProductionRate >= 0f)
                    {
                        // As long as net production rate is above 0f, we can allow oxygen to be added as normal (base produces more than it consumes)
                        baseOxygenDepleteTimer = 0f;
                        return true;
                    }

                    // For values between 0 and 1, we deplete the player's oxygen at the appropriate rate, but still block oxygen from being added (base produces some oxygen, but not enough to sustain the player)
                    baseOxygenDepleteTimer += Time.deltaTime;
                    if (baseOxygenDepleteTimer >= 3f)
                    {
                        Plugin.Log.LogInfo($"Depleting player oxygen by {BaseOxygenUtils.GetOxygenToRemove(baseOxygenNetProductionRate, oxygen.oxygenAvailable)} based on base net production rate of {baseOxygenNetProductionRate}");
                        oxygen.oxygenAvailable -= BaseOxygenUtils.GetOxygenToRemove(baseOxygenNetProductionRate, oxygen.oxygenAvailable);
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
                    {
                        switch (oxygenGeneratorManager.type)
                        {
                            case OxygenGeneratorSize.Small:
                                rate += ProductionRateSmallOxygenGenerator;
                                break;
                            case OxygenGeneratorSize.Large:
                                rate += ProductionRateLargeOxygenGenerator;
                                break;
                        }
                    }
                }
            }
            return rate;
        }

        /// <summary>
        /// Calculates the base's oxygen depletion rate based on its cells
        /// </summary>
        public static float GetDepletionRate(Base _base)
        {
            float totalDepletion = 0f;
            for (var i = 0; i < _base.cells.Length; i++)
            {
                var cell = _base.GetCell(i);
                if (HabitableCellDepletionRates.TryGetValue(cell, out float cellDepletionRate))
                    totalDepletion += cellDepletionRate;

            }
            return totalDepletion;
        }
    }
}