CREATE PROCEDURE [dbo].[sp_UsersSecrets_Create] @Id           UNIQUEIDENTIFIER,
                                               @UserId       UNIQUEIDENTIFIER,
                                               @PasswordSalt NVARCHAR(128)
AS
INSERT INTO [dbo].[UsersSecrets] (Id, UserId, PasswordSalt)
VALUES (@Id, @UserId, @PasswordSalt)
