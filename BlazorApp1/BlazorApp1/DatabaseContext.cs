using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practice4;

public class DatabaseContext : DbContext
{
    public DbSet<User> users { get; set; }
    public DbSet<Message> messages { get; set; }

    public DatabaseContext(bool createAgain)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
        Database.GenerateCreateScript();
    }

    public DatabaseContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=Roflanxlebalo;Database=kekw;Port=5432");
    }
}

public class User
{
    [Key]
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string FIO { get; set; }

    [InverseProperty("Sender")]
    public ICollection<Message> Senders { get; set; } = [];
    [InverseProperty("Receiver")]
    public ICollection<Message> Receivers { get; set; } = [];
}

public class Message
{
    [Key]
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }

    public User Receiver { get; init; }
    public User Sender { get; init; }

    public string Title { get; set; }
    public string Content { get; set; } 
    public DateTime Timestamp { get; set; }
    public bool isRead { get; set; }
}