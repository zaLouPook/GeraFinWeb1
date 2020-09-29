using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminOrderInfoViewModel
    {
        public int InfoId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public int DeliveryId { get; set; }
        public string DeliveryPlace { get; set; }
        public int IsHomeDelivery { get; set; }
        public decimal DeliveryCharge { get; set; }
        public string ActionByUserName { get; set; }
        public string ActionByUserEmail { get; set; }
        public int ActionById { get; set; }
        public string ActionTime { get; set; }
        public int State { get; set; }
        public List<AdminOrderItemViewModel> OrderItemViewModels { get; set; }
    }
}
