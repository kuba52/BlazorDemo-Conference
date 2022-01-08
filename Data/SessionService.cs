using System.Data;
using Dapper;

namespace Conference.Data;

public sealed class SessionService
{
    private readonly IDbConnection _dbConnection;

    public SessionService(IDbConnection dbConnection) =>
        _dbConnection = dbConnection;

    public IReadOnlyList<Session> GetSessions() {

        var paperDictionary = new Dictionary<int, Paper>();

        PaperService t = new PaperService(_dbConnection);

        foreach (var paper in t.GetPapers().OrderBy(a => a.Id)) {
            paperDictionary.Add(paper.Id, paper);
        }
 
        var sessionDictionary = new Dictionary<int, Session>();

        var result = _dbConnection.Query<Session, Author, Lecture, Author, int, Session>(
            @"SELECT s.id as sessionId, s.when AS when,
                a2.id AS authorId, a2.name AS name, a2.surname AS surname,
                l.id AS lectureId, l.when AS when,
                a1.id AS authorId, a1.name AS name, a1.surname AS surname,
                l.paper_id As paperId
            FROM session s JOIN lecture l on s.id = l.session_id
                JOIN author a1 ON l.speaker_id = a1.id JOIN author a2 ON s.chair_id = a2.id",

            (s, a2, l, a1, paperId) =>
            {
                Session? session;

                if (!sessionDictionary.TryGetValue(s.Id, out session))
                {
                    session = s;
                    sessionDictionary.Add(s.Id, session);
                }

                session.Chair = a2;

                Paper? paper;
                                
                if (l is not null) {
                    paperDictionary.TryGetValue(paperId, out paper);
                    l.Paper = paper;
                    l.Speaker = a1;
                    l.Session = session;
                    session.Lectures.Add(l);
                } 

                return session;              
            },
            splitOn: "authorId,lectureId,authorId,paperId");

            return result.Distinct().ToList(); 

    }

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