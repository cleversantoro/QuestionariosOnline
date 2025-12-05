using Microsoft.EntityFrameworkCore;
using Questionarios.Domain.Entities;

namespace Questionarios.Infrastructure;

public class QuestionariosDbContext : DbContext
{
    public QuestionariosDbContext(DbContextOptions<QuestionariosDbContext> options)
        : base(options) { }

    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Response> Responses => Set<Response>();
    public DbSet<ResponseItem> ResponseItems => Set<ResponseItem>();
    public DbSet<AggregatedResult> AggregatedResults => Set<AggregatedResult>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Survey>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.HasMany(x => x.Questions).WithOne(q => q.Survey).HasForeignKey(q => q.SurveyId);
        });

        modelBuilder.Entity<Question>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Text).HasMaxLength(500).IsRequired();
            e.HasMany(x => x.Options).WithOne(o => o.Question).HasForeignKey(o => o.QuestionId);
        });

        modelBuilder.Entity<Option>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Text).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Response>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Items).WithOne(i => i.Response).HasForeignKey(i => i.ResponseId);
        });

        modelBuilder.Entity<ResponseItem>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<AggregatedResult>(e =>
        {
            e.HasKey(x => x.SurveyId);
        });

        modelBuilder.Entity<Question>(e =>
        {
            e.Property(x => x.Order).IsRequired(false);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
