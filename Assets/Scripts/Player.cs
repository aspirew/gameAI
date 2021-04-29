using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    protected GameServer gameServer;

    public bool player;
    bool movesAllowed;
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
        if(movesAllowed && gameServer.gameIsPlayed)
            gameServer.ReceiveMove(moves);
    }
    protected virtual void setMoves(bool m)
    {
        movesAllowed = m;
    }

}
