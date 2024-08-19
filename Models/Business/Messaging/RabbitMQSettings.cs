namespace MotorcycleRental.Models
{
    public class RabbitMQSettings
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public RabbitMQSettings() { }

        public RabbitMQSettings(
            string? hostName,
            string? userName,
            string? password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }
    }
}
