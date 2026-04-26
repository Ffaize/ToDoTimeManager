CREATE PROCEDURE [dbo].[sp_TeamMembers_GetByTeamIdAndUserId]
    @TeamId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, TeamId, UserId, Role
    FROM [dbo].[TeamMembers]
    WHERE TeamId = @TeamId
      AND UserId = @UserId;
END
