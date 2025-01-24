using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    /// <summary>
    /// The Timespan Since Last Request Rule Tests
    /// </summary>
    [TestFixture]
    public class TimespanSinceLastRequestRuleTests
    {
        /// <summary>
        /// The rule
        /// </summary>
        private TimespanSinceLastRequestRule _rule;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _rule = new TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Tests the request allowed when interval exceeds minimum.
        /// </summary>
        [Test]
        public void Test_RequestAllowed_WhenIntervalExceedsMinimum()
        {
            // Arrange
            var accessToken = "accessToken_user4";

            var firstRequest = _rule.IsRequestAllowed(accessToken);

            Thread.Sleep(1000);
            var secondRequest = _rule.IsRequestAllowed(accessToken);

            // Assert
            Assert.IsTrue(firstRequest, "The first request allowed.");
            Assert.IsTrue(secondRequest, "The second request allowed as the interval is above the minimum.");
        }

        /// <summary>
        /// Tests the request not allowed when interval is less than minimum.
        /// </summary>
        [Test]
        public void Test_RequestNotAllowed_WhenIntervalIsLessThanMinimum()
        {
            // Arrange
            var accessToken = "accessToken_user5";

            var firstRequest = _rule.IsRequestAllowed(accessToken);

            var secondRequest = _rule.IsRequestAllowed(accessToken);

            // Assert
            Assert.IsTrue(firstRequest, "The first request allowed.");
            Assert.IsFalse(secondRequest, "The second request denied as it doesn't meet the minimum interval.");
        }
    }
}
