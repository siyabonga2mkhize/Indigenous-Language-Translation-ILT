using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface IFavouriteService
    {
        Task<IEnumerable<Favourite>> GetFavouritesByUserAsync(string userId);
        Task<Favourite> AddFavouriteAsync(Favourite favourite);
        Task<bool> RemoveFavouriteAsync(int id);
    }
}
