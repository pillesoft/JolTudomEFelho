
CREATE PROCEDURE [users].[usp_SessionsCleanup]
	-- Add the parameters for the stored procedure here
	@timeout int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--van-e élõ session
		--IF EXISTS (SELECT * FROM [master].[sys].[sysprocesses] WHERE [program_name] = 'EntityFramework')
		--SET @error = 1

		--Végrehajtás
		IF @error = 0
			BEGIN
				DELETE FROM [JolTudomE].[users].[Sessions]
				WHERE [LastAction] < DATEADD(MINUTE, -@timeout, GETUTCDATE())
			END
		--ELSE
			--RAISERROR('Sessions Cleanup Error. Error Code: %u', 16, 1, @error)
			--WITH SETERROR
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

