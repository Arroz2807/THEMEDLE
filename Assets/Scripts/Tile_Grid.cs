using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tile_Grid : MonoBehaviour
{

    [Header("Configuración")]
    public int maxAttempts = 6; 

    [Header("Prefab y contenedor")]
    public GameObject tilePrefab;   
    public Transform gridContainer; 



    private GridLayoutGroup gridLayoutGroup;


    private Tile[,] tiles;


    private int currentRow = 0;
    private int currentColumn = 0;

    private int wordLength;
    private string secretWord;



    void Awake()
    {
        if (gridContainer == null)
        {
            Debug.LogError("TileGrid: gridContainer no está asignado en el Inspector");
            return;
        }

        gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
            gridLayoutGroup = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
    }


    public void BuildGrid(string word)
    {
        if (gridContainer == null)
        {
            Debug.LogError("TileGrid: gridContainer es null");
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("TileGrid: tilePrefab es null");
            return;
        }


        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);


        secretWord = word.ToUpper();
        wordLength = secretWord.Length;
        currentRow = 0;
        currentColumn = 0;

        ConfigureGridLayout(wordLength);


        tiles = new Tile[maxAttempts, wordLength];

        for (int row = 0; row < maxAttempts; row++)
        {
            for (int col = 0; col < wordLength; col++)
            {

                GameObject tileGO = Instantiate(tilePrefab, gridContainer);
                tileGO.name = $"Tile_{row}_{col}";


                Tile t = tileGO.GetComponent<Tile>();
                if (t == null)
                {
                    Debug.LogError("El prefab Tile no tiene el componente Tile.cs");
                    return;
                }


                tiles[row, col] = t;

                tiles[row, col].Reset();
            }
        }
    }


    private void ConfigureGridLayout(int columns)
    {
        float tileSize = 75f;
        float spacing = 5f;

        gridLayoutGroup.cellSize = new Vector2(tileSize, tileSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
    }


    public void EnterLetter(char letter)
    {

        if (tiles == null) return;


        if (currentColumn >= wordLength) return;


        tiles[currentRow, currentColumn].SetLetter(letter);
        currentColumn++;
    }


    public void DeleteLetter()
    {
        if (tiles == null) return;
        if (currentColumn <= 0) return;

        currentColumn--;
        tiles[currentRow, currentColumn].ClearLetter();
    }


    public bool SubmitRow()
    {
        if (tiles == null) return false;

        // La fila debe estar completa antes de evaluar
        if (currentColumn < wordLength)
        {
            Game_Manager.Instance.uiManager.ShowError("¡Faltan letras!");
            return false;
        }


        string guess = "";
        for (int col = 0; col < wordLength; col++)
            guess += tiles[currentRow, col].GetLetter();

        guess = guess.ToUpper();


        EvaluateRow(guess);

        int attemptsUsed = currentRow + 1;


        if (guess == secretWord)
        {
            Game_Manager.Instance.OnVictory(attemptsUsed);
            return true;
        }


        currentRow++;
        currentColumn = 0;


        if (currentRow >= maxAttempts)
        {
            Game_Manager.Instance.OnDefeat(attemptsUsed);
            return false;
        }

        return false;
    }


    private void EvaluateRow(string guess)
    {
        char[] secretChars = secretWord.ToCharArray();
        bool[] secretUsed = new bool[wordLength];
        bool[] guessUsed = new bool[wordLength];
        TileResult[] results = new TileResult[wordLength];

        
        for (int col = 0; col < wordLength; col++)
        {
            if (guess[col] == secretChars[col])
            {
                results[col] = TileResult.Correct;
                secretUsed[col] = true;
                guessUsed[col] = true;
            }
        }

        
        for (int col = 0; col < wordLength; col++)
        {
            if (guessUsed[col]) continue;

            results[col] = TileResult.Absent; 

            for (int s = 0; s < wordLength; s++)
            {
                if (!secretUsed[s] && guess[col] == secretChars[s])
                {
                    results[col] = TileResult.Present;
                    secretUsed[s] = true;
                    break;
                }
            }
        }

        
        for (int col = 0; col < wordLength; col++)
        {
            
            tiles[currentRow, col].SetResult(results[col]);

            
            char letra = guess[col];
            Game_Manager.Instance.keyboardController.UpdateKeyColor(letra, results[col]);
        }
    }
}