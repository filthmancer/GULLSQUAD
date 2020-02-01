using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IHoverable
{
    void OnEnterHover();
    void OnExitHover();
}

public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
}

namespace Gull
{
    public static partial class Events
    {
        public static class Input
        {
            public class OnStartDrag : EventBase { }
            public class OnEndDrag : EventBase { }
        }
    }
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    void Awake() { instance = this; Gull.Events.Initialise(); }
    public IHoverable hovered;
    public ISelectable selected;

    private GraphicRaycaster raycaster;

    private List<IHoverable> currentHovered = new List<IHoverable>();
    private List<ISelectable> currentSelected = new List<ISelectable>();

    public bool isDragging { get; private set; }
    // Use this for initialization
    void Start()
    {
        raycaster = this.GetComponent<GraphicRaycaster>();
    }

    // Update is called once per frame
    void Update()
    {
        GetHovering();
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Gull.Posters.Input.Post(new Gull.Events.Input.OnStartDrag()));
            StartSelection();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(Gull.Posters.Input.Post(new Gull.Events.Input.OnEndDrag()));
            EndSelection();
        }
    }

    private void GetHovering()
    {
        PointerEventData ped = new PointerEventData(null);
        //Set required parameters, in this case, mouse position
        ped.position = Input.mousePosition;
        //Create list to receive all results
        List<RaycastResult> results = new List<RaycastResult>();
        //Raycast it
        raycaster.Raycast(ped, results);
        List<IHoverable> results_hoverable = new List<IHoverable>();
        foreach (RaycastResult res in results)
        {
            if (res.gameObject.GetComponent<IHoverable>() != null)
                results_hoverable.Add(res.gameObject.GetComponent<IHoverable>());
        }

        //Test for newly entered/exited hovering elements
        List<IHoverable> entering = results_hoverable.FindAll(r => !currentHovered.Contains(r));
        foreach (IHoverable r in entering) r.OnEnterHover();
        List<IHoverable> exiting = currentHovered.FindAll(r => !results_hoverable.Contains(r));
        foreach (IHoverable r in exiting) r.OnExitHover();
        currentHovered = results_hoverable;

        if (isDragging && entering.Count > 0) StartSelection();
    }

    private void StartSelection()
    {
        isDragging = true;
        List<ISelectable> results_selectable = new List<ISelectable>();
        foreach (IHoverable r in currentHovered)
        {
            if (r is ISelectable) results_selectable.Add(r as ISelectable);
        }
        //Test for newly entered/exited hovering elements
        List<ISelectable> entering = results_selectable.FindAll(r => !currentSelected.Contains(r));
        foreach (ISelectable r in entering) r.OnSelect();
        currentSelected = results_selectable;
    }

    private void EndSelection()
    {
        isDragging = false;
        foreach (ISelectable r in currentSelected) r.OnDeselect();
        currentSelected = new List<ISelectable>();
    }

    public Vector3 MouseToScreenPosition()
    {
        Vector3 screenPoint = Input.mousePosition;

        screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width);
        screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);
        return screenPoint;
    }
}
