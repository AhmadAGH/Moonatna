-- =====================================================
-- Script0018: fix the Detergents icon
--
-- Script0017 set [IconClass] = 'jug-detergent'. Font Awesome needs the
-- style prefix and the fa- prefix on the name — the glyph is
-- 'fa-solid fa-jug-detergent'. A bare 'jug-detergent' matches no rule in
-- Font Awesome's stylesheet, so the category drew an empty icon.
-- =====================================================
UPDATE [Lookup].[Categories]
SET [IconClass] = N'fa-solid fa-jug-detergent'
WHERE [Id] = 9 AND [IconClass] = N'jug-detergent';
GO
