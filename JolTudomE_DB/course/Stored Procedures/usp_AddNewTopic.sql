-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_AddNewTopic]
	-- Add the parameters for the stored procedure here
	@name VARCHAR(30),
	@description VARCHAR(255),
	@courseid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

    -- Insert statements for procedure here
	BEGIN TRY

		--Létezik-e ilyen tárgy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseID] = @courseid)
		SET @error = 1

		--Létezik-e már ilyen témakör a tárgyhoz
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicName] = @name AND [CourseID] = @courseid)
		SET @error+= 10

		--végrehajtás
		IF @error = 0
				BEGIN
					--Témakör rögzítése
					INSERT INTO [JolTudomE].[course].[Topics] ([CourseID], [TopicName], [TopicDescription])
					VALUES (@courseid, @name, @description)
				END
			ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Add New Topic Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH

END

