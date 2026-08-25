-- =====================================================
-- Mounatna (codename) - Initial Schema - FINAL DRAFT
-- SQL Server / T-SQL
-- Includes: unified Items (ad-hoc), AutoPromoteAdHoc,
-- FA icon classes, no ShoppingListItems table
-- =====================================================

USE MounatnaDb;
GO

CREATE SCHEMA [Lookup];
GO

-- ============ Users ============
CREATE TABLE [dbo].[Users]
(
    [Id]           INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Users] PRIMARY KEY,
    [FirebaseUid]  NVARCHAR(128) NOT NULL,
    [DisplayName]  NVARCHAR(100) NOT NULL,
    [CreatedAt]    DATETIME2 NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT GETDATE(),
    CONSTRAINT [UQ_Users_FirebaseUid] UNIQUE ([FirebaseUid])
);
GO

-- ============ Families ============
CREATE TABLE [dbo].[Families]
(
    [Id]               INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Families] PRIMARY KEY,
    [Name]             NVARCHAR(100) NOT NULL,
    [JoinCode]         NVARCHAR(8)   NOT NULL,
    [AutoPromoteAdHoc] BIT NOT NULL CONSTRAINT [DF_Families_AutoPromoteAdHoc] DEFAULT (1),
    [CreatedByUserId]  INT NOT NULL,
    [CreatedAt]        DATETIME2 NOT NULL CONSTRAINT [DF_Families_CreatedAt] DEFAULT GETDATE(),
    CONSTRAINT [UQ_Families_JoinCode] UNIQUE ([JoinCode]),
    CONSTRAINT [FK_Families_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])
);
GO

-- ============ FamilyMembers ============
CREATE TABLE [dbo].[FamilyMembers]
(
    [Id]       INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FamilyMembers] PRIMARY KEY,
    [FamilyId] INT NOT NULL,
    [UserId]   INT NOT NULL,
    [Role]     TINYINT NOT NULL CONSTRAINT [DF_FamilyMembers_Role] DEFAULT (1), -- 0 = Owner, 1 = Member
    [JoinedAt] DATETIME2 NOT NULL CONSTRAINT [DF_FamilyMembers_JoinedAt] DEFAULT GETDATE(),
    CONSTRAINT [UQ_FamilyMembers_Family_User] UNIQUE ([FamilyId], [UserId]),
    CONSTRAINT [FK_FamilyMembers_Families] FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families] ([Id]),
    CONSTRAINT [FK_FamilyMembers_Users]    FOREIGN KEY ([UserId])   REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [CK_FamilyMembers_Role] CHECK ([Role] IN (0, 1))
);
GO

-- ============ Lookup.Categories ============
CREATE TABLE [Lookup].[Categories]
(
    [Id]        INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Categories] PRIMARY KEY,
    [NameAr]    NVARCHAR(100) NOT NULL,
    [NameEn]    NVARCHAR(100) NOT NULL,
    [IconClass] NVARCHAR(100) NULL,
    [SortOrder] INT NOT NULL CONSTRAINT [DF_Categories_SortOrder] DEFAULT (0),
    [IsActive]  BIT NOT NULL CONSTRAINT [DF_Categories_IsActive] DEFAULT (1)
);
GO

-- ============ Items (catalog + pantry + ad-hoc, unified) ============
CREATE TABLE [dbo].[Items]
(
    [Id]              INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Items] PRIMARY KEY,
    [FamilyId]        INT NOT NULL,
    [Name]            NVARCHAR(150) NOT NULL,
    [CategoryId]      INT NULL,
    [State]           TINYINT NOT NULL CONSTRAINT [DF_Items_State] DEFAULT (0), -- 0 = Mojoud, 1 = Naqis, 2 = Mukhlis
    [IsAdHoc]         BIT NOT NULL CONSTRAINT [DF_Items_IsAdHoc] DEFAULT (0),
    [ImagePath]       NVARCHAR(500) NULL,
    [CreatedByUserId] INT NOT NULL,
    [UpdatedByUserId] INT NULL,
    [CreatedAt]       DATETIME2 NOT NULL CONSTRAINT [DF_Items_CreatedAt] DEFAULT GETDATE(),
    [UpdatedAt]       DATETIME2 NULL,
    [IsArchived]      BIT NOT NULL CONSTRAINT [DF_Items_IsArchived] DEFAULT (0),
    CONSTRAINT [UQ_Items_Family_Name] UNIQUE ([FamilyId], [Name]),
    CONSTRAINT [FK_Items_Families]   FOREIGN KEY ([FamilyId])        REFERENCES [dbo].[Families] ([Id]),
    CONSTRAINT [FK_Items_Categories] FOREIGN KEY ([CategoryId])      REFERENCES [Lookup].[Categories] ([Id]),
    CONSTRAINT [FK_Items_CreatedBy]  FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Items_UpdatedBy]  FOREIGN KEY ([UpdatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [CK_Items_State] CHECK ([State] IN (0, 1, 2))
);
GO

CREATE INDEX [IX_Items_Family_State] ON [dbo].[Items] ([FamilyId], [State]) WHERE [IsArchived] = 0;
GO

-- ============ Recipes ============
CREATE TABLE [dbo].[Recipes]
(
    [Id]              INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Recipes] PRIMARY KEY,
    [FamilyId]        INT NOT NULL,
    [Name]            NVARCHAR(150) NOT NULL,
    [Steps]           NVARCHAR(MAX) NULL,
    [PhotoPath]       NVARCHAR(500) NULL,
    [CreatedByUserId] INT NOT NULL,
    [CreatedAt]       DATETIME2 NOT NULL CONSTRAINT [DF_Recipes_CreatedAt] DEFAULT GETDATE(),
    [IsArchived]      BIT NOT NULL CONSTRAINT [DF_Recipes_IsArchived] DEFAULT (0),
    CONSTRAINT [FK_Recipes_Families] FOREIGN KEY ([FamilyId])        REFERENCES [dbo].[Families] ([Id]),
    CONSTRAINT [FK_Recipes_Users]    FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])
);
GO

-- ============ RecipeIngredients ============
CREATE TABLE [dbo].[RecipeIngredients]
(
    [Id]           INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RecipeIngredients] PRIMARY KEY,
    [RecipeId]     INT NOT NULL,
    [ItemId]       INT NOT NULL,
    [QuantityText] NVARCHAR(100) NULL,
    [IsOptional]   BIT NOT NULL CONSTRAINT [DF_RecipeIngredients_IsOptional] DEFAULT (0),
    [SortOrder]    INT NOT NULL CONSTRAINT [DF_RecipeIngredients_SortOrder] DEFAULT (0),
    CONSTRAINT [UQ_RecipeIngredients_Recipe_Item] UNIQUE ([RecipeId], [ItemId]),
    CONSTRAINT [FK_RecipeIngredients_Recipes] FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipes] ([Id]),
    CONSTRAINT [FK_RecipeIngredients_Items]   FOREIGN KEY ([ItemId])   REFERENCES [dbo].[Items] ([Id])
);
GO

-- ============ Lookup.Localizations ============
CREATE TABLE [Lookup].[Localizations]
(
    [Id]      INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Localizations] PRIMARY KEY,
    [Key]     NVARCHAR(200) NOT NULL,
    [ValueAr] NVARCHAR(500) NOT NULL,
    [ValueEn] NVARCHAR(500) NOT NULL,
    CONSTRAINT [UQ_Localizations_Key] UNIQUE ([Key])
);
GO
