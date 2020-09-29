using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminProductViewModel
    {
        public int Id { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string EntryDate { get; set; }
        public int EntryMonth { get; set; }
        public int EntryYear { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Profit { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string ActionBy { get; set; }
        public string ActionDone { get; set; }
        public string ActionTime { get; set; }
        public int State { get; set; }
    }
}
