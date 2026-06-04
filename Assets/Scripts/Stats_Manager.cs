using UnityEngine;

public class Stats_Manager : MonoBehaviour
{
    public static Stats_Manager Instance;

    // Propiedades de solo lectura accesibles desde cualquier script
    public int PartidasJugadas { get; private set; }
    public int PartidasGanadas { get; private set; }
    public int PorcentajeVictoria { get; private set; }

    // Claves para PlayerPrefs
    private const string KEY_JUGADAS = "stats_jugadas";
    private const string KEY_GANADAS = "stats_ganadas";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargamos las estadísticas guardadas
        LoadStats();
    }

    private void LoadStats()
    {
        PartidasJugadas = PlayerPrefs.GetInt(KEY_JUGADAS, 0);
        PartidasGanadas = PlayerPrefs.GetInt(KEY_GANADAS, 0);
        CalcularPorcentaje();
    }

    // Llamado por Game_Manager cuando termina una partida
    // won = true si ganó, false si perdió
    public void RegisterResult(bool won)
    {
        PartidasJugadas++;
        if (won) PartidasGanadas++;

        CalcularPorcentaje();
        SaveStats();
    }

    // Calcula el porcentaje como entero (ej: 75)
    private void CalcularPorcentaje()
    {
        PorcentajeVictoria = PartidasJugadas > 0
            ? Mathf.RoundToInt((float)PartidasGanadas / PartidasJugadas * 100)
            : 0;
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt(KEY_JUGADAS, PartidasJugadas);
        PlayerPrefs.SetInt(KEY_GANADAS, PartidasGanadas);
        PlayerPrefs.Save();
    }
}