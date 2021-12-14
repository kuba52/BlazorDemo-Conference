namespace Conference.Data;

public class Author
{
    public int Id { get; init; }

    public string Name { get; init; }

    public string Surname { get; init; }

    public string DisplayName => $"{Surname}, {Name.First()}.";

    public Author(int authorId, string name, string surname) =>
        (Id, Name, Surname) = (authorId, name, surname);
}