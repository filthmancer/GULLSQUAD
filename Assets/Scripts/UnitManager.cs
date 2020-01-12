using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Gull;
public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;
    void Awake()
    {
        instance = this;
    }

    public GameObject dragIcon;
    public UILineRenderer dragPath;
    public Unit hovered;
    public Unit actor;
    public List<Unit> acting;

    public Vector3 actingPoint;
    public Unit actingTarget;

    // Start is called before the first frame update
    void Start()
    {
        acting = new List<Unit>();
        Gull.Posters.Input.Attach<Gull.Events.Input.OnStartDrag>(StartDrag);
        Gull.Posters.Input.Attach<Gull.Events.Input.OnEndDrag>(EndDrag);
    }

    // Update is called once per frame
    void Update()
    {
        if (actor)
        {
            if (InputManager.instance.isDragging)
            {
                dragIcon.SetActive(true);
                dragPath.gameObject.SetActive(true);
                actingPoint = InputManager.instance.MouseToScreenPosition();
                if (actingTarget != null) actingPoint = actingTarget.transform.position;
                //TODO: Find snapping points for actingPoint here
                dragIcon.transform.position = actingPoint;
                if (Vector3.Distance(actingPoint, actor.transform.position) > 15)
                {
                    Vector3 velocity = (actingPoint - actor.transform.position).normalized;
                    Vector2 lineA = dragPath.transform.parent.InverseTransformPoint(actor.transform.position + velocity * 15);
                    Vector2 lineB = dragPath.transform.parent.InverseTransformPoint(actingPoint - velocity * 5);
                    dragPath.Points = new Vector2[]
                    {
                    lineA, lineB
                    };
                }
                else dragPath.Points = new Vector2[] { Vector2.zero, Vector2.zero };
            }
        }

    }

    public void SetTargeted(Unit t)
    {
        ResetUnits();
        hovered = t;
    }
    public void RemoveTargeted(Unit t)
    {
        if (hovered == t)
        {
            if (hovered != actor) ResetUnits();
            hovered = null;
        }
    }
    public void Act()
    {
        actor.ActUpon(actingTarget, actingPoint);
        actor = null;
    }

    public void ResetUnits()
    {
        if (hovered) hovered.transform.rotation = Quaternion.identity;
    }

    public void AddActor(Unit s)
    {
        if (actor == null) actor = s;
    }

    public IEnumerator StartDrag(Gull.Events.Input.OnStartDrag _event)
    {
        return null;
    }
    public IEnumerator EndDrag(Gull.Events.Input.OnEndDrag _event)
    {
        if (actor) Act();
        dragIcon.SetActive(false);
        dragPath.gameObject.SetActive(false);
        return null;
    }
}

