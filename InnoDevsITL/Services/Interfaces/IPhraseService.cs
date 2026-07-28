using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface IPhraseService
    {
        Task<IEnumerable<Phrase>> SearchPhrasesAsync(string searchTerm, int? categoryId);
        Task<Phrase> GetPhraseByIdAsync(int id);
        Task<Phrase> CreatePhraseAsync(Phrase phrase);
        Task<Phrase> UpdatePhraseAsync(Phrase phrase);
        Task<bool> DeletePhraseAsync(int id);
        Task<IEnumerable<Phrase>> GetPhrasesByCategoryAsync(int categoryId);
    }
}