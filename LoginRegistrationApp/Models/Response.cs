namespace LoginRegistrationApp.Models
{
    public class Response
    {
        public int? statusCode { get; set; }
        public string? statusText { get; set; }

        public List<User> users { get; set; }
        public string? userName { get; set; }
    }
}
