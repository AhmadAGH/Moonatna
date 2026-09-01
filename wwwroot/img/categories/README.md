# Category icons

Designed icons for the categories in `[Lookup].[Categories]`. Drop the files
here, then point the category row at one.

## Adding an icon

1. Save the file in this folder, e.g. `dairy.svg`.
2. Switch the category over:

```sql
UPDATE [Lookup].[Categories]
SET [MediaType] = 2, [IconPath] = N'/img/categories/dairy.svg'
WHERE [Id] = 4;
```

`MediaType` picks which icon the app draws:

| MediaType | Column read | Drawn as |
| --------- | ----------- | -------- |
| `1` (default) | `IconClass` — e.g. `fa-solid fa-carrot` | Font Awesome glyph, inherits the surrounding text colour |
| `2` | `IconPath` — e.g. `/img/categories/dairy.svg` | `<img>`, so the file keeps its own colours |

Setting `MediaType = 2` without an `IconPath` falls back to the Font Awesome
class, so a half-finished row never renders a broken image. `IconClass` is
left in place either way — flipping `MediaType` back to `1` restores the glyph.

## What the files should be

- **SVG** preferred (any raster format the browser can show also works).
- **Square**, roughly 64×64 in the viewBox — they're drawn at ~1.2rem in the
  Organize board and ~2.2–2.6rem on the item cards, scaled with `object-fit: contain`.
- Colours are **the file's own**. These icons are drawn as `<img>`, so they do
  not pick up the app's `--primary` the way the Font Awesome glyphs do. If an
  icon should follow the theme colour instead, say so — that needs the SVG
  inlined rather than linked.
- Keep them small; they load on every pantry/shopping/organize render.
