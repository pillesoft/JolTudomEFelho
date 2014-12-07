-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_EditAnswer]
	-- Add the parameters for the stored procedure here
	@questionid INT,
	@answerid INT,
	@answer VARCHAR(50),
	@iscorrect BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY

		--Létezik-e a kérdés
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Questions] WHERE [QuestionID] = @questionid)
		SET @error = 1

		--Létezik-e a válasz
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Answers] WHERE [AnswerID] = @answerid)
		SET @error+= 10

		--Marad-e helyes válasz
		IF (SELECT [IsCorrect] FROM [JolTudomE].[test].[Answers] WHERE [AnswerID] = @answerid) = 1
		AND @iscorrect = 0
		SET @error+= 100

		--Megvan-e adva a válasz
		IF (SELECT @answer) IS NULL
		SET @error+= 1000

		--Összetartozó kérdés válasz pár-e
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[test].[Answers] WHERE [AnswerID] = @answerid AND [QuestionID] = @questionid)
		SET @error+= 10000

		--végrehajtás
		IF @error = 0
			BEGIN	
				--Válasz modosítása
				UPDATE [JolTudomE].[test].[Answers]
				SET [AnswerText] = @answer
				WHERE [AnswerID] = @answerid

				--Helyes válasz modosítása
				IF @iscorrect = 1
					BEGIN
						UPDATE [JolTudomE].[test].[Answers]
						SET [IsCorrect] = 0
						WHERE [QuestionID] = @questionid
						AND [AnswerID] <> @answerid

						UPDATE [JolTudomE].[test].[Answers]
						SET [IsCorrect] = 1
						WHERE [AnswerID] = @answerid
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Edit Answer Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

