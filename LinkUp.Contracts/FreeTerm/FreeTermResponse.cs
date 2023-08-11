namespace LinkUp.Contracts.FreeTerm;

public record FreeTermResponse(
    Guid Id,
    Guid ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
);