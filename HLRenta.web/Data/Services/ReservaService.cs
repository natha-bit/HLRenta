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
                    VehiculoModelo = r.Vehiculo.Modelo
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
                VehiculoModelo = r.Vehiculo.Modelo
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

        public async Task<List<ReservaDto>> ObtenerPorRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");

            return await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r =>
                    r.FechaHoraRecogida.Date >= fechaInicio.Date &&
                    r.FechaHoraDevolucion.Date <= fechaFin.Date)
                .Select(r => new ReservaDto
                {
                    Id = r.Id,
                    FechaHoraRecogida = r.FechaHoraRecogida,
                    FechaHoraDevolucion = r.FechaHoraDevolucion,
                    Estado = r.Estado,
                    LugarRecogida = r.LugarRecogida,
                    LugarDevolucion = r.LugarDevolucion,
                    Subtotal = r.Subtotal,
                    Extras = r.Extras,
                    Total = r.Total,
                    ClienteId = r.ClienteId,
                    ClienteNombre = r.Cliente.Nombre,
                    VehiculoId = r.VehiculoId,
                    VehiculoModelo = r.Vehiculo.Modelo
                })
                .ToListAsync();
        }
        public async Task CrearReservaCompletaAsync(ReservaCompletaDto dto)
        {
            // Verificar si el cliente ya existe por email o teléfono
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == dto.Email || c.Telefono == dto.Telefono);

            Cliente cliente;

            if (clienteExistente != null)
            {
                cliente = clienteExistente;
            }
            else
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
                FechaHoraRecogida = dto.FechaHoraRecogida,
                FechaHoraDevolucion = dto.FechaHoraDevolucion,
                LugarRecogida = dto.LugarRecogida,
                LugarDevolucion = dto.LugarDevolucion,
                Subtotal = dto.Subtotal,
                Extras = dto.Extras,
                Total = dto.Total,
                ClienteId = cliente.Id,
                VehiculoId = dto.VehiculoId,
                Estado = "Pendiente"
            };


            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
        }
        public async Task<ReservaDto>ObtenerPorLicenciaAsync(string numeroLicencia)
        {
            if (string.IsNullOrWhiteSpace(numeroLicencia))
                return null;

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NumeroLicencia.ToLower().Trim() == numeroLicencia.ToLower().Trim());

            if (cliente == null)
                return null;

            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .Where(r => r.ClienteId == cliente.Id)
                .OrderByDescending(r => r.FechaHoraRecogida)
                .FirstOrDefaultAsync();

            if (reserva == null)
                return null;

            return new ReservaDto
            {
                Id = reserva.Id,
                FechaHoraRecogida = reserva.FechaHoraRecogida,
                FechaHoraDevolucion = reserva.FechaHoraDevolucion,
                Estado = reserva.Estado,
                LugarRecogida = reserva.LugarRecogida,
                LugarDevolucion = reserva.LugarDevolucion,
                Subtotal = reserva.Subtotal,
                Extras = reserva.Extras,
                Total = reserva.Total,
                ClienteId = cliente.Id,
                ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}",
                VehiculoId = reserva.VehiculoId,
                VehiculoModelo = reserva.Vehiculo?.Modelo ?? "No asignado"
            };
        }





    }
}
