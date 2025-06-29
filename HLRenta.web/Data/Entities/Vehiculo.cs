using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HLRenta.web.Data.Entities
{
    [Table("Vehiculos")]
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Marca { get; set; }

        [Required]
        public string Modelo { get; set; }

        [Required]
        public int Anio { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Placa { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        public decimal PrecioPorDia { get; set; }

        public List<string> Imagenes { get; set; } = new();

        public ICollection<Reserva> Reservas { get; set; }
    }
}
