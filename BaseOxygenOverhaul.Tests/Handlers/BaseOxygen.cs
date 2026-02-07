using BaseOxygenOverhaul.Handlers;
using NUnit.Framework;

namespace BaseOxygenOverhaul.Tests.Handlers
{
    public static class BaseOxygen_Tests
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
                    Assert.That(BaseOxygen.IsHabitableCellType(cellType), Is.True, $"Expected {cellType} to be a habitable cell type");
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
                    Assert.That(BaseOxygen.IsHabitableCellType(cellType), Is.False, $"Expected {cellType} to not be a habitable cell type");
                }
            }
        }
    }
}