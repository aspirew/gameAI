using UnityEngine;
using System;
using System.Diagnostics;

public class GameServer : MonoBehaviour
{
    [SerializeField] GameController gc;
    [SerializeField] TMPro.TextMeshProUGUI endCaption;

    public Stopwatch stopwatch = new Stopwatch();
    public Player[] players = new Player[2];
    public Board board = new Board(4, 6);
    public Player currentPlayer;
    public bool gameIsPlayed;
    int plyr = 0;

    void Start()
    {
        endCaption.enabled = false;
        gc.UpdateBoard(board);
        stopwatch.Start();
        stopwatch.Stop();
    }

    public void ReceiveMove(int[] moves)
    {
        bool boardState = board.currentPlayer;

        foreach (int move in moves)
        {
            board.MakeMove(move);
        }

        playerStats();

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
            gameIsPlayed = false;
        }

        gc.UpdateBoard(board);

        if (gameIsPlayed && board.currentPlayer != boardState)
        {
            currentPlayer.MovesAllowed = false;
            currentPlayer = players[(++plyr) % 2];
            currentPlayer.MovesAllowed = true;
        }
        else if(gameIsPlayed)
            stopwatch.Start();
        
    }

    public void StartGame()
    {       
        currentPlayer = players[0];
        gameIsPlayed = true;
        currentPlayer.MovesAllowed = true;
    }

    public void RestartGame()
    {
        stopwatch.Reset();        
        players[0].ResetStats();
        players[1].ResetStats();
        currentPlayer.MovesAllowed = false;
        currentPlayer = players[0];
        board = new Board(4, 6);
        plyr = 0;
        endCaption.enabled = false;
        gc.UpdateBoard(board);
        gameIsPlayed = false;
    }

    public void playerStats()
    {
        if(gameIsPlayed)
            foreach (var player in players)
                UnityEngine.Debug.Log(string.Format("PLAYER{0}\nNum of moves: {1} | Total time spend on picking move: {2}", Array.IndexOf(players, player)+1, player.numOfMoves, player.TotalTime()));
    }
}
