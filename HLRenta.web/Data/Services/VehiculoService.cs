using HLRenta.web.Data;
using Microsoft.EntityFrameworkCore;
using RentaCAR.Dtos;
using RentaCAR.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLRenta.web.Data.Services
{
    public interface IVehiculoService
    {
        Task<bool> CrearAsync(VehiculoDto dto);
        Task<bool> ActualizarAsync(int id, VehiculoDto dto);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExistePlacaAsync(string placa);
        Task<VehiculoDto> ObtenerPorIdAsync(int id);
        Task<List<VehiculoDto>> ObtenerTodosAsync();
    }

    public class VehiculoService : IVehiculoService
    {
        private readonly ApplicationDbContext _context;

        public VehiculoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehiculoDto>> ObtenerTodosAsync()
        {
            return await _context.Vehiculos
                .Select(v => new VehiculoDto
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Anio = v.Anio,
                    Color = v.Color,
                    Placa = v.Placa,
                    Tipo = v.Tipo,
                    PrecioPorDia = v.PrecioPorDia,
                    Imagenes = v.Imagenes
                })
                .ToListAsync();
        }

        public async Task<VehiculoDto> ObtenerPorIdAsync(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return null;

            return new VehiculoDto
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
            };
        }

        public async Task<bool> CrearAsync(VehiculoDto dto)
        {
            try
            {
                var vehiculo = new Vehiculo
                {
                    Marca = dto.Marca,
                    Modelo = dto.Modelo,
                    Anio = dto.Anio,
                    Color = dto.Color,
                    Placa = dto.Placa,
                    Tipo = dto.Tipo,
                    PrecioPorDia = dto.PrecioPorDia,
                    Imagenes = dto.Imagenes
                };

                _context.Vehiculos.Add(vehiculo);
                var resultado = await _context.SaveChangesAsync();
                return resultado > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(int id, VehiculoDto dto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return false;

            vehiculo.Marca = dto.Marca;
            vehiculo.Modelo = dto.Modelo;
            vehiculo.Anio = dto.Anio;
            vehiculo.Color = dto.Color;
            vehiculo.Placa = dto.Placa;
            vehiculo.Tipo = dto.Tipo;
            vehiculo.PrecioPorDia = dto.PrecioPorDia;
            vehiculo.Imagenes = dto.Imagenes;

            _context.Vehiculos.Update(vehiculo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return false;

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistePlacaAsync(string placa)
        {
            return await _context.Vehiculos.AnyAsync(v => v.Placa == placa);
        }
    }
}
