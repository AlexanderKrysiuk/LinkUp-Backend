namespace LinkUp.Models;

public class Contractor
{
    public Guid Id {get;}
    public string Name {get;}
    public string Email {get;}
    public string Password {get;}

    public Contractor(Guid Id, string Name, string Email, string Password)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email;
        this.Password = Password;
    }
}