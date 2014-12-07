-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [mobile].[usp_AddEvent]
	-- Add the parameters for the stored procedure here
	@testid INT,
	@eventid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--eventid csak 0 vagy 1 lehet
		IF (SELECT @eventid) NOT IN (0, 1)
		SET @error = 1

		--létezik-e a teszt
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid)
		SET @error+= 10

		--resume csak suspend után jöhet
		IF (SELECT TOP 1 [EventID] FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid ORDER BY RowID DESC) <> 1 AND @eventid = 0
		SET @error+= 100

		--suspend után nem jöhet suspend
		IF (SELECT TOP 1 [EventID] FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid ORDER BY RowID DESC) = 1 AND @eventid = 1
		SET @error+= 1000

		--egy teszt első eseménye csak suspend lehet
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid) AND @eventid <> 1
		SET @error+= 10000

		--lezáratlan-e a teszt
		IF (SELECT [Completed] FROM [JolTudomE].[test].[Times] WHERE [TestID] = @testid) IS NOT NULL
		SET @error+= 100000
		
		--végrehajtás
		IF @error = 0
		BEGIN
			INSERT INTO [JolTudomE].[mobile].[Events] ([TestID], [SequenceID], [EventID], [TimeStamp])
			SELECT
				@testid
				,CASE
					WHEN @eventid = 1
						THEN (SELECT ISNULL(MAX([SequenceID]), 0)+1 FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid)
					WHEN @eventid IN (0, 2)
						THEN (SELECT ISNULL(MAX([SequenceID]), 0) FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid)
				 END
				,@eventid
				,getdate()
		END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'AddEvent Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

