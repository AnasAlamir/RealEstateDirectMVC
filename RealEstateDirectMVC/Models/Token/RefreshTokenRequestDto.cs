namespace MVC_Project.Models.Token
{
    public class RefreshTokenRequestDto
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
