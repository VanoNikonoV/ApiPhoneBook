namespace ApiPhoneBook.Models
{
    public record RequestLogin
    {
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
       
    }
}
