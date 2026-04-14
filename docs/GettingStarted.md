# 1. Operating Systems #
PeggleEdit is a Windows application. Current builds target .NET Framework 4.7.2.

# 2. Downloading and Compiling #

There are three ways to obtain a copy of PeggleEdit:
  1. A **ready-to-use executable** (compiled binary) in an archive for the latest stable release can be downloaded from the [download page](https://github.com/IntelOrca/PeggleEdit/releases).
  1. The **source code** of the latest stable release can be downloaded as an archive from the [download page](https://github.com/IntelOrca/PeggleEdit/releases).
  1. The very latest development source code can be <a href='https://github.com/IntelOrca/PeggleEdit'>cloned from <b>GitHub</b></a>.

If you've gone with option 2 or 3, then you will need to compile PeggleEdit. Open `PeggleEdit.sln` in Visual Studio or run `dotnet build` from the repository root.

# 3. Create a Level Pack #

1. Open `peggleedit.exe`.
2. Use Pack Explorer to right-click **Levels** and add or import a level.
3. Double-click a level to open it in the editor.
4. Use the Tools tab to place pegs, bricks, paths, objects, and generators.
5. Use the Properties window to adjust the selected object.
6. Save the pack as a `.pak` file.
7. Copy the saved pack to the Peggle Nights `levelpacks` folder to test it in the game.

# 4. Useful First Features #

- Use **Undo** and **Redo** from the quick-access toolbar or with `Ctrl+Z` and `Ctrl+Y`.
- Use **Set Background** from a level's right-click menu to crop, resize, and move a background image before importing it.
- Use **Map Path** on the Object tab to create custom movement paths.
- Use **Generator** on the Tools tab for circular or grid layouts, then apply the generator before playing the level in Peggle.
- Use Peg Pen and Brick Pen to draw editable paths. Select the path later, then press `C` to toggle a segment between line and curve.

For a fuller overview, see the [Feature Guide](FeatureGuide.md). If you have questions, see the [FAQ](FrequentlyAskedQuestions.md).
