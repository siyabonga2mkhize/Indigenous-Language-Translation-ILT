namespace PhraseBookk.Models
{
    public class Chatboard
    {
        // Store FAQ items as Key-Value pairs (Question -> Answer)
        private readonly Dictionary<string, string> _faqData;

        public Chatboard(IConfiguration config)
        {
            _faqData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "registration office", "Registration office is at Steve Biko." },
                { "where is the registration office", "Registration office is at Steve Biko." },
                { "where is the campus clinic", "At Ritson and Steve Biko campus." },
                { "campus clinic", "At Ritson and Steve Biko campus." },
                { "where is libriary", "we have 2 at Steve and ML Sultan." },
                { "libriary", "we have 2 at Steve and ML Sultan." }
            };
        }

        public Task<string> AskAsync(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion))
            {
                return Task.FromResult("Please enter a question.");
            }

            string cleanQuery = userQuestion.ToLower().Trim();

            // Search for matching keywords in the user question
            foreach (var faq in _faqData)
            {
                if (cleanQuery.Contains(faq.Key))
                {
                    return Task.FromResult(faq.Value);
                }
            }

            // Fallback response if no keyword matched
            return Task.FromResult("I don't know, please ask on campus.");
        }
    }
}
