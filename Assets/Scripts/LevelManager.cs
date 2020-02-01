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
        level.Rebuild();
        units = new List<Unit>();
        foreach (SpawnPoint p in spawnPoints)
        {
            Unit u = Instantiate(seagull, this.transform, true);
            u.transform.localScale = Vector3.one;
            u.transform.position = p.transform.position;
            units.Add(u);
        }

        foreach(Gull.Unit unit in level.enemies) unit.InitialiseUnit();

    }
    public void EatChip(Chip c, Unit u = null)
    {
        if (u is Seagull && (u as Seagull).stomachCurrent >= (u as Seagull).stomachMax) return;
        if (chips.Contains(c)) chips.Remove(c);
        Destroy(c.gameObject);
        Debug.Log("Ate chippie!");
        StartCoroutine(Gull.Posters.Level.Post(new Gull.Events.Level.EatItem() { Item = c, Unit = u }));
    }

    public Gull.EThreatLevel GetRadialPlayerThreatLevel(Vector3 position, float radius)
    {
        List<Gull.Unit> units;
        return GetRadialThreatLevel(position, radius, true, out units);
    }

    public Gull.EThreatLevel GetRadialEnemyThreatLevel(Vector3 position, float radius)
    {
        List<Gull.Unit> units;
        return GetRadialThreatLevel(position, radius, false, out units);
    }

    public Gull.EThreatLevel GetRadialThreatLevel(Vector3 position, float radius, bool player, out List<Gull.Unit> units)
    {
        units = new List<Gull.Unit>();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position, radius, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Gull.Unit>() != null)
            {
                Gull.Unit unit = hit.transform.GetComponent<Gull.Unit>();
                if (unit.playerOwned != player) continue;
                units.Add(unit);
            }
        }

        //# Check for impossible units in radius
        if(units.Find( u => u.ThreatLevel_enum == EThreatLevel.Impossible)) return EThreatLevel.Impossible;

        //# Calculate outcome threat level
        //# - 1 or more Low units = Low
        //# - 3 or more Low units OR 1 or more Med unit = Med
        //# - 2 or more Med units OR 1 or more High unit = High

        EThreatLevel final = EThreatLevel.None;

        List<Gull.Unit> low_units = units.FindAll(u => u.ThreatLevel_enum == EThreatLevel.Low);
        if(low_units.Count >= 1) final = EThreatLevel.Low;

        List<Gull.Unit> med_units = units.FindAll(u => u.ThreatLevel_enum == EThreatLevel.Medium);
        if(low_units.Count >= 3 || med_units.Count >= 1) final = EThreatLevel.Medium;

        List<Gull.Unit> high_units = units.FindAll(u => u.ThreatLevel_enum == EThreatLevel.High);
        if(med_units.Count >= 2 || high_units.Count >= 1) final = EThreatLevel.High;
        return final;
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

