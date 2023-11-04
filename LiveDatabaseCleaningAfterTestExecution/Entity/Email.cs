namespace LiveDatabaseCleaningAfterTestExecution.Entity;

public class Email
{
    public string BusinessEntityID { get; set; } = null!;
    public string EmailAddressID { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public DateTime ModifiedDate { get; set; }
}
