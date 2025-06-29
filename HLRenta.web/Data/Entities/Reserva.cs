using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HLRenta.web.Data.Entities
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaHoraRecogida { get; set; }

        [Required]
        public DateTime FechaHoraDevolucion { get; set; }

        public string? Estado { get; set; }

        [Required]
        public string LugarRecogida { get; set; }

        [Required]
        public string LugarDevolucion { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Extras { get; set; }
        public decimal Total { get; set; }

        // Relaciones
        [Required]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required]
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }
    }
}
