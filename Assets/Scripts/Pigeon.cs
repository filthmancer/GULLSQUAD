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
        private int ThreatLevel_base;
        private float RadialThreat_radius = 60;
        private int RadialThreat_perPigeon = 1;
        public int RadialThreatIncrease;
        public bool Fleeing;

        protected override void InitialiseUnit()
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
            RadialThreatIncrease = 0;
            ThreatLevel = ThreatLevel_base;

            List<Gull.Unit> units;
            LevelManager.instance.GetRadialThreatLevel(transform.position, RadialThreat_radius, false, out units);

            foreach (Gull.Unit unit in units)
            {
                if (unit is Gull.Pigeon)
                {
                    Gull.Pigeon pigeon = unit as Pigeon;
                    if (pigeon == this || pigeon.Fleeing) continue;
                    RadialThreatIncrease += RadialThreat_perPigeon;
                }
            }
            ThreatLevel += RadialThreatIncrease;
            debugText.text = ThreatLevel.ToString();
        }

        private void CalculateEnemyThreat()
        {
            int playerThreat = LevelManager.instance.GetRadialPlayerThreatLevel(transform.position, RadialThreat_radius);
            debugText.text = ThreatLevel + "/" + playerThreat;
            if (playerThreat > ThreatLevel && !Fleeing)
            {
                Fleeing = true;
                Vector3 vel = Random.insideUnitCircle.normalized;
                Vector3 randomFlight = transform.position + (vel * 400);
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(randomFlight)));
                StartCoroutine(LevelManager.instance.EventPoster.Post(new Pigeon.Flees()));
            }
            else if (playerThreat <= ThreatLevel && Fleeing)
            {
                Fleeing = false;
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(initialPosition)));
                StartCoroutine(LevelManager.instance.EventPoster.Post(new Pigeon.Returns()));
            }
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
