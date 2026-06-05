namespace dev_tech_test_accenture.Services;

public class CoffeeService : ICoffeeService
{
    private const string Key = "coffee_count";

    public bool TryBrew(HttpContext context)
    {
        var count = context.Session.GetInt32(Key) ?? 0;
        count++;

        context.Session.SetInt32(Key, count);

        // Every 5th request fails per session
        if (count % 5 == 0)
        {
            return false;
        }

        return true;
    }

    public virtual bool IsAprilFools() 
    {
        var today = DateTime.UtcNow;
        return today.Month == 4 && today.Day == 1;
    }
}