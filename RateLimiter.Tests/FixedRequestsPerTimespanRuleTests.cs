using NUnit.Framework;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    /// <summary>
    /// The Fixed Requests Per Timespan Rule Tests
    /// </summary>
    [TestFixture]
    public class FixedRequestsPerTimespanRuleTests
    { 
        /// <summary>
        /// The rule
        /// </summary>
        private FixedRequestsPerTimespanRule _rule;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        { 
            _rule = new FixedRequestsPerTimespanRule(10, TimeSpan.FromSeconds(3)); // 10 requests per 10 seconds
        }

        /// <summary>
        /// Tests the request allowed when requests are under limit.
        /// </summary>
        [Test]
        public void Test_RequestAllowed_WhenRequestsAreUnderLimit()
        {
            // Arrange
            var accessToken = "accessToken_user1";
            var now = DateTime.UtcNow;

            for (int i = 1; i <= 10; i++)
            {
                // Act
                var result = _rule.IsRequestAllowed(accessToken);
                // Assert
                Assert.IsTrue(result, $"Request number {i} allowed within the timespan for current accessToken : {accessToken}");
            }
        }

        /// <summary>
        /// Tests the request not allowed when requests exceed limit.
        /// </summary>
        [Test]
        public void Test_RequestNotAllowed_WhenRequestsExceedLimit()
        {
            // Arrange
            var accessToken = "accessToken_user2";
            var now = DateTime.UtcNow;

            for (int i = 1; i <= 10; i++)
            {
                // Act
                _rule.IsRequestAllowed(accessToken);                
            }

            var result = _rule.IsRequestAllowed(accessToken); //  request exceed limit

            // Assert
            Assert.IsFalse(result, $"Request number {11} denied as it exceeds the limit for current accessToken : {accessToken}");
        }

        /// <summary>
        /// Tests the request allowed when request is within new timespan.
        /// </summary>
        [Test]
        public void Test_RequestAllowed_WhenRequestIsWithinNewTimespan()
        {
            // Arrange
            var accessToken = "accessToken_user3";

            // Act
            for (int i = 1; i <= 10; i++)
            {
                // Act
                var result = _rule.IsRequestAllowed(accessToken);
            }

            Thread.Sleep(3000);

            var resultAfterTimespan = _rule.IsRequestAllowed(accessToken);

            // Assert
            Assert.IsTrue(resultAfterTimespan, $"Request number {11}  allowed within NEW timespan for current accessToken : {accessToken}");
        }
    }
}
