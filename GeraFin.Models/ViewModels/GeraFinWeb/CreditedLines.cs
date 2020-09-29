using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class CreditedLines
    {
        [Key]
        public int LineId { get; set; }

        public int CreditedLineId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int BranchId { get; set; }

        public string UOM { get; set; } = "0";

        public int ProductId { get; set; }

        public string Description { get; set; } = "0";

        public string QuickNote { get; set; } = "0";

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

        public int Credited { get; set; }

        public int TransactionTypeId { get; set; }
    }
}
