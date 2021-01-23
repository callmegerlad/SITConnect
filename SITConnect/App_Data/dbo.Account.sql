﻿CREATE TABLE [dbo].[Account] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]          NVARCHAR (50)  NULL,
    [LastName]           NVARCHAR (50)  NULL,
    [Email]              NVARCHAR (50)  NULL UNIQUE,
    [PhoneNumber]        NCHAR (10)     NULL,
    [DOB]                DATE           NULL,
    [CreditCardInfo]     NVARCHAR (MAX) NULL,
    [PasswordHash]       NVARCHAR (MAX) NULL,
    [PasswordSalt]       NVARCHAR (MAX) NULL,
    [DateTimeRegistered] DATETIME       NULL,
    [IV]                 NVARCHAR (MAX) NULL,
    [Key]                NVARCHAR (MAX) NULL,
    [Attempts]           INT            NULL,
    [LastAttempt]        NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

