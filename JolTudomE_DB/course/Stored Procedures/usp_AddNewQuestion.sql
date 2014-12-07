-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_AddNewQuestion]
	-- Add the parameters for the stored procedure here
	@topicid INT,
	@question VARCHAR(255),
	@answer1 VARCHAR(50),
	@answer2 VARCHAR(50),
	@answer3 VARCHAR(50),
	@answer4 VARCHAR(50),
	@correct1 BIT,
	@correct2 BIT,
	@correct3 BIT,
	@correct4 BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

    -- Insert statements for procedure here
	BEGIN TRY

		--Létezik-e ilyen témakör
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicID] = @topicid)
		SET @error = 1

		--Létezik-e már ilyen kérdés a témakörben
		IF EXISTS (SELECT * FROM [JolTudomE].[test].[Questions] WHERE [QuestionText] = @question AND [TopicID] = @topicid)
		SET @error+= 10

		--Különbözőek-e a válaszok
		IF @answer1 = @answer2 OR @answer1 = @answer3 OR @answer1 = @answer4
			OR @answer2 = @answer3 OR @answer2 = @answer4
			OR @answer3 = @answer4
		SET @error+= 100

		--Pontosan egy helyes válasz van-e
		IF CAST(@correct1 AS INT) + CAST(@correct2 AS INT) + CAST(@correct3 AS INT) + CAST(@correct4 AS INT) <> 1
		SET @error+= 1000

		--végrehajtás
		IF @error = 0
			BEGIN
				--Kérdés rögzítése
				INSERT INTO [JolTudomE].[test].[Questions] ([QuestionText], [TopicID])
					VALUES (@question, @topicid)
				INSERT INTO [JolTudomE].[test].[Answers] ([QuestionID], [AnswerText], [IsCorrect])
					VALUES ((SELECT MAX([QuestionID]) FROM [JolTudomE].[test].[Questions]), @answer1, @correct1)
				INSERT INTO [JolTudomE].[test].[Answers] ([QuestionID], [AnswerText], [IsCorrect])
					VALUES ((SELECT MAX([QuestionID]) FROM [JolTudomE].[test].[Questions]), @answer2, @correct2)
				INSERT INTO [JolTudomE].[test].[Answers] ([QuestionID], [AnswerText], [IsCorrect])
					VALUES ((SELECT MAX([QuestionID]) FROM [JolTudomE].[test].[Questions]), @answer3, @correct3)
				INSERT INTO [JolTudomE].[test].[Answers] ([QuestionID], [AnswerText], [IsCorrect])
					VALUES ((SELECT MAX([QuestionID]) FROM [JolTudomE].[test].[Questions]), @answer4, @correct4)
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Add New Question Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH

END

