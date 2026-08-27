-- =====================================================
-- Script0004: nav dock + quick-add fan localizations
-- Key / ValueAr / ValueEn
-- Run once after Script0003.
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'Nav.Main', N'التنقل', N'Navigation'),
(N'Nav.Add', N'إضافة', N'Add'),
(N'QuickAdd.Quick', N'سريعة', N'Quick'),
(N'QuickAdd.AdHoc', N'مؤقت', N'Ad-hoc'),
(N'QuickAdd.Full', N'بتفاصيل', N'Full details'),
(N'QuickAdd.Title.Quick', N'إضافة سريعة', N'Quick add'),
(N'QuickAdd.Title.AdHoc', N'إضافة صنف مؤقت', N'Add ad-hoc item'),
(N'QuickAdd.Title.Full', N'إضافة بتفاصيل', N'Add with details'),
(N'QuickAdd.NamePlaceholder', N'اسم الصنف…', N'Item name…'),
(N'QuickAdd.Photo', N'إضافة صورة', N'Add photo'),
(N'QuickAdd.Submit', N'إضافة', N'Add'),
(N'QuickAdd.UiOnly', N'معاينة فقط — الحفظ في الخطوة القادمة', N'Preview only — saving comes next');
