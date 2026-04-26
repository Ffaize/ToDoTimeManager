CREATE PROCEDURE [dbo].[sp_TeamMembers_Create]
    @Id     UNIQUEIDENTIFIER,
    @TeamId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Role   INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[TeamMembers] (Id, TeamId, UserId, Role)
    VALUES (@Id, @TeamId, @UserId, @Role);
END
