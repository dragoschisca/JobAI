namespace FrontendBlazor.Components.Pages.Auth.HelperClasses;

    public class RegisterResponse
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
