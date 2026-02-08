using NUnit.Framework;
using BaseOxygenOverhaul.Utilities;

namespace BaseOxygenOverhaul.Tests.Handlers
{
    public static class BaseOxygenHandler_Tests
    {
        [TestFixture]
        public static class IsHabitableCellType
        {
            [Test]
            public static void ReturnsTrueForHabitableCellTypes()
            {
                var habitableCellTypes = new[] {
                    Base.CellType.Connector,
                    Base.CellType.Corridor,
                    Base.CellType.LargeRoom,
                    Base.CellType.LargeRoomRotated,
                    Base.CellType.MapRoom,
                    Base.CellType.MapRoomRotated,
                    Base.CellType.Moonpool,
                    Base.CellType.MoonpoolRotated,
                    Base.CellType.Observatory,
                    Base.CellType.Room
                };
                foreach (var cellType in habitableCellTypes)
                {
                    Assert.That(BaseOxygenHandler.IsHabitableCellType(cellType), Is.True, $"Expected {cellType} to be a habitable cell type");
                }
            }

            [Test]
            public static void ReturnsFalseForNonHabitableCellTypes()
            {
                var nonHabitableCellTypes = new[] {
                    Base.CellType.Count,
                    Base.CellType.Empty,
                    Base.CellType.Foundation,
                    Base.CellType.WallFoundationE,
                    Base.CellType.WallFoundationN,
                    Base.CellType.WallFoundationW,
                    Base.CellType.WallFoundationS,
                    Base.CellType.OccupiedByOtherCell,
                    Base.CellType.RechargePlatform
                };
                foreach (var cellType in nonHabitableCellTypes)
                {
                    Assert.That(BaseOxygenHandler.IsHabitableCellType(cellType), Is.False, $"Expected {cellType} to not be a habitable cell type");
                }
            }
        }

        [TestFixture]
        public static class GetOxygenToRemove
        {
            [Test]
            public static void ReturnsZeroWhenPositiveNetProductionRate()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(1f, 50f);
                Assert.That(result, Is.EqualTo(0f));
            }

            [Test]
            public static void ReturnsZeroWhenZeroNetProductionRate()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(0f, 50f);
                Assert.That(result, Is.EqualTo(0f));
            }

            [Test]
            public static void ReturnsCorrectAmountWhenNegativeNetProductionRate()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(-1.5f, 50f);
                Assert.That(result, Is.EqualTo(1.5f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanAvailableOxygen()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(-2f, 1f);
                Assert.That(result, Is.EqualTo(1f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanMaxDepletion()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(-10f, 50f);
                Assert.That(result, Is.EqualTo(3f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanMaxDepletionOrAvailableOxygen()
            {
                var result = BaseOxygenHandler.GetOxygenToRemove(-10f, 2f);
                Assert.That(result, Is.EqualTo(2f));
            }
        }
    }
}