namespace LinkUp.Contracts.FreeTerm;

public record CreateFreeTermRequest(
    Guid ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
);