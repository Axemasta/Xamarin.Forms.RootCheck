using Moq;
using NUnit.Framework;
using RootCheck.Core;
using RootCheck.Maui;

namespace RootCheck.UnitTests
{
    public class RootCheckerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsDeviceRooted_PlatformNotSupported_Should_ReturnFalse()
        {
            var rooted = RootChecker.IsDeviceRooted;

            Assert.False(rooted, "Root check is not supported by platform, it should not be able to detect a root");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetInstance_Should_SetCustomInstance(bool rootedStatus)
        {
            // Arrange
            var mockChecker = new Mock<IChecker>();

            mockChecker.Setup(m => m.IsDeviceRooted())
                .Returns(rootedStatus);

            RootChecker.SetInstance(mockChecker.Object);

            // Act
            var isRooted = RootChecker.IsDeviceRooted;

            // Assert
            Assert.AreEqual(rootedStatus, isRooted);
        }
    }
}