-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_EditCourse]
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

	BEGIN TRY

		--Létezik-e a tárgy
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseID] = @courseid)
		SET @error = 1

		--Van-e változás?
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseID] = @courseid AND [CourseName] = @name AND [CourseDescription] = @description)
		SET @error+= 10

		--Van-e másik ilyen nevű tárgy
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseID] != @courseid AND [CourseName] = @name)
		SET @error+= 100

		--végrehajtás
		IF @error = 0
			BEGIN
				--Tárgynév módosítása
				IF @name IS NOT NULL
					BEGIN
						UPDATE [JolTudomE].[course].[Courses]
						SET [CourseName] = @name
						WHERE [CourseID] = @courseid
					END

				--Leírás módosítása
				IF @description IS NOT NULL
					BEGIN
						UPDATE [JolTudomE].[course].[Courses]
						SET [CourseDescription] = @description
						WHERE [CourseID] = @courseid
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Edit Course Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

