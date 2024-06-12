using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    [SerializeField] private Material _xMat;
    [SerializeField] private Material _oMat;

    [SerializeField] private List<SpawnerElement> _spawners;

    private int _turn = 0;
    private int _maxTurns = 9;
    private bool _isSinglePlayer = true;

    // Put all winning combinations into one variable for ease of access and readability
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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Spawner")
                {
                    Debug.Log(hit.collider.gameObject.name);
                    SpawnerElement spawner = hit.collider.GetComponent<SpawnerElement>();

                    if (!spawner.IsOccupied)
                    {
                        spawner.OccupySpawner(_turn == 0 ? _xMat : _oMat, _turn == 0 ? "X" : "O");

                        if(_maxTurns <= 5)
                        {
                            if (CheckWin(_turn == 0 ? "X" : "O"))
                            {
                                Debug.Log("Player " + (_turn == 0 ? "X" : "O") + " wins!");
                                return;
                            }
                        }

                        _turn = _turn == 0 ? 1 : 0;
                        _maxTurns--;

                        if (_isSinglePlayer && _turn == 1)
                        {
                            AIMakeMove();
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
                return true; // Exit once a winner is found
            }
        }
        return false;
    }

    private void AIMakeMove()
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
                Debug.Log("AI wins!");
                return;
            }

            _turn = 0; // Switch back to the player's turn
            _maxTurns--;
        }
    }

    private int Minimax(List<SpawnerElement> board, int depth, bool isMaximizing)
    {
        if (CheckWin("O")) return 1;
        if (CheckWin("X")) return -1;
        if (_maxTurns == 0) return 0;

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

    public void ResetGame()
    {
        for (int i = 0; i < _spawners.Count; i++)
            _spawners[i].ResetSpawner();

        _maxTurns = 9;
        _turn = 0;
    }
}
