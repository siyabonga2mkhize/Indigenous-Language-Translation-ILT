using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface ITranslationService
    {
        Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId);
        Task<Translation> CreateTranslationAsync(Translation translation);
        Task<Translation> ApproveTranslationAsync(int id, string reviewedBy);
        Task<bool> DeleteTranslationAsync(int id);
    }
}
