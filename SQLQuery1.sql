
SELECT COUNT(*) FROM dbo.Words;
SELECT COUNT(*) FROM dbo.UniGramEntities;

SELECT MIN(Id) FROM dbo.Words;
SELECT MAX(Id) FROM dbo.Words;

--SELECT * FROM dbo.UniGramEntities WHERE WordId = -1;
--SELECT * FROM dbo.Words ORDER BY Id;
	
SELECT Word1 FROM dbo.UniGramEntities WHERE WordId = 12345 ORDER BY Frequency DESC;
		