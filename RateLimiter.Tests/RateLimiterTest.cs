using Moq;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests;

/// <summary>
/// The Rate Limiter Test
/// </summary>
[TestFixture]
public class RateLimiterTest
{
    /// <summary>
    /// The rate limiter
    /// </summary>
    private RateLimiter _rateLimiter;

    /// <summary>
    /// The mock region based request storage
    /// </summary>
    private Mock<IRegionBasedRequestStorage> _mockRegionBasedRequestStorage;

    /// <summary>
    /// The region basedaccess token
    /// </summary>
    private string _regionBasedaccessToken;

    /// <summary>
    /// Setups this instance.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _rateLimiter = new RateLimiter();
        _mockRegionBasedRequestStorage = new Mock<IRegionBasedRequestStorage>();
        _regionBasedaccessToken = "access_user_Token1";

        _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(_regionBasedaccessToken))
            .Returns("US");
    }

    /// <summary>
    /// Tests the is request allowed for request without rules.
    /// </summary>
    [Test]
    public void Test_IsRequestAllowed_ForRequestWithoutRules()
    {
        // Arrange
        var resource = "resource1";
        var accessToken = "access_user_123";

        // Act
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// Tests the is request allowed for fixed requests per timespan rule.
    /// </summary>
    [Test]
    public void Test_IsRequestAllowed_ForFixedRequestsPerTimespanRule()
    {
        // Arrange
        var resource = "resource2";
        var accessToken = "access_user_123";
        var rule = new FixedRequestsPerTimespanRule(5, TimeSpan.FromSeconds(10));
        _rateLimiter.AddRule(resource, rule);

        // Act
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        // Assert
        Assert.IsTrue(result, "Request allowed for request without rules");
    }

    /// <summary>
    /// Tests the request not allowed for fixed requests per timespan rule.
    /// </summary>
    [Test]
    public void Test_RequestNotAllowed_ForFixedRequestsPerTimespanRule()
    {
        // Arrange
        var resource = "resource2";
        var accessToken = "access_user_123";
        var rule = new FixedRequestsPerTimespanRule(1, TimeSpan.FromSeconds(10));
        _rateLimiter.AddRule(resource, rule);

        // Act
        _rateLimiter.IsRequestAllowedFromRule(resource, accessToken); // First request
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken); // Second request        

        // Assert
        Assert.IsFalse(result, "Request allowed for ForFixedRequestsPerTimespanRule");
    }

    /// <summary>
    /// Tests the is request allowed for timespan since last request rule.
    /// </summary>
    [Test]
    public void Test_IsRequestAllowed_ForTimespanSinceLastRequestRule()
    {
        // Arrange
        var resource = "resource3";
        var accessToken = "access_user_124";
        var rule = new TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1));
        _rateLimiter.AddRule(resource, rule);

        // Act
        var firstRequest = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        Thread.Sleep(1000);
        var secondRequest = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        // Assert
        Assert.IsTrue(firstRequest, "The first request allowed.");
        Assert.IsTrue(secondRequest, "The second request allowed as the interval is above the minimum.");
    }

    /// <summary>
    /// Tests the request not allowed for timespan since last request rule.
    /// </summary>
    [Test]
    public void Test_RequestNotAllowed_ForTimespanSinceLastRequestRule()
    {
        // Arrange
        var resource = "resource3";
        var accessToken = "access_user_125";
        var rule = new TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1));
        _rateLimiter.AddRule(resource, rule);

        // Act
        var firstRequest = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        var secondRequest = _rateLimiter.IsRequestAllowedFromRule(resource, accessToken);

        // Assert
        Assert.IsTrue(firstRequest, "The first request allowed.");
        Assert.IsFalse(secondRequest, "The second request denied as it doesn't meet the minimum interval.");
    }

    /// <summary>
    /// Tests the is request allowed region based fixed requests per timespan rule.
    /// </summary>
    [Test]
    public void Test_IsRequestAllowed_RegionBasedFixedRequestsPerTimespanRule()
    {
        // Arrange
        var resource = "resource4";

        var regionBasedRule = new RegionBasedFixedRequestsPerTimespanRule("US", new FixedRequestsPerTimespanRule(10, TimeSpan.FromSeconds(3)), _mockRegionBasedRequestStorage.Object);

        _rateLimiter.AddRule(resource, regionBasedRule);

        _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
            .Returns("US");

        // Act
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, _regionBasedaccessToken);

        // Assert
        Assert.IsTrue(result, "Request allowed when region matches.");
        _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_regionBasedaccessToken), Times.Once);
    }

    /// <summary>
    /// Tests the request not allowed region based fixed requests per timespan rule.
    /// </summary>
    [Test]
    public void Test_RequestNotAllowed_RegionBasedFixedRequestsPerTimespanRule()
    {
        // Arrange
        var resource = "resource4";

        var regionBasedRule = new RegionBasedFixedRequestsPerTimespanRule("US", new FixedRequestsPerTimespanRule(10, TimeSpan.FromSeconds(3)), _mockRegionBasedRequestStorage.Object);

        _rateLimiter.AddRule(resource, regionBasedRule);

        _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
              .Returns("CA");

        // Act
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, _regionBasedaccessToken);

        // Assert
        Assert.IsFalse(result, "Request denied when region does not match.");
        _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_regionBasedaccessToken), Times.Once);
    }

    /// <summary>
    /// Tests the is request allowed region based time span since last request rule.
    /// </summary>
    [Test]
    public void Test_IsRequestAllowed_RegionBasedTimeSpanSinceLastRequestRule()
    {
        // Arrange
        var resource = "resource5";

        var regionBasedRule = new RegionBasedTimeSpanSinceLastRequestRule("US", new  TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1)), _mockRegionBasedRequestStorage.Object);

        _rateLimiter.AddRule(resource, regionBasedRule);

        _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
            .Returns("US");

        // Act 
        Thread.Sleep(3000); // wait 3 seconds after RegionBasedFixedRequestsPerTimespanRule for current  _regionBasedaccessToken 
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, _regionBasedaccessToken);

        // Assert
        Assert.IsTrue(result, "Request allowed when region matches.");
        _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_regionBasedaccessToken), Times.Once);
    }

    [Test]
    public void Test_RequestNotAllowed_RegionBasedTimeSpanSinceLastRequestRule()
    {
        // Arrange
        var resource = "resource5";

        var regionBasedRule = new RegionBasedTimeSpanSinceLastRequestRule("US", new TimespanSinceLastRequestRule(TimeSpan.FromSeconds(1)), _mockRegionBasedRequestStorage.Object);

        _rateLimiter.AddRule(resource, regionBasedRule);

        _mockRegionBasedRequestStorage.Setup(r => r.GetClientRegionByAccessToken(It.IsAny<string>()))
              .Returns("CA");

        // Act
        var result = _rateLimiter.IsRequestAllowedFromRule(resource, _regionBasedaccessToken);

        // Assert
        Assert.IsFalse(result, "Request denied when region does not match.");
        _mockRegionBasedRequestStorage.Verify(r => r.GetClientRegionByAccessToken(_regionBasedaccessToken), Times.Once);
    }

}