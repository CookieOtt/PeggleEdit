# Feature Guide

PeggleEdit is a visual level-pack editor for Peggle Nights. It edits `.pak` level packs, individual `.dat` levels, embedded images, challenge data, and the objects that make up a playable level.

## Main Window

- **Pack Explorer** shows the pack, levels, images, and challenges. Right-click items to add, import, export, rename, delete, edit properties, or set a level background.
- **Level Editor** is the main canvas for placing and editing pegs, bricks, and objects.
- **Properties** shows the selected object's editable values.
- **Entry List** shows the entries in the current level.
- **Status bar** shows contextual controls for the active tool.

Use the View tab to reopen Pack Explorer, Properties, and Entry List if they are closed.

## Files and Packs

- **New / Open / Save / Save As** are available from the file menu and quick-access toolbar.
- PeggleEdit tracks unsaved changes and asks whether to save, discard, or cancel when closing or switching packs.
- New levels automatically receive unique internal filenames.
- Saving and exporting level packs is blocked if two levels have the same internal filename, because Peggle stores level files by filename inside the pack.
- If a pack CFG references a missing level file, PeggleEdit warns and skips that missing level instead of failing with an unclear error.

To play a level pack, save it into the Peggle Nights `levelpacks` folder. See [Frequently Asked Questions](FrequentlyAskedQuestions.md) for the default path.

## Editing Tools

The Tools tab contains the main placement tools:

- **Select** selects, moves, nudges, deletes, orders, and edits objects.
- **Eyedropper** copies the type and settings of an existing object into the matching placement tool.
- **Peg** places round pegs.
- **Brick** places straight bricks.
- **Curved Brick** places curved brick segments.
- **Peg Pen** and **Brick Pen** draw connected paths of pegs or bricks.
- **Circle, Polygon, Rod, Teleport, and Emitter** place non-peg level objects.
- **Generator** creates circular or grid layouts that can be adjusted before applying.

Common shortcuts:

- `Ctrl+Z`: Undo
- `Ctrl+Y` or `Ctrl+Shift+Z`: Redo
- `Ctrl+X`, `Ctrl+C`, `Ctrl+V`: Cut, copy, paste
- `Delete`: Delete selected objects
- `Ctrl+A`: Select all objects
- Arrow keys: Nudge selected objects
- `Home` / `End`: Bring to front / send to back
- `Page Up` / `Page Down`: Move forward / backward in draw order
- `Shift+1` through `Shift+7`: Select the first seven tools in the Tools tab

The status bar shows extra controls when a tool has them.

## Pen Paths

Peg Pen and Brick Pen let you draw paths and then edit them later:

- Click or drag to add points.
- Hold `Ctrl` while placing to snap the angle.
- Right-click to finish the path.
- Select the path later to move its points and curve handles.
- Press `Delete` or `Backspace` while editing a selected path to remove a segment.
- Press `C` to toggle the selected segment between a line and a curve.
- Changing the path interval updates the generated pegs or bricks immediately.

Generators and pen paths are editor helpers. Apply generators before using the level in Peggle.

## Movement

Select one or more objects, then use the Object tab:

- **Type** assigns a built-in movement type, such as vertical, horizontal, circle, arc, wrap, or rotate.
- **Map Path** lets you draw a custom movement path directly on the canvas.
- **Phase** spreads selected moving objects around a movement cycle.
- **Duplicate & Phase** duplicates one moving object and spaces the copies around the cycle.
- **Link Sub-movements** connects nested movement data for advanced movement setups.

Map Path controls:

- Click to set path points.
- Two points create a back-and-forth line path.
- Three or more points create a closed box-style path.
- Hold `Shift` to snap from the previous point.
- Press `Enter` or right-click to apply.
- Press `Backspace` to remove the last point.
- Press `Esc` to cancel.
- Turn on Preview in the View tab to test the result.

See [Advanced Movement](AdvancedMovement.md) for nested movement details.

## Backgrounds and Images

Right-click a level in Pack Explorer and choose **Set Background** to import an image. The import screen lets you:

- Drag the image into position.
- Use the mouse wheel or Scale field to resize it.
- Use **Fit**, **Fill**, and **Center** for quick layout.
- Toggle **Show UI overlay** to preview how the Peggle interface will cover the background.
- Export an 800x600 background into the level when you press OK.

Right-click an image in Pack Explorer to open, rename, delete, or export it.

## View and Theme

The View tab can show or hide:

- Background
- Interface overlay
- Objects
- Collision
- Preview
- Particles

Dark mode is available from PeggleEdit Options.

## Playability Notes

Pegs, bricks, and normal level entries can be used by Peggle. Editor-only generator objects cannot be used by Peggle until applied. PeggleEdit warns when saving a level pack that still contains generator-style objects.
