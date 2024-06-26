using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoSingleton<MainGame>
{
    public enum AIDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    [SerializeField] private Material _xMat;
    [SerializeField] private Material _oMat;
    [SerializeField] private List<SpawnerElement> _spawners;

    [SerializeField] private bool _isSinglePlayer = true;
    [SerializeField] private AIDifficulty _aIDifficulty = AIDifficulty.Easy;

    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _returnButton;

    private static int TOTALTURNS = 9;

    private int _turn = 0;
    private int _turnsLeft = TOTALTURNS;

    private bool _gameOver = false;
    private bool _winnerDeclared = false;

    private int[][] winningCombinations = new int[][]
    {
        new int[] {0, 1, 2}, // Row 1
        new int[] {3, 4, 5}, // Row 2
        new int[] {6, 7, 8}, // Row 3
        new int[] {0, 3, 6}, // Column 1
        new int[] {1, 4, 7}, // Column 2
        new int[] {2, 5, 8}, // Column 3
        new int[] {0, 4, 8}, // Diagonal 1
        new int[] {2, 4, 6}  // Diagonal 2
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_gameOver)
                return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Spawner")
                {
                    SpawnerElement spawner = hit.collider.GetComponent<SpawnerElement>();

                    if (!spawner.IsOccupied)
                    {
                        spawner.OccupySpawner(_turn == 0 ? _xMat : _oMat, _turn == 0 ? "X" : "O");

                        if (_turnsLeft <= 5)
                        {
                            if (CheckWin(_turn == 0 ? "X" : "O"))
                            {
                                ShowEndScreen("Player " + (_turn == 0 ? "X" : "O") + " wins!");
                                Debug.Log("Player " + (_turn == 0 ? "X" : "O") + " wins!");
                                _winnerDeclared = true;
                                return;
                            }
                        }

                        _turn = _turn == 0 ? 1 : 0;
                        _turnsLeft--;

                        if (_isSinglePlayer && _turn == 1)
                        {
                            AIMakeMove();
                        }

                        if (_turnsLeft == 0 && !_winnerDeclared)
                        {
                            ShowEndScreen("It's a draw!");
                            Debug.Log("Draw");
                        }
                    }
                }
            }
        }
    }

    public bool CheckWin(string icon)
    {
        foreach (var combination in winningCombinations)
        {
            if (_spawners[combination[0]].CheckIcon(icon) &&
                _spawners[combination[1]].CheckIcon(icon) &&
                _spawners[combination[2]].CheckIcon(icon))
            {
                return true;
            }
        }
        return false;
    }

    public void SetTwoPlayerMode()
    {
        _isSinglePlayer = false;
    }

    public void SetDifficulty(AIDifficulty difficulty)
    {
        _aIDifficulty = difficulty;
        _isSinglePlayer = true;
    }

    private void AIMakeMove()
    {
        if (_aIDifficulty == AIDifficulty.Easy)
        {
            AIMakeEasyMove();
        }
        else if (_aIDifficulty == AIDifficulty.Normal)
        {
            AIMakeNormalMove();
        }
        else if (_aIDifficulty == AIDifficulty.Hard)
        {
            AIMakeHardMove();
        }
    }

    /// <summary>
    /// Easy mode makes the AI place an icon randomly where it can
    /// </summary>
    private void AIMakeEasyMove()
    {
        // Random move
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < _spawners.Count; i++)
        {
            if (!_spawners[i].IsOccupied)
            {
                availableMoves.Add(i);
            }
        }

        if (availableMoves.Count > 0)
        {
            int moveIndex = availableMoves[Random.Range(0, availableMoves.Count)];
            _spawners[moveIndex].OccupySpawner(_oMat, "O");

            if (CheckWin("O"))
            {
                ShowEndScreen("AI Wins!");
                Debug.Log("AI wins!");
                return;
            }

            _turn = 0;
            _turnsLeft--;
        }
    }

    /// <summary>
    /// Normal mode looks for the player's current positions and checks to see if there is a winning move
    /// If a winning move is found, it will add its icon on that slot
    /// If not, it will put the icon in a random location
    /// </summary>
    private void AIMakeNormalMove()
    {
        for (int i = 0; i < _spawners.Count; i++)
        {
            if (!_spawners[i].IsOccupied)
            {
                _spawners[i].OccupySpawner(_oMat, "O");
                if (CheckWin("O"))
                {
                    ShowEndScreen("AI Wins!");
                    Debug.Log("AI wins!");
                    return;
                }
                _spawners[i].ResetSpawner();

                _spawners[i].OccupySpawner(_xMat, "X");
                if (CheckWin("X"))
                {
                    _spawners[i].OccupySpawner(_oMat, "O");
                    _turn = 0;
                    _turnsLeft--;
                    return;
                }
                _spawners[i].ResetSpawner();
            }
        }
        AIMakeEasyMove();
    }

    /// <summary>
    /// Hard mode makes the AI use Minimax algorithm to determine the best route
    /// </summary> 
    private void AIMakeHardMove()
    {
        int bestScore = int.MinValue;
        int moveIndex = -1;

        for (int i = 0; i < _spawners.Count; i++)
        {
            if (!_spawners[i].IsOccupied)
            {
                _spawners[i].OccupySpawner(_oMat, "O");
                int score = Minimax(_spawners, 0, false);
                _spawners[i].ResetSpawner();

                if (score > bestScore)
                {
                    bestScore = score;
                    moveIndex = i;
                }
            }
        }

        if (moveIndex != -1)
        {
            _spawners[moveIndex].OccupySpawner(_oMat, "O");

            if (CheckWin("O"))
            {
                ShowEndScreen("AI Wins!");
                Debug.Log("AI wins!");
                return;
            }

            _turn = 0;
            _turnsLeft--;
        }
    }

    /// <summary>
    /// Minimax algorithm is a decision rule used in AI to help minimize the possible loss for a worst case scenario
    /// It helps going into the board and finding the best case scenario for the AI to use in order to not lose as hard
    /// </summary>
    /// <param name="board"></param>
    /// <param name="depth"></param>
    /// <param name="isMaximizing"></param>
    /// <returns></returns>
    private int Minimax(List<SpawnerElement> board, int depth, bool isMaximizing)
    {
        if (CheckWin("O")) return 1;
        if (CheckWin("X")) return -1;
        if (_turnsLeft == 0) return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            for (int i = 0; i < board.Count; i++)
            {
                if (!board[i].IsOccupied)
                {
                    board[i].OccupySpawner(_oMat, "O");
                    int score = Minimax(board, depth + 1, false);
                    board[i].ResetSpawner();
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            for (int i = 0; i < board.Count; i++)
            {
                if (!board[i].IsOccupied)
                {
                    board[i].OccupySpawner(_xMat, "X");
                    int score = Minimax(board, depth + 1, true);
                    board[i].ResetSpawner();
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    public void ShowEndScreen(string text)
    {
        _gameOver = true;

        _winnerText.text = text;
        _gamePanel.SetActive(true);

        _playAgainButton.onClick.AddListener(ResetGame);
        _returnButton.onClick.AddListener(ReturnToMenu);
    }

    public void ReturnToMenu()
    {
        ResetGame();
        MainMenu.Instance.GoToMenu();
    }

    public void ResetGame()
    {
        _playAgainButton.onClick.RemoveAllListeners();
        _returnButton.onClick.RemoveAllListeners();

        _gamePanel.SetActive(false);

        for (int i = 0; i < _spawners.Count; i++)
        {
            _spawners[i].ResetSpawner();
        }
        _turnsLeft = TOTALTURNS;
        _turn = 0;

        _gameOver = false;
        _winnerDeclared = false;
    }
}
