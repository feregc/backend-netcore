﻿using AutoMapper;
using SistemaVenta.BLL.Services.Deal;
using SistemaVenta.DAL.Repositories.Deal;
using SistemaVenta.DTO;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<Producto> _productoRepositorio;
        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepositorio, IGenericRepository<Producto> productoRepositorio, IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _productoRepositorio = productoRepositorio;
            _mapper = mapper;
        }

        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVentas, int restarCantDias)
        {
            DateTime? ultimaFecha = tablaVentas.OrderByDescending(v =>
                v.FechaRegistro).Select(v =>
                v.FechaRegistro).First();
            ultimaFecha = ultimaFecha.Value.AddDays(restarCantDias);

            return tablaVentas.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }

        private async Task<int> TotalVentasUltimaSemana()
        {
            int total = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Query();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);
                total = tablaVenta.Count();
            }
            return total;
        }

        private async Task<string> TotalIngresoUltimaSemana()
        {
            decimal resultado = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Query();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);
                resultado = tablaVenta.Select(v => v.Total).Sum(v => v.Value);
            }

            return Convert.ToString(resultado, new CultureInfo("es-HN"));

        }

        private async Task<int> TotalProductos()
        {
            IQueryable<Producto> _productoQuery = await _productoRepositorio.Query();
            int total = _productoQuery.Count();
            return total;
        }

        private async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Query();

            if (_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);
                resultado = tablaVenta
                        .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                        .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                        .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
            }
            return resultado;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard = new DashBoardDTO();
            try
            {
                vmDashBoard.TotalVentas = await TotalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await TotalIngresoUltimaSemana();
                vmDashBoard.TotalProductos = await TotalProductos();

                List<VentasSemanaDTO> listaVentaSemana = new List<VentasSemanaDTO>();
                foreach (KeyValuePair<string, int> item in await VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentasSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value,
                    });
                }
                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
            }
            catch
            {
                throw;
            }

            return vmDashBoard;
        }
    }
}
