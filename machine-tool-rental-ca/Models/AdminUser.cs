namespace machine_tool_rental_ca.Models;

public class AdminUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
