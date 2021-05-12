using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    protected GameServer gameServer;

    public bool player;
    protected bool movesAllowed;
    public int numOfMoves = 0;
    public List<long> timeForMove = new List<long>();
    public bool MovesAllowed { 
        get { return movesAllowed; }
        set { setMoves(value); } 
    }
    public Player(bool plr, GameServer gs)
    {
        gameServer = gs;
        player = plr;
        movesAllowed = player;
    }

    public virtual void MakeMove(int[] moves)
    {
        if (movesAllowed && gameServer.gameIsPlayed)
        {
            timeForMove.Add(gameServer.stopwatch.ElapsedMilliseconds);
            gameServer.stopwatch.Reset();
            numOfMoves += moves.Length;
            gameServer.ReceiveMove(moves);  
        }
    }

    protected virtual void setMoves(bool m)
    {
        movesAllowed = m;
        if(m)
            gameServer.stopwatch.Start();
    }

    public long TotalTime()
    {
        long totalTime = 0;
        foreach (var time in timeForMove)
        {
            totalTime += time;
        }
        return totalTime;
    }

    public void ResetStats()
    {
        timeForMove.Clear();
        numOfMoves = 0;
    }

}
