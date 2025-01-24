using RateLimiter.Interfaces;
using System.Collections.Generic;

namespace RateLimiter.Storage
{
    /// <summary>
    /// The Region Based Request Storage
    /// </summary>
    /// <seealso cref="RateLimiter.Interfaces.IRegionBasedRequestStorage" />
    public class RegionBasedRequestStorage : IRegionBasedRequestStorage
    {
        /// <summary>
        /// The strorage
        /// </summary>
        private readonly Dictionary<string, string> _strorage = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionBasedRequestStorage"/> class.
        /// </summary>
        public RegionBasedRequestStorage()
        {
            _strorage["access_user_Token1"] = "US";
            _strorage["access_user_Token2"] = "EU";
            _strorage["access_user_Token3"] = "UK";
        }

        /// <summary>
        /// Gets the client region by access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public string GetClientRegionByAccessToken(string accessToken)
        {
            return _strorage.TryGetValue(accessToken, out var region) ? region : "N/A";
        }
    }
}