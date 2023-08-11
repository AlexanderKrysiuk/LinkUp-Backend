namespace LinkUp.Contracts.FreeTerm;

public record UpsertFreeTermRequest(
    Guid ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
);