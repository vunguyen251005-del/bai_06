using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ThuongMaiDienTu.Data;
using ThuongMaiDienTu.Models;

namespace ThuongMaiDienTu.Areas.Identity.Pages.Account;

public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RegisterModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = [];

    public class InputModel
    {
        [Required(ErrorMessage = "Vui long nhap ten dang nhap")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(150)]
        public string? FullName { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui long nhap mat khau")]
        [StringLength(100, ErrorMessage = "{0} phai co it nhat {2} va toi da {1} ky tu.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mat khau xac nhan khong khop.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.UserName,
            Email = Input.Email,
            FullName = Input.FullName,
            Address = Input.Address,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            logger.LogInformation("User created a new account with password.");

            if (!await roleManager.RoleExistsAsync(DbInitializer.MemberRole))
            {
                await roleManager.CreateAsync(new IdentityRole(DbInitializer.MemberRole));
            }

            await userManager.AddToRoleAsync(user, DbInitializer.MemberRole);
            await signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(ReturnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
