using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] bool owner;
    [SerializeField] int move;
    [SerializeField] GameServer gameServer;
    public void MakePlayerMove()
    {
        if (gameServer.currentPlayer.player == owner)
        {
            int[] moves = new int[1];
            moves[0] = move;
            gameServer.currentPlayer.MakeMove(moves);
        }
    }

}
