﻿CREATE TABLE [dbo].[EventAttendances]
(
[Id] UNIQUEIDENTIFIER NOT NULL,
	[EventId] UNIQUEIDENTIFIER NOT NULL , 
    [StudentId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_EventAttendances] PRIMARY KEY ([Id])
)
