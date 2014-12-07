-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_GetAnswers]
	-- Add the parameters for the stored procedure here
	@questionid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--Létezik-e ilyen kérdés
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Questions] WHERE [QuestionID] = @questionid)
		SET @error = 1

		--végrehajtás
		IF @error = 0
			BEGIN
				--Kérdések lekérése
				SELECT [QuestionID], [AnswerID], [AnswerText], [IsCorrect] FROM [JolTudomE].[test].[Answers] WHERE [QuestionID] = @questionid
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Get Answers Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

