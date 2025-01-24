namespace RateLimiter.Interfaces
{
    /// <summary>
    /// The IRegionBasedRequestStorage interface
    /// </summary>
    public interface IRegionBasedRequestStorage
    {
        /// <summary>
        /// Gets the client region by access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        string GetClientRegionByAccessToken(string accessToken);
    }
}
