using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject levelsPanel;
    public GameObject settingsPanel;

    [Header("Level Toggles")]
    public Toggle level1Toggle;
    public Toggle level2Toggle;

    void Start()
    {
        string savedLevel = PlayerPrefs.GetString("SavedLevel", "Level_1");

        if (level1Toggle != null) level1Toggle.SetIsOnWithoutNotify(savedLevel == "Level_1");
        if (level2Toggle != null) level2Toggle.SetIsOnWithoutNotify(savedLevel == "Level_2");

        ShowMainPanel();
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        string levelToLoad = "Level_1"; 

        if (level2Toggle != null && level2Toggle.isOn)
        {
            levelToLoad = "Level_2";
        }

        PlayerPrefs.SetString("SavedLevel", levelToLoad);
        PlayerPrefs.Save();

        SceneManager.LoadScene(levelToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Exit the game!");
        Application.Quit();
    }

    public void OpenLevelsPanel()
    {
        mainPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void ShowMainPanel()
    {
        levelsPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        levelsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}