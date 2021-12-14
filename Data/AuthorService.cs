using System.Data;
using Dapper;

namespace Conference.Data;

public class AuthorService
{
    private readonly IDbConnection _dbConnection;

    public AuthorService(IDbConnection dbConnection) => 
        _dbConnection = dbConnection;

    public IReadOnlyList<Author> GetAuthors() =>
        _dbConnection.Query<Author>("SELECT id AS authorId, name, surname FROM author").ToList();

    public void CreateAuthor(CreateAuthor model)
    {
        if (model.Name is null || model.Surname is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        _dbConnection.Execute(
            $"INSERT INTO author (name, surname) VALUES (@Name, @Surname);",
            new { model.Name, model.Surname });
    }
}