CREATE PROCEDURE [dbo].[sp_TwoFactorCodes_DeleteByUserId] @UserId UNIQUEIDENTIFIER
AS
    SET NOCOUNT ON;
    DELETE FROM [dbo].[TwoFactorCodes] WHERE UserId = @UserId;
