using Vectra.Core.Entities;
using Vectra.Core.Interfaces;

namespace Vectra.Core.Services
{
    public class ExactReferenceandAmountMatch : IMatchingStrategy
    {
        string IMatchingStrategy.StrategyName => "Exact Reference & Amount Match";

        public List<MatchResult> Match(List<Transaction> internalRecords, List<Transaction> externalRecords)
        {
            if (internalRecords == null || externalRecords == null)
            {
                return new List<MatchResult>();
            }
            List<MatchResult> results = new();
            var exactMatches = internalRecords.Where(i => i.ReferenceCode != null).Join(externalRecords.Where(e => e.ReferenceCode != null), i => new { i.ReferenceCode, i.Amount }, e => new { e.ReferenceCode, e.Amount }, (i, e) => new { Internal = i, External = e }).ToList();

            foreach (var item in exactMatches)
            {
                results.Add(new MatchResult
                {
                    Id = Guid.NewGuid(),
                    InternalTransactionId = item.Internal.Id,
                    ExternalTransactionId = item.External.Id,
                    InternalTransactionReference = item.Internal.ReferenceCode!,
                    ExternalTransactionReference = item.External.ReferenceCode!,
                    ConfidenceScore = 1.0m,
                    MatchingStrategy = MatchResult.MatchStrategy.ExactReferenceAndAmount,
                    Status = MatchResult.MatchStatus.Confirmed,
                    MatchedAt = DateTimeOffset.UtcNow,
                    Reason = "Automated matching verified identical reference code and absolute transaction amount parameters."
                });
            }
            return results;
        }
    }
}
