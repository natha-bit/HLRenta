namespace RentaCAR.Dtos
{
    public class VehiculoDto
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Color { get; set; }
        public string Placa { get; set; }
        public string Tipo { get; set; }
        public decimal PrecioPorDia { get; set; }
        public List<string> Imagenes { get; set; }
    }
}
