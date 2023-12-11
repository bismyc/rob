using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;


public enum Action
{
    ADD_ASSET,
    REMOVE_ASSET,
    REMOVE_ALL,
    SCALE,
    MOVE,
    NONE
}

public class Presenter : MonoBehaviour
{
    public int activeGameAssetId = -1;
    public int selectedGameAssetId = -1;
    private Model model;
    private View view;
    [SerializeField] private ScaleInteraction scaleInteraction;
    [SerializeField] private MoveInteraction moveInteraction;
    private Stack<Action> executedActions = new Stack<Action>();


    // Start is called before the first frame update
    void Start()
    {
        model = GetComponent<Model>();
        view = GetComponent<View>();
        view.eventSystem.RegisterEvent<OnTapPlane>(HandleOnTapPlane);
        view.eventSystem.RegisterEvent<OnSelectGameAsset>(HandleOnSelectGameAsset);
        view.eventSystem.RegisterEvent<OnActivateGameAsset>(HandleOnActivateGameAsset);
        scaleInteraction.eventSystem.RegisterEvent<OnInteractionActionEnd>(HandleOnInteractionActionEnd);
        moveInteraction.eventSystem.RegisterEvent<OnInteractionActionEnd>(HandleOnInteractionActionEnd);
        scaleInteraction.eventSystem.RegisterEvent<OnInteractionActionBegin>(HandleOnInteractionActionBegin);
        moveInteraction.eventSystem.RegisterEvent<OnInteractionActionBegin>(HandleOnInteractionActionBegin);
        view.HideInteractionUI();
    }


    void HandleOnSelectGameAsset(OnSelectGameAsset eventData)
    {
        if(selectedGameAssetId == eventData.id)
        {
            selectedGameAssetId = -1;
        } else
        {
            selectedGameAssetId = eventData.id;
        }
    }

    void HandleOnActivateGameAsset(OnActivateGameAsset eventData)
    {
        activeGameAssetId = eventData.id;
        if(activeGameAssetId == -1)
        {
            view.HideInteractionUI();
        }

        model.SetActiveGameAssetId(activeGameAssetId);
    }

    void HandleOnTapPlane(OnTapPlane eventData)
    {
        if(selectedGameAssetId == -1)
        {
            return;
        }

        GameAsset gameAsset = model.AddNewAsset(selectedGameAssetId);

        view.PlaceAsset(gameAsset.asset, eventData.worldPos, eventData.screenPos);
        activeGameAssetId = model.GetNumberOfAssetsPlaced() - 1;
        model.SetActiveGameAssetId(activeGameAssetId);
        selectedGameAssetId = -1;
        executedActions.Push(Action.ADD_ASSET);
        SaveAllAssets();
    }


    void HandleOnInteractionActionEnd(OnInteractionActionEnd eventData)
    {
        executedActions.Push(eventData.action);
        SaveAllAssets();
        view.SetViewInputBlock(false);
    }

    void HandleOnInteractionActionBegin(OnInteractionActionBegin eventData)
    {
        view.SetViewInputBlock(true);
    }

    public void RemoveAsset()
    {
        model.RemoveAsset(activeGameAssetId);
        view.HideInteractionUI();
        executedActions.Push(Action.REMOVE_ASSET);
        SaveAllAssets();
        activeGameAssetId = -1;
    }

    public void RemoveAllAssets()
    {
        model.RemoveAllPlacedAssets();
        view.HideInteractionUI();
        executedActions.Push(Action.REMOVE_ALL);
        SaveAllAssets();
        activeGameAssetId = -1;
    }

    public void Undo()
    {
        if(executedActions.Count == 0)
        {
            return;
        }

        Action action = executedActions.Pop();

        switch(action)
        {
            case Action.ADD_ASSET:
                model.UndoAdd();
                break;
            case Action.REMOVE_ASSET:
                model.UndoRemove();
                break;
            case Action.REMOVE_ALL:
                model.UndoRemoveAll();
                break;
            case Action.SCALE:
                scaleInteraction.UndoScale();
                break;
            case Action.MOVE:
                moveInteraction.UndoMove();
                break;
        }
        view.HideInteractionUI();

    }


    public void SaveAllAssets()
    {
        model.SaveAllAssets();
    }


}
