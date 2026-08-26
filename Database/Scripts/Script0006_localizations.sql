-- =====================================================
-- Script0006: UI localizations — recipes (index/details/builder)
-- Key / ValueAr / ValueEn
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'Recipes.Title', N'الوصفات', N'Recipes'),
(N'Recipes.NewButton', N'وصفة جديدة', N'New recipe'),
(N'Recipes.Empty', N'ما فيه وصفات — أضف أول وصفة', N'No recipes yet — add your first one'),
(N'Recipes.CreateTitle', N'وصفة جديدة', N'New recipe'),
(N'Recipes.NamePlaceholder', N'اسم الوصفة', N'Recipe name'),
(N'Recipes.IngredientsTitle', N'المكونات', N'Ingredients'),
(N'Recipes.AddIngredient', N'أضف مكوّن', N'Add ingredient'),
(N'Recipes.IngredientPlaceholder', N'اسم المكوّن', N'Ingredient name'),
(N'Recipes.QuantityPlaceholder', N'الكمية', N'Quantity'),
(N'Recipes.Optional', N'اختياري', N'Optional'),
(N'Recipes.RemoveIngredient', N'حذف المكوّن', N'Remove ingredient'),
(N'Recipes.Save', N'حفظ الوصفة', N'Save recipe'),
(N'Recipes.Error', N'تأكد من الاسم والمكونات وحاول مرة أخرى', N'Check the name and ingredients, then try again'),
(N'Recipes.MissingTitle', N'الناقص من المخزن', N'Missing from your pantry'),
(N'Recipes.AddMissing', N'أضف الناقص إلى قائمة التسوق', N'Add missing to shopping list'),
(N'Recipes.Available', N'موجود', N'Available'),
(N'Recipes.Missing', N'ناقص', N'Missing'),
(N'RecipeBadge.Doable', N'جاهزة', N'Ready to cook'),
(N'RecipeBadge.MissingFew', N'ينقصها شوي', N'Missing a few'),
(N'RecipeBadge.MissingALot', N'ينقصها كثير', N'Missing a lot');
GO
