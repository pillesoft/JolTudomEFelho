-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_CancelTest]
	-- Add the parameters for the stored procedure here
	@testid INT,
	@person INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY

		--saját tesz-e
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid AND [PersonID] = @person)
		SET @error = 1

		--lezáratlan teszt-e
		IF EXISTS (SELECT * FROM [JolTudomE].[test].[Times] WHERE [TestID] = @testid AND [Completed] IS NOT NULL)
		SET @error+= 10

		--végrehajtás
		IF @error = 0
			BEGIN
				DELETE FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid AND [PersonID] = @person
				DELETE FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid
				DELETE FROM [JolTudomE].[test].[Times] WHERE [TestID] = @testid
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Cancel Test Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END

	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

