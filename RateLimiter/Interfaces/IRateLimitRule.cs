namespace RateLimiter.Interfaces
{
    /// <summary> 
    ///     Rate Limit Rule interface
    /// </summary>
    public interface IRateLimitRule
    {
        /// <summary>
        /// Determines whether [is request allowed] [the specified access token].
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>
        ///   <c>true</c> if [is request allowed] [the specified access token]; otherwise, <c>false</c>.
        /// </returns>
        bool IsRequestAllowed(string accessToken);
    }
}