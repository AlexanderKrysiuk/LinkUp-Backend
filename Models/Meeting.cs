namespace LinkUpBackend.Models;
public class Meeting{
    public Guid Id {get;set;}
    public DateTime DateTime {get;set;}
    public int MaxParticipant {get;set;}
    public int Duration {get;set;}
    public string Description {get;set;}
}