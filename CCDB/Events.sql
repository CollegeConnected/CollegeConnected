CREATE TABLE [dbo].[Events]
(
	[EventID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventName] VARCHAR(MAX) NOT NULL, 
    [EventLocation] VARCHAR(100) NOT NULL, 
    [EventStartDateTime] DATETIME2 NOT NULL, 
    [EventEndDateTime] DATETIME2 NOT NULL, 
    [CreatedBy] VARCHAR(100) NOT NULL, 
    [EventStatus] VARCHAR(50) NOT NULL, 

)
