using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class DailyIncome
    {
        [Key]
        public int SalesOrderId { get; set; }
        [Display(Name = "Order Number")]
        public string SalesOrderName { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustomerRefNumber { get; set; }
        [Display(Name = "Sales Type")]
        public int SalesTypeId { get; set; }
        public string Remarks { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Total { get; set; }
        public List<DailyIncomeLine> SalesOrderLines { get; set; } = new List<DailyIncomeLine>();
    }
}