using HLRenta.web.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using HLRenta.web.Data.Entities;

namespace HLRenta.web.Data.Services
{
    public interface IReservaService
    {
        Task<List<ReservaDto>> ObtenerTodasAsync();
        Task<ReservaDto> ObtenerPorIdAsync(int id);
        Task CrearAsync(ReservaDto dto);
        Task<bool> ActualizarAsync(int id, ReservaDto dto);
        Task<bool> EliminarAsync(int id);
        Task<List<ReservaDto>> ObtenerPorRangoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<ReservaDto> ObtenerPorLicenciaAsync(string numeroLicencia);
        Task<bool> ActualizarEstadoAsync(int id, string nuevoEstado);
        Task<List<ReservaDto>> ObtenerActivasAsync();


        Task CrearReservaCompletaAsync(ReservaCompletaDto dto);
    }

    public class ReservaService : IReservaService
    {
        private readonly ApplicationDbContext _context;

        public ReservaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservaDto>> ObtenerTodasAsync()
        {
            return await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Select(r => new ReservaDto
                {
                    Id = r.Id,
                    FechaHoraRecogida = r.FechaHoraRecogida,
                    FechaHoraDevolucion = r.FechaHoraDevolucion,
                    LugarRecogida = r.LugarRecogida,
                    LugarDevolucion = r.LugarDevolucion,
                    Subtotal = r.Subtotal,
                    Extras = r.Extras,
                    Total = r.Total,
                    ClienteId = r.ClienteId,
                    ClienteNombre = r.Cliente.Nombre,
                    VehiculoId = r.VehiculoId,
                }).ToListAsync();
        }

        public async Task<ReservaDto> ObtenerPorIdAsync(int id)
        {
            var r = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null) return null;

            return new ReservaDto
            {
                Id = r.Id,
                FechaHoraRecogida = r.FechaHoraRecogida,
                FechaHoraDevolucion = r.FechaHoraDevolucion,
                LugarRecogida = r.LugarRecogida,
                LugarDevolucion = r.LugarDevolucion,
                Subtotal = r.Subtotal,
                Extras = r.Extras,
                Total = r.Total,
                ClienteId = r.ClienteId,
                ClienteNombre = r.Cliente.Nombre,
                VehiculoId = r.VehiculoId,
            };
        }

