CREATE TABLE [dbo].[Students]
(
	[StudentId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [StudentNumber] VARCHAR(50) NULL, 
    [FirstName] VARCHAR(50) NULL, 
    [MiddleName] VARCHAR(50) NULL, 
    [LastName] VARCHAR(50) NULL, 
    [Address1] VARCHAR(MAX) NULL, 
    [Address2] VARCHAR(MAX) NULL, 
    [ZipCode] VARCHAR(5) NULL, 
    [State] VARCHAR(50) NULL, 
    [PhoneNumber] VARCHAR(50) NULL, 
    [Email] VARCHAR(50) NULL, 
    [GraduationYear] VARCHAR(50) NULL, 
    [BirthDate] DATETIME NULL, 
    [UpdateTimeStamp] DATETIME NULL, 
    [City] VARCHAR(50) NULL
)
