using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Models;

namespace InnoDevsITL.Data.Repositories.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<IEnumerable<Submission>> GetPendingAsync();
        Task<Submission> GetByIdAsync(int id);
        Task<Submission> AddAsync(Submission submission);
        Task<Submission> UpdateAsync(Submission submission);
    }
}
