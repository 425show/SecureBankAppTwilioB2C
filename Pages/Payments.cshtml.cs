
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[ServiceFilter(typeof(VerifyFilter))]
public class PaymentsModel : PageModel
{
    private readonly ILogger<PaymentsModel> _logger;

    public PaymentsModel(ILogger<PaymentsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet() => Page();
}