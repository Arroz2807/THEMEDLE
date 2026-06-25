using UnityEngine;
using UnityEngine.Audio;

public class Settings_Manager : MonoBehaviour
{
    public static Settings_Manager Instance;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Audio Data")]
    public AudioSettingsData audioData;

    public string IdiomaActual { get; private set; } = "es";
    public int TemaActual { get; private set; } = 0;
    public float Volumen { get; private set; } = 0.8f;

    private const string KEY_IDIOMA = "settings_idioma";
    private const string KEY_TEMA = "settings_tema";

    private bool settingsLoaded = false;
    private bool ignorarCambiosVolumen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void Start()
    {
        ApplyVolume(Volumen);
    }

    private void LoadSettings()
    {
        IdiomaActual = PlayerPrefs.GetString(KEY_IDIOMA, "es");
        TemaActual = PlayerPrefs.GetInt(KEY_TEMA, 0);

        if (audioData != null)
        {
            Volumen = Mathf.Clamp01(audioData.volume);

            if (Volumen <= 0.0001f)
            {
                Volumen = 0.8f;
                audioData.volume = Volumen;
            }
        }
        else
        {
            Volumen = 0.8f;
        }

        Debug.Log($"[DIAGNOSTICO] Volumen cargado = {Volumen}");

        ApplyVolume(Volumen);

        settingsLoaded = true;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString(KEY_IDIOMA, IdiomaActual);
        PlayerPrefs.SetInt(KEY_TEMA, TemaActual);

        if (audioData != null)
        {
            audioData.volume = Volumen;
        }

        PlayerPrefs.Save();

        Debug.Log($"[DIAGNOSTICO] Volumen guardado = {Volumen}");
    }

    public void OnIdiomaCambiado(int index)
    {
        IdiomaActual = index == 0 ? "es" : "en";
        SaveSettings();
    }

    public void OnTemaCambiado(int index)
    {
        TemaActual = index;
        SaveSettings();
    }

    public void SetIgnorarCambiosVolumen(bool ignorar)
    {
        ignorarCambiosVolumen = ignorar;
    }

    public void OnVolumenCambiado(float value)
    {
        Debug.Log($"[DIAGNOSTICO] Slider envió value = {value}");

        if (!settingsLoaded)
            return;

        if (ignorarCambiosVolumen)
            return;

        Volumen = Mathf.Clamp01(value);

        Debug.Log($"[DIAGNOSTICO] Volumen asignado = {Volumen}");

        ApplyVolume(Volumen);

        SaveSettings();
    }

    private void ApplyVolume(float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[DIAGNOSTICO] AudioMixer NO asignado");
            return;
        }

        float db =
            value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        audioMixer.SetFloat("MasterVolume", db);

        Debug.Log(
            $"[DIAGNOSTICO] ApplyVolume value={value} db={db}"
        );
    }
}