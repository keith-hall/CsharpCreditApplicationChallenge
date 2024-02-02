namespace CreditApplication.Models;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CreditApplicationDecision
{
    No = 0,
    Yes = 1,
}
