namespace practice4.Client.Service;

public interface IAccount
{
    public User user { get; set; }

}
public class Account : IAccount
{
    public User user { get; set; }

}
