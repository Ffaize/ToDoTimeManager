IF COL_LENGTH('dbo.Users', 'OAuthProvider') IS NULL
    ALTER TABLE [dbo].[Users] ADD OAuthProvider INT NOT NULL DEFAULT 0;
