using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class PurchaseSummary
    {
        [Key]
        public int InvoiceOrderId { get; set; }
        [Display(Name = "Order Number")]
        public string InvoiceOrderName { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        [Display(Name = "Customer Ref. Number")]
        public string CustomerRefNumber { get; set; }
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
        public List<PurchaseSummaryLine> InvoiceOrderLines { get; set; } = new List<PurchaseSummaryLine>();
    }
}