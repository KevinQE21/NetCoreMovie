using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs
{
    public class CineCreacionDTO
    {
        [Required]
        [StringLength(maximumLength: 75)]
        public string Nombre { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }
    }
}
