UPDATE [Lookup].[Categories] SET NameAr = N'منظفات', NameEn = 'Detergents',IconClass = 'jug-detergent' WHERE ID = 9
INSERT INTO [Lookup].[Categories] VALUES
(N'مستلزمات الطبخ',
'Cooking supplies',
'fa-solid fa-utensils',
10,
1,
2,
'/img/categories/cockSupply.svg'
)
INSERT INTO [Lookup].[Categories] VALUES
(N'حبوب',
'Grain',
'fa-solid fa-bowl-rice',
11,
1,
1,
null
)
INSERT INTO [Lookup].[Categories] VALUES
(N'بهارات',
'Spices',
'fa-solid fa-mortar-pestle',
12,
1,
1,
null
)

INSERT INTO [Lookup].[Categories] VALUES
(N'مفرحات',
'Snacks',
'fa-solid fa-cookie',
13,
1,
1,
null
)

INSERT INTO [Lookup].[Categories] VALUES
(N'مستلزمات الطفل',
'Baby essentials',
'fa-solid fa-baby',
14,
1,
1,
null
)

UPDATE Lookup.Categories SET IconPath = '/img/categories/vegetable.svg',MediaType = 2 WHERE ID = 1
UPDATE Lookup.Categories SET IconPath = '/img/categories/fruits.svg',MediaType = 2 WHERE ID = 2
UPDATE Lookup.Categories SET IconPath = '/img/categories/milk.svg',MediaType = 2 WHERE ID = 4