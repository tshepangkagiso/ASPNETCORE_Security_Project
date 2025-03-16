namespace WebApp_Identity.Settings
{
    public class SmtpSettings
    {
        public string Host {  get; set; } = string.Empty;
        public int Port { get; set; } 
        public string Login { get; set; } = string.Empty;
        public string MasterKey { get; set; } = string.Empty;
        public bool EnableSSL { get; set; }
    }
}
