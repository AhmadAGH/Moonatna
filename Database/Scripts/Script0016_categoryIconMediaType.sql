-- =====================================================
-- Script0016: a category's icon can be a designed local file
--
-- [MediaType] decides how the icon is drawn:
--   1 = Font Awesome class, read from [IconClass]  (every existing row)
--   2 = local icon file,    read from [IconPath]   (the designer's SVGs,
--                                                   e.g. /img/categories/dairy.svg)
--
-- Existing rows default to 1 and keep rendering exactly as before.
-- To move a category onto a designed icon, drop the file in
-- wwwroot/img/categories/ and point the row at it:
--
--   UPDATE [Lookup].[Categories]
--   SET [MediaType] = 2, [IconPath] = N'/img/categories/dairy.svg'
--   WHERE [Id] = <id>;
--
-- Clearing [IconPath] (or setting [MediaType] back to 1) falls straight
-- back to the Font Awesome class, which is left in place either way.
-- =====================================================
ALTER TABLE [Lookup].[Categories]
    ADD [MediaType] TINYINT NOT NULL CONSTRAINT [DF_Categories_MediaType] DEFAULT (1),
        [IconPath]  NVARCHAR(255) NULL;
GO
