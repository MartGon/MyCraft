using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    // Menu
    public GameObject mainMenu;
    public GameObject playMenu;

    // Widgets
    public InputField seedInputField;
    public InputField scalerInputField;
    public Toggle treesToggle;

    public void handlePlayButton()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        seedInputField.text = 1234.ToString();
        scalerInputField.text = 0.0175f.ToString();
    }

    public void handleContinueButton()
    {
        int seed = int.Parse(seedInputField.text);
        float scaler = float.Parse(scalerInputField.text);

        PlayerPrefs.SetInt("seed", seed);
        PlayerPrefs.SetFloat("scaler", scaler);
        PlayerPrefs.SetInt("trees", treesToggle.isOn ? 1 : 0);

        SceneManager.LoadScene("MyCraftGame");
    }

    public void handleCancelButton()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
    }

    public void handleExitButton()
    {
        Application.Quit();
    }
}
