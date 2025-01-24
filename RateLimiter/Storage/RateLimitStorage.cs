using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter.Storage
{
    /// <summary>
    /// the Rate Limit Storage
    /// </summary>
    public class RateLimitStorage
    {
        /// <summary>
        /// The request storage
        /// </summary>
        private readonly ConcurrentDictionary<string, List<DateTime>> _requestStorage;

        // Private static field to hold the singleton instance
        private static readonly Lazy<RateLimitStorage> _instance = new Lazy<RateLimitStorage>(() => new RateLimitStorage());

        /// <summary>
        /// Private constructor to prevent instantiation from outside
        /// </summary>
        private RateLimitStorage()
        {
            _requestStorage = new ConcurrentDictionary<string, List<DateTime>>();
        }

        /// <summary>
        /// Public static property to access the singleton instance
        /// </summary>
        public static RateLimitStorage Instance => _instance.Value;

        /// <summary>
        /// Add a request timestamp for a given access token
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="timestamp">The timestamp.</param>
        public void AddRequest(string accessToken, DateTime timestamp)
        {
            if (!_requestStorage.ContainsKey(accessToken))
            {
                _requestStorage[accessToken] = new List<DateTime>();
            }
            _requestStorage[accessToken].Add(timestamp);
        }

        /// <summary>
        /// Get the list of requests for a given access token        
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public List<DateTime> GetRequests(string accessToken)
        {
            return _requestStorage.ContainsKey(accessToken) ? _requestStorage[accessToken] : new List<DateTime>();
        }
    }
}
