using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Events;

public class MoveInteraction : MonoBehaviour
{

    public GameObject moveObj;
    [SerializeField] private Model model;

    private Stack<Tuple<GameObject, Vector3>> moveHistory = new Stack<Tuple<GameObject, Vector3>>();
    public EventSystem eventSystem { get; private set; }  // event manager interface instance

    private void Awake()
    {
        eventSystem = new EventSystem();
    }

    public void OnBeginDrag()
    {
        moveObj = model.GetActiveGameObject();
        if (moveObj)
        {
            moveHistory.Push(new Tuple<GameObject, Vector3>(moveObj, moveObj.transform.position));
            eventSystem.SendEvent<OnInteractionActionBegin>(new OnInteractionActionBegin { action = Action.MOVE });
        }
    }

    public void OnDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Plane")
            {
                moveObj.transform.position = new Vector3(hit.point.x, moveObj.transform.position.y, hit.point.z);
                this.transform.parent.transform.position = Input.mousePosition;
            }
        }
    }

    public void OnEndDrag()
    {
        eventSystem.SendEvent<OnInteractionActionEnd>(new OnInteractionActionEnd { action = Action.MOVE });
    }

    public void UndoMove()
    {
        if (moveHistory.Count > 0)
        {
            Tuple<GameObject, Vector3> lastMoveAction = moveHistory.Pop();
            lastMoveAction.Item1.transform.position = lastMoveAction.Item2;
        }
    }
}
