namespace LinkUp.Contracts.FreeTerm;

public record CreateFreeTermRequest(
    Contractor.Id ContractorId,
    DateTime StartDateTime,
    DateTime EndDateTime
)