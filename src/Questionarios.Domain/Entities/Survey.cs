namespace Questionarios.Domain.Entities;

public class Survey
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = default!;
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public bool IsClosed { get; private set; }

    public ICollection<Question> Questions { get; private set; } = new List<Question>();

    private Survey() { }

    public Survey(string title, DateTime startAt, DateTime endAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (endAt <= startAt)
            throw new ArgumentException("EndAt must be greater than StartAt.");

        Title = title;
        StartAt = startAt;
        EndAt = endAt;
    }

    public void Update(string title, DateTime startAt, DateTime endAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (endAt <= startAt)
            throw new ArgumentException("EndAt must be greater than StartAt.");

        Title = title;
        StartAt = startAt;
        EndAt = endAt;
    }

    public bool IsActive(DateTime now) =>
        !IsClosed && now >= StartAt && now <= EndAt;

    public void Close() => IsClosed = true;
}
