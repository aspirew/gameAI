using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameServer gameServer;

    public bool player;
    bool movesAllowed;
    public bool MovesAllowed { 
        get { return movesAllowed; }
        set { setMoves(value); }
    }
    void Start()
    {
        movesAllowed = player;
    }

    public virtual void MakeMove(int[] moves)
    {
        if(movesAllowed)
            gameServer.ReceiveMove(moves);
    }
    protected virtual void setMoves(bool m)
    {
        movesAllowed = m;
    }

}
