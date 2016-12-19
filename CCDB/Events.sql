CREATE TABLE [dbo].[Events]
(
	[EventID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventName] VARCHAR(MAX) NOT NULL, 
    [EventLocation] VARCHAR(100) NOT NULL, 
    [EventStartTime] DATETIME NOT NULL, 
    [EventEndTime] DATETIME NOT NULL, 
    [CreatedBy] VARCHAR(100) NOT NULL, 
    [EventDate] DATE NOT NULL, 
    [EventStatus] VARCHAR(50) NOT NULL, 

)
