# Lantern Hollow
2D platformer. MonoGame 3.8, DesktopVK (Mac), .NET 10, C#.

## Commands
- Build: dotnet build -warnaserror
- Test:  dotnet test
- Run:   dotnet run --project Platforms/DesktopVK

## Map
- Source/Game1.cs    the game loop, shared by every platform project
- Source/Tuning.cs   all game numbers (pixels and seconds, Y points down)
- Tests/             xUnit tests, run with dotnet test
- Content/Assets/    raw art and sound; register each file in Content/Builder/Builder.cs
- Platforms/DesktopVK/  entry point only, no game logic

## Rules
- All game numbers live in Source/Tuning.cs.
- Never edit a test to make it pass.
- Keep changes small. Touch only files named
  in the task. Ask before adding a package.
- Edit files with the editor tools (write_to_file or replace_in_file).
- Never create or change files with a terminal command, a Python script, sed, echo or a heredoc. - Use the terminal only for: dotnet build, dotnet test, dotnet run, git.
- After every change: build, then test.



## Not now
No menus, no save system, no multiplayer.
