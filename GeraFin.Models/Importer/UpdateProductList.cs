using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.Importer
{
    public partial class UpdateProductList
    {
        [Key]
        public int Id { get; set; }
        public int FileUploadId { get; set; }
        public string Unit { get; set; }
        public string ProductName { get; set; }
        public string SupplierCode { get; set; }
        public string Category { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string UOM { get; set; }
        public string Supplier { get; set; }
        public string Price { get; set; }
    }
}

