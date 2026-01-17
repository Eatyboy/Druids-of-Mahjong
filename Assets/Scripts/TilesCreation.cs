using UnityEngine;

public class TilesCreation : MonoBehaviour
{
    public TilesManager manager;    
    
    public GameObject tilePrefab;
    
    public Transform tileContainer;

    public void Start()
    {
        CreateTiles();
    }

    public void CreateTiles()
    {
        foreach(MajongTiles tile in manager.tilesList)
        {
            GameObject button = Instantiate(tilePrefab, tileContainer);

            TilesDisplay controller = button.GetComponent<TilesDisplay>();

            RectTransform position = button.GetComponent<RectTransform>();

            controller.setTile(tile);

            controller.setSuitText(tile.suit.ToString());
            
            controller.SetValueText(tile.value.ToString());
        }
    }
}