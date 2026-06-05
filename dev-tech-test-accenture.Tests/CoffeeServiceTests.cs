using dev_tech_test_accenture.Services;
using Microsoft.AspNetCore.Http;

namespace dev_tech_test_accenture.Tests;

public class CoffeeServiceTests
{
    private static HttpContext CreateContextWithSession(int? existingCount = null)
    {
        var session = new FakeSession();
        if (existingCount.HasValue)
            session.SetInt32("coffee_count", existingCount.Value);
 
        var context = new DefaultHttpContext();
        context.Session = session;
        return context;
    }
    
    [Fact]
    public void TryBrew_FirstRequest_ReturnsTrue()
    {
        var svc = new CoffeeService();
        var ctx = CreateContextWithSession();
 
        var result = svc.TryBrew(ctx);
 
        Assert.True(result);
    }
    
    [Theory]
    [InlineData(1)]   
    [InlineData(2)]  
    [InlineData(3)]   
    public void TryBrew_NonFifthRequest_ReturnsTrue(int existingCount)
    {
        var svc = new CoffeeService();
        var ctx = CreateContextWithSession(existingCount);
 
        var result = svc.TryBrew(ctx);
 
        Assert.True(result);
    }

    [Theory]
    [InlineData(4)]   
    [InlineData(9)]   
    [InlineData(14)]  
    public void TryBrew_EveryFifthRequest_ReturnsFalse(int existingCount)
    {
        var svc = new CoffeeService();
        var ctx = CreateContextWithSession(existingCount);
 
        var result = svc.TryBrew(ctx);
 
        Assert.False(result);
    }

    [Fact]
    public void TryBrew_IncrememtsCounterInSession()
    {
        var svc = new CoffeeService();
        var ctx = CreateContextWithSession(0);
 
        svc.TryBrew(ctx);
 
        Assert.Equal(1, ctx.Session.GetInt32("coffee_count"));
    }

    [Fact]
    public void TryBrew_CounterAccumulatesAcrossMultipleCalls()
    {
        var svc = new CoffeeService();
        var ctx = CreateContextWithSession();
 
        var results = Enumerable.Range(0, 10)
            .Select(_ => svc.TryBrew(ctx))
            .ToList();
 
        Assert.False(results[4]);
        Assert.False(results[9]);
 
        foreach (var i in new[] { 0, 1, 2, 3, 5, 6, 7, 8 })
            Assert.True(results[i]);
    }
    
    [Fact]
    public void TryBrew_SessionIsolated_DifferentContextsHaveIndependentCounters()
    {
        var svc = new CoffeeService();
        var ctx1 = CreateContextWithSession();
        var ctx2 = CreateContextWithSession();
 
        for (var i = 0; i < 4; i++) svc.TryBrew(ctx1);
        var blockedCtx1 = svc.TryBrew(ctx1);
 
        var allowedCtx2 = svc.TryBrew(ctx2);
 
        Assert.False(blockedCtx1);
        Assert.True(allowedCtx2);
    }
    
    [Fact]
    public void IsAprilFools_OnAprilFirst_ReturnsTrue()
    {
        var svc = new TestableCoffeeService(new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc));
 
        Assert.True(svc.IsAprilFools());
    }
    
    [Theory]
    [InlineData(1, 1)]  
    [InlineData(3, 31)]  
    [InlineData(4, 2)]   
    [InlineData(12, 25)] 
    public void IsAprilFools_NonAprilFirst_ReturnsFalse(int month, int day)
    {
        var svc = new TestableCoffeeService(new DateTime(2025, month, day, 0, 0, 0, DateTimeKind.Utc));
 
        Assert.False(svc.IsAprilFools());
    }
 
    [Fact]
    public void IsAprilFools_MidnightAprilFirst_ReturnsTrue()
    {
        var svc = new TestableCoffeeService(new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc));
 
        Assert.True(svc.IsAprilFools());
    }
 
    [Fact]
    public void IsAprilFools_EndOfAprilFirst_ReturnsTrue()
    {
        var svc = new TestableCoffeeService(new DateTime(2025, 4, 1, 23, 59, 59, DateTimeKind.Utc));
 
        Assert.True(svc.IsAprilFools());
    }
}
public class TestableCoffeeService : CoffeeService
{
    private readonly DateTime _now;
 
    public TestableCoffeeService(DateTime now) => _now = now;
 
    public override bool IsAprilFools()
        => _now.Month == 4 && _now.Day == 1;
}