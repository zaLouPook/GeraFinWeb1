using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal DefaultBuyingPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal DefaultSellingPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SpecialPrice { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        [Display(Name = "UOM")]
        public int UnitOfMeasureId { get; set; }
        public int ProductTypeId { get; set; }
        public bool Active { get; set; }
        public int RowCol { get; set; }
    }
}
