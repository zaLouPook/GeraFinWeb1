using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class PurchaseSummaryLine
    {
        [Key]
        public int InvoiceOrderLineId { get; set; }
        public int InvoiceOrderId { get; set; }
        public PurchaseSummary PurchaseSummary { get; set; }
        public string Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string SupplierName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TotalExclVAT { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal VAT { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TotalInclVAT { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SupplierInvoiceTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SupplierInvoiceVAT { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SupplierTotalinclVAT { get; set; }
        public string SupplierInvoiceNo { get; set; }
    }
}