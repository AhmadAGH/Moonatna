-- =====================================================
-- Script0019: make sure [dbo].[Items] actually has [Quantity]
--
-- Script0012_addItemQuantity.sql was never listed as an <EmbeddedResource>
-- in Moonatna.csproj, so DbUp has never run it in any environment.
-- Dev happens to have the column anyway — it was applied out of band, and
-- dev's journal records it under "Script0012_itemQuantity.sql", a filename
-- that never existed in git. Production never got it.
--
-- Editing an item writes [Quantity] (ItemsRepository.UpdateAsync), so on
-- Production that UPDATE fails with "Invalid column name 'Quantity'" while
-- working locally. Reads were fine: they go through SELECT *, and Dapper
-- just leaves the property null when the column isn't there.
--
-- Guarded rather than a plain ALTER, because it has to be safe on both:
-- dev skips it, Production adds the column. A bare ALTER would fail on dev
-- and Program.cs throws on a failed migration, taking the app down on boot.
-- =====================================================
IF COL_LENGTH('dbo.Items', 'Quantity') IS NULL
BEGIN
    ALTER TABLE [dbo].[Items]
        ADD [Quantity] INT NULL
            CONSTRAINT [CK_Items_Quantity] CHECK ([Quantity] IS NULL OR [Quantity] > 0);
END
GO
