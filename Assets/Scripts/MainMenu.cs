using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoSingleton<MainMenu>
{
    [Header("Panel Objects")]
    [SerializeField] private GameObject _welcomePanel;
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _difficultyPanel;

    [Space(8), Header("Main Buttons")]
    [SerializeField] private Button _vsAIButton;
    [SerializeField] private Button _vsPlayerButton;

    [Space(8), Header("Difficulty Buttons")]
    [SerializeField] private Button _easyButton;
    [SerializeField] private Button _normalButton;
    [SerializeField] private Button _hardButton;

    [Space(8), Header("Game Engine")]
    [SerializeField] private GameObject _gameEngine;

    private MainGame _mainGame;

    private void Start()
    {
        _mainGame = MainGame.Instance;

        _vsAIButton.onClick.AddListener(PlayVSAI);
        _vsPlayerButton.onClick.AddListener(PlayVSPlayer);

        _easyButton.onClick.AddListener(SetDifficultyToEasy);
        _normalButton.onClick.AddListener(SetDifficultyToNormal);
        _hardButton.onClick.AddListener(SetDifficultyToHard);

        GoToMenu();
    }

    private void PlayVSPlayer()
    {
        _mainGame.SetTwoPlayerMode();

        GoToGame();
    }

    private void PlayVSAI()
    {
        _mainPanel.SetActive(false);
        _difficultyPanel.SetActive(true);
    }

    public void SetDifficulty(MainGame.AIDifficulty difficulty)
    {
        _mainGame.SetDifficulty(difficulty);

        GoToGame();
    }

    public void SetDifficultyToEasy()
    {
        SetDifficulty(MainGame.AIDifficulty.Easy);
    }

    public void SetDifficultyToNormal()
    {
        SetDifficulty(MainGame.AIDifficulty.Normal);
    }

    public void SetDifficultyToHard()
    {
        SetDifficulty(MainGame.AIDifficulty.Hard);
    }

    public void GoToGame()
    {
        _welcomePanel.SetActive(false);
        _gameEngine.SetActive(true);
    }

    public void GoToMenu()
    {
        _welcomePanel.SetActive(true);
        _mainPanel.SetActive(true);

        _difficultyPanel.SetActive(false);
        _gameEngine.SetActive(false);
    }
}
