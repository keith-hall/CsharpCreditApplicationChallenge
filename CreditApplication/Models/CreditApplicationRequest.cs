namespace CreditApplication.Models;

public record CreditApplicationRequest(
    decimal CreditAmount,
    int TermMonths,
    decimal PreexistingCreditAmount
);
