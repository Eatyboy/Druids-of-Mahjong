using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Attach to any GameObject. Right-click the component in the Inspector
/// and choose "Run Parry Tests" to execute all test cases.
/// Results are printed to the Console.
/// </summary>
public class MahjongParryTester : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Tile construction
    //
    // Tile is a plain [Serializable] class constructed via new Tile(MahjongTile).
    // MahjongTile is a ScriptableObject we can't instantiate in tests, but
    // Tile.GetTileID() derives the ID entirely from its own suit+rank fields,
    // so we can pass null for baseTileData safely — GetTileID() never touches it.
    // -------------------------------------------------------------------------

    private static Tile MakeTile(TileSuit suit, int rank)
    {
        // Bypass the constructor (which reads from baseTileData) by creating
        // a blank instance and setting fields directly.
        var baseData = ScriptableObject.CreateInstance<MahjongTile>();
        baseData.suit = suit;
        baseData.rank = rank;
        var tile = new Tile(baseData);
        return tile;
    }

    // Shorthand helpers — one per suit
    private static Tile B(int rank) => MakeTile(TileSuit.Bamboo,    rank); // Bamboo  1–9  → IDs  1–9
    private static Tile D(int rank) => MakeTile(TileSuit.Dot,       rank); // Dot     1–9  → IDs 10–18
    private static Tile C(int rank) => MakeTile(TileSuit.Character, rank); // Char    1–9  → IDs 19–27
    private static Tile W(int rank) => MakeTile(TileSuit.Wind,      rank); // Wind    1–4  → IDs 29–32
    private static Tile Dr(int rank)=> MakeTile(TileSuit.Dragon,    rank); // Dragon  1–3  → IDs 33–35

    // -------------------------------------------------------------------------
    // Test runner
    // -------------------------------------------------------------------------

    [ContextMenu("Run Parry Tests")]
    public async void RunParryTests()
    {
        int passed = 0, failed = 0;

        async Task Run(string name, List<Tile> hand, Tile attack, MahjongHandTypes expected)
        {
            var augmentedHand = hand?.Append(attack).ToList();
            var (type, tiles) = await MahjongHands.GetOptimalHandAsync(augmentedHand, attack);

            // For any non-None result the returned hand must contain the exact attack tile
            bool attackPresent = expected == MahjongHandTypes.None
                || (tiles != null && tiles.Any(t => t == attack));

            bool ok = type == expected && attackPresent;

            if (ok)
            {
                passed++;
                Debug.Log($"<color=green>PASS</color> [{name}]  →  {type}" +
                          (tiles != null ? $"  ({tiles.Count} tiles)" : ""));
            }
            else
            {
                failed++;
                string got = tiles != null
                    ? $"{type} ({tiles.Count} tiles, attackPresent={attackPresent})"
                    : $"{type} (null)";
                Debug.LogError($"<color=red>FAIL</color> [{name}]  expected={expected}  got={got}");
            }
        }

        // =====================================================================
        // NULL / EMPTY GUARDS
        // =====================================================================

        await Run("Null hand",
            null,
            B(1),
            MahjongHandTypes.None);

        await Run("Empty hand",
            new List<Tile>(),
            B(1),
            MahjongHandTypes.None);

        await Run("No matching combination",
            new List<Tile> { B(3), D(5), D(7) },
            B(1),
            MahjongHandTypes.None);

        // =====================================================================
        // PAIR
        // =====================================================================

        await Run("Pair — attack completes pair",
            new List<Tile> { B(5) },
            B(5),
            MahjongHandTypes.Pair);

        await Run("Pair — no partner in hand",
            new List<Tile> { B(3), D(7) },
            B(5),
            MahjongHandTypes.None);

        // =====================================================================
        // SET
        // =====================================================================

        await Run("Set — attack is 3rd tile",
            new List<Tile> { B(3), B(3) },
            B(3),
            MahjongHandTypes.Set);

        // =====================================================================
        // RUN
        // =====================================================================

        await Run("Run — attack is low tile",
            new List<Tile> { B(2), B(3) },
            B(1),
            MahjongHandTypes.Run);

        await Run("Run — attack is middle tile",
            new List<Tile> { B(1), B(3) },
            B(2),
            MahjongHandTypes.Run);

        await Run("Run — attack is high tile",
            new List<Tile> { B(1), B(2) },
            B(3),
            MahjongHandTypes.Run);

        await Run("Run — cross-suit should not form",
            new List<Tile> { B(2), D(3) },
            B(1),
            MahjongHandTypes.None);

        await Run("Run — honors cannot form run",
            new List<Tile> { W(2), W(3) },
            W(1),
            MahjongHandTypes.None);

        // =====================================================================
        // QUAD
        // =====================================================================

        await Run("Quad — attack is 4th tile",
            new List<Tile> { B(7), B(7), B(7) },
            B(7),
            MahjongHandTypes.Quad);

        // =====================================================================
        // TWO PAIRS
        // =====================================================================

        await Run("TwoPairs — attack completes one pair",
            new List<Tile> { B(5), D(9), D(9) },
            B(5),
            MahjongHandTypes.TwoPairs);

        await Run("TwoPairs — both pairs present, attack is one",
            new List<Tile> { B(5), W(2), W(2) },
            B(5),
            MahjongHandTypes.TwoPairs);

        // =====================================================================
        // THREE PAIRS
        // =====================================================================

        await Run("ThreePairs — attack contributes one pair",
            new List<Tile> { B(5), D(9), D(9), C(3), C(3) },
            B(5),
            MahjongHandTypes.ThreePairs);

        // =====================================================================
        // SET AND RUN
        // =====================================================================

        await Run("SetAndRun — attack is part of the set",
            new List<Tile> { B(2), B(2), D(4), D(5), D(6) },
            B(2),
            MahjongHandTypes.SetAndRun);

        await Run("SetAndRun — attack is part of the run",
            new List<Tile> { B(2), B(2), B(2), D(5), D(6) },
            D(4),
            MahjongHandTypes.SetAndRun);

        // =====================================================================
        // TWO RUNS
        // =====================================================================

        await Run("TwoRuns — attack starts one run, second run already present",
            new List<Tile> { B(2), B(3), D(1), D(2), D(3) },
            B(1),
            MahjongHandTypes.TwoRuns);

        await Run("TwoRuns — attack is middle of one run",
            new List<Tile> { B(1), B(3), D(4), D(5), D(6) },
            B(2),
            MahjongHandTypes.TwoRuns);

        // =====================================================================
        // TWO SETS
        // =====================================================================

        await Run("TwoSets — attack completes one set",
            new List<Tile> { B(4), B(4), D(6), D(6), D(6) },
            B(4),
            MahjongHandTypes.TwoSets);

        // =====================================================================
        // TWO QUADS
        // =====================================================================

        await Run("TwoQuads — attack is 4th tile of one quad",
            new List<Tile> { B(1), B(1), B(1), D(3), D(3), D(3), D(3) },
            B(1),
            MahjongHandTypes.TwoQuads);

        // =====================================================================
        // THREE SETS
        // =====================================================================

        await Run("ThreeSets — attack completes one of three sets",
            new List<Tile> { B(2), B(2), D(5), D(5), D(5), C(8), C(8), C(8) },
            B(2),
            MahjongHandTypes.ThreeSets);

        // =====================================================================
        // NINE RUN
        // =====================================================================

        await Run("NineRun — attack fills the missing rank",
            new List<Tile> { B(1), B(2), B(3), B(4), B(6), B(7), B(8), B(9) },
            B(5),
            MahjongHandTypes.NineRun);

        await Run("NineRun — wrong suit for completion",
            new List<Tile> { B(1), B(2), B(3), B(4), B(5), B(6), B(7), B(8) },
            D(9),  // Dot 9, not Bamboo — cannot complete Bamboo nine-run
            MahjongHandTypes.None);

        await Run("NineRun — attack is rank 1 of complete sequence",
            new List<Tile> { B(2), B(3), B(4), B(5), B(6), B(7), B(8), B(9) },
            B(1),
            MahjongHandTypes.NineRun);

        await Run("NineRun — attack is rank 9 of complete sequence",
            new List<Tile> { B(1), B(2), B(3), B(4), B(5), B(6), B(7), B(8) },
            B(9),
            MahjongHandTypes.NineRun);

        // =====================================================================
        // ALL PAIRS  (14-tile hand)
        // =====================================================================

        await Run("AllPairs — attack completes 7th pair",
            new List<Tile>
            {
                B(1), B(1),
                B(2), B(2),
                D(3), D(3),
                D(4), D(4),
                C(5), C(5),
                C(6), C(6),
                W(1),            // lone wind — attack pairs it
            },
            W(1),
            MahjongHandTypes.AllPairs);

        // =====================================================================
        // FULL WIN  (14-tile hand)
        // =====================================================================

        await Run("FullWin — attack is the pair tile",
            new List<Tile>
            {
                // Four complete melds
                B(1), B(2), B(3),
                B(4), B(5), B(6),
                D(1), D(2), D(3),
                D(4), D(5), D(6),
                W(2),            // one tile of the pair already in hand
            },
            W(2),                // attack completes the pair
            MahjongHandTypes.FullWin);

        await Run("FullWin — attack completes the 4th meld",
            new List<Tile>
            {
                W(1), W(1),      // pair
                B(1), B(2), B(3),
                B(4), B(5), B(6),
                D(1), D(2), D(3),
                D(5), D(6),      // meld 4 needs D(4)
            },
            D(4),
            MahjongHandTypes.FullWin);

        // =====================================================================
        // PRIORITY: higher hand type must win over lower
        // =====================================================================

        // Attack B(3) appears 3× in hand → set possible.
        // Hand also has B(4) B(5) → run B3-B4-B5 possible using the 4th B3.
        // SetAndRun (enum 7) beats Set (enum 2) and Run (enum 3).
        await Run("Priority — SetAndRun beats plain Set or Run",
            new List<Tile> { B(3), B(3), B(4), B(5) },
            B(3),
            MahjongHandTypes.SetAndRun);

        // TwoPairs (enum 5) beats Pair (enum 1) when a second pair is available
        await Run("Priority — TwoPairs beats Pair",
            new List<Tile> { B(5), D(9), D(9) },
            B(5),
            MahjongHandTypes.TwoPairs);

        // =====================================================================
        // TIE-BREAK: same hand type → higher max TileID wins
        // =====================================================================

        // Two Run opportunities using attack tile B(2):
        //   Run A: B(1)-B(2)-B(3) — max TileID = B(3) = 4
        //   Run B: B(2)-B(3)-B(4) — max TileID = B(4) = 5  ← should win
        // Both share B(3) so they are NOT disjoint → TwoRuns is impossible.
        // Tiebreak picks Run B since 5 > 4.
        await Run("TieBreak — Run with higher max TileID preferred",
            new List<Tile> { B(1), B(3), B(4) },
            B(2),
            MahjongHandTypes.Run);

        // TwoPairs tiebreak: attack B(2) pairs with hand B(2); second pair can be
        // D(3)+D(3) (maxID=13) or D(7)+D(7) (maxID=17). D(7) wins the tiebreak.
        // Only one extra pair present at a time to avoid ThreePairs firing:
        await Run("TieBreak — TwoPairs with higher-ID second pair preferred",
            new List<Tile> { B(2), D(7), D(7) },
            B(2),
            MahjongHandTypes.TwoPairs); // B2+B2 with D7+D7; maxID = D(7) = 17

        // =====================================================================
        // REFERENCE IDENTITY: attack tile must be the EXACT instance
        // =====================================================================

        // Hand has two B(5) tiles already. We create a SEPARATE B(5) as the attack.
        // The parry should succeed because there are now 3× B(5) total (set).
        var handB5a = B(5);
        var handB5b = B(5);
        var attackB5 = B(5); // distinct instance, same value
        await Run("Identity — attack is distinct instance from hand tiles",
            new List<Tile> { handB5a, handB5b },
            attackB5,
            MahjongHandTypes.Set);

        // Hand has one B(5). Attack is a DIFFERENT B(5) instance.
        // Only 2× B(5) total → Pair expected.
        var handB5c  = B(5);
        var attackB5b = B(5);
        await Run("Identity — pair using attack's own reference",
            new List<Tile> { handB5c },
            attackB5b,
            MahjongHandTypes.Pair);

        // =====================================================================
        // SUMMARY
        // =====================================================================

        int total = passed + failed;
        string color = failed == 0 ? "green" : "red";
        Debug.Log($"<color={color}>===  Parry Tests: {passed}/{total} passed" +
                  (failed > 0 ? $",  {failed} FAILED" : "  ✓") + "  ===</color>");
    }
}