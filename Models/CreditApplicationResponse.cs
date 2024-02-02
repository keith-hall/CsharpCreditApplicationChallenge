namespace CreditApplication.Models;

public record CreditApplicationResponse(
    CreditApplicationDecision Decision,
    decimal? InterestRate
);
