using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;

namespace InnoDevsITL.Services.Implementations
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepository _favouriteRepository;

        public FavouriteService(IFavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

        public Task<Favourite> AddFavouriteAsync(Favourite favourite)
        {
            return _favouriteRepository.AddAsync(favourite);
        }

        public Task<IEnumerable<Favourite>> GetFavouritesByUserAsync(string userId)
        {
            return _favouriteRepository.GetByUserAsync(userId);
        }

        public Task<bool> RemoveFavouriteAsync(int id)
        {
            return _favouriteRepository.RemoveAsync(id);
        }
    }
}
