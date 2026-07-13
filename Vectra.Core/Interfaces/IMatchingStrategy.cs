using Vectra.Core.Entities;

namespace Vectra.Core.Interfaces
{
    public interface IMatchingStrategy
    {
        string StrategyName { get; }
        List<MatchResult> Match(List<Transaction> internalRecords, List<Transaction> externalRecords);
    }

}
