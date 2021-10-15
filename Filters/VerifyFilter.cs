using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class VerifyFilter: IAsyncPageFilter
{
    private readonly IVerification _verification;
    private IMemoryCache _cache;

    public VerifyFilter(IVerification verification, IMemoryCache cache)
    {
        _verification = verification;
        _cache = cache;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        _cache.TryGetValue<bool>("isVerified", out var isVerified);

        if(!isVerified)
        {
            string phoneNumber = context.HttpContext.User.Claims.First(x => x.Type.Contains("PhoneNumber")).Value;
            var result = await _verification.StartVerificationAsync(phoneNumber, "sms");
            if(result.IsValid)
            {
                context.Result = new RedirectResult("Verify");
            }
            else
            {
                context.Result = new RedirectResult("/");
            }
        }
        else
        {
            await next.Invoke();
        }
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }
}