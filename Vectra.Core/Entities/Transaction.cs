using System.ComponentModel.DataAnnotations;

namespace Vectra.Core.Entities
{
    /// <summary>
    /// CORE-DM-001: Transaction Entity
    /// Represents a first-class financial transaction domain entity.
    /// </summary>
    public class Transaction
    {
        public Guid Id { get; set; }
        [Required]
        public DateTimeOffset TransactionDate { get; set; }
        [Required]
        public DateTimeOffset? SettlementDate { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "CurrencyCode must be a valid 3-letter ISO 4217 code.")]
        public string CurrencyCode { get; set; } = string.Empty;
        [Required]
        public TransactionType Type { get; set; }
        public string RawDescription { get; set; } = string.Empty;
        public string NormalisedDescription { get; set; } = string.Empty;
        public string DataSourceIdentifier { get; set; } = string.Empty;  // source origin tag
        public string? ReferenceCode { get; set; }
        public string? CounterpartyName { get; set; }
        public List<DataQualityFlag> DataQualityFlags { get; set; } = new();
        public DateTimeOffset IngestionTimestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Represents the financial direction of the transaction.
        /// </summary>
        public enum TransactionType
        {
            Debit = 1,
            Credit
        }
        public enum DataQualityFlag
        {
            None = 0,
            MissingReferenceCode = 1,
            TimezoneNaiveDatetime = 2,
            DescriptionTruncated = 3,
            DecimalFormattingDiscrepancy = 4
        }
    }

}
