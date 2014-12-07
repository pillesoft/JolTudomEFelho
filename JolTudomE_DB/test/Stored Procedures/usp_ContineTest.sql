-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [test].[usp_ContineTest]
	-- Add the parameters for the stored procedure here
	@personid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		DECLARE @testid INT
		--létezik-e a user
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid)
		SET @error = 1

		--a usernek van tesztje és az utolsó lezáratlan
		IF NOT EXISTS (	SELECT DISTINCT 
							T.[Completed]
						FROM [JolTudomE].[test].[Results] R 
						JOIN [JolTudomE].[test].[Times] T ON R.[TestID] = T.[TestID] 
						WHERE R.[PersonID] = @personid 
						AND R.[TestID] = (SELECT MAX([TestID]) 
										  FROM [JolTudomE].[test].[Results] 
										  WHERE R.[PersonID] = @personid)
						AND T.[Completed] IS NULL
					  )
		SET @error+=10

		--suspend vole-e az utolsó event a teszthez
		SET @testid = (SELECT MAX([TestID]) FROM [JolTudomE].[test].[Results] WHERE [PersonID] = @personid)
		IF (SELECT TOP 1 [EventID] FROM [JolTudomE].[mobile].[Events] WHERE [TestID] = @testid ORDER BY [RowID] DESC) != 1
		SET @error+=100

		--végrehajtás
		IF @error = 0
		BEGIN

			EXECUTE [JolTudomE].[mobile].[usp_AddEvent] @testid, 0

			SELECT R.[TestID], R.[QuestionID], Q2.[QuestionText], R.[AnswerID], A2.[AnswerText], R.[PersonID]
			FROM [JolTudomE].[test].[Results] R
			JOIN [JolTudomE].[test].[Answers] A2 ON R.AnswerID = A2.AnswerID
			JOIN [JolTudomE].[test].[Questions] Q2 ON R.[QuestionID] = Q2.[QuestionID]
			WHERE R.[TestID] = @testid
			AND R.[QuestionID] NOT IN (SELECT [QuestionID]
									   FROM [JolTudomE].[test].[Results]
									   WHERE [TestID] = R.[TestID]
									   GROUP BY [QuestionID]
									   HAVING SUM(CONVERT(INT, [CheckedAnswer])) > 0)
			ORDER BY R.[QuestionID], NEWID()
		END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Continue Test Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

