using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class DropDown
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}