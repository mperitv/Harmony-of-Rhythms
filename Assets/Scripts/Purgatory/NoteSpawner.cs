using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private RhythmGameManager gameManager;
    [SerializeField] private FallingNote notePrefab;
    [SerializeField] private float spawnAheadDistance = 10f;
    [SerializeField] private float[] laneY = { 1.5f, 0.5f, -0.5f, -1.5f };
    [SerializeField] private float noteTravelTime = 2.5f;

    private readonly List<ScheduledNote> chart = new List<ScheduledNote>();
    private int nextChartIndex;

    public void SetChart(IEnumerable<ScheduledNote> notes)
    {
        chart.Clear();
        chart.AddRange(notes);
        chart.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
        nextChartIndex = 0;
    }

    public void Tick(float songTime)
    {
        while (nextChartIndex < chart.Count)
        {
            ScheduledNote entry = chart[nextChartIndex];
            float spawnTime = entry.hitTime - noteTravelTime;

            if (songTime < spawnTime)
            {
                break;
            }

            SpawnNote(entry.lane);
            nextChartIndex++;
        }
    }

    private void SpawnNote(LaneType lane)
    {
        float spawnX = gameManager.HitLineX + spawnAheadDistance;
        float moveSpeed = gameManager.GetNoteMoveSpeed(spawnAheadDistance, noteTravelTime);
        Vector3 position = new Vector3(spawnX, laneY[(int)lane], 0f);
        FallingNote note = Instantiate(notePrefab, position, Quaternion.identity);
        note.Initialize(lane, moveSpeed, gameManager);
        gameManager.RegisterActiveNote(note);
    }
}

[System.Serializable]
public struct ScheduledNote
{
    public float hitTime;
    public LaneType lane;

    public ScheduledNote(float hitTime, LaneType lane)
    {
        this.hitTime = hitTime;
        this.lane = lane;
    }
}
