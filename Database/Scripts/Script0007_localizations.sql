-- =====================================================
-- Script0007: UI localizations — recipe details/edit/delete
-- Key / ValueAr / ValueEn
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'Recipes.EditTitle', N'تعديل الوصفة', N'Edit recipe'),
(N'Recipes.EditButton', N'تعديل', N'Edit'),
(N'Recipes.DeleteButton', N'حذف', N'Delete'),
(N'Recipes.DeleteConfirmTitle', N'حذف الوصفة؟', N'Delete this recipe?'),
(N'Recipes.DeleteConfirmBody', N'سيتم إخفاء الوصفة نهائيًا عن قائمتك.', N'This recipe will be hidden from your list.'),
(N'Recipes.CancelButton', N'إلغاء', N'Cancel'),
(N'Common.Confirm', N'تأكيد', N'Confirm');
GO
