using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using System;

public class ScaleInteraction : MonoBehaviour
{
    private float minScale = 1.0f;
    private float maxScale = 2.0f;
    private float dragMax = 30.0f;
    public GameObject scaleObj;
    [SerializeField] private Model model;
    public Transform pivot;
    public Vector3 startPos;

    private Stack<Tuple<GameObject, Vector3>> scaleHistory = new Stack<Tuple<GameObject, Vector3>>();
    public EventSystem eventSystem { get; private set; }  // event manager interface instance

    private void Awake()
    {
        eventSystem = new EventSystem();
    }


    public void OnBeginDrag()
    {
        scaleObj = model.GetActiveGameObject();
        if(scaleObj)
        {
            startPos = transform.position;
            pivot = scaleObj.transform.Find("Pivot");
            scaleHistory.Push(new Tuple<GameObject, Vector3>(scaleObj, scaleObj.transform.localScale));
            eventSystem.SendEvent<OnInteractionActionBegin>(new OnInteractionActionBegin { action = Action.SCALE });
        }
    }


    public void OnDrag()
    {
        if (scaleObj)
        {
            float dist = Input.mousePosition.y - startPos.y;
            Transform pivotParent = pivot.parent;
            Vector3 pivotPos = pivot.position;
            pivot.parent = scaleObj.transform;
            float newScale = Mathf.Clamp(minScale + (dist / dragMax), minScale, maxScale);

            scaleObj.transform.localScale = new Vector3(newScale, newScale, newScale);
            this.transform.parent.transform.localScale = new Vector3(newScale, newScale, newScale);
            scaleObj.transform.position += pivotPos - pivot.position;
            pivot.parent = pivotParent;
        }
    }

    public void OnEndDrag()
    {
        eventSystem.SendEvent<OnInteractionActionEnd>(new OnInteractionActionEnd { action = Action.SCALE });
    }

    public void UndoScale()
    {
        if(scaleHistory.Count > 0)
        {
            Tuple<GameObject, Vector3> lastScaleAction = scaleHistory.Pop();
            lastScaleAction.Item1.transform.localScale = lastScaleAction.Item2;
        }
    }
}