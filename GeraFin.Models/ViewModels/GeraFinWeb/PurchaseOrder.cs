using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderName { get; set; }
        public int BranchId { get; set; }
        public int VendorId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int PurchaseTypeId { get; set; }
        public string AccountType { get; set; }
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SupplierInvTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SupplierVatAmount { get; set; }
        public bool Recieved { get; set; }
        public int? Visable { get; set; } = new int?(1);
        public int TransactionTypeId { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();
    }
}