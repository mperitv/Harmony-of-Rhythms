using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewThreatChart", menuName = "Harmony/Threat Chart")]
public class RhythmChart : ScriptableObject
{
    public ChartEntry[] threats;

    public List<ScheduledThreat> Build()
    {
        var list = new List<ScheduledThreat>();

        if (threats == null)
        {
            return list;
        }

        for (int i = 0; i < threats.Length; i++)
        {
            ChartEntry entry = threats[i];
            if (entry.hitTime < 0f)
            {
                continue;
            }

            list.Add(new ScheduledThreat(entry.hitTime, entry.direction));
        }

        list.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
        return list;
    }
}

[System.Serializable]
public struct ChartEntry
{
    public float hitTime;
    public ThreatDirection direction;
}
