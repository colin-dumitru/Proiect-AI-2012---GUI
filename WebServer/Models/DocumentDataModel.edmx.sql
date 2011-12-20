
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/20/2011 23:11:32
-- Generated from EDMX file: D:\Proiect-AI--GUI-IUSTINA\WebServer\Models\DocumentDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WebServerDatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Documents'
CREATE TABLE [dbo].[Documents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DocumentOutputs'
CREATE TABLE [dbo].[DocumentOutputs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DocumentId] int  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Document] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Documents'
ALTER TABLE [dbo].[Documents]
ADD CONSTRAINT [PK_Documents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DocumentOutputs'
ALTER TABLE [dbo].[DocumentOutputs]
ADD CONSTRAINT [PK_DocumentOutputs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------