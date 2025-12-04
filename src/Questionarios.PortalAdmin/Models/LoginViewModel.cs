using System.ComponentModel.DataAnnotations;

namespace Questionarios.PortalAdmin.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe um e-mail ou usuário.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool RememberMe { get; set; }

    [Display(Name = "Idioma")]
    public string? Language { get; set; } = "pt";
}
