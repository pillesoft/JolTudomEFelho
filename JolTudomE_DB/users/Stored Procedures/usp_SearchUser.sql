-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [users].[usp_SearchUser]
	-- Add the parameters for the stored procedure here
	@roleid INT,
	@prefix VARCHAR(6),
	@firstname VARCHAR(26),
	@middlename VARCHAR(26),
	@lastname VARCHAR(26),
	@username VARCHAR(9)
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

		--prefix hossz ellenõrzés
		IF (LEN(@prefix) > 5)
		SET @error+= 10

		--firstname hossz ellenõrzés
		IF (LEN(@firstname) > 25)
		SET @error+= 100

		--middlename hossz ellenõrzés
		IF (LEN(@middlename) > 25)
		SET @error+= 1000

		--lastname hossz ellenõrzés
		IF (LEN(@lastname) > 25)
		SET @error+= 10000

		--username hossz ellenõrzés
		IF (LEN(@username) > 8)
		SET @error+= 100000

		--végrehajtás
		IF @error = 0
			BEGIN
				SELECT DISTINCT P.[Prefix], P.[FirstName], P.[MiddleName], P.[LastName], P.[UserName], P.[PersonID],
				CASE 
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
				WHERE ISNULL(P.[Prefix], '') LIKE ISNULL(@prefix, '%')
				AND P.[FirstName] LIKE ISNULL(@firstname, '%')
				AND ISNULL(P.[MiddleName], '') LIKE ISNULL(@middlename, '%')
				AND P.[LastName] LIKE ISNULL(@lastname, '%')
				AND P.[UserName] LIKE ISNULL(@username, '%')
				ORDER BY [RoleID], P.[FirstName], P.[MiddleName], P.[LastName]
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Search User Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

