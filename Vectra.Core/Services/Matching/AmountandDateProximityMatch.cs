using Vectra.Core.Entities;
using Vectra.Core.Interfaces;

namespace Vectra.Core.Services;

public class AmountandDateProximityMatch : IMatchingStrategy
{
    private const int MAX_DAYS = 2;
    public string StrategyName => "Amount & Date Proximity Match";

    public List<MatchResult> Match(List<Transaction> internalRecords, List<Transaction> externalRecords)
    {
        if (internalRecords == null || externalRecords == null)
        {
            return new List<MatchResult>();
        }
        List<MatchResult> results = new();
        HashSet<Guid> claimedInternalIds = new();
        HashSet<Guid> claimedExternalIds = new();

        var amountMatches = internalRecords.Join(externalRecords, i => i.Amount, e => e.Amount, (i, e) => new { Internal = i, External = e });
        var proximityMatches = amountMatches.Where(a => Math.Abs((a.Internal.TransactionDate - a.External.TransactionDate).TotalDays) <= MAX_DAYS);

        foreach (var item in proximityMatches)
        {
            if (claimedInternalIds.Contains(item.Internal.Id) || claimedExternalIds.Contains(item.External.Id))
            {
                continue;
            }
            decimal score = default;
            var dayDifference = Math.Abs((item.Internal.TransactionDate - item.External.TransactionDate).TotalDays);
            if (item.Internal.TransactionDate.Day == item.External.TransactionDate.Day)
            {
                score = 0.95m;
            }
            else if (dayDifference > 0.95 && dayDifference <= 1.05)
            {
                score = 0.9m;
            }
            else if (dayDifference > 1.95 && dayDifference <= 2.05)
            {
                score = 0.85m;
            }
            claimedInternalIds.Add(item.Internal.Id);
            claimedExternalIds.Add(item.External.Id);
            results.Add(new MatchResult
            {
                Id = Guid.NewGuid(),
                InternalTransactionId = item.Internal.Id,
                ExternalTransactionId = item.External.Id,
                InternalTransactionReference = item.Internal.ReferenceCode!,
                ExternalTransactionReference = item.External.ReferenceCode!,
                ConfidenceScore = score,
                MatchingStrategy = MatchResult.MatchStrategy.AmountAndDateProximity,
                Status = MatchResult.MatchStatus.ProbableMatch,
                MatchedAt = DateTimeOffset.UtcNow,
                Reason = "Automated matching verified identical reference code and proximity match on transaction date parameters."
            });
        }
        return results;
    }
}
