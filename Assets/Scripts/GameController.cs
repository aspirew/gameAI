using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] Captions;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBoard(Board board)
    {
        for (int i = 0; i < Captions.Length; i++)
        {
            Captions[i].text = board.fields[i].ToString();
        }
    }
}
