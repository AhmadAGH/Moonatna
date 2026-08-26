-- =====================================================
-- Script0008: badge labels → المونة branding
-- =====================================================
UPDATE [Lookup].[Localizations] SET [ValueAr] = N'المونة ناقصة شوي'  WHERE [Key] = N'RecipeBadge.MissingFew';
UPDATE [Lookup].[Localizations] SET [ValueAr] = N'المونة مرة ناقصة' WHERE [Key] = N'RecipeBadge.MissingALot';
GO
