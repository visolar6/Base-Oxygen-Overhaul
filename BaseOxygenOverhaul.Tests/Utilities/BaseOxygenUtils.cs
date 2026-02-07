using BaseOxygenOverhaul.Utilities;
using NUnit.Framework;

namespace BaseOxygenOverhaul.Tests.Utilities
{
    public static class BaseOxygenUtils_Tests
    {
        [TestFixture]
        public static class GetOxygenToRemove
        {
            [Test]
            public static void ReturnsZeroWhenPositiveNetProductionRate()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(1f, 50f);
                Assert.That(result, Is.EqualTo(0f));
            }

            [Test]
            public static void ReturnsZeroWhenZeroNetProductionRate()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(0f, 50f);
                Assert.That(result, Is.EqualTo(0f));
            }

            [Test]
            public static void ReturnsCorrectAmountWhenNegativeNetProductionRate()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(-1.5f, 50f);
                Assert.That(result, Is.EqualTo(1.5f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanAvailableOxygen()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(-2f, 1f);
                Assert.That(result, Is.EqualTo(1f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanMaxDepletion()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(-10f, 50f);
                Assert.That(result, Is.EqualTo(3f));
            }

            [Test]
            public static void DoesNotRemoveMoreThanMaxDepletionOrAvailableOxygen()
            {
                var result = BaseOxygenUtils.GetOxygenToRemove(-10f, 2f);
                Assert.That(result, Is.EqualTo(2f));
            }
        }
    }
}