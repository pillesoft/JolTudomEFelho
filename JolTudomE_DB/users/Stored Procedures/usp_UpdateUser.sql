
CREATE PROCEDURE [users].[usp_UpdateUser]
	-- Add the parameters for the stored procedure here
	@personid INT,
	@username VARCHAR(50),
	@prefix VARCHAR(5),
	@lastname VARCHAR(25),
	@middlename VARCHAR(25),
	@firstname VARCHAR(25),
	@role TINYINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @error INT = 0

	BEGIN TRY

		--létezik-e már ilyen user
		IF NOT EXISTS (SELECT * FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid)
		SET @error = 1

		--Username karakterhossz
		IF NOT (LEN(@username) BETWEEN 5 AND 8)
		SET @error+= 10

		----Password karakterhossz
		--IF NOT (LEN(@password) >= 8)
		--SET @error+= 100

		--Role check
		IF NOT @role between 1 AND 3
		SET @error+= 1000

		--Végrehajtás
		IF @error = 0
			BEGIN
			
				UPDATE [JolTudomE].[users].[Person]
				SET [UserName] = ISNULL(@username, [UserName])
				, [Prefix] = ISNULL(@prefix, [Prefix])
				, [LastName] = ISNULL(@lastname, [LastName])
				, [MiddleName] = ISNULL(@middlename, [MiddleName])
				, [FirstName] = ISNULL(@firstname, [FirstName])
				WHERE [PersonID] = @personid

				DECLARE @currentrole TINYINT =	(
													SELECT
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
													WHERE [PersonID] = @personid
												)

				IF @currentrole = 1
					BEGIN
						--IF @role = 1
							--BEGIN
								--UPDATE [JolTudomE].[users].[Student]
								--SET [StudentID] = ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							--END

						IF @role = 2
							BEGIN
								DELETE FROM [JolTudomE].[users].[Student]
								WHERE [StudentID] = 
									(
										SELECT P.[PersonID]
										FROM [JolTudomE].[users].[Person] P
										JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID]
										WHERE P.[PersonID] = @personid
									)

								INSERT INTO [JolTudomE].[users].[Teacher] ([TeacherID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END

						IF @role = 3
							BEGIN
								DELETE FROM [JolTudomE].[users].[Student]
								WHERE [StudentID] = 
									(
										SELECT P.[PersonID]
										FROM [JolTudomE].[users].[Person] P
										JOIN [JolTudomE].[users].[Student] S ON P.[PersonID] = S.[StudentID]
										WHERE P.[PersonID] = @personid
									)

								INSERT INTO [JolTudomE].[users].[Admin] ([AdminID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END
					END
				IF @currentrole = 2
					BEGIN
						IF @role = 1
							BEGIN
								DELETE FROM [JolTudomE].[users].[Teacher]
								WHERE [TeacherID] =
									(
										SELECT P.[PersonID]
										FROM [JolTudomE].[users].[Person] P
										JOIN [JolTudomE].[users].[Teacher] S ON P.[PersonID] = S.[TeacherID]
										WHERE P.[PersonID] = @personid
									)

								INSERT INTO [JolTudomE].[users].[Student] ([StudentID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END

						IF @role = 3
							BEGIN
								DELETE FROM [JolTudomE].[users].[Teacher]
								WHERE [TeacherID] = 
									(
										SELECT P.[PersonID]
										FROM [JolTudomE].[users].[Person] P
										JOIN [JolTudomE].[users].[Teacher] S ON P.[PersonID] = S.[TeacherID]
										WHERE P.[PersonID] = @personid
									)

								INSERT INTO [JolTudomE].[users].[Admin] ([AdminID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END
					END

				IF @currentrole = 3
					BEGIN
						IF @role = 1
							BEGIN
								DELETE FROM [JolTudomE].[users].[Admin]
								WHERE [AdminID] =
											(
												SELECT P.[PersonID]
												FROM [JolTudomE].[users].[Person] P
												JOIN [JolTudomE].[users].[Admin] S ON P.[PersonID] = S.[AdminID]
												WHERE P.[PersonID] = @personid
											)

								INSERT INTO [JolTudomE].[users].[Student] ([StudentID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END
						IF @role = 2
							BEGIN
								DELETE FROM [JolTudomE].[users].[Admin]
								WHERE [AdminID] =
											(
												SELECT P.[PersonID]
												FROM [JolTudomE].[users].[Person] P
												JOIN [JolTudomE].[users].[Admin] S ON P.[PersonID] = S.[AdminID]
												WHERE P.[PersonID] = @personid
											)

								INSERT INTO [JolTudomE].[users].[Teacher] ([TeacherID]) VALUES ((SELECT [PersonID] FROM [JolTudomE].[users].[Person] WHERE [PersonID] = @personid))
							END
					END
			END
		ELSE
			BEGIN
				DECLARE @message VARCHAR(400) = 'Update User Error. Error Code: ' + CAST(@error AS VARCHAR(100));
				THROW 50000, @message, 1
			END
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END

