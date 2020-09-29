using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class StockTakeLine
    {
        [Key]
        public int StockTakeLineId { get; set; }
        public int StockTakeId { get; set; }
        public int StockId { get; set; }
        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal Price { get; set; }
        public int BranchId { get; set; }
        public StockTake StockTake { get; set; }
        public string ProductName { get; set; }
        public string UnitOfMeasure { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal StoreRoomCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal StoreRoomAmt { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal VarianceCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal VarianceAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal OpeningBalanceCount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public Decimal OpeningBalanceAmount { get; set; }
    }
}