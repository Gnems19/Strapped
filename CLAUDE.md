# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**Strapped** is a 2D pixel art Unity platformer evolving into a Hollow Knight / Silksong-style exploration sidescroller. Originally a game jam entry, the game is being expanded with new areas, NPCs, and exploration mechanics.

**Lore:** You've been abducted by an evil corporation and strapped with remote explosives. You must escape their clutches.

**Art constraints:** Flat 2D pixel art, Gameboy-inspired 4-color palette, characters max 32x32 pixels. Sprites authored in Aseprite and imported natively via the Unity Aseprite package.

## Development Commands

This is a Unity project — there is no CLI build or test runner. All building, running, and testing is done through the **Unity Editor** (open `Assets/Scenes/BossLevel.unity`).

- **Run the game**: Press Play in the Unity Editor
- **Build**: File → Build Settings → Build
- **No automated tests exist** in this project

## Architecture

### Custom Physics Player (no Rigidbody)

`PlayerController.cs` implements fully custom 2D physics using raycasts — **the player has no Rigidbody2D**. It manually handles:
- Horizontal movement with acceleration/deceleration
- Gravity with variable fall speed (min/max interpolated by apex proximity)
- Jump with coyote time (`_coyoteTimeThreshold`) and jump buffering (`_jumpBuffer`)
- Apex bonus speed boost at the peak of a jump
- Collision via `OverlapBox` + incremental position resolution (`_freeColliderIterations`)

`PlayerExtras.cs` defines the shared interfaces and structs used across all player scripts:
- `IPlayerController` — consumed by `PlayerAnimator` and `PlayerSoundController`
- `IPlayerDeathController` — consumed by `PlayerController` (checks `IsDead` each frame)
- `FrameInput` struct — carries X, JumpDown, JumpUp each frame
- `RayRange` struct — used by the collision system

### Interface-Driven Design

Systems communicate through interfaces rather than concrete types:
- `IPlayerController` / `IPlayerDeathController` — player state exposed to animators and other scripts
- `IBossController` — boss state (IsDead, IsSpecialAttacking, IsSpeaking)
- `IPowerOutlet` — outlet state (PluggedIn) consumed by BossController

### Death System

