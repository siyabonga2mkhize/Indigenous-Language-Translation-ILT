using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface IPhraseService
    {
        Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId);
        Task<Phrase> GetByIdAsync(int id);
        Task<Phrase> CreatePhraseAsync(Phrase phrase);
        Task<Phrase> UpdatePhraseAsync(Phrase phrase);
        Task<bool> DeletePhraseAsync(int id);
    }
}
