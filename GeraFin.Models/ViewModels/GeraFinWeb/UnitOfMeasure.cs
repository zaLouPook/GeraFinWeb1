using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class UnitOfMeasure
    {
        [Key]
        public int UnitOfMeasureId { get; set; }
        [Required]
        public string UnitOfMeasureName { get; set; }
        public string Description { get; set; }
    }
}
