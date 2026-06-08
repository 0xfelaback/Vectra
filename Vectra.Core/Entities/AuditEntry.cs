namespace Vectra.Core.Entities
{
    /// <summary>
    /// CORE-DM-004: AuditEntry Entity
    /// Represents an immutable audit trail entry for system operations and changes.
    /// </summary>
    public class AuditEntry
    {
        // Target Identification
        public long Id { get; private set; } // append-only tables typically benefit from sequential identity keys
        public TargetEntity TargetEntityType { get; private set; }
        public string TargetEntityId { get; private set; } = string.Empty; // stored as string to support both Guid and composite keys

        // Context Elements
        public string ActionPerformed { get; private set; } = string.Empty; // e.g., "StatusChanged", "ManualResolution"
        public string Actor { get; private set; } = string.Empty; // e.g., "System", "User:admin@company.com"
        public DateTimeOffset Timestamp { get; private set; }

        // State Snapshots
        public string BeforeState { get; private set; } = string.Empty; // Serialized JSON payload before change
        public string AfterState { get; private set; } = string.Empty; // Serialized JSON payload after change

        /// <summary>
        /// Parameterized constructor enforces data immutability during application creation.
        /// </summary>
        public AuditEntry(
            string targetEntityId,
            string actionPerformed,
            string actor,
            DateTimeOffset timestamp,
            string beforeState,
            string afterState)
        {
            TargetEntityId = string.IsNullOrEmpty(targetEntityId) ? throw new ArgumentNullException(nameof(targetEntityId)) : targetEntityId;
            ActionPerformed = string.IsNullOrEmpty(actionPerformed) ? throw new ArgumentNullException(nameof(actionPerformed)) : actionPerformed;
            Actor = string.IsNullOrEmpty(actor) ? throw new ArgumentNullException(nameof(actor)) : actor;
            Timestamp = timestamp;
            BeforeState = beforeState ?? throw new ArgumentNullException(nameof(beforeState));
            AfterState = afterState ?? throw new ArgumentNullException(nameof(afterState));
        }

        public enum TargetEntity
        {
            Transaction = 1, MatchResult, ReconciliationJob
        }
    }
}
