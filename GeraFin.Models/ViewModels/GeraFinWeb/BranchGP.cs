using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class BranchGP
    {
        [Key]
        public int GPId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public string Year { get; set; } = "0";
        public int BranchId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Income { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Costs { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal PreliminaryGP { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal PreliminaryGPPerc { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Variances { get; set; }
        public string FinalGP { get; set; } = "0";
        public string FinalGPPerc { get; set; } = "0";
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TotalInvoices { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal TotalPurchases { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Balance { get; set; }
        public string Comments { get; set; } = "0";
    }
}
