-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_CheckedAnswer]
	-- Add the parameters for the stored procedure here
	@testid INT,
	@questionid INT,
	@answerid INT,
	@complete BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--létezõ teszt, kérdés, válasz kombináció
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid AND [QuestionID] = @questionid AND ([AnswerID] = @answerid OR @answerid IS NULL))
		SET @error = 1

		--van-e megadva válasz
		IF @answerid IS NULL
		SET @error+= 10

		--a kérdéshez már van megjelölt válasz
		IF (SELECT SUM(CAST([CheckedAnswer] AS INT)) FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid AND [QuestionID] = @questionid) > 0
		SET @error+= 100

		--végrehajtás
		IF @error = 0
			BEGIN
				UPDATE [JolTudomE].[test].[Results]
				SET [CheckedAnswer] = 1
				WHERE [TestID] = @testid
				AND
				[QuestionID] = @questionid
				AND
				[AnswerID] = @answerid

				UPDATE [JolTudomE].[test].[Times]
				SET [Completed] = GETDATE()
				WHERE [TestID] = @testid
				AND
				@complete = 1
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Checked Answer Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

