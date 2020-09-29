using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class TransactionType
    {
        [Key]
        public int TransactionTypeId { get; set; }
        [Required]
        public string TransactionTypeName { get; set; }
        public string TransactionTypeDescription { get; set; }
    }
}
