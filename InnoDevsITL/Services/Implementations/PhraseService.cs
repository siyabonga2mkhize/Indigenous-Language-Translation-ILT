using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;

namespace InnoDevsITL.Services.Implementations
{
    public class PhraseService : IPhraseService
    {
        private readonly IPhraseRepository _phraseRepository;

        public PhraseService(IPhraseRepository phraseRepository)
        {
            _phraseRepository = phraseRepository;
        }

        public Task<Phrase> CreatePhraseAsync(Phrase phrase)
        {
            return _phraseRepository.AddAsync(phrase);
        }

        public Task<bool> DeletePhraseAsync(int id)
        {
            return _phraseRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId)
        {
            return _phraseRepository.SearchAsync(searchTerm, categoryId);
        }

        public Task<Phrase> GetByIdAsync(int id)
        {
            return _phraseRepository.GetByIdAsync(id);
        }

        public Task<Phrase> UpdatePhraseAsync(Phrase phrase)
        {
            return _phraseRepository.UpdateAsync(phrase);
        }
    }
}
