WHILE (SELECT COUNT(*) FROM (
	SELECT FULLTEXTCATALOGPROPERTY(fts.[name], 'PopulateStatus') as [Status]
	FROM sys.dm_fts_active_catalogs fts
	WHERE fts.database_id = DB_ID()
	  AND (@IndexName IS NULL OR @IndexName = fts.[name])) as t
WHERE t.[Status] in (1,6,7,9)) > 0
BEGIN
	WAITFOR DELAY '00:00:00.100'
END