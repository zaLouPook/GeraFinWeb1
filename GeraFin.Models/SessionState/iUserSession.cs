using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.SessionState
{
    public class iUserSession
    {
        [Key]
        public int iSessionId { get; set; }
        public string SessionId { get; set; }
        public string vcSessionLoginKey { get; set; }
        public int iSessionIsAdmin { get; set; }
        public int iSessionBranchKey { get; set; }
        public bool Active { get; set; }
        public string dtSessionStart { get; set; }
        public string dtSessionEnd { get; set; }
        public string vcMachineName { get; set; }
        public string vcOSVersion { get; set; }
    }
}