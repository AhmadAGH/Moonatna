-- Adds optional decimal quantity to items for pantry and shopping lists
ALTER TABLE [dbo].[Items]
    ADD [Quantity] DECIMAL(6,2) NULL
        CONSTRAINT [CK_Items_Quantity] CHECK ([Quantity] IS NULL OR [Quantity] > 0);
