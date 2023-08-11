namespace LinkUp.Models;

public class FreeTerm
{
    public Guid Id {get;}
    public Guid ContractorId {get;}
    public DateTime StartDateTime {get;}
    public DateTime EndDateTime {get;}

    public FreeTerm(Guid Id, Guid ContractorId, DateTime StartDateTime, DateTime EndDateTime)
    {
        this.Id = Id;
        this.ContractorId = ContractorId;
        this.StartDateTime = StartDateTime;
        this.EndDateTime = EndDateTime;
    }
}