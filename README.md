# Harmony of Rhythms

A 2D rhythm action game built in Unity. When heaven's rhythm breaks, a human is chosen to restore it—first through the trials of purgatory, then by repairing the great clock in heaven.

## About

**Harmony of Rhythms** blends lane-based rhythm defense with a story-driven journey:

1. **Purgatory**  Walk forward while blocking colored enemy beams in sync with the music.
2. **Heaven**   Repair the celestial clock using beats!



## Controls

| Key | Action |
|-----|--------|
| `D` | Red lane |
| `F` | Blue lane |
| `J` | Yellow lane |
| `K` | Green lane |

Press the matching key when a note reaches the hit line.


## Project Structure

```
Assets/
├── Music/              # Soundtrack files
├── Prefabs/            # Purgatory Note prefab
├── Scenes/             # GameScene
└── Scripts/Purgatory/  # Gameplay scripts
    ├── RhythmGameManager.cs
    ├── NoteSpawner.cs
    ├── FallingNote.cs
    ├── LaneType.cs
    └── CameraFollow.cs
```

## Music

| Track | Use | Credit |
|-------|-----|--------|
| Techno Purgatory | Purgatory gameplay | [LudoLoon Studio](https://ludoloonstudio.itch.io) |
| Infinity Heaven | Heaven section *(planned)* | [HyuN](https://www.youtube.com/c/HyuNPianoOfficial) |
