
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class VerifyModel: PageModel
{
    private readonly IVerification _verification;
    private readonly ILogger<VerifyModel> _logger;

    private IMemoryCache _cache;
    
    public VerifyModel(IVerification verification,ILogger<VerifyModel> logger, IMemoryCache cache)
    {
        _verification = verification;
        _logger = logger;
        _cache = cache;
    }
    
    [BindProperty]
    public InputModel? Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Enter the Verification Code")]
        public string? Code { get; set; }
    }
    
    public ActionResult OnGet()
    {
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        var userPhoneNumber = HttpContext.User.Claims.First(x => x.Type.Contains("PhoneNumber")).Value;
        if (ModelState.IsValid)
        {
            if(Input == null || Input.Code == null)
            {
                return Page();
            }
            var result = await _verification.CheckVerificationAsync(userPhoneNumber, Input.Code);
            if (result.IsValid)
            {
                _cache.Set<bool>("isVerified", true, new DateTimeOffset(DateTime.Now.AddMinutes(2)));
                return RedirectToPage("Payments");
            }
            
            foreach (var error in result.Errors!)
            {
                _logger.Log(LogLevel.Information, $"Verification Failed: {error}");

                ModelState.AddModelError(string.Empty, error);
            }
        }

        return Page();
    }
}