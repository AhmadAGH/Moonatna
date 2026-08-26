-- =====================================================
-- Script0005: fix item-state labels — موجود / ناقص / خلص
-- =====================================================
UPDATE [Lookup].[Localizations] SET [ValueAr] = N'ناقص' WHERE [Key] = N'ItemState.RunningLow';
UPDATE [Lookup].[Localizations] SET [ValueAr] = N'خلص'  WHERE [Key] = N'ItemState.OutOfStock';
GO
