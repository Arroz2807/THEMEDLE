using UnityEngine;
using UnityEngine.Audio;

public class Settings_Manager : MonoBehaviour
{
    public static Settings_Manager Instance;

    [Header("Audio")]
    public AudioMixer audioMixer;

    public string IdiomaActual { get; private set; } = "es";
    public int TemaActual { get; private set; } = 0;   // 0=Oscuro, 1=Claro, etc.
    public float Volumen { get; private set; } = 0.8f;

    private const string KEY_IDIOMA = "settings_idioma";
    private const string KEY_TEMA = "settings_tema";
    private const string KEY_VOLUMEN = "settings_volumen";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    private void LoadSettings()
    {
        IdiomaActual = PlayerPrefs.GetString(KEY_IDIOMA, "es");
        TemaActual = PlayerPrefs.GetInt(KEY_TEMA, 0);
        Volumen = PlayerPrefs.GetFloat(KEY_VOLUMEN, 0.8f);
        ApplyVolume(Volumen);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString(KEY_IDIOMA, IdiomaActual);
        PlayerPrefs.SetInt(KEY_TEMA, TemaActual);
        PlayerPrefs.SetFloat(KEY_VOLUMEN, Volumen);
        PlayerPrefs.Save();
    }

    // Dropdown idioma: 0=Español, 1=Inglés
    public void OnIdiomaCambiado(int index)
    {
        IdiomaActual = index == 0 ? "es" : "en";
        SaveSettings();
    }

    // Dropdown tema: 0=Oscuro, 1=Claro (podés agregar más opciones)
    public void OnTemaCambiado(int index)
    {
        TemaActual = index;
        SaveSettings();
    }

    // Slider volumen: valor entre 0 y 1
    public void OnVolumenCambiado(float value)
    {
        Volumen = value;
        ApplyVolume(value);
        SaveSettings();
    }

    private void ApplyVolume(float value)
    {
        // Solo aplicamos si el audioMixer fue asignado en el Inspector
        // Si no está asignado, simplemente no hace nada (no rompe el juego)
        if (audioMixer == null) return;

        float db = value > 0.001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("MasterVolume", db);
    }
}