using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class NumberSequence
    {
        [Key]
        public int NumberSequenceId { get; set; }
        [Required]
        public string NumberSequenceName { get; set; }
        [Required]
        public string Module { get; set; }
        [Required]
        public string Prefix { get; set; }
        public int LastNumber { get; set; }
    }
}
