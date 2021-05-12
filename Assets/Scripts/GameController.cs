using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum Players
{
    Player,
    Minimax,
    Alfabeta
}

public class GameController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] Captions;
    [SerializeField] TileController[] Tiles;
    [SerializeField] Button startButton;
    [SerializeField] Button randomMoveButton;
    [SerializeField] TMP_Dropdown player1Dropdown;
    [SerializeField] TMP_Dropdown player2Dropdown;

    [SerializeField] GameServer gameServer;

    [SerializeField] int deep;
    [SerializeField] int heuristic;
    [SerializeField] GameObject stone;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartButtonOnClick);
        randomMoveButton.onClick.AddListener(MakeRandomMove);
    }

    public void StartButtonOnClick()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(RestartButtonOnClick);
        var text = startButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Restart";

        if (player1Dropdown.value == (int)Players.Player)
            gameServer.players[0] = new Player(true, gameServer);
        else if (player1Dropdown.value == (int)Players.Minimax)
            gameServer.players[0] = new AI(true, gameServer, 0, heuristic, deep);
        else
            gameServer.players[0] = new AI(true, gameServer, 1, heuristic, deep);

        if (player2Dropdown.value == (int)Players.Player)
            gameServer.players[1] = new Player(false, gameServer);
        else if (player2Dropdown.value == (int)Players.Minimax)
            gameServer.players[1] = new AI(false, gameServer, 0, heuristic, deep);
        else
            gameServer.players[1] = new AI(false, gameServer, 1, heuristic, deep);

        gameServer.StartGame();
    }

    public void RestartButtonOnClick()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartButtonOnClick);
        var text = startButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Start";
        gameServer.RestartGame();
    }

    public void MakeRandomMove()
    {
        var allowedMoves = gameServer.board.GetPossibleMoves();
        int[] move = new int[1] { Random.Range(allowedMoves[0], allowedMoves[allowedMoves.Count-1]+1) };
        gameServer.ReceiveMove(move);
    }

    public void UpdateBoard(Board board)
    {
        for (int i = 0; i < Captions.Length; i++)
        {
            Captions[i].text = board.fields[i].ToString();
            Tiles[i].DrawStones(board.fields[i], stone);
        }
    }
}
