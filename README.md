# Harmony of Rhythms

A 2D rhythm action game built in Unity. When heaven's rhythm breaks, a human is chosen to restore it—first through the trials of purgatory, then by repairing the great clock in heaven.

## About

**Harmony of Rhythms** blends rhythm defense with a story-driven journey:

1. **Purgatory** — Walk forward while blocking colored enemy beams in sync with the music.
2. **Heaven**  — Repair the celestial clock using beat-based mechanics.



## Controls

| Key | Action |
|-----|--------|
| `D` | Left melee |
| `K` | Right melee |
| `F` | Upper-left |
| `J` | Upper-right |
| `Space` | Ground front (jump) |

Press the matching key when a threat reaches its hit marker.


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


