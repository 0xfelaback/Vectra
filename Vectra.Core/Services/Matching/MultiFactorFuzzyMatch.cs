using Vectra.Core.Entities;
using Vectra.Core.Interfaces;

namespace Vectra.Core.Services;

public class MultiFactorFuzzyMatch : IMatchingStrategy
{
    public string StrategyName => "Multi-Factor Fuzzy Match";
    private float DescriptionSimilarityScore(string description1, string description2)
    {
        // Levenshtein distance algorithm
        int m = description1.Length;
        int n = description2.Length;

        int[,] dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
            dp[i, 0] = i;

        for (int j = 1; j <= n; j++)
            dp[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (description1[i - 1] == description2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j - 1], dp[i - 1, j]),
                        dp[i, j - 1]
                    ) + 1;
                }
            }
        }

        int distance = dp[m, n];
        // normalize to 0.0 - 1.0
        int maxLen = Math.Max(m, n);

        if (maxLen == 0) return 1.0f;
        return 1.0f - ((float)distance / maxLen);
    }
    private float DateProximityScore(DateTimeOffset date1, DateTimeOffset date2)
    {
        TimeSpan difference = date1 - date2;
        int daysApart = Math.Abs(difference.Days);

        return daysApart switch
        {
            0 => 1.0f,
            1 => 0.8f,
            2 => 0.6f,
            _ => 0.0f
        };
    }

    private float AmountVarianceScore(decimal amount1, decimal amount2)
    {
        if (amount1 == amount2) return 1.0f;

        decimal difference = Math.Abs(amount1 - amount2);
        const decimal tolerance = 0.05m; // 5 cents tolerance for minor fees/rounding

        if (difference <= tolerance)
        {
            return 1.0f - ((float)(difference / tolerance) * 0.5f);
        }

        return 0.0f;
    }

    public List<MatchResult> Match(List<Transaction> internalRecords, List<Transaction> externalRecords)
    {
        // TODO: FIX You are collecting all unique IDs, assuming an internal record and its matching external record share the exact same Id. In a real system, the ledger row and the bank row always have completely different database Id values!
        //var allRecordIds = internalRecords.Select(i => i.ReferenceCode).Union(externalRecords.Select(i => i.ReferenceCode));
        //var Internals = internalRecords.ToDictionary(i => i.Id);
        //var Externals = externalRecords.ToDictionary(i => i.Id);

        List<MatchResult> results = new();
        HashSet<Guid> claimedInternalIds = new();
        HashSet<Guid> claimedExternalIds = new();

        foreach (var record in internalRecords)
        {
            foreach (var item in externalRecords)
            {
                if (claimedInternalIds.Contains(record.Id) || claimedExternalIds.Contains(item.Id))
                {
                    continue;
                }
                var dayDifference = Math.Abs((record.TransactionDate - item.TransactionDate).TotalDays);
                if (dayDifference > 3) continue;

                float descriptionScore = DescriptionSimilarityScore(record!.NormalisedDescription, item!.NormalisedDescription);
                float dateScore = DateProximityScore(record.TransactionDate, item.TransactionDate);
                float amountScore = AmountVarianceScore(record.Amount, item.Amount);
                decimal finalScore = (decimal)((descriptionScore * 0.5) + (dateScore * 0.3) + (amountScore * 0.2));

                if (finalScore >= 0.70m)
                {
                    claimedInternalIds.Add(record.Id);
                    claimedExternalIds.Add(item.Id);
                    results.Add(new MatchResult
                    {
                        Id = Guid.NewGuid(),
                        InternalTransactionId = record.Id,
                        ExternalTransactionId = item.Id,
                        InternalTransactionReference = record.ReferenceCode!,
                        ExternalTransactionReference = item.ReferenceCode!,
                        ConfidenceScore = finalScore,
                        MatchingStrategy = MatchResult.MatchStrategy.MultiFactorFuzzy,
                        Status = MatchResult.MatchStatus.ProbableMatch,
                        MatchedAt = DateTimeOffset.UtcNow,
                        Reason = $"Multi-factor metrics aggregated a composite confidence score of {finalScore:P0}."
                    });
                }
            }
        }

        return results;
    }
}
