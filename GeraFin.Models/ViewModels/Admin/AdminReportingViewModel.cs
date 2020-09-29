using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminReportingViewModel
    {
        public decimal Total { get; set; }
        public decimal Profit { get; set; }
        public decimal Quantity { get; set; }
        public decimal DeliveryCharge { get; set; }
        public List<AdminOrderItemViewModel> OrderItemViewModels { get; set; }
        public List<decimal> Profits { get; set; }
        public List<decimal> Totals { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
