using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class WeeklySignOff
    {
        [Key]
        public int WeeklySignOffId { get; set; }
        public string WeeklySignOffName { get; set; }
        public int BranchId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime WeekEnd { get; set; }
        public string CustomerRefNumber { get; set; }
        public int WeeklySignOffTypeId { get; set; }
        public string Remarks { get; set; }
        public double Total { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal WeeklyCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal WeeklyAmout { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal MonthlyCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal MonthlyAmount { get; set; }
 
        public List<WeeklySignOffLine> WeeklySignOffLines { get; set; } = new List<WeeklySignOffLine>();
    }
}