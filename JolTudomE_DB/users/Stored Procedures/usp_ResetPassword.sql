
CREATE PROCEDURE [users].[usp_ResetPassword]
	-- Add the parameters for the stored procedure here
	@personid VARCHAR(50),
	@password VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @error INT = 0

	BEGIN TRY

		--létezik-e ilyen user
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid)
		SET @error = 1

		--Password karakterhossz
		IF NOT (LEN(@password) >= 8)
		SET @error+= 10

		--Végrehajtás
		IF @error = 0
			BEGIN

				DECLARE @userguid UNIQUEIDENTIFIER
				SET @userguid = NEWID()

				UPDATE [JolTudomE].[users].[Person]
				SET [Password] = HASHBYTES('SHA1', @password + CONVERT(CHAR(255), @userguid)),
				[UserGuid] = @userguid
				WHERE [PersonID] = @personid

			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Reset Password Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

