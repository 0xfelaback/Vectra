using Vectra.Core.Entities;

namespace Vectra.Core.Services
{
    public class MatchingEngine
    {
        public MatchResult ExactMatch(Transaction internalTransaction, Transaction externalTransaction) { }
        public MatchResult FuzzyMatch(Transaction internalTransaction, Transaction externalTransaction) { }
        public MatchResult RuleBasedMatch(Transaction internalTransaction, Transaction externalTransaction) { }
        public MatchResult PartialAmountMatch(Transaction internalTransaction, Transaction externalTransaction) { }
    }
}
