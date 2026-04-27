using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject panelCategories; 
    public GameObject panelSound;
    public GameObject panelEnemy;
    public GameObject panelSkin;

    [Header("Audio")]
    public Slider[] musicSliders;
    public Toggle[] toggleMusics;
    public Slider[] sfxSliders;
    public Toggle[] toggleSFXs;

    [Header("Other")]
    public Slider enemySlider;
    public TextMeshProUGUI textEnemy;
    public TMP_Dropdown skinDropdown;

    void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVol", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVol", 1f);
        bool musicOn = PlayerPrefs.GetInt("MusicMute", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;

        foreach (var slider in musicSliders)
        {
            if (slider != null)
            {
                slider.SetValueWithoutNotify(musicVol);
                slider.interactable = musicOn;
            }
        }

        foreach (var slider in sfxSliders)
        {
            if (slider != null)
            {
                slider.SetValueWithoutNotify(sfxVol);
                slider.interactable = sfxOn;
            }
        }


        foreach (var toggle in toggleMusics) if (toggle != null) toggle.SetIsOnWithoutNotify(musicOn);
        foreach (var toggle in toggleSFXs) if (toggle != null) toggle.SetIsOnWithoutNotify(sfxOn);

        if (enemySlider) 
        {
            enemySlider.SetValueWithoutNotify(PlayerPrefs.GetInt("EnemyCount", 1));
            if (textEnemy) textEnemy.text = enemySlider.value.ToString();
        }
        if (skinDropdown) skinDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("PlayerSkin", 0));

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVol);
            AudioManager.Instance.SetSFXVolume(sfxVol);
            AudioManager.Instance.SetMusicMute(!musicOn);
            AudioManager.Instance.SetSFXMute(!sfxOn);
        }
    }
    private bool isUpdatingUI = false;
    public void ChangeMusicVolume(float volume)
    {
        if (isUpdatingUI) return; 
        isUpdatingUI = true;      

        PlayerPrefs.SetFloat("MusicVol", volume);
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.Save(); 

        foreach (var slider in musicSliders)
        {
            if (slider != null && slider.value != volume)
                slider.value = volume; 
        }

        isUpdatingUI = false;     
    }

    public void ChangeSFXVolume(float volume)
    {
        if (isUpdatingUI) return;
        isUpdatingUI = true;

        PlayerPrefs.SetFloat("SFXVol", volume);
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.Save();

        foreach (var slider in sfxSliders)
        {
            if (slider != null && slider.value != volume)
                slider.value = volume;
        }

        isUpdatingUI = false;
    }

    public void ToggleMusic(bool isOn)
    {
        if (isUpdatingUI) return;
        isUpdatingUI = true;

        PlayerPrefs.SetInt("MusicMute", isOn ? 1 : 0);
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicMute(!isOn);
        PlayerPrefs.Save();

        foreach (var toggle in toggleMusics)
        {
            if (toggle != null && toggle.isOn != isOn)
                toggle.isOn = isOn;
        }

        foreach (var slider in musicSliders)
        {
            if (slider != null) slider.interactable = isOn;
        }

        isUpdatingUI = false;
    }

    public void ToggleSFX(bool isOn)
    {
        if (isUpdatingUI) return;
        isUpdatingUI = true;

        PlayerPrefs.SetInt("SFXMute", isOn ? 1 : 0);
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXMute(!isOn);
        PlayerPrefs.Save();

        foreach (var toggle in toggleSFXs)
        {
            if (toggle != null && toggle.isOn != isOn)
                toggle.isOn = isOn;
        }

        foreach (var slider in sfxSliders)
        {
            if (slider != null) slider.interactable = isOn;
        }

        isUpdatingUI = false;
    }

    private void UpdateEnemy(float value) 
    { 
        int val = Mathf.RoundToInt(value); 
        if(textEnemy) textEnemy.text = val.ToString(); 
        PlayerPrefs.SetInt("EnemyCount", val); 
        PlayerPrefs.Save();
    }

    public void PlusEnemy() 
    { 
        if(enemySlider && enemySlider.value < 3) 
        {
            enemySlider.value++; 
            UpdateEnemy(enemySlider.value);
        }
    }

    public void MinusEnemy() 
    { 
        if(enemySlider && enemySlider.value > 1) 
        {
            enemySlider.value--; 
            UpdateEnemy(enemySlider.value);
        }
    }

    public void ChangeSkin(int index) 
    { 
        PlayerPrefs.SetInt("PlayerSkin", index); 
        PlayerPrefs.Save(); 

        PlayerController livePlayer = FindFirstObjectByType<PlayerController>();
        if (livePlayer != null)
        {
            livePlayer.SetSkin(index);
        }
    }

    public void ShowSoundPanel() 
    { 
        CloseSubPanels(); 
        if (panelCategories != null) panelCategories.SetActive(false); 
        if (panelSound != null) panelSound.SetActive(true); 
        if (settingsPanel != null && settingsPanel.GetComponent<Image>() != null) 
            settingsPanel.GetComponent<Image>().enabled = false; 
    }

    public void ShowEnemyPanel() 
    { 
        CloseSubPanels(); 
        if (panelCategories != null) panelCategories.SetActive(false); 
        if (panelEnemy != null) panelEnemy.SetActive(true); 
        if (settingsPanel != null && settingsPanel.GetComponent<Image>() != null) 
            settingsPanel.GetComponent<Image>().enabled = false; 
    }

    public void ShowSkinPanel() 
    { 
        CloseSubPanels(); 
        if (panelCategories != null) panelCategories.SetActive(false); 
        if (panelSkin != null) panelSkin.SetActive(true); 
        if (settingsPanel != null && settingsPanel.GetComponent<Image>() != null) 
            settingsPanel.GetComponent<Image>().enabled = false; 
    }

    public void CloseSubPanels() 
    { 
        if (panelSound != null) panelSound.SetActive(false); 
        if (panelEnemy != null) panelEnemy.SetActive(false); 
        if (panelSkin != null) panelSkin.SetActive(false); 
        if (panelCategories != null) panelCategories.SetActive(true); 
        if (settingsPanel != null && settingsPanel.GetComponent<Image>() != null) 
            settingsPanel.GetComponent<Image>().enabled = true; 
    }

    public void CloseFullSettings() 
    { 
        CloseSubPanels(); 
        if (settingsPanel != null) settingsPanel.SetActive(false); 
    }
}