using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Data.Repositories.Interfaces
{
    public interface IFavouriteRepository
    {
        Task<IEnumerable<Favourite>> GetByUserAsync(string userId);
        Task<Favourite> AddAsync(Favourite favourite);
        Task<bool> RemoveAsync(int id);
    }
}
