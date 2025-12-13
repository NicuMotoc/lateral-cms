namespace LateralCMS.Auth;

public class AuthUser
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string[] Roles { get; set; } = [];
}
