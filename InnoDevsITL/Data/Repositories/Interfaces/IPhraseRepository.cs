using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Data.Repositories.Interfaces
{
    public interface IPhraseRepository
    {
        Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId);
        Task<Phrase> GetByIdAsync(int id);
        Task<Phrase> AddAsync(Phrase phrase);
        Task<Phrase> UpdateAsync(Phrase phrase);
        Task<bool> DeleteAsync(int id);
    }
}
