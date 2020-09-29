using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class PurchaseOrderLine
    {
        [Key]
        public int PurchaseOrderLineId { get; set; }
        public int PurchaseOrderId { get; set; }
        public string UOM { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public string QuickNote { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SpecialPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
        public bool CreditLine { get; set; }
        public int Credited { get; set; }
    }
}