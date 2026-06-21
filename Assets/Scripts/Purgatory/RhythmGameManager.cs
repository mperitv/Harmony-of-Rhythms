using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RhythmGameManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private ThreatSpawner threatSpawner;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform worldScroll;
    [SerializeField] private RhythmChart rhythmChart;
    [SerializeField] private HitPoints hitPoints;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private ScoreDisplay scoreDisplay;
    [SerializeField] private float songStartDelay = 1f;
    [SerializeField] private float passThreshold = 0.4f;
    [SerializeField] private float maxRhythm = 100f;
    [SerializeField] private float scrollSpeed = 0.8f;
    [SerializeField] private float perfectMs = 25f;
    [SerializeField] private float earlyPerfectMs = 50f;
    [SerializeField] private float latePerfectMs = 50f;
    [SerializeField] private float earlyMs = 100f;
    [SerializeField] private float lateMs = 100f;
    [SerializeField] private float missMs = 100f;
    [SerializeField] private float missCooldown = 0.4f;
    [SerializeField] private float feedbackDuration = 0.9f;
    [SerializeField] private float feedbackRiseSpeed = 0.35f;
    [SerializeField] private Vector3 groundFeedbackOffset = new Vector3(0f, -0.55f, 0f);

    public float PassThreshold => passThreshold;
    public bool IsPlaying { get; private set; }

    public float RhythmBar { get; private set; }
    public int Score { get; private set; }
    public int Combo { get; private set; }

    private readonly List<Threat> activeThreats = new List<Threat>();
    private readonly List<HitFeedbackEntry> hitFeedbacks = new List<HitFeedbackEntry>();
    private float songStartTime;
    private float lastMissTime = -999f;
    private bool songStarted;
    private int cachedScreenHeight;
    private GUIStyle feedbackStyle;

    private struct HitFeedbackEntry
    {
        public Vector3 worldPosition;
        public string text;
        public float spawnTime;
        public HitFeedbackTone tone;
    }

    private void Start()
    {
        RhythmBar = maxRhythm;
        songStartTime = Time.time + songStartDelay;
        threatSpawner.SetChart(rhythmChart.Build());
        RefreshScore();
    }

    private void Update()
    {
        if (!songStarted && Time.time >= songStartTime)
        {
            songStarted = true;
            IsPlaying = true;
            musicSource.Play();
        }

        if (scrollSpeed > 0f && worldScroll != null)
        {
            worldScroll.position += Vector3.left * (scrollSpeed * Time.deltaTime);
        }

        ReadInput();
        UpdateHitFeedbacks();

        if (!IsPlaying)
        {
            return;
        }

        threatSpawner.Tick(musicSource.time);
        PurgeNullThreats();

        if (RhythmBar <= 0f)
        {
            IsPlaying = false;
        }

        if (!musicSource.isPlaying && musicSource.time > 0f)
        {
            IsPlaying = false;
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

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            TryInput(ThreatDirection.GroundFront);
        }
        else if (keyboard.dKey.wasPressedThisFrame)
        {
            TryInput(ThreatDirection.LeftMelee);
        }
        else if (keyboard.kKey.wasPressedThisFrame)
        {
            TryInput(ThreatDirection.RightMelee);
        }
        else if (keyboard.fKey.wasPressedThisFrame)
        {
            TryInput(ThreatDirection.UpperLeft);
        }
        else if (keyboard.jKey.wasPressedThisFrame)
        {
            TryInput(ThreatDirection.UpperRight);
        }
    }

    public void RegisterActiveThreat(Threat threat)
    {
        activeThreats.Add(threat);
    }

    public bool TryGradeTiming(float errorMs, out HitGrade grade)
    {
        grade = HitGrade.Perfect;

        if (Mathf.Abs(errorMs) > missMs)
        {
            return false;
        }

        if (errorMs >= -perfectMs && errorMs <= perfectMs)
        {
            grade = HitGrade.Perfect;
            return true;
        }

        if (errorMs >= -earlyPerfectMs && errorMs < -perfectMs)
        {
            grade = HitGrade.EarlyPerfect;
            return true;
        }

        if (errorMs > perfectMs && errorMs <= latePerfectMs)
        {
            grade = HitGrade.LatePerfect;
            return true;
        }

        if (errorMs >= -earlyMs && errorMs < -earlyPerfectMs)
        {
            grade = HitGrade.Early;
            return true;
        }

        if (errorMs > latePerfectMs && errorMs <= lateMs)
        {
            grade = HitGrade.Late;
            return true;
        }

        return false;
    }

    public void RemoveThreat(Threat threat)
    {
        activeThreats.Remove(threat);
    }

    public void ShowHitFeedback(Vector3 worldPosition, string text, HitFeedbackTone tone)
    {
        hitFeedbacks.Add(new HitFeedbackEntry
        {
            worldPosition = worldPosition,
            text = text,
            spawnTime = Time.time,
            tone = tone
        });
    }

    public void RegisterPassMiss(Vector3 worldPosition)
    {
        PurgeNullThreats();
        TryApplyMiss(worldPosition);
    }

    private void TryInput(ThreatDirection direction)
    {
        PurgeNullThreats();

        if (direction == ThreatDirection.GroundFront)
        {
            if (playerController == null)
            {
                return;
            }

            if (playerController.IsJumping)
            {
                return;
            }

            playerController.StartJump();
        }

        float songTime = musicSource.time;
        Threat threat = GetBestTimedThreat(direction, songTime, out float errorMs);

        if (threat == null || !TryGradeTiming(errorMs, out HitGrade grade))
        {
            ApplyInputMiss(direction);
            return;
        }

        if (direction == ThreatDirection.GroundFront)
        {
            threat.MarkHitKeepMoving();
            ApplyGrade(grade, GetGroundFeedbackPosition());
            return;
        }

        Vector3 feedbackPosition = threat.transform.position;
        ApplyGrade(grade, feedbackPosition);
        threat.ResolveFromInput(true);
    }

    private void ApplyGrade(HitGrade grade, Vector3 feedbackPosition)
    {
        Combo++;
        HitFeedbackTone tone = grade == HitGrade.Perfect ? HitFeedbackTone.Perfect : HitFeedbackTone.Timing;
        ShowHitFeedback(feedbackPosition, GradeLabel(grade), tone);

        switch (grade)
        {
            case HitGrade.Perfect:
                Score += 100;
                RhythmBar = Mathf.Min(maxRhythm, RhythmBar + 6f);
                break;
            case HitGrade.EarlyPerfect:
            case HitGrade.LatePerfect:
                Score += 50;
                RhythmBar = Mathf.Min(maxRhythm, RhythmBar + 5f);
                break;
            case HitGrade.Early:
            case HitGrade.Late:
                Score -= 50;
                RhythmBar = Mathf.Max(0f, RhythmBar - 3f);
                break;
        }

        RefreshScore();
    }

    private void ApplyInputMiss(ThreatDirection direction)
    {
        if (direction == ThreatDirection.GroundFront)
        {
            TryApplyMiss(GetGroundFeedbackPosition());
            return;
        }

        if (hitPoints == null)
        {
            return;
        }

        TryApplyMiss(hitPoints.GetPosition(direction));
    }

    private void TryApplyMiss(Vector3 worldPosition)
    {
        if (Time.time - lastMissTime < missCooldown)
        {
            return;
        }

        lastMissTime = Time.time;
        Combo = 0;
        Score -= 100;
        RhythmBar = Mathf.Max(0f, RhythmBar - 8f);
        RefreshScore();
        ShowHitFeedback(worldPosition, "MISS", HitFeedbackTone.Miss);
    }

    private void RefreshScore()
    {
        if (scoreDisplay == null)
        {
            return;
        }

        scoreDisplay.Refresh(Score);
    }

    private static string GradeLabel(HitGrade grade)
    {
        switch (grade)
        {
            case HitGrade.Early:
                return "EARLY";
            case HitGrade.EarlyPerfect:
                return "EARLY PERFECT";
            case HitGrade.Perfect:
                return "PERFECT";
            case HitGrade.LatePerfect:
                return "LATE PERFECT";
            case HitGrade.Late:
                return "LATE";
            default:
                return "MISS";
        }
    }

    private Vector3 GetGroundFeedbackPosition()
    {
        return player.position + groundFeedbackOffset;
    }

    private Threat GetBestTimedThreat(ThreatDirection direction, float songTime, out float errorMs)
    {
        Threat best = null;
        errorMs = 0f;
        float bestAbsError = float.MaxValue;

        for (int i = 0; i < activeThreats.Count; i++)
        {
            Threat threat = activeThreats[i];
            if (threat == null || !threat.IsActive || !threat.CanReceiveInput || threat.Direction != direction)
            {
                continue;
            }

            float threatErrorMs = threat.GetTimingErrorMs(songTime);
            float absError = Mathf.Abs(threatErrorMs);

            if (absError > missMs || absError >= bestAbsError)
            {
                continue;
            }

            bestAbsError = absError;
            best = threat;
            errorMs = threatErrorMs;
        }

        return best;
    }

    private void UpdateHitFeedbacks()
    {
        float now = Time.time;

        for (int i = hitFeedbacks.Count - 1; i >= 0; i--)
        {
            if (now - hitFeedbacks[i].spawnTime >= feedbackDuration)
            {
                hitFeedbacks.RemoveAt(i);
            }
        }
    }

    private void PurgeNullThreats()
    {
        for (int i = activeThreats.Count - 1; i >= 0; i--)
        {
            if (activeThreats[i] == null)
            {
                activeThreats.RemoveAt(i);
            }
        }
    }

    private void EnsureFeedbackStyle()
    {
        if (feedbackStyle != null && cachedScreenHeight == Screen.height)
        {
            return;
        }

        cachedScreenHeight = Screen.height;
        Font hudFont = GUI.skin.label.font;
        int feedbackFontSize = Mathf.Max(20, Mathf.RoundToInt(Screen.height * 0.028f));

        feedbackStyle = new GUIStyle(GUI.skin.label)
        {
            font = hudFont,
            fontSize = feedbackFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
    }

    private void OnGUI()
    {
        EnsureFeedbackStyle();
        DrawHitFeedbacks();

        if (!IsPlaying && songStarted && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void DrawHitFeedbacks()
    {
        if (gameplayCamera == null)
        {
            return;
        }

        float now = Time.time;

        for (int i = 0; i < hitFeedbacks.Count; i++)
        {
            HitFeedbackEntry entry = hitFeedbacks[i];
            float age = now - entry.spawnTime;
            float alpha = 1f - Mathf.Clamp01(age / feedbackDuration);
            Vector3 worldPosition = entry.worldPosition + Vector3.up * (age * feedbackRiseSpeed);
            Vector3 screenPosition = gameplayCamera.WorldToScreenPoint(worldPosition);

            if (screenPosition.z < 0f)
            {
                continue;
            }

            GUIContent content = new GUIContent(entry.text);
            Vector2 size = feedbackStyle.CalcSize(content);
            float x = screenPosition.x - size.x * 0.5f;
            float y = Screen.height - screenPosition.y - size.y * 0.5f;

            Color previousColor = GUI.color;
            Color textColor = GetFeedbackColor(entry.tone);
            GUI.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            GUI.Label(new Rect(x, y, size.x, size.y), content, feedbackStyle);
            GUI.color = previousColor;
        }
    }

    private static Color GetFeedbackColor(HitFeedbackTone tone)
    {
        switch (tone)
        {
            case HitFeedbackTone.Perfect:
                return new Color(0.35f, 0.95f, 0.45f);
            case HitFeedbackTone.Timing:
                return new Color(1f, 0.92f, 0.35f);
            case HitFeedbackTone.Miss:
                return new Color(1f, 0.35f, 0.35f);
            default:
                return Color.white;
        }
    }
}
