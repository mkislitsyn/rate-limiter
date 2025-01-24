using RateLimiter.Interfaces;
using RateLimiter.Storage;
using System;
using System.Linq;

namespace RateLimiter.Rules
{
    /// <summary>
    /// The Timespan Since Last Request Rule
    /// </summary>
    /// <seealso cref="RateLimiter.Interfaces.IRateLimitRule" />
    public class TimespanSinceLastRequestRule : IRateLimitRule
    {
        /// <summary>
        /// The minimum interval
        /// </summary>
        private readonly TimeSpan _minInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimespanSinceLastRequestRule"/> class.
        /// </summary>
        /// <param name="minInterval">The minimum interval.</param>
        public TimespanSinceLastRequestRule(TimeSpan minInterval)
        {
            _minInterval = minInterval;
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
            var lastRequest = RateLimitStorage.Instance.GetRequests(accessToken).LastOrDefault();

            if (currentTime - lastRequest < _minInterval) // if second Request less than _minInterval
            {
                return false;
            }

            RateLimitStorage.Instance.AddRequest(accessToken, currentTime); // Add new record in to the storage
            return true;
        }
    }
}