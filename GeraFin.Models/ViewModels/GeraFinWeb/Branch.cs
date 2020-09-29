using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        [Required]
        public string BranchName { get; set; }
        public string Description { get; set; }
        public bool LoginRequired { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RegionalEmail { get; set; }
        public string ContactPerson { get; set; }
        public bool ViewBranch { get; set; }
    }
}
