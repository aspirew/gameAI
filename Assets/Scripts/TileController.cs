using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [SerializeField] bool owner;
    [SerializeField] int move;
    [SerializeField] GameServer gameServer;
    [SerializeField] bool playable;
    List<GameObject> stones = new List<GameObject>();

    public void OnMouseDown()
    {
        if (gameServer.currentPlayer.player == owner && playable)
        {
            gameServer.currentPlayer.MakeMove(new int[1] { move });
        }
    }

    public void DrawStones(int newNumOfStones, GameObject stone)
    {
        float xpos = gameObject.transform.position.x;
        float ypos = gameObject.transform.position.y;
        float xSize = gameObject.transform.TransformPoint(0, 0, 0).x - gameObject.transform.TransformPoint(1, 1, 0).x;
        float ySize = gameObject.transform.TransformPoint(0, 0, 0).y - gameObject.transform.TransformPoint(1, 1, 0).y;

        for (int i = stones.Count - newNumOfStones; i > 0; i--)
        {
            Destroy(stones[i-1].gameObject);
            stones.RemoveAt(i-1);
        }

        for(int i = newNumOfStones - stones.Count; i > 0; i--)
        {
            float newXPos = Random.Range(xpos - xSize / 3, xpos + xSize / 3);
            float newYPos = Random.Range(ypos - ySize / 3, ypos + ySize / 3);
            stones.Add(Instantiate(stone, new Vector3(newXPos, newYPos, 0), Quaternion.identity));
        }
    }

}
