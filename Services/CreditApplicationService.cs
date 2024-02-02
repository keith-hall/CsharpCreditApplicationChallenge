namespace CreditApplication.Services;
using CreditApplication.Config;
using CreditApplication.Models;
using Microsoft.Extensions.Options;

using System.Text.Json;


public class CreditApplicationService : ICreditApplicationService
{
    private readonly ILogger<CreditApplicationService> _logger;
    private readonly CreditApplicationServiceOptions _config;

    public CreditApplicationService(
        ILogger<CreditApplicationService> logger,
        IOptions<CreditApplicationServiceOptions> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public IEnumerable<string> GetValidationErrors(CreditApplicationRequest application)
    {
        if (application.CreditAmount <= 0) {
            yield return "credit amount invalid";
        }
        if (application.TermMonths <= 0) {
            yield return "term invalid";
        }
        if (application.PreexistingCreditAmount < 0) {
            yield return "preexisting credit amount invalid";
        }
    }

    public CreditApplicationResponse MakeDecision(CreditApplicationRequest application)
    {
        if (GetValidationErrors(application).Any()) {
            throw new ArgumentException(nameof(application));
        }

        if (IsNegativeDecision(application)) {
            return new CreditApplicationResponse(CreditApplicationDecision.No, null);
        }
        
        return new CreditApplicationResponse(
            CreditApplicationDecision.Yes,
            ComputeInterestRate(application)
        );
    }

    private bool IsNegativeDecision(CreditApplicationRequest application)
    {
        if (application.TermMonths < _config.MinimumTerm) {
            return true;
        }
        if (application.CreditAmount < _config.MinimumAppliedAmount) {
            return true;
        }
        if (application.CreditAmount > _config.MaximumAppliedAmount) {
            return true;
        }
        return false;
    }

    private decimal ComputeInterestRate(CreditApplicationRequest application)
    {
        // TODO: ideally the config would be parsed into a dictionary of:
        //       - decimal key representing future debt upper bound
        //       - decimal value representing interest rate
        //       but due to time constraints, the parsing is happening here upon usage
        foreach (KeyValuePair<decimal, decimal> entry in _config.InterestRateBands
            .Select(entry => new KeyValuePair<decimal, decimal>(
                entry.Key == "-1" ? decimal.MaxValue : decimal.Parse(entry.Key),
                entry.Value
            ))
            // sort the entries by future debt upper bound in ascending order
            .OrderBy(entry => entry.Key)) {

            var totalFutureDebt = application.PreexistingCreditAmount + 
                Decimal.Round(
                    CalculateCompoundInterest(
                        application.CreditAmount,
                        entry.Value,
                        application.TermMonths
                    ),
                    2, MidpointRounding.AwayFromZero
                );
            if (totalFutureDebt < entry.Key) {
                _logger.LogInformation($"Term: {application.TermMonths}, " +
                    $"Pre existing credit amount: {application.PreexistingCreditAmount}, " +
                    $"Credit amount: {application.CreditAmount}, " +
                    $"Total future debt: {totalFutureDebt}, " +
                    $"Interest rate: {entry.Value}");
                return entry.Value;
            }
        }
        // this line should be unreachable because of the use of decimal.MaxValue
        throw new InvalidOperationException("Unable to compute interest rate");
    }

    private decimal CalculateCompoundInterest(decimal amount, decimal interestRate, int term) {
        return amount * (decimal)Math.Pow(1 + (double)interestRate / 100, term);
    }
}
