using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<IEnumerable<Submission>> GetPendingSubmissionsAsync();
        Task<Submission> GetSubmissionByIdAsync(int id);
        Task<Submission> CreateSubmissionAsync(Submission submission);
        Task<Submission> UpdateSubmissionAsync(Submission submission);
        Task<bool> ApproveSubmissionAsync(int id, string reviewerId);
        Task<bool> RejectSubmissionAsync(int id);
        Task<IEnumerable<Submission>> GetSubmissionsByUserAsync(string userId);
    }
}