using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vectra.Core.Entities;

namespace Vectra.Core.Services
{

    public sealed class DataNormalizer
    {
        /*
        builder.Services.AddSingleton<DataNormalizer>(sp => 
        new DataNormalizer(abbreviations, fillerWords));
        */
        private string DEFAULT_TIMEZONE = "W. Central Africa Standard Time";
        private readonly Dictionary<string, string> _abbreviationDictionary;
        private readonly HashSet<string> _fillerWordsTextSet;
        /// <summary>
        /// Constructor accepts dynamic dictionaries loaded from configuration options.
        /// </summary>
        public DataNormalizer(Dictionary<string, string>? abbreviations = null, IEnumerable<string>? fillerWords = null)
        {
            _abbreviationDictionary = abbreviations ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _fillerWordsTextSet = fillerWords != null
                ? new HashSet<string>(fillerWords, StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        private void NormaizeAmount(Transaction transaction)
        {
            if (transaction is null) return;
            if (transaction.Amount < 0)
            {
                transaction.Amount = Math.Abs(transaction.Amount);
                transaction.Type = Transaction.TransactionType.Debit;
            }
            else { transaction.Type = Transaction.TransactionType.Credit; }

            // matching shouldn't fail due to micro-cent differences, thus 4 place precision
            transaction.Amount = Math.Round(transaction.Amount, 4);
        }

        private void TimezoneNormalization(Transaction transaction)
        {
            if (transaction == null) return;
            bool isNaive = transaction.DataQualityFlags.Contains(Transaction.DataQualityFlag.TimezoneNaiveDatetime);
            if (isNaive)
            {
                TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(DEFAULT_TIMEZONE);
                DateTime rawClockTime = transaction.TransactionDate.DateTime;
                DateTimeOffset correctedOffsetTime = new DateTimeOffset(rawClockTime, info.GetUtcOffset(rawClockTime));
                transaction.TransactionDate = correctedOffsetTime.ToUniversalTime();
            }
            else { transaction.TransactionDate = transaction.TransactionDate.ToUniversalTime(); }


            if (transaction.SettlementDate.HasValue)
            {
                transaction.SettlementDate = transaction.SettlementDate.Value.ToUniversalTime();
            }

        }
        private void DescriptionNormalization(Transaction transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.RawDescription) || transaction == null) return;
            string text = transaction.RawDescription.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z\s]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = ExpandAndCleanWords(text);
            transaction.NormalisedDescription = text;

            string ExpandAndCleanWords(string cleanText)
            {
                string[] words = cleanText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                List<string> processedWords = new List<string>();

                foreach (string word in words)
                {
                    if (_fillerWordsTextSet.Contains(word))
                    {
                        continue;
                    }

                    if (_abbreviationDictionary.TryGetValue(word, out string? expandedWord))
                    {
                        processedWords.Add(expandedWord);
                    }
                    else
                    {
                        processedWords.Add(word);
                    }
                }
                return string.Join(" ", processedWords);
            }


        }

        private (List<Transaction> CleanRecords, List<ReconciliationException> Exceptions) DuplicatePreScreening(List<Transaction> transactions, Guid JobId)
        {
            if (transactions == null || transactions.Count <= 1) return (new List<Transaction>(), new List<ReconciliationException>());
            List<Transaction> cleanRecords = new();
            List<ReconciliationException> exceptions = new();

            var sortedTransactions = transactions.GroupBy(x => new { x.Amount, x.TransactionDate.Date, x.NormalisedDescription }).ToList();
            foreach (var item in sortedTransactions)
            {
                var transactionsGroup = item.ToList();
                if (item.Count() == 1)
                {
                    cleanRecords.Add(transactionsGroup[0]);
                }
                else if (item.Count() > 1)
                {
                    exceptions.Add(new ReconciliationException
                    {
                        Type = ReconciliationException.ReconciliationExceptionType.DuplicateDetected,
                        JobId = JobId,
                        AffectedTransactionIds = transactionsGroup.Select(t => t.Id).ToList(),
                        Description = "Multiple identical transactions detected for the same amount, date, and normalized description.",
                    });
                }
                else { continue; }
            }

            return (cleanRecords, exceptions);
        }

    }


}
