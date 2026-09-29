namespace XFundingHub.Application.Services;

public class ApplicationIdGenerator
{
    public string Generate(int applicationNumber)
    {
        return $"LA{applicationNumber}";
    }
}
