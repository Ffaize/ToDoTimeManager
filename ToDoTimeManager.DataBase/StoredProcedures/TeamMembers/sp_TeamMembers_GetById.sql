CREATE PROCEDURE [dbo].[sp_TeamMembers_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TeamId, UserId, Role
    FROM [dbo].[TeamMembers]
    WHERE Id = @Id;
END
