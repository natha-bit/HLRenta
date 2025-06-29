using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HLRenta.web.Data.Entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string NumeroLicencia { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}
