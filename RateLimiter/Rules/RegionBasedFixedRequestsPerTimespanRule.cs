using RateLimiter.Interfaces; 

namespace RateLimiter.Rules
{
    /// <summary>
    /// The Region Based Fixed Requests Per TimespanRule
    /// </summary>
    /// <seealso cref="RateLimiter.Interfaces.IRateLimitRule" />
    public class RegionBasedFixedRequestsPerTimespanRule : IRateLimitRule
    {
        /// <summary>
        /// The fixed requests per timespan rule
        /// </summary>
        private readonly FixedRequestsPerTimespanRule _fixedRequestsPerTimespanRule;

        /// <summary>
        /// The region based request storage
        /// </summary>
        private readonly IRegionBasedRequestStorage _regionBasedRequestStorage;

        /// <summary>
        /// The region
        /// </summary>
        private readonly string _region = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionBasedFixedRequestsPerTimespanRule"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="fixedRequestsPerTimespanRule">The fixed requests per timespan rule.</param>
        public RegionBasedFixedRequestsPerTimespanRule(string region, FixedRequestsPerTimespanRule fixedRequestsPerTimespanRule, IRegionBasedRequestStorage  regionBasedRequestStorage)
        {
            _fixedRequestsPerTimespanRule = fixedRequestsPerTimespanRule;
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
                return _fixedRequestsPerTimespanRule.IsRequestAllowed(accessToken);
            }

            return false;
        }
    }
}
