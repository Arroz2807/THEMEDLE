using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    // ── Paneles ──────────────────────────────────────────────────────────
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelSelectionMenu;
    public GameObject panelHowToPlay;
    public GameObject panelSettings;
    public GameObject panelStats;
    public GameObject panelGame;
    public GameObject panelResult;

    // ── Panel_SelectionMenu ────────────────────────────────────────────────────────
    [Header("Panel SelectionMenu")]
    public TMP_Dropdown dropdownCategoria;
    public TMP_Dropdown dropdownDificultad;

    // ── Panel_Game ────────────────────────────────────────────────────────
    [Header("Panel Game")]
    public TMP_Text txtCategory;
    public TMP_Text txtErrorMessage;

    // ── Panel_Stats ───────────────────────────────────────────────────────
    [Header("Panel Stats")]
    public TMP_Text txtPartidasJugadas;
    public TMP_Text txtPartidasGanadas;
    public TMP_Text txtPorcentaje;
    // Tiempo en segundos que se muestra Stats antes de avanzar a Result
    public float statsDisplayTime = 3f;

    // ── Panel_Result ──────────────────────────────────────────────────────
    [Header("Panel Result")]
    public TMP_Text txtResultStatus;    // "¡CORRECTO!" o "INCORRECTO"
    public TMP_Text txtResultWord;      // "La palabra era: TIGRE" (siempre visible)
    public TMP_Text txtResultAttempts;  // "La adivinaste en X intentos" o "Usaste 6 intentos"

    // ── Panel_Settings ────────────────────────────────────────────────────
    [Header("Panel Settings")]
    public Slider sliderVolumen;
    public TMP_Dropdown dropdownTema;   // reemplaza el Toggle
    public TMP_Dropdown dropdownIdioma;

    // ─────────────────────────────────────────────────────────────────────

    void Start()
    {
        // Al iniciar la escena, mostramos solo el menú principal
        UpdateUI(GameState.Menu);
    }

    public void UpdateUI(GameState state)
    {
        HideAllPanels();

        switch (state)
        {
            case GameState.Menu:
                panelMenu.SetActive(true);
                break;

            case GameState.SelectionMenu:
                panelSelectionMenu.SetActive(true);
                break;

            case GameState.HowToPlay:
                panelHowToPlay.SetActive(true);
                break;

            case GameState.Settings:
                panelSettings.SetActive(true);
                break;

            case GameState.Playing:
                panelGame.SetActive(true);
                break;

            case GameState.Stats:
                // Actualizamos los textos y mostramos el panel
                RefreshStatsPanel();
                panelStats.SetActive(true);
                // Arrancamos el timer para avanzar a Result
                StartCoroutine(WaitAndShowResult());
                break;

            case GameState.Result:
                // Mostramos solo el panel de resultado, sin el juego de fondo
                RefreshResultPanel();
                panelResult.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        StopAllCoroutines();

        // Verificamos que cada referencia no sea null antes de desactivar
        if (panelMenu) panelMenu.SetActive(false);
        if (panelSelectionMenu) panelSelectionMenu.SetActive(false);
        if (panelHowToPlay) panelHowToPlay.SetActive(false);
        if (panelSettings) panelSettings.SetActive(false);
        if (panelStats) panelStats.SetActive(false);
        if (panelGame) panelGame.SetActive(false);
        if (panelResult) panelResult.SetActive(false);
    }

    // Timer: después de statsDisplayTime segundos, avanza a Result
    private IEnumerator WaitAndShowResult()
    {
        yield return new WaitForSeconds(statsDisplayTime);
        Game_Manager.Instance.ProceedToResult();
    }

    // Actualiza los textos de Panel_Stats con los datos reales
    private void RefreshStatsPanel()
    {
        Stats_Manager sm = Stats_Manager.Instance;
        txtPartidasJugadas.text = "Partidas Jugadas: " + sm.PartidasJugadas;
        txtPartidasGanadas.text = "Partidas Ganadas: " + sm.PartidasGanadas;
        txtPorcentaje.text = "Porcentaje de Victoria: " + sm.PorcentajeVictoria + "%";
    }

    // Arma Panel_Result según si ganó o perdió
    private void RefreshResultPanel()
    {
        Game_Manager gm = Game_Manager.Instance;
        bool won = gm.LastGameWon;
        int attempts = gm.LastAttemptsUsed;
        string word = gm.LastSecretWord;

        // Estado: siempre muestra la palabra correcta
        txtResultStatus.text = won ? "¡CORRECTO!" : "¡INCORRECTO!";
        txtResultWord.text = "La palabra era: " + word.ToUpper();

        // Intentos: mensaje diferente según resultado
        if (won)
            txtResultAttempts.text = "La adivinaste en " + attempts + " intento(s)";
        else
            txtResultAttempts.text = "Usaste los " + attempts + " intentos";
    }

    // Muestra un error temporario en Panel_Game
    public void ShowError(string message)
    {
        txtErrorMessage.text = message;
        txtErrorMessage.gameObject.SetActive(true);
        StopCoroutine("HideErrorAfterDelay");
        StartCoroutine(HideErrorAfterDelay(2f));
    }

    private IEnumerator HideErrorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        txtErrorMessage.gameObject.SetActive(false);
    }

    public void SetCategoryLabel(string category)
    {
        txtCategory.text = "Categoría: " + category.ToUpper();
    }

    // ── BOTONES ───────────────────────────────────────────────────────────

    public void OnBtn_Play()
        => Game_Manager.Instance.ChangeState(GameState.SelectionMenu);

    public void OnBtn_HowToPlay()
        => Game_Manager.Instance.ChangeState(GameState.HowToPlay);

    public void OnBtn_Settings()
        => Game_Manager.Instance.ChangeState(GameState.Settings);

    public void OnBtn_Back()
        => Game_Manager.Instance.ChangeState(GameState.Menu);

    public void OnBtn_BackToStats()
        => Game_Manager.Instance.ChangeState(GameState.Stats);

    public void OnBtn_Home()
        => Game_Manager.Instance.ChangeState(GameState.Menu);

    public void OnBtn_PlayAgain()
        => Game_Manager.Instance.ChangeState(GameState.SelectionMenu);

    public void OnBtn_StatsToResult()
        => Game_Manager.Instance.ProceedToResult();

    public void OnBtn_Close()
        => Application.Quit();

    // En Panel_SelectionMenu: el botón Comenzar llama a este método
    public void OnBtn_StartGame()
    {
        // Leemos la categoría del dropdown (0=Animales, 1=Paises, 2=Comidas)
        string[] categorias = { "Animales", "Paises", "Comidas" };
        string categoria = categorias[dropdownCategoria.value];

        // Leemos la dificultad (0=Fácil, 1=Medio, 2=Difícil)
        string[] dificultades = { "Facil", "Medio", "Dificil" };
        string dificultad = dificultades[dropdownDificultad.value];

        // Iniciamos el juego con esos parámetros
        Game_Manager.Instance.StartGame(categoria, dificultad);
    }
}