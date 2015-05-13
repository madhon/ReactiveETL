CREATE TABLE [dbo].[People] (
    [id]        INT            IDENTITY (1, 1) NOT NULL,
    [userid]    INT            NOT NULL,
    [firstname] NVARCHAR (255) NOT NULL,
    [lastname]  NVARCHAR (255) NOT NULL,
    [email]     NVARCHAR (255) NOT NULL
);

