# Strapped

Unity project for the short retro platformer `Strapped`.

Public page: https://gnems19.itch.io/strapped

You have been inexplicably abducted by an evil corporation, your purpose unknown and your fate uncertain. Trapped and bound with remote explosives threatening your every move, you must summon every ounce of courage to escape their clutches.

## Project Layout

- `Assets/Scenes/MainMenu.unity`: main menu scene.
- `Assets/Scenes/StartLevel.unity`: opening gameplay scene.
- `Assets/Scenes/BossLevel.unity`: boss fight scene.
- `Assets/Scenes/EndScene.unity`: ending scene.
- `Assets/Scripts/PlayerScripts`: player movement, animation, death, audio.
- `Assets/Scripts/EnemyScripts`: enemy and boss logic.
- `Assets/Scripts/InteractableItemsScripts`: doors, dialogue, outlets, and interactables.
- `Assets/Scripts/UIScripts`: menu, audio, mobile controls, and screen-fit helpers.

## Build Order

The build settings currently use this scene order:

1. `MainMenu`
2. `StartLevel`
3. `BossLevel`
4. `EndScene`

## Menu Sizing Note

The main menu uses a mix of `CanvasScaler`-driven UI and `SpriteRenderer`-based menu art. The `FitSpriteToScreen` helper in `Assets/Scripts/UIScripts/FitSpriteToScreen.cs` now applies its scale in `Start()` immediately instead of waiting one frame. That avoids the visible resize/pop that happened after the menu scene loaded.

## Opening The Project

Open the repository root in Unity Hub and use the scene files under `Assets/Scenes`.

## Future Features

- Raise the overall visual contrast in gameplay areas so hazards and enemies read faster.
- Make the first enemy more visible at a glance, especially against nearby background tiles.
- Investigate pixel-art-friendly highlight treatments in Unity, such as subtle glow, rim light, outline, or emissive-style effects that do not break the art style.
