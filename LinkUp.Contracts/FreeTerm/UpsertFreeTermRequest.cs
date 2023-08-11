namespace LinkUp.Contracts.FreeTerm;

public record UpsertFreeTermRequest(
    Guid Id,
    Contractor.Id ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
)