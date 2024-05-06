namespace practice4.Client;
public class User
{
    public int Id { get; set; } = -1;
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string FIO { get; set; } = "";
}

public class Message
{
    public int Id { get; set; } = 0;
    public int SenderId { get; set; } = -1;
    public int ReceiverId { get; set; } = -1;
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime? Timestamp { get; set; } = null;
    public bool isRead { get; set; } = false;
    public string SenderFIO { get; set; } = "";
    public string RecieverFIO { get; set; } = "";
    public string RecieverLogin { get; set; } = "";
    public string SenderLogin { get; set; } = "";


}

public class Filter
{
    public bool OnlyUnRead { get; set; } = false;
    public IReadOnlyList<DateTime> DateRange { get; set; } 
        = [DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime()];

    public string SenderLogin = "";
}