using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal DefaultBuyingPrice { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public int UnitOfMeasureId { get; set; }
        public int IsAlocated { get; set; }
        public int PurchaseOrderId { get; set; }
        public bool IsTransferedStock { get; set; }
        public int Notify { get; set; }
        public int TransferFromBranchId { get; set; }
        public int TransferToBranchId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TotalStock { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal StockTakeQuantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal StockTakeAmount { get; set; }
    }
}