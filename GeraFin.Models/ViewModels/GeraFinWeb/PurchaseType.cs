using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class PurchaseType
    {
        [Key]
        public int PurchaseTypeId { get; set; }
        [Required]
        public string PurchaseTypeName { get; set; }
        public string Description { get; set; }
    }
}
