using UnityEngine;
using UnityEngine.UI;

public class MusicSettingsManager : MonoBehaviour
{
    public static MusicSettingsManager Instance; // Singleton instance

    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource musicSource;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    void Start()
    {
        // Remove existing listeners to avoid unintended calls
        musicToggle.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();

        // Load saved preferences
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Apply preferences to UI
        musicToggle.isOn = musicOn;
        volumeSlider.value = volume;

        // Apply preferences to music source
        musicSource.volume = volume;
        musicSource.mute = !musicOn;

        // Add listeners back after initialization
        musicToggle.onValueChanged.AddListener(OnToggleMusic);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;
        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
    }

    private void OnVolumeChanged(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
