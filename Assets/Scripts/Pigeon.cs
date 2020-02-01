using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gull
{

    public class Pigeon : Unit
    {
        public class Flees : LevelManager.LevelEvent
        {
        }
        public class Returns : LevelManager.LevelEvent
        {
        }

        private Vector3 initialPosition;
        [SerializeField] private int ThreatLevel_base;
        [SerializeField] private float RadialThreat_radius = 60;
        [SerializeField] private int RadialThreat_perPigeon = 1;
        [SerializeField] private int RadialThreat_LowThreshold = 2;
        [SerializeField] private int RadialThreat_MediumThreshold = 6;
        public int RadialThreatIncrease;
        public bool Fleeing;

        public string swarm;

        public override void InitialiseUnit()
        {
            ThreatLevel_base = ThreatLevel;
            initialPosition = transform.position;

            LevelManager.instance.EventPoster.Attach<Flees>(_ev =>
            {
                CalculatePigeonSwarmThreat();
            });
            LevelManager.instance.EventPoster.Attach<Returns>(_ev =>
            {
                CalculatePigeonSwarmThreat();
            });
            CalculatePigeonSwarmThreat();
        }

        protected override void UpdateUnit()
        {
            CalculateEnemyThreat();
        }

        private void CalculatePigeonSwarmThreat()
        {
            if(Fleeing)
            {
                ThreatLevel_enum = EThreatLevel.None;
                return;
            }
            RadialThreatIncrease = 0;
            int other_pigeon_total = 0;

            List<Gull.Unit> units = LevelManager.instance.level.enemies.FindAll(u => 
            {
                if(u is Pigeon) return(u as Pigeon).swarm == this.swarm;
                return false;
            });

            //LevelManager.instance.GetRadialThreatLevel(transform.position, RadialThreat_radius, false, out units);

            foreach (Gull.Unit unit in units)
            {
                if (unit is Gull.Pigeon)
                {
                    Gull.Pigeon pigeon = unit as Pigeon;
                    if (pigeon == this || pigeon.Fleeing) continue;
                    other_pigeon_total += RadialThreat_perPigeon;
                }
            }

            if(other_pigeon_total < RadialThreat_LowThreshold) ThreatLevel_enum = EThreatLevel.None;
            else if(other_pigeon_total < RadialThreat_MediumThreshold) ThreatLevel_enum = EThreatLevel.Low;
            else ThreatLevel_enum = EThreatLevel.Medium;
        }

        private void CalculateEnemyThreat()
        {
            EThreatLevel playerThreat = LevelManager.instance.GetRadialPlayerThreatLevel(transform.position, RadialThreat_radius);
            Debug.Log(playerThreat);
            if (playerThreat >= ThreatLevel_enum && !Fleeing)
            {
                Fleeing = true;
                ThreatLevel_enum = EThreatLevel.None;
                Vector3 vel = Random.insideUnitCircle.normalized;
                Vector3 randomFlight = transform.position + (vel * 400);
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(randomFlight)));
                StartCoroutine(LevelManager.instance.EventPoster.Post(new Pigeon.Flees()));
            }
            else if ((playerThreat < ThreatLevel_enum || playerThreat == EThreatLevel.None) && Fleeing)
            {
                Fleeing = false;
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(initialPosition)));
                StartCoroutine(LevelManager.instance.EventPoster.Post(new Pigeon.Returns()));
            }
            debugText.text = ThreatLevel_enum.ToString();
        }
        /*   public override void InteractWith<T>(T obj)
          {
              if (obj is Gull.Seagull)
              {
                  CalculateEnemyThreat();
              }
          }

          public override void ExitInteract<T>(T obj)
          {
              if (obj is Gull.Seagull)
              {
                  CalculateEnemyThreat();
              }
          } */

        protected override IEnumerator MoveTowardsTargetPoint(Vector3 point)
        {
            acting = true;
            while (Vector3.Distance(body.transform.position, point) > 0.2F)
            {
                Vector3 nextPoint = Vector3.MoveTowards(body.transform.position, point, moveSpeed);
                if (nextPoint.x > body.transform.position.x) TurnBodyRight();
                else if (nextPoint.x < body.transform.position.x) TurnBodyLeft();
                body.transform.position = nextPoint;
                yield return null;
            }
            body.transform.position = point;
            acting = false;
        }
        public override int GetThreatLevel()
        {
            if (Fleeing) return 0;
            return ThreatLevel;
        }
    }
}
