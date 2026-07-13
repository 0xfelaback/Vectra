using Vectra.Core.Entities;
using Vectra.Core.Interfaces;
using Vectra.Core.Services;

public class ReconciliationEngine
{
    private readonly List<IMatchingStrategy> _strategies;

    public ReconciliationEngine(ExactReferenceandAmountMatch exactStrategy, AmountandDateProximityMatch proximityStrategy, MultiFactorFuzzyMatch fuzzyStrategy)
    {
        _strategies = new List<IMatchingStrategy> { exactStrategy, proximityStrategy, fuzzyStrategy };
    }

    public List<MatchResult> RunReconciliation(List<Transaction> internalRecords, List<Transaction> externalRecords)
    {
        List<MatchResult> combinedResults = new();
        List<Transaction> activeInternal = internalRecords.ToList();
        List<Transaction> activeExternal = externalRecords.ToList();

        foreach (var strategy in _strategies)
        {
            List<MatchResult> strategyMatches = strategy.Match(activeInternal, activeExternal);
            if (!strategyMatches.Any()) continue;
            combinedResults.AddRange(strategyMatches);

            var matchedInternalIds = strategyMatches.Select(m => m.InternalTransactionId).ToHashSet();
            var matchedExternalIds = strategyMatches.Select(m => m.ExternalTransactionId).ToHashSet();

            activeInternal = activeInternal.Where(i => !matchedInternalIds.Contains(i.Id)).ToList();
            activeExternal = activeExternal.Where(e => !matchedExternalIds.Contains(e.Id)).ToList();
        }
        return combinedResults;
    }
}
