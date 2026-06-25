using UnityEngine;

[CreateAssetMenu(
    fileName = "AudioSettingsData",
    menuName = "Settings/Audio Settings Data"
)]
public class AudioSettingsData : ScriptableObject
{
    [Range(0f, 1f)]
    public float volume = 0.8f;
}