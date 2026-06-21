# Harmony of Rhythms

A 2D rhythm action game built in Unity. When heaven's rhythm breaks, a human is chosen to restore it—first through the trials of purgatory, then by repairing the great clock in heaven.

## About

**Harmony of Rhythms** blends rhythm defense with a story-driven journey:

1. **Purgatory** — Walk forward while blocking colored enemy beams in sync with the music.
2. **Heaven** *(planned)* — Repair the celestial clock using beat-based mechanics.

## Current Status

Early prototype. The purgatory section is playable:

- Five directional hitboxes (left, right, upper-left, upper-right, ground front)
- Perfect / Early Perfect / Late Perfect / Early / Late / Miss grading
- Manual chart entries in `Assets/Charts/`
- Score HUD and world-space hit feedback
- Placeholder art and test chart

## Controls

| Key | Action |
|-----|--------|
| `D` | Left melee |
| `K` | Right melee |
| `F` | Upper-left |
| `J` | Upper-right |
| `Space` | Ground front (jump) |

Press the matching key when a threat reaches its hit marker.

## Requirements

- Unity 6 (6000.x) or the version listed in `ProjectSettings/ProjectVersion.txt`
- Universal Render Pipeline (2D)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/mperitv/Harmony-of-Rhythms.git
   ```
2. Open the project in Unity Hub.
3. Open `Assets/Scenes/GameScene.unity`.
4. Press Play and focus the Game view so keyboard input works.

## Project Structure

```
Assets/
├── Charts/             # Manual threat charts
├── Music/              # Soundtrack files
├── Prefabs/            # Threat and hit point prefabs
├── Scenes/             # GameScene
└── Scripts/Purgatory/  # Gameplay scripts
    ├── RhythmGameManager.cs
    ├── ThreatSpawner.cs
    ├── Threat.cs
    ├── HitPoints.cs
    ├── PlayerController.cs
    ├── RhythmChart.cs
    └── ScoreDisplay.cs
```

## Music

| Track | Use | Credit |
|-------|-----|--------|
| Techno Purgatory | Purgatory gameplay | [LudoLoon Studio](https://ludoloonstudio.itch.io/techno-purgatory) — CC BY 4.0 |
| Infinity Heaven | Heaven section *(planned)* | [HyuN](https://www.youtube.com/c/HyuNPianoOfficial) |

## Roadmap

- [ ] Beat-synced charts for Techno Purgatory
- [ ] Purgatory visuals and enemy designs
- [ ] Heaven scene and clock repair mechanic
- [ ] Story intro (human chosen to restore the rhythm)
- [ ] Polish, juice, and UI

## License

Game code and project files: see repository license.

Third-party music retains its original license. Credit the artists when required.
