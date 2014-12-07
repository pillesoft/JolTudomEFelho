-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_CleanupTests]
	-- Add the parameters for the stored procedure here
	@timeout INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY

		--végrehajtás
		IF @error = 0
			BEGIN
				DELETE FROM [JolTudomE].[test].[Results] WHERE [TestID] IN (SELECT [TestID] FROM [JolTudomE].[test].[Times] WHERE DATEDIFF(HH, [Generated], GETDATE()) >= @timeout AND [Completed] IS NULL)
				DELETE FROM [JolTudomE].[mobile].[Events] WHERE [TestID] IN (SELECT [TestID] FROM [JolTudomE].[test].[Times] WHERE DATEDIFF(HH, [Generated], GETDATE()) >= @timeout AND [Completed] IS NULL)
				DELETE FROM [JolTudomE].[test].[Times] WHERE DATEDIFF(HH, [Generated], GETDATE()) >= @timeout AND [Completed] IS NULL
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Cleanup Tests Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

