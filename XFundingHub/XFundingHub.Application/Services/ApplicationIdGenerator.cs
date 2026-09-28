namespace XFundingHub.Application.Services;

public class ApplicationIdGenerator
{
    private int _nextId = 1001;

    public string Generate()
    {
        return $"LA{_nextId++}";
    }
}
