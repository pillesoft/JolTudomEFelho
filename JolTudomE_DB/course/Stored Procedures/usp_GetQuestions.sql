-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_GetQuestions]
	-- Add the parameters for the stored procedure here
	@topicid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--Létezik-e ilyen témakör
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicID] = @topicid)
		SET @error = 1

		--végrehajtás
		IF @error = 0
			BEGIN
				--Kérdések lekérése
				SELECT [QuestionID], [QuestionText] FROM [JolTudomE].[test].[Questions] WHERE [TopicID] = @topicid
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Get Questions Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

