-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [users].[usp_Authenticate]
	-- Add the parameters for the stored procedure here
	@username VARCHAR(50),
	@password VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY
		
		--létezik-e a user
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [UserName] = @username)
		SET @error = 1

		--Username karakterhossz
		IF NOT (LEN(@username) BETWEEN 5 AND 8)
		SET @error+= 10

		--jelszó ellenőrzés
		DECLARE @personID INT
		DECLARE @userguid UNIQUEIDENTIFIER
		SET @personID = (SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [UserName] = @username)
		SET @userguid = (SELECT [UserGuid] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personID)
		IF NOT (HASHBYTES('SHA1', @password + CONVERT(CHAR(255), @userguid)) = (SELECT [Password] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personID))
		SET @error+= 100

		--Végrehajtás
		IF @error = 0
			BEGIN
				--Student Role
				IF EXISTS (SELECT * FROM [JolTudomE].[users].[Person] P JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID] WHERE [PersonID] = @personID)
					BEGIN
						SELECT [PersonID], [Prefix], [LastName], [MiddleName], [FirstName], 1 [RoleID]
						FROM [JolTudomE].[users].[Person] P
						JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID]
						WHERE [PersonID] = @personID
					END

				--Teacher Role
				IF EXISTS (SELECT * FROM [JolTudomE].[users].[Person] P JOIN [JolTudomE].[users].[Teacher] T ON P.[PersonID] = T.[TeacherID] WHERE [PersonID] = @personID)
					BEGIN
						SELECT [PersonID], [Prefix], [LastName], [MiddleName], [FirstName], 2 [RoleID]
						FROM [JolTudomE].[users].[Person] P
						JOIN [JolTudomE].[users].[Teacher] T ON P.[PersonID] = T.[TeacherID]
						WHERE [PersonID] = @personID
					END

				--Admin Role
				IF EXISTS (SELECT * FROM [JolTudomE].[users].[Person] P JOIN [JolTudomE].[users].[Admin] A ON P.[PersonID] = A.[AdminID] WHERE [PersonID] = @personID)
					BEGIN
						SELECT [PersonID], [Prefix], [LastName], [MiddleName], [FirstName], 3 [RoleID]
						FROM [JolTudomE].[users].[Person] P
						JOIN [JolTudomE].[users].[Admin] A ON P.[PersonID] = A.[AdminID]
						WHERE [PersonID] = @personID
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Authentication Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

