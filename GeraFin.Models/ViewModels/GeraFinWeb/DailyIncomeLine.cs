using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class DailyIncomeLine
    {
        [Key]
        public int SalesOrderLineId { get; set; }
        [Display(Name = "Sales Order")]
        public DateTime SalesOrderLineDate { get; set; }
        public int SalesOrderId { get; set; }
        [Display(Name = "Sales Order")]
        public DailyIncome DailyIncome { get; set; }
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