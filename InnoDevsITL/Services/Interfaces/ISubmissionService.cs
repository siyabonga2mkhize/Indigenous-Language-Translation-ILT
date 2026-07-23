using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<IEnumerable<Submission>> GetPendingSubmissionsAsync();
        Task<Submission> SubmitAsync(Submission submission);
        Task<Submission> ReviewSubmissionAsync(int id, bool approve, string reviewedBy);
    }
}
