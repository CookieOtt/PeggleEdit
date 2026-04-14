# Generators

Generators are editor-only objects that generate multiple pegs or bricks automatically. They are useful for layouts where positions and spacing are easier to calculate than place by hand.

Because they are objects that can only be read by PeggleEdit, saved levels containing them cannot be opened in Peggle. You can save your level without applying the generators but when you want to play them in Peggle, you must select your generators and click the apply button on the Tools tab on the ribbon at the top of the main window.

PeggleEdit warns when saving a pack that still contains generator-style objects.

## Circle Generators

**Peg Generator** creates a circle or oval of pegs. Important properties include the X radius, Y radius, number of pegs, and peg settings.

**Brick Generator** creates a circular arrangement of bricks. Important properties include the radius, brick count, brick length, and brick width.

## Grid Generators

**Peg Grid Generator** creates a rectangular grid of pegs. Important properties include column spacing, row spacing, and alternate row offset. Use an alternate row offset of about half the column spacing for a honeycomb-style pattern.

**Brick Grid Generator** creates a grid of bricks. Important properties include column spacing, row spacing, alternate row offset, brick length, brick width, and brick rotation.

## Using a Generator

1. Open the Tools tab.
2. Choose **Generator** and select a peg or brick generator type.
3. Place the generator in the level.
4. Select the generator and adjust its properties in the Properties window.
5. When the result looks right, click **Apply** on the Tools tab.

After applying, the generated pegs or bricks become normal level objects and the generator is removed.
