-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_EditQuestion]
	-- Add the parameters for the stored procedure here
	@questionid INT,
	@question VARCHAR(255)
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

		--Van-e változás
		IF EXISTS (SELECT * FROM [JolTudomE].[test].[Questions] WHERE [QuestionID] = @questionid AND [QuestionText] = @question)
		SET @error+= 10

		--végrehajtás
		IF @error = 0
			BEGIN
				--Kérdés modosítása
				UPDATE [JolTudomE].[test].[Questions]
				SET [QuestionText] = @question
				WHERE [QuestionID] = @questionid
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Edit Question Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

