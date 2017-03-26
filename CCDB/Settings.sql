CREATE TABLE [dbo].[Settings]
(
	[id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EmailUsername] VARCHAR(MAX) NULL, 
    [EmailPassword] VARCHAR(50) NULL, 
    [EmailPort] VARCHAR(5) NULL, 
    [EmailHostName] VARCHAR(100) NULL, 
    [EventEmailMessageBody] TEXT NULL
)
