using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tile_Grid : MonoBehaviour
{
    // ── CONFIGURACIÓN DE LA GRILLA ────────────────────────────────────────

    [Header("Configuración")]
    public int maxAttempts = 6; // filas: cuántos intentos tiene el jugador

    [Header("Prefab y contenedor")]
    public GameObject tilePrefab;   // el prefab de un tile individual
    public Transform gridContainer; // el TileGrid_Container de la escena

    // ── ESTADO INTERNO ────────────────────────────────────────────────────

    // Referencia al Grid Layout Group del contenedor
    private GridLayoutGroup gridLayoutGroup;

    // Grilla 2D de tiles: [fila][columna]
    private Tile[,] tiles;

    // Fila actual (intento en curso), columna actual (posición de la letra)
    private int currentRow = 0;
    private int currentColumn = 0;

    // Largo de la palabra actual (se calcula al iniciar cada partida)
    private int wordLength = 5;

    // La palabra secreta que el jugador tiene que adivinar
    private string secretWord = "";

    // ── AWAKE ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (gridContainer == null)
        {
            Debug.LogError("TileGrid: gridContainer no está asignado en el Inspector");
            return;
        }

        // Buscamos o creamos el Grid Layout Group en el contenedor
        gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null)
            gridLayoutGroup = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
    }

    // ── INICIALIZACIÓN ────────────────────────────────────────────────────

    // Crea la grilla de tiles en la escena
    // Se llama desde Game_Manager cuando empieza una partida
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

        // Limpiamos cualquier tile que pudiera haber de una partida anterior
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        // Guardamos la palabra secreta y su largo
        secretWord = word.ToUpper();
        wordLength = secretWord.Length;
        currentRow = 0;
        currentColumn = 0;

        // Configuramos el Grid Layout Group con tamaño fijo
        ConfigureGridLayout(wordLength);

        // Inicializamos el array 2D
        tiles = new Tile[maxAttempts, wordLength];

        // Creamos los tiles fila por fila, columna por columna
        for (int row = 0; row < maxAttempts; row++)
        {
            for (int col = 0; col < wordLength; col++)
            {
                // Instanciamos el prefab dentro del contenedor
                GameObject tileGO = Instantiate(tilePrefab, gridContainer);
                tileGO.name = $"Tile_{row}_{col}";

                // Verificamos que el prefab tenga el componente Tile
                Tile t = tileGO.GetComponent<Tile>();
                if (t == null)
                {
                    Debug.LogError("El prefab Tile no tiene el componente Tile.cs");
                    return;
                }

                // Guardamos la referencia en la grilla 2D
                tiles[row, col] = t;

                // Lo reseteamos a su estado inicial
                tiles[row, col].Reset();
            }
        }
    }

    // Tamaño fijo para todos los niveles de dificultad
    // El jugador siempre ve tiles del mismo tamaño
    private void ConfigureGridLayout(int columns)
    {
        // Tamaño reducido para que quepan bien en pantalla
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

    // ── INPUT DEL JUGADOR ─────────────────────────────────────────────────

    // Agrega una letra al tile actual de la fila en curso
    public void EnterLetter(char letter)
    {
        // No hacemos nada si la grilla todavía no está inicializada
        if (tiles == null) return;

        // No agregamos más letras si la fila ya está completa
        if (currentColumn >= wordLength) return;

        // Asignamos la letra al tile correspondiente
        tiles[currentRow, currentColumn].SetLetter(letter);
        currentColumn++;
    }

    // Borra la última letra ingresada
    public void DeleteLetter()
    {
        if (tiles == null) return;
        if (currentColumn <= 0) return;

        currentColumn--;
        tiles[currentRow, currentColumn].ClearLetter();
    }

    // Evalúa la fila actual cuando el jugador presiona Enter/Confirmar
    // Retorna true si ganó, false en los demás casos
    public bool SubmitRow()
    {
        if (tiles == null) return false;

        // La fila debe estar completa antes de evaluar
        if (currentColumn < wordLength)
        {
            Game_Manager.Instance.uiManager.ShowError("¡Faltan letras!");
            return false;
        }

        // Construimos la palabra ingresada leyendo los tiles de la fila
        string guess = "";
        for (int col = 0; col < wordLength; col++)
            guess += tiles[currentRow, col].GetLetter();

        guess = guess.ToUpper();

        // Evaluamos letra por letra y pintamos los tiles con colores
        EvaluateRow(guess);

        int attemptsUsed = currentRow + 1;

        // ¿El jugador acertó la palabra completa?
        if (guess == secretWord)
        {
            Game_Manager.Instance.OnVictory(attemptsUsed);
            return true;
        }

        // Avanzamos a la siguiente fila
        currentRow++;
        currentColumn = 0;

        // ¿Se agotaron los intentos?
        if (currentRow >= maxAttempts)
        {
            Game_Manager.Instance.OnDefeat(attemptsUsed);
            return false;
        }

        return false;
    }

    // Evalúa cada letra del guess contra la palabra secreta y pinta los tiles
    private void EvaluateRow(string guess)
    {
        char[] secretChars = secretWord.ToCharArray();
        bool[] secretUsed = new bool[wordLength];
        bool[] guessUsed = new bool[wordLength];
        TileResult[] results = new TileResult[wordLength];

        // PRIMERA PASADA: letras en posición exacta → verde
        for (int col = 0; col < wordLength; col++)
        {
            if (guess[col] == secretChars[col])
            {
                results[col] = TileResult.Correct;
                secretUsed[col] = true;
                guessUsed[col] = true;
            }
        }

        // SEGUNDA PASADA: letras presentes en otra posición → amarillo
        for (int col = 0; col < wordLength; col++)
        {
            if (guessUsed[col]) continue;

            results[col] = TileResult.Absent; // por defecto gris

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

        // Pintamos cada tile Y cada tecla del teclado con su color
        for (int col = 0; col < wordLength; col++)
        {
            // Pintamos el tile de la grilla
            tiles[currentRow, col].SetResult(results[col]);

            // Pintamos la tecla correspondiente en el teclado virtual
            // La prioridad es: Correct > Present > Absent
            // Si la tecla ya es verde, no la pisamos con amarillo o gris
            char letra = guess[col];
            Game_Manager.Instance.keyboardController.UpdateKeyColor(letra, results[col]);
        }
    }
}