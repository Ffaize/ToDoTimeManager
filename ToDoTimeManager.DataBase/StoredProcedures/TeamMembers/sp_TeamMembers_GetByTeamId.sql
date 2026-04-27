CREATE PROCEDURE [dbo].[sp_TeamMembers_GetByTeamId] @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TeamId, UserId, Role
    FROM [dbo].[TeamMembers]
    WHERE TeamId = @TeamId;
END
