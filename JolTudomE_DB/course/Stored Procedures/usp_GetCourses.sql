-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [course].[usp_GetCourses]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY
		
		--végrehajtás
		IF @error = 0
			BEGIN
				--Tárgyak lekérése
				SELECT [CourseID], [CourseName],[CourseDescription]  FROM [JolTudomE].[course].[Courses]
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Get Courses Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

