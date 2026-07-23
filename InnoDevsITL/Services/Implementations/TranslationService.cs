using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;

namespace InnoDevsITL.Services.Implementations
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationRepository _translationRepository;

        public TranslationService(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public Task<Translation> ApproveTranslationAsync(int id, string reviewedBy)
        {
            return _translationRepository.GetByIdAsync(id).ContinueWith(task =>
            {
                var translation = task.Result;
                if (translation == null)
                {
                    return null;
                }

                translation.IsApproved = true;
                translation.Language = translation.Language;
                return _translationRepository.UpdateAsync(translation);
            }).Unwrap();
        }

        public Task<bool> DeleteTranslationAsync(int id)
        {
            return _translationRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId)
        {
            return _translationRepository.GetByPhraseIdAsync(phraseId);
        }

        public Task<Translation> CreateTranslationAsync(Translation translation)
        {
            return _translationRepository.AddAsync(translation);
        }
    }
}
