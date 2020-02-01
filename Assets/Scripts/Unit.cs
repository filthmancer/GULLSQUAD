using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using TMPro;

namespace Gull
{
    public enum EThreatLevel
    {
        None = 0, Low = 1, Medium = 2, High = 3, Impossible = 4
    }
    public class Unit : Gull.Object
    {
        protected BoxCollider2D collider;
        protected float animTime = 0.25F;
        public float moveSpeed = 0.7F;
        public int ThreatLevel
        {
            get { return ThreatLevel_cached; }
            set
            {
                ThreatLevel_cached = value;
                ThreatLevel_enum = CalculateThreatLevel(value);
            }
        }
        [SerializeField] private int ThreatLevel_cached;
        public EThreatLevel ThreatLevel_enum;
        public static EThreatLevel CalculateThreatLevel(int value)
        {
            if(value == 0) return EThreatLevel.None;
            if(value <= 3) return EThreatLevel.Low;
            if(value <= 7) return EThreatLevel.Medium;
            if(value <= 12) return EThreatLevel.High;
            return EThreatLevel.Impossible;
        }
        public Transform body;
        public TextMeshProUGUI debugText;
        public bool playerOwned;
        protected bool acting;

        void Awake()
        {
            //ThreatLevel_cached = ThreatLevel_enum;
        }
        // Use this for initialization
        void Start()
        {
            collider = this.gameObject.GetComponent<BoxCollider2D>();

            
        }
        public virtual void InitialiseUnit()
        {

        }
        // Update is called once per frame
        void Update()
        {
            UpdateUnit();
        }

        protected virtual void UpdateUnit()
        {

        }

        public virtual void ActUpon(Gull.Object _object, Vector3 _point)
        {
            if (_object != null)
            {

            }
            else
            {
                StartMotionCoroutine(new MotionGroup(MoveTowardsTargetPoint(_point)));
            }
        }

        protected class MotionGroup
        {
            public IEnumerator MotionRoutine, EndRoutine;
            public MotionGroup(IEnumerator m, IEnumerator e = null)
            {
                MotionRoutine = m;
                EndRoutine = e;
            }
        }
        protected Coroutine motionCoroutine;
        protected MotionGroup motionCurrent;
        protected void StartMotionCoroutine(MotionGroup motion)
        {
            StopMotion();
            StartCoroutine(SetupMotion(motion));
        }

        IEnumerator SetupMotion(MotionGroup motion)
        {
            while (motionCoroutine != null) yield return null;
            motionCurrent = motion;
            motionCoroutine = StartCoroutine(YieldForMotion(motion));
        }
        IEnumerator YieldForMotion(MotionGroup motion)
        {
            if (motion.MotionRoutine != null) yield return motion.MotionRoutine;
            if (motion.EndRoutine != null) yield return motion.EndRoutine;
            motionCurrent = null;
            motionCoroutine = null;
        }

        protected void StopMotion(bool finish = true)
        {
            if (motionCoroutine != null)
            {
                StopCoroutine(motionCoroutine);
                motionCoroutine = null;
            }
            if (motionCurrent != null && motionCurrent.EndRoutine != null && finish)
                motionCoroutine = StartCoroutine(motionCurrent.EndRoutine);
        }

        protected virtual IEnumerator MoveTowardsTargetPoint(Vector3 point)
        {
            acting = true;
            float footAnimSpeedReal = moveSpeed;
            while (Vector3.Distance(transform.position, point) > 0.2F)
            {
                Vector3 nextPoint = Vector3.MoveTowards(transform.position, point, moveSpeed);
                if (nextPoint.x > transform.position.x) TurnBodyRight();
                else if (nextPoint.x < transform.position.x) TurnBodyLeft();
                transform.position = nextPoint;
                yield return null;
            }
            transform.position = point;
            acting = false;
        }

        protected void TurnBodyLeft()
        {
            body.transform.localScale = new Vector3(-1, 1, 1);
        }

        protected void TurnBodyRight()
        {
            body.transform.localScale = new Vector3(1, 1, 1);
        }

        public virtual int GetThreatLevel() { return (int)ThreatLevel; }


    }
}
