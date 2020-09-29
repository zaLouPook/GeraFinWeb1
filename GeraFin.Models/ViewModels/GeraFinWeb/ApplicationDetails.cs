namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class ApplicationDetails
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationName { get; set; }
        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
        public string ServerInstanceName { get; set; }
        public string SQLServerConnectionString { get; set; }
    }
}
