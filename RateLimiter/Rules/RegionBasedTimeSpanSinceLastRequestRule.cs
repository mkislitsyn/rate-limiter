using RateLimiter.Interfaces;

namespace RateLimiter.Rules
{
    /// <summary>
    /// The Region Based Time Span Since Last Request Rule
    /// </summary>
    /// <seealso cref="RateLimiter.Interfaces.IRateLimitRule" />
    public class RegionBasedTimeSpanSinceLastRequestRule : IRateLimitRule
    {
        /// <summary>
        /// The timespan since last request rule
        /// </summary>
        private readonly TimespanSinceLastRequestRule _timespanSinceLastRequestRule;

        /// <summary>
        /// The region based request storage
        /// </summary>
        private readonly IRegionBasedRequestStorage _regionBasedRequestStorage;

        /// <summary>
        /// The region
        /// </summary>
        private readonly string _region = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionBasedTimeSpanSinceLastRequestRule"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="timespanSinceLastRequestRule">The timespan since last request rule.</param>
        public RegionBasedTimeSpanSinceLastRequestRule(string region, TimespanSinceLastRequestRule timespanSinceLastRequestRule, IRegionBasedRequestStorage regionBasedRequestStorage)
        {
            _timespanSinceLastRequestRule = timespanSinceLastRequestRule;
            _regionBasedRequestStorage = regionBasedRequestStorage;
            _region = region;
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
            var region = _regionBasedRequestStorage.GetClientRegionByAccessToken(accessToken);

            if (region != null && region == _region)
            {
                return _timespanSinceLastRequestRule.IsRequestAllowed(accessToken);
            }

            return false;
        }
    }
}
