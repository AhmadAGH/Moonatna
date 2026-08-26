-- =====================================================
-- Script0009: UI localizations — organize, ad-hoc tag, shopping add/purchased
-- Key / ValueAr / ValueEn
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'Organize.Title', N'التنظيم', N'Organize'),
(N'Organize.Hint', N'اسحب العنصر إلى التصنيف المناسب', N'Drag an item onto the right category'),
(N'Item.AdHoc', N'مؤقت', N'One-off'),
(N'Shopping.Purchased', N'تم الشراء', N'Purchased'),
(N'Shopping.AddPlaceholder', N'شيء تحتاجه هالمرة…', N'Something you need this time…'),
(N'Shopping.AddButton', N'إضافة', N'Add');
GO
