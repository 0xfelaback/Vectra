using System.Transactions;

namespace Vectra.Core.Interfaces
{
    public interface IMatchingStrategy
    {
        string StrategyName { get; }
        int Priority { get; }
        MatchCandidate? Match(Transaction internalTx, Transaction externalTx, object? configuration = null);
    }

    public class MatchCandidate
    {
        public Transaction InternalTransaction { get; set; } = null!;
        public Transaction ExternalTransaction { get; set; } = null!;
        public decimal ConfidenceScore { get; set; }
        public string StrategyName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
