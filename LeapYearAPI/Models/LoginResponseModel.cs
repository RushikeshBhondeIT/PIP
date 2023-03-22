namespace LeapYearAPI.Models
{
    public class LoginResponseModel
    {
        public string? token { get; set; }
        public string expiration { get; set; } = DateTime.Now.AddMinutes(10).ToString();
    }
}
