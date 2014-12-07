-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_Eval]
	-- Add the parameters for the stored procedure here
	@testid INT,
	@person INT,
	@callerid INT,
	@roleid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--létezik-e a teszt
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid)
		SET @error = 1

		--létezik-e a személy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @person)
		SET @error+= 10

		--összetartozik-e a személy és a teszt
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Results] WHERE [TestID] = @testid AND [PersonID] = @person)
		SET @error+= 100

		--jogosúltság ellenőrzés
		IF @person != @callerid AND @roleid = 1
		SET @error+= 1000

		--végrehajtás
		IF @error = 0
			BEGIN
				SELECT DISTINCT
				Q.[QuestionText]
				,M.[AnswerText] ChekedAnswer
				,C.[AnswerText] CorrectAnswer
				FROM [JolTudomE].[test].[Results] R
				JOIN [JolTudomE].[test].[Questions] Q ON R.[QuestionID] = Q.[QuestionID]
				JOIN (
					SELECT Q.[QuestionID], A.[AnswerText]
					FROM [JolTudomE].[test].[Answers] A
					JOIN [JolTudomE].[test].[Questions] Q ON A.[QuestionID] = Q.[QuestionID]
					WHERE [IsCorrect] = 1) C ON R.[QuestionID] = C.[QuestionID]
				LEFT JOIN (
					SELECT A.[AnswerText], R1.[QuestionID]
					FROM [JolTudomE].[test].[Results] R1
					JOIN [JolTudomE].[test].[Answers] A ON R1.[AnswerID] = A.[AnswerID]
					WHERE [CheckedAnswer] = 1
					AND
					[TestID] = @testid) M ON R.[QuestionID] = M.[QuestionID]
				WHERE R.[TestID] = @testid
				AND
				R.[PersonID] = @person
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Eval Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

