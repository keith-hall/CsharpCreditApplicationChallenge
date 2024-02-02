namespace CreditApplication.Config;

public class CreditApplicationServiceOptions
{
    public const string CreditApplicationService = "CreditApplicationService";

    public int MinimumTerm { get; set; }
    public decimal MinimumAppliedAmount { get; set; }
    public decimal MaximumAppliedAmount { get; set; }
    public IDictionary<string, decimal> InterestRateBands { get; set; } = null!;
}
