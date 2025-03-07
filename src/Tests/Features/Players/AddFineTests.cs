﻿using DynamoLeagueBlazor.Client.Features.Players;
using DynamoLeagueBlazor.Shared.Features.Players;
using DynamoLeagueBlazor.Shared.Utilities;

namespace DynamoLeagueBlazor.Tests.Features.Fines;

public class AddFineServerTests : IntegrationTestBase
{
    private static AddFineRequest CreateFakeValidRequest()
    {
        var faker = new AutoFaker<AddFineRequest>()
            .RuleFor(f => f.PlayerId, 1);

        return faker.Generate();
    }

    [Fact]
    public async Task GivenUnauthenticatedUser_ThenDoesNotAllowAccess()
    {
        var application = GetUnauthenticatedApplication();

        var client = application.CreateClient();

        var stubRequest = CreateFakeValidRequest();
        var response = await client.PostAsJsonAsync(AddFineRouteFactory.Uri, stubRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenAnyAuthenticatedUser_WhenAValidFine_ThenSavesIt()
    {
        var application = GetUserAuthenticatedApplication();

        var stubTeam = CreateFakeTeam();
        await AddAsync(stubTeam);

        var mockPlayer = CreateFakePlayer();
        mockPlayer.TeamId = stubTeam.Id;
        await AddAsync(mockPlayer);

        var stubRequest = CreateFakeValidRequest();
        stubRequest.PlayerId = mockPlayer.Id;

        var client = application.CreateClient();

        var response = await client.PostAsJsonAsync(AddFineRouteFactory.Uri, stubRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var fine = await FirstOrDefaultAsync<Fine>();
        fine.Should().NotBeNull();
        fine!.PlayerId.Should().Be(stubRequest.PlayerId);
        fine.Status.Should().BeFalse();
        fine.Reason.Should().Be(stubRequest.FineReason);
        fine.Amount.Should().Be(FineUtilities.CalculateFineAmount(mockPlayer.ContractValue));
    }
}

public class AddFineUITests : UITestBase
{
    [Fact]
    public async Task GivenAnInvalidForm_ThenDoesNotSubmit()
    {
        var result = new FineDetailResult { PlayerId = int.MaxValue, ContractValue = RandomString, FineAmount = RandomString };
        GetHttpHandler.When(HttpMethod.Get)
            .RespondsWithJson(result)
            .Verifiable();

        GetHttpHandler.When(HttpMethod.Post, AddFineRouteFactory.Uri)
            .Verifiable();

        var cut = await RenderMudDialogAsync<AddFine>();

        var submitButton = cut.Find("button");
        submitButton.Click();

        GetHttpHandler.Verify((request) => request.Method(HttpMethod.Get.Method), IsSent.Once);
        GetHttpHandler.VerifyNoOtherRequests();
    }

    [Fact]
    public async Task GivenAValidForm_WhenSubmitIsClicked_ThenSavesTheForm()
    {
        var result = new FineDetailResult { PlayerId = int.MaxValue, ContractValue = RandomString, FineAmount = RandomString };
        GetHttpHandler.When(HttpMethod.Get)
            .RespondsWithJson(result)
            .Verifiable();

        GetHttpHandler.When(HttpMethod.Post, AddFineRouteFactory.Uri)
            .Respond(message => Task.FromResult(message.CreateResponse(HttpStatusCode.OK)))
            .Verifiable();

        var cut = await RenderMudDialogAsync<AddFine>(new DialogParameters
        {
            { nameof(AddFine.PlayerId), int.MaxValue }
        });

        // Fill the form and click submit.
        var fineReason = cut.Find($"#{nameof(AddFineRequest.FineReason)}");
        fineReason.Change(RandomString);

        var submitButton = cut.Find("button");
        submitButton.Click();

        MockSnackbar.Verify(s => s.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>(), It.IsAny<string>()));
        GetHttpHandler.Verify();
    }
}

public class AddFineRequestValidatorTests : IntegrationTestBase
{
    private readonly AddFineRequestValidator _validator = null!;

    public AddFineRequestValidatorTests()
    {
        _validator = GetRequiredService<AddFineRequestValidator>();

    }

    [Theory]
    [InlineData(0, "Test", false)]
    [InlineData(1, "", false)]
    [InlineData(1, null, false)]
    [InlineData(1, "Test", true)]
    public void GivenDifferentRequests_ThenReturnsExpectedResult(int playerId, string reason, bool expectedResult)
    {
        var request = new AddFineRequest { PlayerId = playerId, FineReason = reason };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedResult);
    }
}

public class FineDetailRequestValidatorTests : IntegrationTestBase
{
    private readonly FineDetailRequestValidator _validator = null!;

    public FineDetailRequestValidatorTests()
    {
        _validator = GetRequiredService<FineDetailRequestValidator>();
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void GivenDifferentRequests_ThenReturnsExpectedResult(int playerId, bool expectedResult)
    {
        var request = new FineDetailRequest { PlayerId = playerId };

        var result = _validator.Validate(request);

        result.IsValid.Should().Be(expectedResult);
    }
}