`PlayerDeathController` (on the player's parent GameObject) sets `IsDead = true` and calls `RestartGame()` (which loads scene index 1) when:
1. Player falls below y = -10
2. Player enters a trigger tagged `"EnemySight"`
3. Player enters a trigger tagged `"HomingMissile"`

When dead, `PlayerController` stops reading real input and forces `X=0, JumpUp=true` to let the death animation play out.

### Boss Fight Sequence

1. Player walks into the `EnterBossAreaScript` trigger → camera zooms, boss music plays, boss GameObject activates, entrance door closes
2. `BossScript` (on the boss) chases the player horizontally (Y never changes), accumulating `time_passed`; after 2 seconds it triggers the `LaunchingMissiles` animator bool
3. The boss animation calls `LaunchMissile()` as an **animation event** → activates `MissleLaunchScript` GameObject
4. `MissleLaunchScript` spawns 12 homing missiles in a staggered grid (2×6) with coroutine delays every 5 seconds
5. `HomingMissile` uses `Rigidbody2D` angular velocity to home in on the player. On ground collision it destroys itself and `Instantiate`s an explosion prefab + plays explosion sound
6. Boss dies when the player presses **E** near `OutletScript` (within 1.5 units) → triggers `PluggedOut` animation on both the outlet and boss → `BossController` reads `IPowerOutlet.PluggedIn == false` and sets `IsDead = true`

### SoundManager

Singleton (`SoundManager.Instance`) with two `AudioSource`s: background music and SFX. Call `PlaySFX(clip)` for one-shots. `PlayBossMusic()` switches BGM to `level2Song`. Volume is controlled independently for music vs SFX.

### Logger

`Logger` is a `ScriptableObject` (serialized field on many MonoBehaviours). It wraps `Debug.Log` and can be toggled off in the Inspector without removing log calls from code. Always inject it via `[SerializeField]` rather than using `Debug.Log` directly.

### Tags Used for Gameplay Logic

| Tag | Purpose |
|-----|---------|
| `"Player"` | Targeted by missiles, detected by DetectionZone |
| `"EnemySight"` | Trigger that kills the player on contact |
| `"HomingMissile"` | Trigger that kills the player on contact |
| `"Ground"` | Layer missiles explode against |

### Script Organization

```
Assets/Scripts/
  PlayerScripts/     — PlayerController, PlayerAnimator, PlayerDeathController, PlayerSoundController
                       PlayerExtras.cs (shared interfaces + structs, no MonoBehaviour)
  EnemyScripts/      — EnemyScript, BossScript, BossController, BossAnimator, DetectionZone, IBossController
  InteractableItemsScripts/ — PowerOutlet, PowerOutletAnimator, BedScript, DoorScript, IPowerOutlet, CompyDialogue
  UIScripts/         — SoundManager, MenuManagerScript, SettingsScript, volume sliders
  CameraScript.cs    — Horizontal follow with left/right border clamps
  parallax.cs        — Parallax background scrolling
  DialogueScript.cs  — Stub only, not implemented (CompyDialogue is the active dialogue system)
Assets/              — Root-level scripts are in-progress / working copies:
                       HomingMissile.cs, MissleLaunchScript.cs, OutletScript.cs,
                       EnterBossAreaScript.cs, DestroyAfterAnimation.cs
```

> Root-level `Assets/*.cs` scripts are the actively used versions for boss-fight mechanics. The older `Assets/Scripts/EnemyScripts/HomingMissile.cs` was deleted. `Assets/BossScript.cs` at the root is a stale duplicate — the canonical boss script is `Assets/Scripts/EnemyScripts/BossScript.cs`. Archived duplicate scenes live in `Assets/Archived/`.

### NPC Dialogue (CompyDialogue)

`CompyDialogue.cs` is attached to the `Terrain/Compy` tilemap in StartLevel. It auto-detects the tilemap bounds center for interaction range. Features:
- Press **E** near Compy to trigger dialogue (typewriter effect)
- `static int TimesInteracted` tracks interactions across scene reloads
- After 3 interactions, auto-shows "Go!" without pressing E
- Builds a world-space Canvas at runtime with Gameboy colors (`#0f380f` bg, `#8bac0f` text)
- Uses Electronic Highway Sign SDF font (loaded from TMP Resources)
- Canvas must use sorting layer `"Player"` with high order to render above tilemap layers

### Sorting Layers (back to front)

`Default → Parallax → Background → Interactable Objects → Ground → Enemie → Player`

Any world-space UI (dialogue bubbles, etc.) must use `"Player"` sorting layer to appear above all tilemap layers.

### Mobile Controls

`MobileControls.cs` is a self-bootstrapping singleton. It creates its own screen-overlay Canvas with directional, jump, and interact buttons at runtime. Hidden on desktop; visible on Android/iOS. Uses `DontDestroyOnLoad` to persist across scenes. The component must exist in any gameplay scene (currently in StartLevel and BossLevel).

### TODO: Aspect Ratio

The `PixelPerfectCamera` reference resolution is **288x160 (9:5 = 1.8:1)**, not exactly 16:9 (1.778:1). It handles letterboxing/pillarboxing via `cropFrameX/Y + stretchFill`. To switch to true 16:9, change `refResolutionX/Y` to **320x180** (or 256x144) on the Main Camera's PixelPerfectCamera in both StartLevel and BossLevel. This will change how much of the world is visible, so level geometry may need adjusting.

Also: `InteractionButtonPressed.png` does not exist yet — the interact button currently tints darker on press instead of swapping sprites.
