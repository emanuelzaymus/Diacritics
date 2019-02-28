--DELETE FROM dbo.UniGramEntities;
--DELETE FROM dbo.Words;

SELECT COUNT(*) FROM dbo.Words;
SELECT COUNT(*) FROM dbo.UniGramEntities;
SELECT * FROM dbo.UniGramEntities WHERE WordId = -1;