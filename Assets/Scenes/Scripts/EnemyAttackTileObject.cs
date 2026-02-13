using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyAttackTileObject : MonoBehaviour
{
    public RectTransform rt;

    public Tile tileData;
    public Image tileBackImage;
    public Image tileFaceImage;

    [SerializeField] private TextMeshProUGUI tmpElement;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(Tile tile)
    {
        tileData = tile;
        tmpElement.text = tile.rank.ToString() + " of " + tile.suit.ToString();
    }
}
