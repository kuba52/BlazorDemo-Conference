namespace Conference.Data;

public class Paper 
{
    public int Id { get; init; }

    public string Name { get; init; }

    public string Classification { get; init; }

    public List<Author> Authors { get; init; } = new List<Author>();

    public Paper(int paperId, string name, string classification) =>
        (Id, Name, Classification) = (paperId, name, classification);
}