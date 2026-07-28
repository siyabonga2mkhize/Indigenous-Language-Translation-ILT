using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Data.Repositories.Interfaces
{
    public interface ITranslationRepository
    {
        Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId);
        Task<Translation> GetByIdAsync(int id);
        Task<Translation> AddAsync(Translation translation);
        Task<Translation> UpdateAsync(Translation translation);
        Task<bool> DeleteAsync(int id);
    }
}
