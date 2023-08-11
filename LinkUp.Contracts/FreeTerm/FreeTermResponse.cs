namespace LinkUp.Contracts.FreeTerm;

public record CreateFreeTermRequest(
    Guid Id,
    Contractor.Id ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
)