using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositories.Deal;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositories
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbcontext;

        public VentaRepository(DbventaContext dbcontext) : base(dbcontext) 
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> ToRegister(Venta model)
        {
            Venta ventaGenerada = new Venta();

            using( var transaction = _dbcontext.Database.BeginTransaction() )
            {
                try
                {
                    foreach( DetalleVenta dv in model.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbcontext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;
                        _dbcontext.Productos.Update(productoEncontrado);
                    }

                    await _dbcontext.SaveChangesAsync();

                    NumeroDocumento correlativo = _dbcontext.NumeroDocumentos.First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbcontext.NumeroDocumentos.Update(correlativo);

                    await _dbcontext.SaveChangesAsync();

                    int cantDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantDigitos));
                    string numVenta = ceros + correlativo.UltimoNumero.ToString();

                    numVenta = numVenta.Substring(numVenta.Length - cantDigitos, cantDigitos);

                    model.NumeroDocumento = numVenta;

                    await _dbcontext.Venta.AddAsync(model);

                    await _dbcontext.SaveChangesAsync();

                    ventaGenerada = model;

                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                
                return ventaGenerada;
            }
        }
    }
}
