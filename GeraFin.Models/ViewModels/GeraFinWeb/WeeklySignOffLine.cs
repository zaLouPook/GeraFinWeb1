using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class WeeklySignOffLine
    {
        [Key]
        public int WeeklySignOffLineId { get; set; }
        public int WeeklySignOffId { get; set; }
        public WeeklySignOff WeeklySignOff { get; set; }
        public string ProductName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Sun { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Mon { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Tue { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Wed { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Thu { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Fri { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Sat { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal QTY { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal ZAR { get; set; }
    }
}