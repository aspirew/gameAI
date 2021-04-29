using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] bool owner;
    [SerializeField] int move;
    [SerializeField] GameServer gameServer;
    public void OnMouseDown()
    {
        if (gameServer.currentPlayer.player == owner)
        {
            gameServer.currentPlayer.MakeMove(new int[1] { move });
        }
    }

}
