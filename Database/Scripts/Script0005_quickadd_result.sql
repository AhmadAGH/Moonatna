-- =====================================================
-- Script0005: quick-add result toasts (backend wiring)
-- Key / ValueAr / ValueEn
-- Run once after Script0004.
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'QuickAdd.Added', N'تمت الإضافة', N'Added'),
(N'QuickAdd.Error', N'تعذّرت الإضافة — حاول مجدداً', N'Couldn''t add — try again');
