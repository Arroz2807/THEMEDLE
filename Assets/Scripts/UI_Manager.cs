using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_Manager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelSelectionMenu;
    public GameObject panelHowToPlay;
    public GameObject panelSettings;
    public GameObject panelStats;
    public GameObject panelGame;
    public GameObject panelResult;

    [Header("Panel SelectionMenu")]
    public TMP_Dropdown dropdownCategoria;
    public TMP_Dropdown dropdownDificultad;

    [Header("Panel Game")]
    public TMP_Text txtCategory;
    public TMP_Text txtErrorMessage;

    [Header("Panel Stats")]
    public TMP_Text txtPartidasJugadas;
    public TMP_Text txtPartidasGanadas;
    public TMP_Text txtPorcentaje;

    public float statsDisplayTime = 3f;

    [Header("Panel Result")]
    public TMP_Text txtResultStatus;
    public TMP_Text txtResultWord;
    public TMP_Text txtResultAttempts;

    [Header("Panel Settings")]
    public Slider sliderVolumen;
    public TMP_Dropdown dropdownTema;
    public TMP_Dropdown dropdownIdioma;

    void Start()
    {
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
                StartCoroutine(SyncVolumeSliderNextFrame());
                break;

            case GameState.Playing:
                panelGame.SetActive(true);
                break;

            case GameState.Stats:

                RefreshStatsPanel();
                panelStats.SetActive(true);

                StartCoroutine(WaitAndShowResult());
                break;

            case GameState.Result:

                RefreshResultPanel();
                panelResult.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        StopAllCoroutines();

        if (panelMenu) panelMenu.SetActive(false);
        if (panelSelectionMenu) panelSelectionMenu.SetActive(false);
        if (panelHowToPlay) panelHowToPlay.SetActive(false);
        if (panelSettings)
        {
            if (Settings_Manager.Instance != null)
                Settings_Manager.Instance.SetIgnorarCambiosVolumen(true);
            panelSettings.SetActive(false);
        }
        if (panelStats) panelStats.SetActive(false);
        if (panelGame) panelGame.SetActive(false);
        if (panelResult) panelResult.SetActive(false);
    }

    private IEnumerator WaitAndShowResult()
    {
        yield return new WaitForSeconds(statsDisplayTime);
        Game_Manager.Instance.ProceedToResult();
    }

    private IEnumerator SyncVolumeSliderNextFrame()
    {
        yield return new WaitForEndOfFrame();

        if (sliderVolumen != null && Settings_Manager.Instance != null)
        {
            Debug.Log($"[DIAGNOSTICO] SyncVolumeSliderNextFrame: aplicando {Settings_Manager.Instance.Volumen} al slider. Valor actual del slider antes = {sliderVolumen.value}");
            sliderVolumen.SetValueWithoutNotify(Settings_Manager.Instance.Volumen);
            Debug.Log($"[DIAGNOSTICO] SyncVolumeSliderNextFrame: valor del slider despues = {sliderVolumen.value}");
        }

        if (Settings_Manager.Instance != null)
            Settings_Manager.Instance.SetIgnorarCambiosVolumen(false);
    }

    private void RefreshStatsPanel()
    {
        Stats_Manager sm = Stats_Manager.Instance;
        txtPartidasJugadas.text = "Partidas Jugadas: " + sm.PartidasJugadas;
        txtPartidasGanadas.text = "Partidas Ganadas: " + sm.PartidasGanadas;
        txtPorcentaje.text = "Porcentaje de Victoria: " + sm.PorcentajeVictoria + "%";
    }

    private void RefreshResultPanel()
    {
        Game_Manager gm = Game_Manager.Instance;
        bool won = gm.LastGameWon;
        int attempts = gm.LastAttemptsUsed;
        string word = gm.LastSecretWord;

        txtResultStatus.text = won ? "¡CORRECTO!" : "¡INCORRECTO!";
        txtResultWord.text = "La palabra era: " + word.ToUpper();

        if (won)
            txtResultAttempts.text = "La adivinaste en " + attempts + " intento(s)";
        else
            txtResultAttempts.text = "Usaste los " + attempts + " intentos";
    }

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

    public void OnBtn_StartGame()
    {
        string[] categorias = { "Animales", "Paises", "Comidas" };
        string categoria = categorias[dropdownCategoria.value];

        string[] dificultades = { "Facil", "Medio", "Dificil" };
        string dificultad = dificultades[dropdownDificultad.value];

        Game_Manager.Instance.StartGame(categoria, dificultad);
    }
}