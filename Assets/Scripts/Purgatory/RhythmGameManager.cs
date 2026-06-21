using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RhythmGameManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private Transform player;
    [SerializeField] private float bpm = 140f;
    [SerializeField] private float songStartDelay = 1f;
    [SerializeField] private float hitLineOffset = 2.3f;
    [SerializeField] private float missDistanceBehindHitLine = 1.5f;
    [SerializeField] private float perfectWindow = 1.2f;
    [SerializeField] private float goodWindow = 2f;
    [SerializeField] private float maxRhythm = 100f;
    [SerializeField] private float walkSpeed = 0f;
    [SerializeField] private bool useBuiltInTestChart = true;
    [SerializeField] private int testChartBeats = 80;

    public float HitLineX => player.position.x + hitLineOffset;
    public float MissLineX => HitLineX - missDistanceBehindHitLine;
    public bool IsPlaying { get; private set; }

    public float RhythmBar { get; private set; }
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public string LastFeedback { get; private set; } = string.Empty;

    private readonly List<FallingNote> activeNotes = new List<FallingNote>();
    private float songStartTime;
    private bool songStarted;
    private GUIStyle labelStyle;
    private GUIStyle feedbackStyle;
    private GUIStyle boxStyle;

    private void Start()
    {
        RhythmBar = maxRhythm;
        songStartTime = Time.time + songStartDelay;

        if (useBuiltInTestChart)
        {
            noteSpawner.SetChart(BuildTestChart());
        }

        if (musicSource != null)
        {
            musicSource.loop = true;
        }
    }

    private void Update()
    {
        if (!songStarted && Time.time >= songStartTime)
        {
            songStarted = true;
            IsPlaying = true;
            musicSource.Play();
        }

        if (walkSpeed > 0f)
        {
            player.position += Vector3.right * (walkSpeed * Time.deltaTime);
        }

        ReadInput();

        if (!IsPlaying)
        {
            return;
        }

        noteSpawner.Tick(musicSource.time);
        PurgeNullNotes();

        if (RhythmBar <= 0f)
        {
            IsPlaying = false;
            LastFeedback = "RHYTHM BROKEN";
        }
    }

    private void ReadInput()
    {
        if (!IsPlaying)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.dKey.wasPressedThisFrame)
        {
            TryHitLane(LaneType.D);
        }
        else if (keyboard.fKey.wasPressedThisFrame)
        {
            TryHitLane(LaneType.F);
        }
        else if (keyboard.jKey.wasPressedThisFrame)
        {
            TryHitLane(LaneType.J);
        }
        else if (keyboard.kKey.wasPressedThisFrame)
        {
            TryHitLane(LaneType.K);
        }
    }

    public float GetNoteMoveSpeed(float spawnAheadDistance, float travelTime)
    {
        float speed = spawnAheadDistance / travelTime - walkSpeed;
        return Mathf.Max(0.5f, speed);
    }

    public void RegisterActiveNote(FallingNote note)
    {
        activeNotes.Add(note);
    }

    public void TryHitLane(LaneType lane)
    {
        PurgeNullNotes();

        FallingNote note = GetClosestNote(lane, out float distance);
        if (note == null)
        {
            LastFeedback = "MISS";
            return;
        }

        if (!note.TryHit(perfectWindow, goodWindow, out bool perfect))
        {
            LastFeedback = "MISS";
            return;
        }

        activeNotes.Remove(note);
        Combo++;
        Score += perfect ? 100 : 50;
        RhythmBar = Mathf.Min(maxRhythm, RhythmBar + (perfect ? 6f : 3f));
        LastFeedback = perfect ? "PERFECT" : "GOOD";
    }

    public void RegisterNoteResult(bool wasHit)
    {
        PurgeNullNotes();

        if (wasHit)
        {
            return;
        }

        Combo = 0;
        RhythmBar = Mathf.Max(0f, RhythmBar - 8f);
        LastFeedback = "MISS";
    }

    private FallingNote GetClosestNote(LaneType lane, out float distance)
    {
        FallingNote closest = null;
        distance = float.MaxValue;

        for (int i = 0; i < activeNotes.Count; i++)
        {
            FallingNote note = activeNotes[i];
            if (note == null || !note.IsActive || note.Lane != lane)
            {
                continue;
            }

            float noteDistance = note.DistanceToHitLine();
            if (noteDistance < distance)
            {
                distance = noteDistance;
                closest = note;
            }
        }

        return closest;
    }

    private void PurgeNullNotes()
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i] == null)
            {
                activeNotes.RemoveAt(i);
            }
        }
    }

    private List<ScheduledNote> BuildTestChart()
    {
        float beat = 60f / bpm;
        var notes = new List<ScheduledNote>();

        for (int i = 2; i < testChartBeats; i += 2)
        {
            notes.Add(new ScheduledNote(i * beat, (LaneType)(i % 4)));
        }

        return notes;
    }

    private void EnsureHudStyles()
    {
        if (labelStyle != null)
        {
            return;
        }

        int fontSize = Mathf.Max(22, Mathf.RoundToInt(Screen.height * 0.032f));

        boxStyle = new GUIStyle(GUI.skin.box);
        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        feedbackStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize + 6,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(1f, 0.92f, 0.35f) }
        };
    }

    private void OnGUI()
    {
        EnsureHudStyles();

        float width = Screen.width * 0.45f;
        float line = labelStyle.fontSize + 10f;
        GUI.Box(new Rect(12, 12, width, line * 3f + 24f), GUIContent.none, boxStyle);
        GUI.Label(new Rect(24, 20, width, line), "D Red | F Blue | J Yellow | K Green", labelStyle);
        GUI.Label(new Rect(24, 20 + line, width, line), $"Score {Score}  Combo {Combo}  Rhythm {RhythmBar:0}", labelStyle);
        GUI.Label(new Rect(24, 20 + line * 2f, width, line), LastFeedback, feedbackStyle);

        if (!IsPlaying && songStarted && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
