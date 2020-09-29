using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.WebClient
{
    public class UserConfig
    {
        [Key]
        public int ConfigId { get; set; }
        public int UserId { get; set; }
        public int Branchid { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SMTPHostServer { get; set; }
        public string POP3HostServer { get; set; }
        public int SMTPPort { get; set; }
        public int POP3Port { get; set; }
        public bool SMTPEnableSSL { get; set; }
        public bool POP3EnableSSL { get; set; }
    }
}
