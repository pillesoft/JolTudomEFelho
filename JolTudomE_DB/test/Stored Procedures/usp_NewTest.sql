-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_NewTest]
	-- Add the parameters for the stored procedure here
	@count INT,
	@topicid TopicIDs READONLY,
	@personid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY
		
		--léteznek-e a témakörök
		IF (SELECT COUNT(1) FROM [JolTudomE].[course].[Topics] WHERE [TopicID] IN (SELECT * FROM @topicid)) <> (SELECT COUNT(*) FROM @topicid)
		SET @error = 1

		--kérdés szám ellenőrzés
		IF ((SELECT COUNT(*) FROM [JolTudomE].[test].[Questions] WHERE [TopicID] IN (SELECT * FROM @topicid)) < @count)
		SET @error+= 10

		--létezik-e a személy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid)
		SET @error+= 100

		--végrehajtás
		IF @error = 0
			BEGIN
				--Kezdés rögzítése
				INSERT INTO [JolTudomE].[test].[Times] ([TestID])
				SELECT (SELECT ISNULL(MAX([TestID])+1, 1) FROM [JolTudomE].[test].[Times])
				--Kérdések
				INSERT INTO [JolTudomE].[test].[Results] ([TestID], [QuestionID], [AnswerID], [PersonID])
				SELECT (SELECT MAX([TestID]) FROM [JolTudomE].[test].[Times]), Q.QuestionID, A.AnswerID, @personid
				FROM (
				SELECT TOP (@count) *
				FROM [JolTudomE].[test].[Questions]
				WHERE [TopicID] IN (SELECT * FROM @topicid)
				ORDER BY NEWID()
				) Q
				JOIN [JolTudomE].[test].[Answers] A ON Q.QuestionID = A.[QuestionID]

				SELECT R2.[TestID], R2.[QuestionID], Q2.[QuestionText], R2.[AnswerID], A2.[AnswerText], R2.[PersonID]
				FROM [JolTudomE].[test].[Results] R2
				JOIN [JolTudomE].[test].[Answers] A2 ON R2.AnswerID = A2.AnswerID
				JOIN [JolTudomE].[test].[Questions] Q2 ON R2.[QuestionID] = Q2.[QuestionID]
				WHERE R2.[TestID] = (SELECT MAX([TestID]) FROM [JolTudomE].[test].[Results])
				ORDER BY R2.[QuestionID], NEWID()
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'New Test Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

