using System.Transactions;

namespace Vectra.Core.Interfaces
{
    public interface IConfidenceScorer
    {
        decimal ComputeScore(Transaction internalTx, Transaction externalTx, object? configuration = null);
        Entities.MatchResult.MatchStatus ClassifyThreshold(decimal score, object? configuration = null);
    }
}
