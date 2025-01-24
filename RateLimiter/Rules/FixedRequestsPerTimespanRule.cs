using RateLimiter.Interfaces;
using RateLimiter.Storage;
using System;
using System.Linq;

namespace RateLimiter.Rules
{
    /// <summary>
    /// The Fixed Requests Per Timespan Rule
    /// </summary>
    /// <seealso cref="RateLimiter.Interfaces.IRateLimitRule" />
    public class FixedRequestsPerTimespanRule : IRateLimitRule
    {
        /// <summary>
        /// The maximum requests
        /// </summary>
        private readonly int _maxRequests;

        /// <summary>
        /// The time span
        /// </summary>
        private readonly TimeSpan _timeSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRequestsPerTimespanRule"/> class.
        /// </summary>
        /// <param name="maxRequests">The maximum requests.</param>
        /// <param name="timeSpan">The time span.</param>
        public FixedRequestsPerTimespanRule(int maxRequests, TimeSpan timeSpan)
        {
            _maxRequests = maxRequests;
            _timeSpan = timeSpan;
        }

        /// <summary>
        /// Determines whether [is request allowed] [the specified access token].
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>
        ///   <c>true</c> if [is request allowed] [the specified access token]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestAllowed(string accessToken)
        {
            var currentTime = DateTime.UtcNow;
            var requests = RateLimitStorage.Instance.GetRequests(accessToken);
            var totalRequests = requests.Where(x => x >= currentTime - _timeSpan).ToList(); // gat all requests for control time

            if (totalRequests.Count >= _maxRequests)
            {
                return false;
            }

            RateLimitStorage.Instance.AddRequest(accessToken, currentTime);
            return true;
        }
    }
}
