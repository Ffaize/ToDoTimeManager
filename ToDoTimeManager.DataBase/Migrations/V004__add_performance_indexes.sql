-- =============================================================
-- Migration: Add indexes for frequently queried columns
-- =============================================================

-- ToDos: filtering by assigned user
CREATE NONCLUSTERED INDEX [IX_ToDos_AssignedTo]
    ON [dbo].[ToDos] ([AssignedTo])
    WHERE [AssignedTo] IS NOT NULL;

-- ToDos: filtering by project
CREATE NONCLUSTERED INDEX [IX_ToDos_ProjectId]
    ON [dbo].[ToDos] ([ProjectId])
    WHERE [ProjectId] IS NOT NULL;

-- ToDos: filtering by team
CREATE NONCLUSTERED INDEX [IX_ToDos_TeamId]
    ON [dbo].[ToDos] ([TeamId])
    WHERE [TeamId] IS NOT NULL;

-- TeamMembers: access control checks by user
CREATE NONCLUSTERED INDEX [IX_TeamMembers_UserId]
    ON [dbo].[TeamMembers] ([UserId]);

-- TimeLogs: filtering by user + date (statistics, dashboard)
CREATE NONCLUSTERED INDEX [IX_TimeLogs_UserId_LogDate]
    ON [dbo].[TimeLogs] ([UserId], [LogDate]);

-- TimeLogs: filtering by todo
CREATE NONCLUSTERED INDEX [IX_TimeLogs_ToDoId]
    ON [dbo].[TimeLogs] ([ToDoId]);

-- ActivityLogs: filtering by user
CREATE NONCLUSTERED INDEX [IX_ActivityLogs_UserId]
    ON [dbo].[ActivityLogs] ([UserId]);

-- ActivityLogs: filtering by todo
CREATE NONCLUSTERED INDEX [IX_ActivityLogs_ToDoId]
    ON [dbo].[ActivityLogs] ([ToDoId])
    WHERE [ToDoId] IS NOT NULL;
