using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gull;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    void Awake()
    {
        instance = this;
        EventPoster = new Tin.Events.EventRoutine<LevelEvent>();
    }
    public List<Unit> units;

    public Level level;
    public List<Chip> chips { get { return level.chips; } }
    public List<SpawnPoint> spawnPoints { get { return level.spawnPoints; } }

    public Unit seagull;
    public class LevelEvent { }
    public Tin.Events.EventRoutine<LevelEvent> EventPoster;

    void Start()
    {
        units = new List<Unit>();
        foreach (SpawnPoint p in spawnPoints)
        {
            Unit u = Instantiate(seagull, this.transform, true);
            if (units.Count > 0) u.ThreatLevel = 2;
            u.transform.localScale = Vector3.one;
            u.transform.position = p.transform.position;
            units.Add(u);
        }


    }
    public void EatChip(Chip c, Unit u = null)
    {
        if (u is Seagull && (u as Seagull).stomachCurrent >= (u as Seagull).stomachMax) return;
        if (chips.Contains(c)) chips.Remove(c);
        Destroy(c.gameObject);
        Debug.Log("Ate chippie!");
        StartCoroutine(Gull.Posters.Level.Post(new Gull.Events.Level.EatItem() { Item = c, Unit = u }));
    }

    public int GetRadialPlayerThreatLevel(Vector3 position, float radius)
    {
        List<Gull.Unit> units;
        return GetRadialThreatLevel(position, radius, true, out units);
    }

    public int GetRadialEnemyThreatLevel(Vector3 position, float radius)
    {
        List<Gull.Unit> units;
        return GetRadialThreatLevel(position, radius, false, out units);
    }

    public int GetRadialThreatLevel(Vector3 position, float radius, bool player, out List<Gull.Unit> units)
    {
        units = new List<Gull.Unit>();
        int finalThreat = 0;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position, radius, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Gull.Unit>() != null)
            {
                Gull.Unit unit = hit.transform.GetComponent<Gull.Unit>();
                if (unit.playerOwned != player) continue;
                finalThreat += unit.GetThreatLevel();
                units.Add(unit);
            }
        }
        return finalThreat;
    }
}

namespace Gull
{
    public static partial class Events
    {
        public static class Level
        {
            public class EatItem : EventBase
            {
                public Chip Item;
                public Unit Unit;
            }
        }
    }
}

