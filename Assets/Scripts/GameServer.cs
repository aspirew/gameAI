using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : MonoBehaviour
{
    [SerializeField] GameController gc;
    [SerializeField] Player[] players;
    [SerializeField] TMPro.TextMeshProUGUI endCaption;

    public Board board = new Board(4, 6);
    public Player currentPlayer;
    int plyr = 0;

    void Start()
    {
        endCaption.enabled = false;
        gc.UpdateBoard(board);
        currentPlayer.MovesAllowed = true;
    }

    public void ReceiveMove(int[] moves)
    {
        
        bool boardState = board.currentPlayer;

        foreach (int move in moves)
        {
            Debug.Log("Moving: " + move);
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
}
