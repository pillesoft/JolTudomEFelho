-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_AddNewCourse]
	-- Add the parameters for the stored procedure here
	@name VARCHAR(30),
	@description VARCHAR(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

    -- Insert statements for procedure here
	BEGIN TRY

		--Létezik-e már ilyen tárgy
		IF EXISTS (SELECT * FROM [JolTudomE].[course].[Courses] WHERE [CourseName] = @name)
		SET @error = 1

		--végrehajtás
		IF @error = 0
			BEGIN
				--Tárgy rögzítése
				INSERT INTO [JolTudomE].[course].[Courses] ([CourseName], [CourseDescription])
					VALUES (@name, @description)
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Add New Course Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

