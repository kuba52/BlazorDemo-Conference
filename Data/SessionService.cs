using System.Data;
using Dapper;

namespace Conference.Data;

public sealed class SessionService
{
    private readonly IDbConnection _dbConnection;

    public SessionService(IDbConnection dbConnection) =>
        _dbConnection = dbConnection;

    public IReadOnlyList<Session> GetSessions() =>
        _dbConnection.Query<Session, Author, Session>(
            @"SELECT s.id AS sessionId, s.when, a.id AS authorId, a.name, a.surname 
                FROM session s
                JOIN author a
                    ON s.chair_id = a.id",
            (s, a) =>
            {
                s.Chair = a;
                return s;
            },
            splitOn: "authorId")
            .ToList();

    public void CreateSession(CreateSession model)
    {
        if (model.ChairId is null)
        {
            throw new ArgumentNullException();
        }

        _dbConnection.Execute(
            $"INSERT INTO session (\"when\", chair_id) VALUES (@When, @ChairId);",
            new { model.When, model.ChairId });
    }
}