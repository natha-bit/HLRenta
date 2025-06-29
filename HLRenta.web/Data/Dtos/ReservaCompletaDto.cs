namespace HLRenta.web.Data.Dtos
{
    public class ReservaCompletaDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string NumeroLicencia { get; set; }

        public DateTime FechaHoraRecogida { get; set; }
        public DateTime FechaHoraDevolucion { get; set; }
        public string LugarRecogida { get; set; }
        public string LugarDevolucion { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Extras { get; set; }
        public decimal Total { get; set; }

        public int VehiculoId { get; set; }
    }
}
