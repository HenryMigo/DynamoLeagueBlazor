﻿using DynamoLeagueBlazor.Server.Features.Admin.Shared;
using Microsoft.Extensions.Logging;

namespace DynamoLeagueBlazor.Tests.Features.Admin.Shared;

public class PlayerHeadshotServiceTests : IntegrationTestBase
{
    [Fact]
    public async Task WhenAPlayerIsMatchedOnNameAndPosition_ThenThePlayerHeadshotIsUpdated()
    {
        var mockHttpHandler = new MockHttpHandler();
        var httpClient = new HttpClient(mockHttpHandler);

        var stubPlayerDataResult = new AutoFaker<PlayerDataResult>().Generate();
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayersUri)
            .RespondsWithJson(stubPlayerDataResult);

        var matchingPlayer = stubPlayerDataResult.Data.Players.First();

        var stubPlayerMetricDataResult = new AutoFaker<PlayerMetricDataResult>().Generate();
        stubPlayerMetricDataResult.Data.Player.PlayerId = matchingPlayer.PlayerId;
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayerUri(matchingPlayer.PlayerId))
            .RespondsWithJson(stubPlayerMetricDataResult);

        var sut = new PlayerHeadshotService(httpClient, Mock.Of<ILogger<PlayerHeadshotService>>());

        var headshot = await sut.FindPlayerHeadshotUrlAsync(matchingPlayer.FullName, matchingPlayer.Position, CancellationToken.None);

        headshot.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task WhenAPlayerIsNotMatchedOnName_ThenReturnsNothing()
    {
        var mockHttpHandler = new MockHttpHandler();
        var httpClient = new HttpClient(mockHttpHandler);

        var stubPlayerDataResult = new AutoFaker<PlayerDataResult>().Generate();
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayersUri)
            .RespondsWithJson(stubPlayerDataResult);

        var matchingPlayer = stubPlayerDataResult.Data.Players.First();

        var stubPlayerMetricDataResult = new AutoFaker<PlayerMetricDataResult>().Generate();
        stubPlayerMetricDataResult.Data.Player.PlayerId = matchingPlayer.PlayerId;
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayerUri(matchingPlayer.PlayerId))
            .RespondsWithJson(stubPlayerMetricDataResult);

        var sut = new PlayerHeadshotService(httpClient, Mock.Of<ILogger<PlayerHeadshotService>>());

        var headshot = await sut.FindPlayerHeadshotUrlAsync(RandomString, matchingPlayer.Position, CancellationToken.None);

        headshot.Should().BeNull();
    }

    [Fact]
    public async Task WhenAPlayerIsNotMatchedOnPosition_ThenReturnsNothing()
    {
        var mockHttpHandler = new MockHttpHandler();
        var httpClient = new HttpClient(mockHttpHandler);

        var stubPlayerDataResult = new AutoFaker<PlayerDataResult>().Generate();
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayersUri)
            .RespondsWithJson(stubPlayerDataResult);

        var matchingPlayer = stubPlayerDataResult.Data.Players.First();

        var stubPlayerMetricDataResult = new AutoFaker<PlayerMetricDataResult>().Generate();
        stubPlayerMetricDataResult.Data.Player.PlayerId = matchingPlayer.PlayerId;
        mockHttpHandler.When(HttpMethod.Get, PlayerHeadshotRouteFactory.GetPlayerUri(matchingPlayer.PlayerId))
            .RespondsWithJson(stubPlayerMetricDataResult);

        var sut = new PlayerHeadshotService(httpClient, Mock.Of<ILogger<PlayerHeadshotService>>());

        var headshot = await sut.FindPlayerHeadshotUrlAsync(matchingPlayer.FullName, RandomString, CancellationToken.None);

        headshot.Should().BeNull();
    }
}
