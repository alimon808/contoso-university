namespace ContosoUniversity.Data
{
    public class AdminIdentityOptions
    {
        public string Role { get; } = "Administrator";
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
