using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance {get; private set;}

    [Header("References and Such")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float maxHorizontalTileOffset;

    [Header("Hand/Tiles")]
    [SerializeField] private int defaultHandSize = 14;
    [SerializeField] private GameObject tileObj;
    [SerializeField] private float tileOffsetX;

    public List<Tile> currentHand;
    public List<Tile> selectedTiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        currentHand = new();

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

    public void SelectTile(Tile tile)
    {
        selectedTiles.Add(tile);
        tile.gameObject.transform.Translate(0.0f, 0.1f, 0.0f);
        List<Tile> optimalHand = PickOptimalHand();
        foreach(Tile t in currentHand)
        {
            t.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        foreach(Tile t in optimalHand)
        {
            t.gameObject.GetComponent<Image>().color = new Color(0.8f, 1f, 1f, 1f);
        }
    }

    public void DeselectTile(Tile tile)
    {
        selectedTiles.Remove(tile);
        tile.gameObject.transform.Translate(0.0f, -0.1f, 0.0f);
        List<Tile> optimalHand = PickOptimalHand();
        foreach(Tile t in currentHand)
        {
            t.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        foreach(Tile t in optimalHand)
        {
            t.gameObject.GetComponent<Image>().color = new Color(0.8f, 1f, 1f, 1f);
        }
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
        float offsetPerTile = tileObj.GetComponent<RectTransform>().rect.width + 0.1f;
        
        // if too many tiles, they should overlap
        if (offsetPerTile * numTiles > maxHorizontalTileOffset * 2.0f)
        {
            offsetPerTile = maxHorizontalTileOffset / (0.5f * (float)numTiles);
        }

        float initOffsetX = (0.5f * offsetPerTile) * (1.0f - (float)numTiles); 

        for (int i = 0; i < numTiles; i++)
        {
            currentHand[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new(tileOffsetX + initOffsetX + offsetPerTile * i, 0.0f);
        }
    }

    public void SortTiles()
    {
        
    }

    // O(n)
    public List<Tile> PickOptimalHand()
    {
        List<Tile> optimalHand = new();
        int optimalHandValue = 0;
        foreach (Tile st in selectedTiles)
        {
            List<Tile> testHandStraight = new();
            List<Tile> testHandTriplet = new();
            testHandTriplet.Add(st);
            testHandStraight.Add(st);
            int straightValue = 1;
            int tripletValue = 1;

            // find best combination for selected tile; straight or triplet
            foreach (Tile ht in currentHand)
            {
                if (st.Equals(ht)) continue;
                if (ht.GetSuitFromType(ht.type) != st.GetSuitFromType(st.type)) continue;

                // triplets
                if (!ContainsTileType(ht.type, testHandTriplet) &&
                    ((int)ht.type + 1 == (int)st.type || (int)ht.type - 1 == (int)st.type))
                {
                    testHandTriplet.Add(ht);
                    tripletValue += 1;
                }

                // straights
                if (st.type == ht.type)
                {
                    testHandStraight.Add(ht);
                    straightValue += 1;
                }
            }

            // check with current optimal hand (for ties, always choose straight)
            if (testHandTriplet.Count > optimalHand.Count /*testHandValue > optimalHandValue*/)
            {
                optimalHand = testHandTriplet;
            }
            if (testHandStraight.Count >= testHandTriplet.Count /*testHandValue > optimalHandValue*/)
            {
                optimalHand = testHandStraight;
            }
        }

        return optimalHand;
    }
    
    private bool ContainsTileType(TileType typeToCheck, List<Tile> tiles)
    {
        foreach (Tile t in tiles)
        {
            if (t.type == typeToCheck)
            {
                return true;
            }
        }
        return false;
    }
}
