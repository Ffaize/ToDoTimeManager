CREATE PROCEDURE [dbo].[sp_TeamMembers_DeleteByTeamIdAndUserId] @TeamId UNIQUEIDENTIFIER,
                                                                @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE
    FROM [dbo].[TeamMembers]
    WHERE TeamId = @TeamId
      AND UserId = @UserId;
END
