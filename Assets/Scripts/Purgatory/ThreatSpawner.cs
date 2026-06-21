using System.Collections.Generic;
using UnityEngine;

public class ThreatSpawner : MonoBehaviour
{
    [SerializeField] private RhythmGameManager gameManager;
    [SerializeField] private HitPoints hitPoints;
    [SerializeField] private Threat threatPrefab;
    [SerializeField] private float horizontalSpawnDistance = 8f;
    [SerializeField] private float verticalSpawnHeight = 4.5f;
    [SerializeField] private float groundSpawnHeight = 5f;
    [SerializeField] private float threatTravelTime = 1.7f;

    private readonly List<ScheduledThreat> chart = new List<ScheduledThreat>();
    private int nextChartIndex;

    public float ThreatTravelTime => threatTravelTime;

    public void SetChart(IEnumerable<ScheduledThreat> threats)
    {
        chart.Clear();
        chart.AddRange(threats);
        chart.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
        nextChartIndex = 0;
    }

    public void Tick(float songTime)
    {
        while (nextChartIndex < chart.Count)
        {
            ScheduledThreat entry = chart[nextChartIndex];
            float spawnTime = entry.hitTime - threatTravelTime;

            if (songTime < spawnTime)
            {
                break;
            }

            SpawnThreat(entry.direction, entry.hitTime);
            nextChartIndex++;
        }
    }

    private void SpawnThreat(ThreatDirection direction, float idealHitTime)
    {
        Vector3 targetPosition = hitPoints.GetPosition(direction);
        Vector3 spawnPosition;

        switch (direction)
        {
            case ThreatDirection.LeftMelee:
                spawnPosition = targetPosition + Vector3.left * horizontalSpawnDistance;
                break;
            case ThreatDirection.RightMelee:
                spawnPosition = targetPosition + Vector3.right * horizontalSpawnDistance;
                break;
            case ThreatDirection.UpperLeft:
                spawnPosition = targetPosition + new Vector3(-horizontalSpawnDistance * 0.65f, verticalSpawnHeight, 0f);
                break;
            case ThreatDirection.UpperRight:
                spawnPosition = targetPosition + new Vector3(horizontalSpawnDistance * 0.65f, verticalSpawnHeight, 0f);
                break;
            case ThreatDirection.GroundFront:
                spawnPosition = targetPosition + Vector3.up * groundSpawnHeight;
                break;
            default:
                return;
        }

        float travelDistance = Vector3.Distance(spawnPosition, targetPosition);
        float moveSpeed = travelDistance / threatTravelTime;
        Vector3 velocity = (targetPosition - spawnPosition).normalized * moveSpeed;

        Threat threat = Instantiate(threatPrefab, spawnPosition, Quaternion.identity);
        threat.Initialize(direction, idealHitTime, velocity, targetPosition, gameManager);
        gameManager.RegisterActiveThreat(threat);
    }
}

[System.Serializable]
public struct ScheduledThreat
{
    public float hitTime;
    public ThreatDirection direction;

    public ScheduledThreat(float hitTime, ThreatDirection direction)
    {
        this.hitTime = hitTime;
        this.direction = direction;
    }
}
