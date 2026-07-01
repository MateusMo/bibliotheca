using System.ComponentModel.DataAnnotations;
using Bibliotheca.Application.Dtos.User;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _authService.RegisterAsync(new CreateUserDto
        {
            Name = Input.Name,
            Email = Input.Email,
            Password = Input.Password
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        return RedirectToPage("/Account/Login");
    }
}

public class RegisterInputModel
{
    // Nome pode repetir, sem validação de unicidade.
    [Required(ErrorMessage = "Informe o nome")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    // Unicidade de e-mail validada no UserService (EmailExistsAsync).
    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    // Senha pode repetir, sem validação de unicidade.
    [Required(ErrorMessage = "Informe a senha")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter ao menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}