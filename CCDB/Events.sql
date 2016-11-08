CREATE TABLE [dbo].[Events]
(
	[EventID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventName] VARCHAR(MAX) NOT NULL, 
    [EventLocation] NCHAR(10) NOT NULL, 
    [EventStart] DATETIME NOT NULL, 
    [EventEnd] DATETIME NOT NULL
)
