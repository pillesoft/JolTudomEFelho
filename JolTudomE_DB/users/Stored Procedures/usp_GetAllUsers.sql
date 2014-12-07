-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [users].[usp_GetAllUsers]
	-- Add the parameters for the stored procedure here
	@roleid INT,
	@sroleid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--role ellenõrzés
		IF NOT @roleid BETWEEN 2 AND 3
		SET @error = 1

		----létezik-e a keresett role
		--IF NOT @sroleid BETWEEN 1 AND 3
		--SET @error+= 10

		--végrehajtás
		IF @error = 0
			BEGIN
				--All Students
				IF @sroleid = 1
				BEGIN
					SELECT DISTINCT
						[Prefix] COLLATE Hungarian_Technical_CI_AS [Prefix]
						,[FirstName] COLLATE Hungarian_Technical_CI_AS [FirstName]
						,[MiddleName] COLLATE Hungarian_Technical_CI_AS [MiddleName]
						,[LastName] COLLATE Hungarian_Technical_CI_AS [LastName]
						,[PersonID]
						,1 [RoleID]
					FROM [JolTudomE].[users].[Person] P
					JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID]
					ORDER BY [LastName] COLLATE Hungarian_Technical_CI_AS, [MiddleName] COLLATE Hungarian_Technical_CI_AS, [FirstName] COLLATE Hungarian_Technical_CI_AS
				END

				--All Teachers
				IF @sroleid = 2
				BEGIN
					SELECT DISTINCT
						[Prefix] COLLATE Hungarian_Technical_CI_AS [Prefix]
						,[FirstName] COLLATE Hungarian_Technical_CI_AS [FirstName]
						,[MiddleName] COLLATE Hungarian_Technical_CI_AS [MiddleName]
						,[LastName] COLLATE Hungarian_Technical_CI_AS [LastName]
						,[PersonID]
						,2 [RoleID]
					FROM [JolTudomE].[users].[Person] P
					JOIN [JolTudomE].[users].[Teacher] T ON P.[PersonID] = T.[TeacherID]
					ORDER BY [LastName] COLLATE Hungarian_Technical_CI_AS, [MiddleName] COLLATE Hungarian_Technical_CI_AS, [FirstName] COLLATE Hungarian_Technical_CI_AS
				END

				--All Admins
				IF @sroleid = 3
				BEGIN
					SELECT DISTINCT
						[Prefix] COLLATE Hungarian_Technical_CI_AS [Prefix]
						,[FirstName] COLLATE Hungarian_Technical_CI_AS [FirstName]
						,[MiddleName] COLLATE Hungarian_Technical_CI_AS [MiddleName]
						,[LastName] COLLATE Hungarian_Technical_CI_AS [LastName]
						,[PersonID]
						,3 [RoleID]
					FROM [JolTudomE].[users].[Person] P
					JOIN [JolTudomE].[users].[Admin] A ON P.[PersonID] = A.[AdminID]
					ORDER BY [LastName] COLLATE Hungarian_Technical_CI_AS, [MiddleName] COLLATE Hungarian_Technical_CI_AS, [FirstName] COLLATE Hungarian_Technical_CI_AS
				END
				--All Users
				IF @sroleid IS NULL
				BEGIN
					SELECT DISTINCT
						[Prefix] COLLATE Hungarian_Technical_CI_AS [Prefix]
						,[FirstName] COLLATE Hungarian_Technical_CI_AS [FirstName]
						,[MiddleName] COLLATE Hungarian_Technical_CI_AS [MiddleName]
						,[LastName] COLLATE Hungarian_Technical_CI_AS [LastName]
						,[PersonID]
						,CASE 
							WHEN S.StudentID IS NOT NULL 
							THEN 1
							WHEN T.TeacherID IS NOT NULL
							THEN 2
							WHEN A.AdminID IS NOT NULL
							THEN 3
						END [RoleID]
					FROM [JolTudomE].[users].[Person] P
					LEFT JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID]
					LEFT JOIN [JolTudomE].[users].[Teacher] T ON P.[PersonID] = T.[TeacherID]
					LEFT JOIN [JolTudomE].[users].[Admin] A ON P.[PersonID] = A.[AdminID]
					ORDER BY [RoleID], [LastName] COLLATE Hungarian_Technical_CI_AS, [MiddleName] COLLATE Hungarian_Technical_CI_AS, [FirstName] COLLATE Hungarian_Technical_CI_AS
				END
			END
		ELSE
		BEGIN
			DECLARE @message VARCHAR(400) = 'Get All Users Error. Error Code: ' + CAST(@error AS VARCHAR(100));
			THROW 50000, @message, 1
		END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

