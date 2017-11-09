IF EXISTS (
		SELECT *
		FROM [sysobjects]
		WHERE [id] = OBJECT_ID(N'[dbo].[spAnalytics_ETL_Campus]')
			AND OBJECTPROPERTY([id], N'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].spAnalytics_ETL_Campus
GO

-- EXECUTE [dbo].[spAnalytics_ETL_Campus] 
CREATE PROCEDURE [dbo].spAnalytics_ETL_Campus
AS
BEGIN
	DECLARE @RowsUpdated INT = 0
		,@RowsInserted INT = 0

	INSERT INTO AnalyticsSourceCampus (
		[CampusId]
		,[Name]
		,[Description]
		,[IsActive]
		,[ShortCode]
		,[Url]
		,[LocationId]
		,[PhoneNumber]
		,[LeaderPersonAliasId]
		,[ServiceTimes]
		,[Order]
		,[Count]
		,[Guid]
		)
	SELECT [Id]
		,[Name]
		,[Description]
		,[IsActive]
		,[ShortCode]
		,[Url]
		,[LocationId]
		,[PhoneNumber]
		,[LeaderPersonAliasId]
		,[ServiceTimes]
		,[Order]
		,1
		,NEWID()
	FROM Campus
	WHERE Id NOT IN (
			SELECT CampusId
			FROM AnalyticsSourceCampus
			)

	SET @RowsInserted = @@ROWCOUNT;

	UPDATE sc 
	SET sc.[CampusId] = c.[Id]
		,sc.[Name] = c.[Name]
		,sc.[Description] = c.[Description]
		,sc.[IsActive] = c.[IsActive]
		,sc.[ShortCode] = c.[ShortCode]
		,sc.[Url] = c.[Url]
		,sc.[LocationId] = c.[LocationId]
		,sc.[PhoneNumber] = c.[PhoneNumber]
		,sc.[LeaderPersonAliasId] = c.[LeaderPersonAliasId]
		,sc.[ServiceTimes] = c.[ServiceTimes]
		,sc.[Order] = c.[Order]
	FROM AnalyticsSourceCampus sc
	join [Campus] c on sc.[CampusId] = c.[Id]
	WHERE sc.[Name] != c.[Name]
		AND sc.[Description] != c.[Description]
		AND sc.[IsActive] != c.[IsActive]
		AND sc.[ShortCode] != c.[ShortCode]
		AND sc.[Url] != c.[Url]
		AND sc.[LocationId] != c.[LocationId]
		AND sc.[PhoneNumber] != c.[PhoneNumber]
		AND sc.[LeaderPersonAliasId] != c.[LeaderPersonAliasId]
		AND sc.[ServiceTimes] != c.[ServiceTimes]
		AND sc.[Order] != c.[Order]

	SET @RowsUpdated = @@ROWCOUNT;

	-- delete any Family records that no longer exist the [Group] table (or are no longer GroupType of family)
	DELETE
	FROM AnalyticsSourceCampus
	WHERE CampusId NOT IN (
			SELECT Id
			FROM [Campus]
			)

	SELECT @RowsInserted [RowsInserted]
		,@RowsUpdated [RowsUpdated]
END
