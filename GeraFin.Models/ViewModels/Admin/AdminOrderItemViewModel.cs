using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminOrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal Profit { get; set; }
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string OrderMonth { get; set; }
        public string OrderYear { get; set; }
        public int IsNew { get; set; }
        public int IsDelivered { get; set; }
        public int IsReceived { get; set; }
        public string LastActionBy { get; set; }
        public string ActionTime { get; set; }
        public string ActionDone { get; set; }
        public int State { get; set; }
    }
}
