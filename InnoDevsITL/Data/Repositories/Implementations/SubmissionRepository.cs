using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Data.Repositories.Implementations
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly InnoDbContext _context;

        public SubmissionRepository(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<Submission> AddAsync(Submission submission)
        {
            await _context.Submissions.AddAsync(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<Submission> GetByIdAsync(int id)
        {
            return await _context.Submissions
                .Include(s => s.Phrase)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Submission>> GetPendingAsync()
        {
            return await _context.Submissions
                .Include(s => s.Phrase)
                .Where(s => !s.IsApproved)
                .ToListAsync();
        }

        public async Task<Submission> UpdateAsync(Submission submission)
        {
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();
            return submission;
        }
    }
}
