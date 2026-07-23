using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;

namespace InnoDevsITL.Services.Implementations
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;

        public SubmissionService(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        public Task<Submission> SubmitAsync(Submission submission)
        {
            return _submissionRepository.AddAsync(submission);
        }

        public Task<Submission> ReviewSubmissionAsync(int id, bool approve, string reviewedBy)
        {
            return _submissionRepository.GetByIdAsync(id).ContinueWith(task =>
            {
                var submission = task.Result;
                if (submission == null)
                {
                    return null;
                }

                submission.IsApproved = approve;
                submission.ReviewedBy = reviewedBy;
                return _submissionRepository.UpdateAsync(submission);
            }).Unwrap();
        }

        public Task<IEnumerable<Submission>> GetPendingSubmissionsAsync()
        {
            return _submissionRepository.GetPendingAsync();
        }
    }
}
