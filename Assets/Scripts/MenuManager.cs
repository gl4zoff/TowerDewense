using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject lvlCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private AudioMixerGroup masterMixer;
    [SerializeField] private Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;


    void Start()
    {
        if (!PlayerPrefs.HasKey("Master"))
            PlayerPrefs.SetFloat("Master", master.value);
        else
            master.value = PlayerPrefs.GetFloat("Master");

        if (!PlayerPrefs.HasKey("Music"))
            PlayerPrefs.SetFloat("Music", music.value);
        else
            music.value = PlayerPrefs.GetFloat("Music");

        if (!PlayerPrefs.HasKey("SFX"))
            PlayerPrefs.SetFloat("SFX", sfx.value);
        else
            sfx.value = PlayerPrefs.GetFloat("SFX");

        ChangeEffectsVolume();
        ChangeMasterVolume();
        ChangeMusicVolume();
    }

    public void PlayButtonClick()
    {
        lvlCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void SettingsButtonClick()
    {
        settingsCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void SettingsExit()
    {
        settingsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void LevelExit()
    {
        lvlCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    public void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
    public void ChangeMasterVolume()
    {
        masterMixer.audioMixer.SetFloat("Master", Mathf.Lerp(-80, 0, master.value));
        PlayerPrefs.SetFloat("Master", master.value);
    }
    public void ChangeMusicVolume()
    {
        masterMixer.audioMixer.SetFloat("Music", Mathf.Lerp(-80, 0, music.value));
        PlayerPrefs.SetFloat("Music", music.value);
    }
    public void ChangeEffectsVolume()
    {
        masterMixer.audioMixer.SetFloat("Effects", Mathf.Lerp(-80, 0, sfx.value));
        PlayerPrefs.SetFloat("SFX", sfx.value);
    }
}
