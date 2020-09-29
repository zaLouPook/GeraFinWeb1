using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class DailyIssuesLine
    {
        [Key]
        public int DailyIssuesLineId { get; set; }
        [Display(Name = "Daily Issues")]
        public int DailyIssuesId { get; set; }
        [Display(Name = "Daily Issue")]
        public DailyIssues DailyIssues { get; set; }
        [Display(Name = "Product Item")]
        public int ProductId { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
    }
}