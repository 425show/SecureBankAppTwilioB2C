using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecureBankAppDemo.Pages;

[AllowAnonymous]
public class AccountsModel : PageModel
{
    private readonly ILogger<AccountsModel> _logger;

    public AccountsModel(ILogger<AccountsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}