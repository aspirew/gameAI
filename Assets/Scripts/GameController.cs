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

    [SerializeField] int p1Deep;
    [SerializeField] int p2Deep;
    [SerializeField] int p1Heuristic;
    [SerializeField] int p2Heuristic;
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
            gameServer.players[0] = new AI(true, gameServer, 0, p1Heuristic, p1Deep);
        else
            gameServer.players[0] = new AI(true, gameServer, 1, p1Heuristic, p1Deep);

        if (player2Dropdown.value == (int)Players.Player)
            gameServer.players[1] = new Player(false, gameServer);
        else if (player2Dropdown.value == (int)Players.Minimax)
            gameServer.players[1] = new AI(false, gameServer, 0, p2Heuristic, p2Deep);
        else
            gameServer.players[1] = new AI(false, gameServer, 1, p2Heuristic, p2Deep);

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
