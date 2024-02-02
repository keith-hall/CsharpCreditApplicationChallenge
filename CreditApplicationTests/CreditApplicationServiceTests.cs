namespace CreditApplicationTests;

using CreditApplication.Config;
using CreditApplication.Models;
using CreditApplication.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

public class CreditApplicationServiceTests
{
    private readonly ICreditApplicationService _service;

    public CreditApplicationServiceTests()
    {
        var config = new CreditApplicationServiceOptions();
        config.MinimumAppliedAmount = 2000;
        config.MaximumAppliedAmount = 69000;
        config.MinimumTerm = 1;
        config.InterestRateBands = new Dictionary<string, decimal>()
        {
            { "20000", 3 },
            { "39000", 4 },
            { "40000", 4 },
            { "59000", 5 },
            { "60000", 5 },
            { "-1", 6 },
        };

        _service = new CreditApplicationService(NullLogger<CreditApplicationService>.Instance, Options.Create(config));
    }

    [Fact]
    public void TestArgumentExceptionThrownWhenInvalidRequest()
    {
        // arrange
        var request = new CreditApplicationRequest(
            CreditAmount: -1,
            TermMonths: 12,
            PreexistingCreditAmount: 0);

        // act & assert
        Assert.Throws<ArgumentException>(() => _service.MakeDecision(request));
    }

    [Fact]
    public void TestDecisionIsNo_When_AppliedAmountLessThan2000()
    {
        // arrange
        var request = new CreditApplicationRequest(
            CreditAmount: 1000,
            TermMonths: 12,
            PreexistingCreditAmount: 0);

        // act
        var result = _service.MakeDecision(request);

        // assert
        Assert.Equal(CreditApplicationDecision.No, result.Decision);
        Assert.Null(result.InterestRate);
    }

    [Theory]
    [InlineData(2000)]
    [InlineData(4500)]
    [InlineData(68999)]
    public void TestDecisionIsYes_When_AppliedBetween2000And69000(int amount)
    {
        // arrange
        var request = new CreditApplicationRequest(
            CreditAmount: amount,
            TermMonths: 12,
            PreexistingCreditAmount: 0);

        // act
        var result = _service.MakeDecision(request);

        // assert
        Assert.Equal(CreditApplicationDecision.Yes, result.Decision);
        Assert.NotNull(result.InterestRate);
    }

    [Theory]
    [InlineData(2000, 12, 0, 3)]
    [InlineData(2000, 12, 18000, 4)]
    [InlineData(40000, 1, 10000, 5)]
    [InlineData(60000, 1, 0, 6)]
    [InlineData(60001, 1, 0, 6)]
    public void TestInterestMatchesExpectation(int creditAmount, int term, int preexistingCreditAmount, int expectedInterestRate)
    {
        // arrange
        var request = new CreditApplicationRequest(
            CreditAmount: creditAmount,
            TermMonths: term,
            PreexistingCreditAmount: preexistingCreditAmount);

        // act
        var result = _service.MakeDecision(request);

        // assert
        Assert.Equal(CreditApplicationDecision.Yes, result.Decision);
        Assert.Equal(expectedInterestRate, result.InterestRate);
    }
}
