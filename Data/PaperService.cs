using System.Data;
using Dapper;

namespace Conference.Data;

public class PaperService
{
    private readonly IDbConnection _dbConnection;

    public PaperService(IDbConnection dbConnection) => _dbConnection = dbConnection;

    public IReadOnlyList<Paper> GetPapers()
    {
        var paperDictionary = new Dictionary<int, Paper>();

        var papers = _dbConnection.Query<Paper, Author, Paper>(
            @"SELECT p.id AS paperId, p.name, p.classification, a.id AS authorId, a.name, a.surname 
                FROM paper p
                LEFT JOIN paper_author pa
                    ON p.id = pa.paper_id
                LEFT JOIN author a
                    ON pa.author_id = a.id",
            (p, a) => 
            {
                Paper? paper;

                if (!paperDictionary.TryGetValue(p.Id, out paper))
                {
                    paper = p;
                    paperDictionary.Add(paper.Id, paper);
                }

                if (a is not null)
                {
                    paper.Authors.Add(a);
                }

                return paper;
            },
            splitOn: "authorId");

        return papers.Distinct().ToList();
    }

    public void CreatePaper(CreatePaper model)
    {
        if (model.Name is null || model.Classification is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        using var transaction = _dbConnection.BeginTransaction();

        var paperId = _dbConnection.QuerySingle<int>(
            $"INSERT INTO paper (name, classification) VALUES (@Name, @Classification) RETURNING id;",
            new { model.Name, model.Classification });

        _dbConnection.Execute(
            $"INSERT INTO paper_author (paper_id, author_id) VALUES (@PaperId, @AuthorId);",
            model.AuthorIds.Select(a => new {PaperId = paperId, AuthorId = a})
        );

        transaction.Commit();
    }
}