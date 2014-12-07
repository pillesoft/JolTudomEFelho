-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_GetTopics]
	-- Add the parameters for the stored procedure here
	@courseid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--Létezik-e ilyen tárgy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseID] = @courseid)
		SET @error = 1

		--végrehajtás
		IF @error = 0
			BEGIN
				--Témakörök lekérése
				SELECT [TopicID], [TopicName], [TopicDescription] FROM [JolTudomE].[course].[Topics] WHERE [CourseID] = @courseid
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Get Topics Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

