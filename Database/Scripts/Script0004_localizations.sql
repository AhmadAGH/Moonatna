-- =====================================================
-- Script0004: UI localizations — pantry, shopping, item states
-- Key / ValueAr / ValueEn
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'Pantry.Title', N'المخزن', N'Pantry'),
(N'Pantry.AddPlaceholder', N'اسم العنصر الجديد…', N'New item name…'),
(N'Pantry.AddButton', N'إضافة', N'Add'),
(N'Pantry.Empty', N'المخزن فارغ — أضف أول عنصر', N'Your pantry is empty — add your first item'),
(N'Shopping.Title', N'قائمة التسوق', N'Shopping list'),
(N'Shopping.CopyList', N'نسخ القائمة', N'Copy list'),
(N'Shopping.Copied', N'تم النسخ', N'Copied'),
(N'Shopping.Empty', N'قائمة التسوق فارغة', N'Your shopping list is empty'),
(N'Shopping.PurchaseButton', N'اشتريت', N'Bought'),
(N'ItemState.Available', N'موجود', N'Available'),
(N'ItemState.RunningLow', N'أريده', N'Running low'),
(N'ItemState.OutOfStock', N'مخلص', N'Out of stock'),
(N'Common.Uncategorized', N'بدون تصنيف', N'Uncategorized');
GO
