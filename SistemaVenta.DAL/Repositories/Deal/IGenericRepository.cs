using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace SistemaVenta.DAL.Repositories.Deal
{
    public interface IGenericRepository<Tmodel> where Tmodel : class
    {
        Task<Tmodel> Get(Expression<Func<Tmodel, bool>> filter);
        Task<Tmodel> Create( Tmodel model );
        Task<bool> Update(Tmodel model );
        Task<bool> Delete(Tmodel model );
        Task<IQueryable<Tmodel>> Query(Expression<Func<Tmodel, bool>> filtro = null);
    }
}
