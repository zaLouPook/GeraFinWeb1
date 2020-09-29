using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class Specials
    {
        [Key]
        public int SpecialId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string VendorName { get; set; }
        public int BranchId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal OriginalPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SpecialPrice { get; set; }
        public string Month { get; set; }
        public DateTime SpecialFrom { get; set; }
        public DateTime SpecialTo { get; set; }
        public bool Expired { get; set; }
    }
}