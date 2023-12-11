using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct GameAsset
{
    public GameAsset(int type, GameObject gameObject)
    {
        asset = gameObject;
        typeId = type;
    }
    public GameObject asset;
    public int typeId;
}

public struct ModifiedData
{
    public ModifiedData(Vector3 p, Vector3 s)
    {
        pos = p;
        scale = s;
    }
    public Vector3 pos;
    public Vector3 scale;
}


public class Model : MonoBehaviour
{
    [SerializeField] private GameObject[] assetTemplates = new GameObject[2];

    [SerializeField] private Transform assetsParent;

    public List<GameAsset> placedAssets = new List<GameAsset>();
    //public Stack<Tuple<int, ModifiedData>> removedAssets = new Stack<Tuple<int, ModifiedData>>(); //if removed assets were not inactive, I would have used this stack.
    public Stack<GameAsset> removedAssets = new Stack<GameAsset>();

    private int activeGameAssetId = -1;

    private Storage storage;

    private void Start()
    {
        storage = new Storage();
        AddSavedAssets();
    }

    void AddSavedAssets()
    {
        List<Tuple<int, Vector3, Vector3>> savedAssets = storage.LoadSavedAssets();

        for (int i = 0; i < savedAssets.Count; i++)
        {
            GameAsset gameAsset = AddNewAsset(savedAssets[i].Item1);
            gameAsset.asset.transform.position = new Vector3(savedAssets[i].Item2.x, gameAsset.asset.transform.position.y, savedAssets[i].Item2.z);
            gameAsset.asset.transform.localScale = new Vector3(savedAssets[i].Item3.x, savedAssets[i].Item3.y, savedAssets[i].Item3.z);
        }
    }

    public void SaveAllAssets()
    {
        storage.ResetAllAssetData();
        for(int i = 0; i < placedAssets.Count; i++)
        {
            if(placedAssets[i].asset.activeInHierarchy)
            {
                storage.SaveAsset(new Tuple<int, Vector3, Vector3>(
                    placedAssets[i].typeId,
                    placedAssets[i].asset.transform.position,
                    placedAssets[i].asset.transform.localScale)
                    );
            }
        }
    }

    public GameAsset AddNewAsset(int typeId)
    {
        int placedId = placedAssets.Count;
        placedAssets.Add(new GameAsset(typeId, Instantiate(assetTemplates[typeId])));
        placedAssets[placedId].asset.name = placedId.ToString();
        placedAssets[placedId].asset.transform.SetParent(assetsParent);
        return placedAssets[placedId];
    }

    public GameAsset AddRemovedAsset(int typeId, ModifiedData data)
    {
        int placedId = placedAssets.Count;
        placedAssets.Add(new GameAsset(typeId, Instantiate(assetTemplates[typeId])));
        placedAssets[placedId].asset.name = placedId.ToString();
        placedAssets[placedId].asset.transform.position = data.pos;
        placedAssets[placedId].asset.transform.localScale = data.scale;
        placedAssets[placedId].asset.transform.SetParent(assetsParent);
        return placedAssets[placedId];
    }

    public void RemoveAsset(int index)
    {
        if(placedAssets[index].asset.activeInHierarchy)
        {
            removedAssets.Push(placedAssets[index]);
            placedAssets[index].asset.SetActive(false);
        }

    }

    public void RemoveAllPlacedAssets()
    {
        for (int i = 0; i < placedAssets.Count; i++)
        {
            RemoveAsset(i);
        }
    }

    private void RemoveLastAdded()
    {
        for (int i = placedAssets.Count - 1; i > 0; i--)
        {
            if(placedAssets[i].asset.activeInHierarchy)
            {
                RemoveAsset(i);
                break;
            }

        }
    }

    public void UndoAdd()
    {
        if (placedAssets.Count > 0)
        {
            RemoveLastAdded();
            SaveAllAssets();
        }
    }

    public void UndoRemove()
    {
        if(removedAssets.Count > 0)
        {
            removedAssets.Peek().asset.SetActive(true);
            removedAssets.Pop();
            SaveAllAssets();
        }
    }

    public void UndoRemoveAll()
    {
        while (removedAssets.Count > 0)
        {
            UndoRemove();
        }
    }


    public GameObject GetActiveGameObject()
    {
        if(activeGameAssetId != -1)
        {
            return placedAssets[activeGameAssetId].asset;
        }

        return null;
    }

    public void SetActiveGameAssetId(int index)
    {
        activeGameAssetId = index;
    }

    public int GetNumberOfAssetsPlaced()
    {
        return placedAssets.Count;
    }

    public static void Swap(List<GameAsset> list, int indexA, int indexB)
    {
        GameAsset tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexA].asset.name = indexA.ToString();
        list[indexB] = tmp;
        list[indexB].asset.name = indexB.ToString();
    }

}
