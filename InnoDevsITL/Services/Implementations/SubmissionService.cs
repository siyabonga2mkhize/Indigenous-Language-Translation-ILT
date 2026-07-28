using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Services.Implementations
{
    public class SubmissionService : ISubmissionService
    {
        private readonly InnoDbContext _context;

        public SubmissionService(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetPendingSubmissionsAsync()
        {
            return await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .Where(s => !s.IsApproved)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<Submission> GetSubmissionByIdAsync(int id)
        {
            return await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Submission> CreateSubmissionAsync(Submission submission)
        {
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<Submission> UpdateSubmissionAsync(Submission submission)
        {
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<bool> ApproveSubmissionAsync(int id, string reviewerId)
        {
            var submission = await GetSubmissionByIdAsync(id);
            if (submission == null)
                return false;

            submission.IsApproved = true;
            submission.ReviewedBy = reviewerId;

            if (submission.Phrase != null)
            {
                submission.Phrase.IsActive = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectSubmissionAsync(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Phrase)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
                return false;

            if (submission.Phrase != null)
            {
                _context.Phrases.Remove(submission.Phrase);
            }

            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByUserAsync(string userId)
        {
            return await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }
    }
}