using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private int defaultHandSize = 14;

    public List<Tile> currentHand;

    private void Awake()
    {
        currentHand = new(defaultHandSize);
    }
}
