using System.Transactions;

namespace Vectra.Core.Interfaces
{
    public interface IReconciliationEngine
    {
        Task<ReconciliationJobResult> ExecuteRunAsync(
                IEnumerable<Transaction> internalRecords,
                IEnumerable<Transaction> externalRecords,
                object? configuration = null);
    }
}
