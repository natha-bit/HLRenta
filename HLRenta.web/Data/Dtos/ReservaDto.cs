namespace RentaCAR.Dtos
{
    public class ReservaDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraRecogida { get; set; }
        public DateTime FechaHoraDevolucion { get; set; }
        public string LugarRecogida { get; set; }
        public string LugarDevolucion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Extras { get; set; }
        public decimal Total { get; set; }

        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public int VehiculoId { get; set; }
        public string VehiculoModelo { get; set; }
    }
}
