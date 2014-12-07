
CREATE PROCEDURE [users].[usp_AddNewUser]
	-- Add the parameters for the stored procedure here
	@username VARCHAR(50),
	@prefix VARCHAR(5),
	@lastname VARCHAR(25),
	@middlename VARCHAR(25),
	@firstname VARCHAR(25),
	@password VARCHAR(50),
	@role TINYINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY

		--létezik-e már ilyen user
		IF EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [UserName] = @username)
		SET @error = 1

		--Username karakterhossz
		IF NOT (LEN(@username) BETWEEN 5 AND 8)
		SET @error+= 10

		--Password karakterhossz
		IF NOT (LEN(@password) >= 8)
		SET @error+= 100

		--Role check
		IF NOT @role between 1 AND 3
		SET @error+= 1000

		--Végrehajtás
		IF @error = 0
			BEGIN
				DECLARE @userguid UNIQUEIDENTIFIER = NEWID()
			
				INSERT INTO [JolTudomE].[users].[Person] ([UserName], [Prefix], [LastName], [MiddleName], [FirstName], [Password], [UserGuid])
				VALUES (@username, @prefix, @lastname, @middlename, @firstname, HASHBYTES('SHA1', @password + CONVERT(CHAR(255), @userguid)), @userguid)

				IF @role = 1
					BEGIN
						INSERT INTO [JolTudomE].[users].[Student] ([StudentID]) VALUES ((SELECT MAX([PersonID]) FROM [JolTudomE].[users].[Person]))
					END

				IF @role = 2
					BEGIN
						INSERT INTO [JolTudomE].[users].[Teacher] ([TeacherID]) VALUES ((SELECT MAX([PersonID]) FROM [JolTudomE].[users].[Person]))
					END

				IF @role = 3
					BEGIN
						INSERT INTO [JolTudomE].[users].[Admin] ([AdminID]) VALUES ((SELECT MAX([PersonID]) FROM [JolTudomE].[users].[Person]))
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Add New User Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

