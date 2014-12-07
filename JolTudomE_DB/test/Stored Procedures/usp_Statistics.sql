-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_Statistics]
	-- Add the parameters for the stored procedure here
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

		--létezik-e a személy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @person)
		SET @error = 1

		--jogosúltság ellenõrzés
		IF @person != @callerid AND @roleid = 1
		SET @error+= 10

		--létezik-e a kérő
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @callerid)
		SET @error+= 100

		--végrehajtás
		IF @error = 0
		BEGIN
			SELECT
				R.[TestID]
				,T.[Generated]
				,COUNT(DISTINCT R.[QuestionID]) [Questions]
				,ISNULL((SELECT
					COUNT(*)
					FROM (
						SELECT Q.[QuestionID], A.[AnswerID]
						FROM [JolTudomE].[test].[Answers] A
						JOIN [JolTudomE].[test].[Questions] Q ON A.[QuestionID] = Q.[QuestionID]
						WHERE [IsCorrect] = 1) C
					JOIN (
						SELECT A.[AnswerID], R1.[QuestionID], R1.[TestID]
						FROM [JolTudomE].[test].[Results] R1
						LEFT JOIN [JolTudomE].[test].[Answers] A ON R1.[AnswerID] = A.[AnswerID]
						WHERE [CheckedAnswer] = 1) M ON C.[AnswerID] = M.[AnswerID]
					WHERE M.[AnswerID] = C.[AnswerID]
					AND
					M.[TestID] = R.[TestID]
					GROUP BY M.[TestID]), 0) [CorrectAnswer]
				,CONVERT(NUMERIC(5, 2), (CONVERT(NUMERIC(5, 2), (ISNULL((SELECT
					COUNT(*)
					FROM (
						SELECT Q.[QuestionID], A.[AnswerID]
						FROM [JolTudomE].[test].[Answers] A
						JOIN [JolTudomE].[test].[Questions] Q ON A.[QuestionID] = Q.[QuestionID]
						WHERE [IsCorrect] = 1) C
					JOIN (
						SELECT A.[AnswerID], R1.[QuestionID], R1.[TestID]
						FROM [JolTudomE].[test].[Results] R1
						LEFT JOIN [JolTudomE].[test].[Answers] A ON R1.[AnswerID] = A.[AnswerID]
						WHERE [CheckedAnswer] = 1) M ON C.[AnswerID] = M.[AnswerID]
					WHERE M.[AnswerID] = C.[AnswerID]
					AND
					M.[TestID] = R.[TestID]
					GROUP BY M.[TestID]), 0)))
					/
					CONVERT(NUMERIC(5, 2), COUNT(DISTINCT R.[QuestionID])))) [Percent]
				,CONVERT(TIME(0),DATEADD(SS, ISNULL(-(SELECT SUM([Duration])
													 FROM (SELECT [TestID], [SequenceID], DATEDIFF(SS, MIN([TimeStamp]), MAX([TimeStamp])) Duration
														  FROM [JolTudomE].[mobile].[Events]
														  WHERE [TestID] = R.[TestID]
														  GROUP BY [TestID], [SequenceID]) A), 0), T.[Completed] - T.[Generated])) [TotalTime]
			FROM [JolTudomE].[test].[Results] R
			JOIN [JolTudomE].[test].[Questions] Q ON R.[QuestionID] = Q.[QuestionID]
			JOIN [JolTudomE].[test].[Times] T ON R.[TestID] = T.[TestID]
			WHERE R.[PersonID] = @person
			AND
			T.[Completed] IS NOT NULL
			GROUP BY T.[Generated], T.[Completed], R.[TestID] 
		END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Statistics Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

