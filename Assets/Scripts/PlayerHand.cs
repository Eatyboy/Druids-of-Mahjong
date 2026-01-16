using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [Header("References and Such")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float maxHorizontalTileOffset;

    [Header("Hand/Tiles")]
    [SerializeField] private int defaultHandSize = 14;
    [SerializeField] private GameObject tileObj;
    [SerializeField] private float tileOffsetX;

    public List<Tile> currentHand;

    private void Awake()
    {
        currentHand = new();
    }
    
    private void Start()
    {
        for (int i = 0; i < defaultHandSize; i++)
        {
            GameObject newTile = Instantiate(tileObj, this.gameObject.transform);
            currentHand.Add(newTile.GetComponent<Tile>());
        }

        RepositionTiles();
    }

    public void AddTile(Tile tile)
    {

    }

    public void RemoveTile(Tile tile)
    {

    }

    // clear
    public void ClearTiles()
    {
        foreach (Tile t in currentHand)
        {
            Destroy(t.gameObject);
        }
    }

    public void RepositionTiles()
    {
        int numTiles = currentHand.Count;
        float offsetPerTile = tileObj.transform.localScale.x + 0.1f;
        
        // if too many tiles, they should overlap
        if (offsetPerTile * numTiles > maxHorizontalTileOffset * 2.0f)
        {
            offsetPerTile = maxHorizontalTileOffset / (0.5f * (float)numTiles);
        }

        float initOffsetX = (0.5f * offsetPerTile) * (1.0f - (float)numTiles); 

        for (int i = 0; i < numTiles; i++)
        {
            currentHand[i].gameObject.transform.position = new(tileOffsetX + initOffsetX + offsetPerTile * i, currentHand[i].gameObject.transform.position.y, 0.0f);
        }
    }

    public void SortTiles()
    {
        
    }
}
