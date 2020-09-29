using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class StockTake
    {
        [Key]
        public int StockTakeId { get; set; }
        [Display(Name = "Order Number")]
        public string StockTakeName { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        [Display(Name = "Customer Ref. Number")]
        public string CustomerRefNumber { get; set; }
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
        public bool Rollover { get; set; }
        public List<StockTakeLine> StockTakeLines { get; set; } = new List<StockTakeLine>();
    }
}