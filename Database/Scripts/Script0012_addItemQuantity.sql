-- Script0012: optional numeric quantity per item (e.g. "2" بجانب "أرز بسمتي").
-- Nullable — most items have no quantity tracked, it's an opt-in detail.
ALTER TABLE [dbo].[Items]
    ADD [Quantity] INT NULL CONSTRAINT [CK_Items_Quantity] CHECK ([Quantity] IS NULL OR [Quantity] > 0);
GO
