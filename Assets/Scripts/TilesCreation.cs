using System;
using System.Numerics;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

public class TilesCreation : MonoBehaviour
{
    public TilesManager manager;    
    
    public GameObject tilePrefab;

    public GameObject containerPrefab;

    public Transform tileContainer;

    public void Start()
    {
        CreateTiles();
    }

    public void CreateTiles()
    {
        foreach(MajongTiles tileData in manager.tilesList)
        {
            //Create containers which have buttons inside of them
            GameObject majongContainer = Instantiate(containerPrefab,
                                                                tileContainer);

            GameObject majongButton = Instantiate(tilePrefab,
                                                    majongContainer.transform);
            
            majongButton.transform.localPosition = UnityEngine.Vector3.zero;

            TilesDisplay controller = majongButton.GetComponent<TilesDisplay>();

            //Placec holder for now until images are added
            setName(majongContainer, $"{tileData.suit} {tileData.value} Container");

            setName(majongButton, $"{tileData.suit} {tileData.value}");

            setIdentity(controller, tileData);
            
            //Adding select and deselect
            Button buttonAction = majongButton.GetComponent<Button>();

            UnityEngine.Vector2 originalPosition = majongButton.transform.localPosition;

            buttonAction.onClick.AddListener(() =>
            {
                OnClickEvent(tileData, majongButton, originalPosition);
            });
        }
    }

    public void OnClickEvent(MajongTiles tile, GameObject majong,
                                    UnityEngine.Vector2 originalPosition)
    {
        RectTransform position = majong.GetComponent<RectTransform>();
        
        //Select and deselect, probably will also call combo detection method
        if (!tile.selected)
        {
            position.anchoredPosition = new UnityEngine.Vector2(
                                            position.anchoredPosition.x, 50f);

            tile.selected = true;
        }
        else
        {
            position.anchoredPosition = originalPosition;

            tile.selected = false;
        }

        // Debug.Log($"{tile.suit} {tile.value}");

    }

    //Placeholder method for now until images are added
    //Used to know what tile is presented
    public void setIdentity(TilesDisplay controller, MajongTiles tile)
    {
        controller.setTile(tile);

        controller.setSuitText(tile.suit.ToString());
        
        controller.SetValueText(tile.value.ToString());
    }

    //Set the name of object
    public void setName(GameObject obj, String input)
    {
        obj.name = input;
    }
}