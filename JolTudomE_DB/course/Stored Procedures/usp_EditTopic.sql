-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_EditTopic]
	-- Add the parameters for the stored procedure here
	@name VARCHAR(50),
	@description VARCHAR(255),
	@topicid INT,
	@courseid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--Létezik-e a témakör
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicID] = @topicid)
		SET @error = 1

		--Van-e változás
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicID] = @topicid AND [TopicName] = @name AND [TopicDescription] = @description)
		SET @error+= 10

		--Van-e másik ilyen nevû témakör a tárgyban
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Topics] WHERE [TopicID] != @topicid AND [CourseID] = @courseid AND [TopicName] = @name)
		SET @error+= 100

		--végrehajtás
		IF @error = 0
			BEGIN
				--Témakörnév módosítása
				IF @name IS NOT NULL
					BEGIN
						UPDATE [JolTudomE].[course].[Topics]
						SET [TopicName] = @name
						WHERE [TopicID] = @topicid
					END

				--Leírás módosítása
				IF @description IS NOT NULL
					BEGIN
						UPDATE [JolTudomE].[course].[Topics]
						SET [TopicDescription] = @description
						WHERE [TopicID] = @topicid
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Edit Topic Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