        public async Task CrearAsync(ReservaDto dto)
        {
            var r = new Reserva
            {
                FechaHoraRecogida = dto.FechaHoraRecogida,
                FechaHoraDevolucion = dto.FechaHoraDevolucion,
                LugarRecogida = dto.LugarRecogida,
                LugarDevolucion = dto.LugarDevolucion,
                Subtotal = dto.Subtotal,
                Extras = dto.Extras,
                Total = dto.Total,
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId
            };

            _context.Reservas.Add(r);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActualizarAsync(int id, ReservaDto dto)
        {
            var r = await _context.Reservas.FindAsync(id);
            if (r == null) return false;

            r.FechaHoraRecogida = dto.FechaHoraRecogida;
            r.FechaHoraDevolucion = dto.FechaHoraDevolucion;
            r.LugarRecogida = dto.LugarRecogida;
            r.LugarDevolucion = dto.LugarDevolucion;
            r.Subtotal = dto.Subtotal;
            r.Extras = dto.Extras;
            r.Total = dto.Total;
            r.ClienteId = dto.ClienteId;
            r.VehiculoId = dto.VehiculoId;

            _context.Reservas.Update(r);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var r = await _context.Reservas.FindAsync(id);
            if (r == null) return false;

            _context.Reservas.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservaDto>> ObtenerPorRangoAsync(DateTime fechaInicio,
                                                                 DateTime fechaFin)
        {
            // Normaliza el rango [00:00 del inicio, 23:59 del fin]
            var inicio = fechaInicio.Date;
            var fin = fechaFin.Date.AddDays(1).AddTicks(-1);

            return await _context.Reservas
                .Where(r => r.FechaHoraRecogida >= inicio &&
                            r.FechaHoraRecogida <= fin)
                .Select(r => new ReservaDto
                {
                    Id = r.Id,
                    ClienteNombre = r.Cliente.Nombre + " " + r.Cliente.Apellido,
                    Vehiculo = new VehiculoDto   // ← objeto completo
                    {
                        Marca = r.Vehiculo.Marca,
                        Modelo = r.Vehiculo.Modelo,
                        Placa = r.Vehiculo.Placa
                        // agrega las propiedades que uses en la UI
                    },
                    FechaHoraRecogida = r.FechaHoraRecogida,
                    FechaHoraDevolucion = r.FechaHoraDevolucion,
                    Total = r.Total,
                    Estado = r.Estado
                })
                .ToListAsync();
        }

        public async Task CrearReservaCompletaAsync(ReservaCompletaDto dto)
        {
            // 1) Buscar cliente por licencia
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NumeroLicencia == dto.NumeroLicencia);

            // 2) Crear si no existe
            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    NumeroLicencia = dto.NumeroLicencia
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();           
            }

            var reserva = new Reserva
            {
                ClienteId = cliente.Id,            
                VehiculoId = dto.VehiculoId,
                FechaHoraRecogida = dto.FechaHoraRecogida,
                FechaHoraDevolucion = dto.FechaHoraDevolucion,
                LugarRecogida = dto.LugarRecogida,
                LugarDevolucion = dto.LugarDevolucion,
                Subtotal = dto.Subtotal,
                Extras = dto.Extras,
                Total = dto.Total,
                Estado = "pendiente"
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
        }
        public async Task<ReservaDto> ObtenerPorLicenciaAsync(string numeroLicencia)
        {
            if (string.IsNullOrWhiteSpace(numeroLicencia))
                return null;

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NumeroLicencia.ToLower().Trim() == numeroLicencia.ToLower().Trim());

            if (cliente == null)
                return null;

            var reserva = await _context.Reservas
                .AsNoTracking()
                .Where(r => r.ClienteId == cliente.Id)
                .OrderByDescending(r => r.FechaHoraRecogida)
                .FirstOrDefaultAsync();

            if (reserva == null)
                return null;

            var vehiculo = await _context.Vehiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == reserva.VehiculoId);

            if (vehiculo == null)
                return null;

            return new ReservaDto
            {
                Id = reserva.Id,
                FechaHoraRecogida = reserva.FechaHoraRecogida,
                FechaHoraDevolucion = reserva.FechaHoraDevolucion,
                LugarRecogida = reserva.LugarRecogida,
                LugarDevolucion = reserva.LugarDevolucion,
                Subtotal = reserva.Subtotal,
                Extras = reserva.Extras,
                Total = reserva.Total,
                Estado = reserva.Estado,
                Cliente = new ClienteDto
                {
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono
                },
                Vehiculo = new VehiculoDto
                {
                    Id = vehiculo.Id,
                    Marca = vehiculo.Marca,
                    Modelo = vehiculo.Modelo,
                    Anio = vehiculo.Anio,
                    Color = vehiculo.Color,
                    Placa = vehiculo.Placa,
                    Tipo = vehiculo.Tipo,
                    PrecioPorDia = vehiculo.PrecioPorDia,
                    Imagenes = vehiculo.Imagenes
                }
            };
        }



        public async Task<bool> ActualizarEstadoAsync(int id, string nuevoEstado)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return false;

            reserva.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservaDto>> ObtenerActivasAsync()
        {
            try
            {
                var hoy = DateTime.Today;

                var lista = await _context.Reservas
                    .Where(r => r.Estado == "Confirmada" && r.FechaHoraDevolucion >= hoy)
                    .Select(r => new ReservaDto
                    {
                        Id = r.Id,
                        VehiculoId = r.VehiculoId,
                        FechaHoraRecogida = r.FechaHoraRecogida,
                        FechaHoraDevolucion = r.FechaHoraDevolucion
                    })
                    .ToListAsync();

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ObtenerActivasAsync: {ex.Message}");
                return new List<ReservaDto>(); // ← esto resuelve el error
            }
        }









    }
}
