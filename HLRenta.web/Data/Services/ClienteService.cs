using HLRenta.web.Data;
using HLRenta.web.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using HLRenta.web.Data.Entities;

namespace RentaCAR.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> ObtenerTodosAsync();
        Task<ClienteDto> ObtenerPorIdAsync(int id);
        Task CrearAsync(ClienteDto dto);
        Task<bool> ActualizarAsync(int id, ClienteDto dto);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteLicenciaAsync(string numeroLicencia);
    }

    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDto>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    NumeroLicencia = c.NumeroLicencia
                }).ToListAsync();
        }

        public async Task<ClienteDto> ObtenerPorIdAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                NumeroLicencia = cliente.NumeroLicencia
            };
        }

        public async Task CrearAsync(ClienteDto dto)
        {
            var cliente = new Cliente
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

        public async Task<bool> ActualizarAsync(int id, ClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Nombre = dto.Nombre;
            cliente.Apellido = dto.Apellido;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;
            cliente.NumeroLicencia = dto.NumeroLicencia;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteLicenciaAsync(string numeroLicencia)
        {
            return await _context.Clientes.AnyAsync(c => c.NumeroLicencia == numeroLicencia);
        }
    }
}
