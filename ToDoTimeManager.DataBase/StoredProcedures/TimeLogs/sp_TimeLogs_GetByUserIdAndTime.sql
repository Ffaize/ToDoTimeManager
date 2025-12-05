CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserIdAndTime]
    @UserId UNIQUEIDENTIFIER,
    @DaysAgo INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@DaysAgo = -1)
        BEGIN
            SELECT *
            FROM TimeLogs
            WHERE [UserId] = @UserId
            ORDER BY LogDate DESC
        END
    ELSE
        BEGIN
            SELECT *
            FROM TimeLogs
            WHERE [UserId] = @UserId
              AND LogDate >= DATEADD(DAY, -@DaysAgo, GETUTCDATE())
            ORDER BY LogDate DESC
        END
END
