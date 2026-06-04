using UnityEngine;

public enum GameState
{
    Menu, SelectionMenu, HowToPlay, Settings, Playing, Stats, Result
}

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;

    public GameState CurrentState { get; private set; }

    [Header("Managers")]
    public UI_Manager uiManager;
    public Word_Manager wordManager;
    public Stats_Manager statsManager;
    public Tile_Grid tileGrid;
    public Keyboard_Controller keyboardController;
    public Input_Handler inputHandler;

    // Resultado de la última partida (para Panel_Result)
    private bool _lastGameWon;
    private int _lastAttemptsUsed;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        uiManager.UpdateUI(newState);
    }

    // Se llama cuando el jugador presiona "Comenzar" en Panel_SelectionMenu
    public void StartGame(string category, string difficulty)
    {
        // 1. Elegimos la palabra secreta
        wordManager.SelectWord(category, difficulty);

        // 2. Activamos el panel de juego ANTES de construir la grilla
        // Esto es necesario para que los tiles se inicialicen correctamente
        uiManager.UpdateUI(GameState.Playing);

        // 3. Construimos la grilla con la longitud de la palabra elegida
        tileGrid.BuildGrid(wordManager.SecretWord);

        // 4. Construimos el teclado y lo conectamos a la grilla
        keyboardController.BuildKeyboard(tileGrid);

        // 5. Activamos el input de teclado físico
        inputHandler.Activate(tileGrid);

        // 6. Actualizamos la UI con la categoría
        uiManager.SetCategoryLabel(category);
    }

    // Llamado por Tile_Grid cuando el jugador gana
    public void OnVictory(int attemptsUsed)
    {
        _lastGameWon = true;
        _lastAttemptsUsed = attemptsUsed;
        statsManager.RegisterResult(true);
        inputHandler.Deactivate(); // bloqueamos el input
        ChangeState(GameState.Stats);
    }

    // Llamado por Tile_Grid cuando se agotan los intentos
    public void OnDefeat(int attemptsUsed)
    {
        _lastGameWon = false;
        _lastAttemptsUsed = attemptsUsed;
        statsManager.RegisterResult(false);
        inputHandler.Deactivate();
        ChangeState(GameState.Stats);
    }

    // Llamado por UI_Manager cuando termina el delay de Stats
    public void ProceedToResult()
    {
        ChangeState(GameState.Result);
    }

    // Getters para Panel_Result
    public bool LastGameWon => _lastGameWon;
    public int LastAttemptsUsed => _lastAttemptsUsed;
    public string LastSecretWord => wordManager.SecretWord;
}