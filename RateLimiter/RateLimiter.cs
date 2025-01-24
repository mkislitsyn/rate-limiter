using RateLimiter.Interfaces; 
using System.Collections.Generic;

namespace RateLimiter
{
    /// <summary>
    /// The Rate Limiter
    /// </summary>
    public class RateLimiter
    {
        /// <summary>
        /// The resource rules
        /// </summary>
        private readonly Dictionary<string, List<IRateLimitRule>> _resourceRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiter"/> class.
        /// </summary>
        public RateLimiter()
        {
            _resourceRules = new Dictionary<string, List<IRateLimitRule>>();
        }

        /// <summary>
        /// Adds the rule.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="rule">The rule.</param>
        public void AddRule(string resourceName, IRateLimitRule rule)
        {
            if (!_resourceRules.ContainsKey(resourceName))
            {
                _resourceRules[resourceName] = new List<IRateLimitRule>();
            }
            _resourceRules[resourceName].Add(rule);
        }

        /// <summary>
        /// Determines whether [is request allowed from rule] [the specified resource name].
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>
        ///   <c>true</c> if [is request allowed from rule] [the specified resource name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRequestAllowedFromRule(string resourceName, string accessToken)
        {
            if (!_resourceRules.ContainsKey(resourceName))
            {
                return true;
            }

            var rules = _resourceRules[resourceName];

            foreach (var rule in rules)
            {
                if (!rule.IsRequestAllowed(accessToken))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
