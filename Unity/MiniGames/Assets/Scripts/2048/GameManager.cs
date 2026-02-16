using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Game2048 : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 4;
    public float tileSize = 75f;
    public float spacing = 10f;
    public GameObject tilePrefab;
    public Transform gridParent;
    public float moveSpeed = 10f;

    private int[,] grid;
    private GameObject[,] tileObjects;
    
    void Start()
    {
        grid = new int[gridSize, gridSize];
        tileObjects = new GameObject[gridSize, gridSize];

        // Spawn deux tuiles initiales
        SpawnTile();
        SpawnTile();
        UpdateTilesPosition();
    }


    void Update()
    {
        // Déplacement fluide des tuiles
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (tileObjects[x, y] != null)
                    tileObjects[x, y].GetComponent<RectTransform>().anchoredPosition =
                        Vector2.Lerp(tileObjects[x, y].GetComponent<RectTransform>().anchoredPosition,
                                     GetTilePosition(x, y), moveSpeed * Time.deltaTime);
    }

    // --- INPUT SYSTEM ---
    public void OnMove1(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        if (input == Vector2.zero) return;

        Vector2Int dir = Vector2Int.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            dir = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            dir = input.y > 0 ? Vector2Int.down : Vector2Int.up;

        Move(dir);
    }

    // --- LOGIQUE DU JEU ---
    void Move(Vector2Int dir)
    {
        bool moved = false;

        for (int i = 0; i < gridSize; i++)
        {
            int[] line = new int[gridSize];
            for (int j = 0; j < gridSize; j++)
            {
                int x = dir.x != 0 ? (dir.x > 0 ? gridSize - 1 - j : j) : i;
                int y = dir.y != 0 ? (dir.y > 0 ? gridSize - 1 - j : j) : i;
                line[j] = grid[x, y];
            }

            int[] merged = MergeLine(line);

            for (int j = 0; j < gridSize; j++)
            {
                int x = dir.x != 0 ? (dir.x > 0 ? gridSize - 1 - j : j) : i;
                int y = dir.y != 0 ? (dir.y > 0 ? gridSize - 1 - j : j) : i;

                if (grid[x, y] != merged[j])
                {
                    moved = true;
                    grid[x, y] = merged[j];

                    // Créer ou supprimer le GameObject tile
                    if (merged[j] != 0)
                    {
                        if (tileObjects[x, y] == null)
                        {
                            tileObjects[x, y] = Instantiate(tilePrefab, gridParent);
                            tileObjects[x, y].GetComponent<RectTransform>().anchoredPosition = GetTilePosition(x, y);
                        }
                        tileObjects[x, y].GetComponentInChildren<TextMeshProUGUI>().text = merged[j].ToString();
                    }
                    else
                    {
                        if (tileObjects[x, y] != null)
                        {
                            Destroy(tileObjects[x, y]);
                            tileObjects[x, y] = null;
                        }
                    }
                }
            }
        }

        if (moved)
            SpawnTile();
    }

    int[] MergeLine(int[] line)
    {
        List<int> newLine = new List<int>();
        for (int i = 0; i < line.Length; i++)
            if (line[i] != 0) newLine.Add(line[i]);

        for (int i = 0; i < newLine.Count - 1; i++)
        {
            if (newLine[i] == newLine[i + 1])
            {
                newLine[i] *= 2;
                newLine[i + 1] = 0;
            }
        }

        List<int> finalLine = new List<int>();
        foreach (var val in newLine)
            if (val != 0) finalLine.Add(val);

        while (finalLine.Count < gridSize) finalLine.Add(0);

        return finalLine.ToArray();
    }

    void SpawnTile()
    {
        List<Vector2Int> empty = new List<Vector2Int>();
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (grid[x, y] == 0) empty.Add(new Vector2Int(x, y));

        if (empty.Count == 0) return;

        Vector2Int pos = empty[Random.Range(0, empty.Count)];
        grid[pos.x, pos.y] = Random.value < 0.9f ? 2 : 4;

        tileObjects[pos.x, pos.y] = Instantiate(tilePrefab, gridParent);
        tileObjects[pos.x, pos.y].GetComponent<RectTransform>().anchoredPosition = GetTilePosition(pos.x, pos.y);
        tileObjects[pos.x, pos.y].GetComponentInChildren<TextMeshProUGUI>().text = grid[pos.x, pos.y].ToString();
    }

    void UpdateTilesPosition()
    {
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (tileObjects[x, y] != null)
                    tileObjects[x, y].GetComponent<RectTransform>().anchoredPosition = GetTilePosition(x, y);
    }

    Vector2 GetTilePosition(int x, int y)
    {
        float startX = -((gridSize - 1) * (tileSize + spacing)) / 2;
        float startY = ((gridSize - 1) * (tileSize + spacing)) / 2;
        return new Vector2(startX + x * (tileSize + spacing), startY - y * (tileSize + spacing));
    }
}
