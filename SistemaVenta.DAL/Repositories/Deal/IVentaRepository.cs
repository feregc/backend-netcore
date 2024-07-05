using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositories.Deal
{
    public interface IVentaRepository: IGenericRepository<Venta>
    {
        Task<Venta> ToRegister(Venta model);
    }
}
