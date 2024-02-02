namespace CreditApplication.Services;
using CreditApplication.Models;

public interface ICreditApplicationService
{
    IEnumerable<string> GetValidationErrors(CreditApplicationRequest application);
    CreditApplicationResponse MakeDecision(CreditApplicationRequest application);
}
