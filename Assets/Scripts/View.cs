using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class View : MonoBehaviour
{

    public GameObject objectInteractionUI;

    public EventSystem eventSystem { get; private set; }  // event manager interface instance

    private Vector3 touchStart;

    private bool mViewInputBlock = false;

    private void Awake()
    {
        eventSystem = new EventSystem();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                eventSystem.SendEvent<OnActivateGameAsset>(new OnActivateGameAsset { id = -1 });
                int activeAssetId = -1;
                if (hit.transform.tag == "Asset")
                {
                    int.TryParse(hit.collider.gameObject.name, out activeAssetId);

                    if(activeAssetId != -1)
                    {
                        eventSystem.SendEvent<OnActivateGameAsset>(new OnActivateGameAsset { id = activeAssetId });
                        ShowInteractionUI(Input.mousePosition, hit.collider.gameObject.transform.localScale);
                    }


                } else if (hit.transform.tag == "Plane")
                {
                    eventSystem.SendEvent<OnTapPlane>(new OnTapPlane { worldPos = hit.point, screenPos = Input.mousePosition });;
                    
                }
            }

            touchStart = GetWorldPosition();
        }


        if (!mViewInputBlock && Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - GetWorldPosition();
            Camera.main.transform.position += direction;
        }
    }

    private Vector3 GetWorldPosition()
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }

    public void OnObjectSelection(int identifier)
    {
        eventSystem.SendEvent<OnSelectGameAsset>(new OnSelectGameAsset { id = identifier });
    }

    public void PlaceAsset(GameObject asset, Vector3 worldPos, Vector3 screenPos)
    {
        asset.transform.position = new Vector3(worldPos.x, asset.transform.position.y, worldPos.z);
        ShowInteractionUI(screenPos, Vector3.one);
    }

    void ShowInteractionUI(Vector3 pos, Vector3 scale)
    {
        objectInteractionUI.SetActive(true);
        objectInteractionUI.transform.position = pos;
        objectInteractionUI.transform.localScale = scale;
    }

    public void HideInteractionUI()
    {
        objectInteractionUI.SetActive(false);
        objectInteractionUI.transform.localScale = Vector3.one;
    }

    public void SetViewInputBlock(bool isActive)
    {
        mViewInputBlock = isActive;
    }
}
