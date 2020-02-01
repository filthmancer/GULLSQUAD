using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Gull
{
    public class Seagull : Unit, IHoverable, ISelectable
    {
        private float moveSpeedInit;
        public float footAnimSpeed = 7F;
        public Transform leftFoot, rightFoot, sittingPosition;
        public Transform leftFootUp, rightFootUp;
        protected Vector3 body_initPosition, leftFoot_initPosition, rightFoot_initPosition;
        public int stomachCurrent, stomachMax = 5;
        public EThreatLevel ThreatLevel_Real;
        public override void InitialiseUnit()
        {
            body.transform.DOMove(sittingPosition.position, animTime);
            body_initPosition = body.transform.localPosition;
            leftFoot_initPosition = leftFoot.transform.localPosition;
            rightFoot_initPosition = rightFoot.transform.localPosition;
            moveSpeedInit = moveSpeed;
            Gull.Posters.Level.Attach<Gull.Events.Level.EatItem>(_event =>
            {
                if (_event.Unit == this) stomachCurrent = Mathf.Clamp(stomachCurrent + 1, 0, stomachMax);
                moveSpeed = Mathf.Clamp(moveSpeedInit * (1.0F - ((float)stomachCurrent / (float)stomachMax)), moveSpeedInit * 0.4F, moveSpeedInit);
            });

        }

        protected override void UpdateUnit()
        {
            ThreatLevel_Real = LevelManager.instance.GetRadialPlayerThreatLevel(transform.position, 60);
            debugText.text = ThreatLevel_Real.ToString();
        }
        public void OnEnterHover()
        {
            if (acting) return;
            body.transform.DOMove(transform.position, animTime);
            UnitManager.instance.SetTargeted(this);
            //Debug.Log("HOVERING GULL");
        }
        public void OnExitHover()
        {
            if (acting) return;
            if (UnitManager.instance.actor != this) body.transform.DOMove(sittingPosition.position, animTime);
            UnitManager.instance.RemoveTargeted(this);
            //Debug.Log("EXITED HOVER GULL");
        }

        public void OnSelect()
        {
            if (acting)
            {
                StopMotion();
                return;
            }
            UnitManager.instance.AddActor(this);
            //Debug.Log("SELECTED GULL");
        }
        public void OnDeselect()
        {
            //Debug.Log("DESELECTED GULL");
        }

        protected override IEnumerator MoveTowardsTargetPoint(Vector3 point)
        {
            acting = true;
            body.transform.DOMove(transform.position, Time.deltaTime);
            float footAnimSpeedReal = footAnimSpeed * moveSpeed;
            while (Vector3.Distance(transform.position, point) > 0.2F)
            {
                Vector3 nextPoint = Vector3.MoveTowards(transform.position, point, moveSpeed);

                if (CheckEnemyThreatLevel(nextPoint) && IsGreaterThreat(transform.position, nextPoint)) yield break;
                if (nextPoint.x > transform.position.x) TurnBodyRight();
                else if (nextPoint.x < transform.position.x) TurnBodyLeft();
                transform.position = nextPoint;

                leftFoot.transform.localPosition = Vector3.Lerp(leftFoot_initPosition, leftFootUp.localPosition, Mathf.PingPong(Time.time * footAnimSpeedReal, 1.0F));
                rightFoot.transform.localPosition = Vector3.Lerp(rightFoot_initPosition, rightFootUp.localPosition, 1.0F - Mathf.PingPong(Time.time * footAnimSpeedReal, 1.0F));
                yield return null;
            }
            transform.position = point;
        }

        IEnumerator SitDown()
        {
            leftFoot.transform.DOLocalMove(leftFoot_initPosition, animTime);
            rightFoot.transform.DOLocalMove(rightFoot_initPosition, animTime);
            body.transform.DOMove(sittingPosition.position, animTime);
            acting = false;
            yield return null;
        }

        bool IsGreaterThreat(Vector3 positionA, Vector3 positionB)
        {
            return (LevelManager.instance.GetRadialEnemyThreatLevel(positionB, 40) > LevelManager.instance.GetRadialEnemyThreatLevel(positionA, 40));
        }

        bool CheckEnemyThreatLevel(Vector3 position)
        {
            return (LevelManager.instance.GetRadialEnemyThreatLevel(position, 40) > ThreatLevel_Real);
        }


        public override void InteractWith<T>(T obj)
        {
            /*  if (obj is Pigeon)
             {
                 Pigeon pigeon = obj as Pigeon;
                 if (pigeon.ThreatLevel >= ThreatLevel)
                 {
                     StopMotion();
                 }
             } */
        }

        public override void ActUpon(Gull.Object _object, Vector3 _point)
        {
            if (_object != null)
            {

            }
            else
            {
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(_point), SitDown()));
            }
        }
    }
}
