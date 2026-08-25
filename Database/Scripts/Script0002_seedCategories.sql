-- ============ Seed: Categories (FA free solid classes) ============
INSERT INTO [Lookup].[Categories] ([NameAr], [NameEn], [IconClass], [SortOrder]) VALUES
(N'خضار',            N'Vegetables',     N'fa-solid fa-carrot',          1),
(N'فواكه',           N'Fruits',         N'fa-solid fa-apple-whole',     2),
(N'لحوم ودواجن',     N'Meat & Poultry', N'fa-solid fa-drumstick-bite',  3),
(N'ألبان وأجبان',    N'Dairy & Cheese', N'fa-solid fa-cheese',          4),
(N'بهارات وتوابل',   N'Spices',         N'fa-solid fa-pepper-hot',      5),
(N'معلبات',          N'Canned Goods',   N'fa-solid fa-box',             6),
(N'مخبوزات',         N'Bakery',         N'fa-solid fa-bread-slice',     7),
(N'مشروبات',         N'Beverages',      N'fa-solid fa-mug-saucer',      8),
(N'مستلزمات منزلية', N'Household',      N'fa-solid fa-pump-soap',       9),
(N'أخرى',            N'General',        N'fa-solid fa-basket-shopping', 99);
GO