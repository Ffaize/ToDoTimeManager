CREATE PROCEDURE [dbo].[sp_Users_Update] @Id UNIQUEIDENTIFIER,
                                         @UserName NVARCHAR(256),
                                         @Email NVARCHAR(256),
                                         @Password NVARCHAR(512),
                                         @UserRole INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users
    SET Username = @UserName,
        Email    = @Email,
        Password = @Password,
        UserRole = @UserRole
    WHERE Id = @Id;
END
