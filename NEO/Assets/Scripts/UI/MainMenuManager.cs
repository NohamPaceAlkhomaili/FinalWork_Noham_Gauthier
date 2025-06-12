using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject welcomePanel;
    public GameObject mainPanel;
    public GameObject modeSelectionPanel;

    [Header("Buttons")]
    public Button buttonWelcome;
    public Button buttonBeatSaber;
    public Button buttonEndlessRunner;
    public Button buttonTutorial;
    public Button buttonSoloPlay;
    public Button button1v1;
    public Button buttonBack;

    private bool hasStarted = false;

    void Start()
    {
        welcomePanel.SetActive(true);
        mainPanel.SetActive(false);
        modeSelectionPanel.SetActive(false);

        buttonWelcome.onClick.AddListener(ShowMainMenu);
        buttonBeatSaber.onClick.AddListener(OnBeatSaberClicked);
        buttonEndlessRunner.onClick.AddListener(OnEndlessRunnerClicked);
        buttonTutorial.onClick.AddListener(OnTutorialClicked);
        buttonSoloPlay.onClick.AddListener(OnSoloPlayClicked);
        button1v1.onClick.AddListener(On1v1Clicked);
        buttonBack.onClick.AddListener(OnBackClicked);
    }

    void Update()
    {
        if (!hasStarted && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            ShowMainMenu();
            hasStarted = true;
        }
    }

    void ShowMainMenu()
    {
        welcomePanel.SetActive(false);
        mainPanel.SetActive(true);
        modeSelectionPanel.SetActive(false);
    }

    private void OnBeatSaberClicked() => SceneManager.LoadScene("RhytmeGame");
    private void OnEndlessRunnerClicked() => TogglePanels(mainPanel, modeSelectionPanel);
    private void OnTutorialClicked() => SceneManager.LoadScene("Tutorial");
    private void OnSoloPlayClicked() => SceneManager.LoadScene("GameSolo");
    private void On1v1Clicked() => SceneManager.LoadScene("Game1v1");
    private void OnBackClicked() => TogglePanels(modeSelectionPanel, mainPanel);

    private void TogglePanels(GameObject disablePanel, GameObject enablePanel)
    {
        disablePanel.SetActive(false);
        enablePanel.SetActive(true);
    }
}
