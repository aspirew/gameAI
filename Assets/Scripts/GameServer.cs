using UnityEngine;

public class GameServer : MonoBehaviour
{
    [SerializeField] GameController gc;
    [SerializeField] TMPro.TextMeshProUGUI endCaption;


    public Player[] players = new Player[2];
    public Board board = new Board(4, 6);
    public Player currentPlayer;
    public bool gameIsPlayed;
    int plyr = 0;

    void Start()
    {
        endCaption.enabled = false;
        gc.UpdateBoard(board);
    }

    public void ReceiveMove(int[] moves)
    {

        bool boardState = board.currentPlayer;

        foreach (int move in moves)
        {
            board.MakeMove(move);
        }

        if (board.isFinished())
        {
            bool? winner = board.finishGame();
            if (winner == null)
                endCaption.text = "Draw";
            else if((bool)winner)
                endCaption.text = "Player 1 wins";
            else
                endCaption.text = "Player 2 wins";
            endCaption.enabled = true;
        }

        gc.UpdateBoard(board);

        if (board.currentPlayer != boardState)
        {
            currentPlayer.MovesAllowed = false;
            currentPlayer = players[(++plyr) % 2];
            currentPlayer.MovesAllowed = true;
        }
        
    }

    public void StartGame()
    {
        currentPlayer = players[0];
        gameIsPlayed = true;
        currentPlayer.MovesAllowed = true;
    }

    public void RestartGame()
    {
        currentPlayer.MovesAllowed = false;
        currentPlayer = players[0];
        board = new Board(4, 6);
        plyr = 0;
        endCaption.enabled = false;
        gc.UpdateBoard(board);
        gameIsPlayed = false;
    }
}
