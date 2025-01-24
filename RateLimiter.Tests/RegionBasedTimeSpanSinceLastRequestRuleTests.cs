using Moq;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System;

namespace RateLimiter.Tests
{
    /// <summary>
    /// The Region Based Time Span Since Last Request Rule Tests
    /// </summary>
    [TestFixture]
    public class RegionBasedTimeSpanSinceLastRequestRuleTests
    {
        /// <summary>
        /// The mock region based request storage
        /// </summary>
        private Mock<IRegionBasedRequestStorage> _mockRegionBasedRequestStorage;

        /// <summary>
        /// The rule
        /// </summary>
        private RegionBasedTimeSpanSinceLastRequestRule _rule;

        /// <summary>
        /// The access token
        /// </summary>
        private string _accessToken;

        /// <summary>
        /// The Timespan Since Last Request Rule
        /// </summary>
        private TimespanSinceLastRequestRule _timespanSinceLastRequestRule;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _timespanSinceLastRequestRule = new TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1));

            _mockRegionBasedRequestStorage = new Mock<IRegionBasedRequestStorage>();

            _rule = new RegionBasedTimeSpanSinceLastRequestRule("EU", _timespanSinceLastRequestRule, _mockRegionBasedRequestStorage.Object);
            _accessToken = "access_user_Token2";

            _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(_accessToken))
                .Returns("US");
        }

        /// <summary>
        /// Tests the request allowed when region matches.
        /// </summary>
        [Test]
        public void Test_RequestAllowed_WhenRegionMatches()
        {
            // Arrange
            _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
                .Returns("EU");

            // Act
            var result = _rule.IsRequestAllowed(_accessToken);

            // Assert
            Assert.IsTrue(result, "Request allowed when region matches.");
            _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_accessToken), Times.Once);
        }

        /// <summary>
        /// Tests the request not allowed when region does not match.
        /// </summary>
        [Test]
        public void Test_RequestNotAllowed_WhenRegionDoesNotMatch()
        {
            // Arrange
            _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
                .Returns("CA");

            // Act
            var result = _rule.IsRequestAllowed(_accessToken);

            // Assert
            Assert.IsFalse(result, "Request denied when region does not match.");
            _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_accessToken), Times.Once);
        }
    }
}