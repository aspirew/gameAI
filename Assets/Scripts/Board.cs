using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{

    int totalFields;
    public int[] fields;
    public bool currentPlayer = true;

    public Board(int numOfStones, int numOfFields)
    {
        totalFields = numOfFields * 2 + 2;
        fields = new int[totalFields];

        for(int i = 0; i < fields.Length; i++)
        {
            fields[i] = numOfStones;
        }

        fields[totalFields / 2 - 1] = 0;
        fields[totalFields - 1] = 0;
    }

    public Board(int[] fields, bool currentPlayer, int totalFields)
    {
        this.fields = fields;
        this.currentPlayer = currentPlayer;
        this.totalFields = totalFields;
    }

    public Board Clone()
    {
        var fieldsCopy = fields.Clone();
        return new Board((int[])fieldsCopy, currentPlayer, totalFields);
    }

    public List<int> GetPossibleMoves()
    {
        List<int> possibleMoves = new List<int>();
        if (currentPlayer)
        {
            for (int i = 0; i < totalFields / 2 - 1; i++)
            {
                if (fields[i] > 0)
                    possibleMoves.Add(i);
            }
        }
        else
        {
            for (int i = totalFields / 2; i < totalFields - 1; i++)
            {
                if (fields[i] > 0)
                    possibleMoves.Add(i);
            }
        }

        return possibleMoves;
    }

    public bool MakeMove(int field)
    {
        if (!GetPossibleMoves().Contains(field))
        {
            Debug.Log("Invalid move!");
            return currentPlayer;
        }

        int stonesToDistribute = fields[field];

        fields[field] = 0;

        for (int i = 0; i < stonesToDistribute; i++)
        {
            field = (field + 1) % totalFields;
            fields[field]++;
        }

        if (fields[field] == 1 && field != 13 && fields[totalFields - 2 - field] > 0)
            stealStones(field);

        if ((currentPlayer && field != totalFields / 2 - 1) || (!currentPlayer && field != totalFields - 1))
            currentPlayer ^= true;

        return currentPlayer;

    }

    public bool isFinished()
    {
        bool finished = true;
        for(int i = 0; i < 6; i++)
        {
            if (fields[i] > 0) finished = false;
        }

        if (finished) return true;

        finished = true;
        for (int i = 7; i < 13; i++)
        {
            if (fields[i] > 0) finished = false;
        }

        return finished;
    }

    public bool? finishGame()
    {
        for (int i = 0; i < 6; i++)
        {
            fields[totalFields / 2 - 1] += fields[i];
            fields[i] = 0;
        }

        for (int i = 7; i < 13; i++)
        {
            fields[totalFields - 1] += fields[i];
            fields[i] = 0;
        }

        if (fields[totalFields / 2 - 1] == fields[totalFields - 1])
            return null;

        return fields[totalFields / 2 - 1] > fields[totalFields - 1];
    }

    void stealStones(int position)
    {
        if (currentPlayer && position >= 0 && position < 6)
        {
            fields[position] = 0;
            fields[totalFields / 2 - 1]++;
            fields[totalFields / 2 - 1] += fields[totalFields - 2 - position];
            fields[totalFields - 2 - position] = 0;
        }
        else if (!currentPlayer && position > 6 && position < 13)
        {
            fields[position] = 0;
            fields[totalFields - 1]++;
            fields[totalFields - 1] += fields[totalFields - 2 - position];
            fields[totalFields - 2 - position] = 0;
        }

    }

    public int getPlayerScore(bool player)
    {
        if (player)
            return fields[totalFields / 2 - 1];

        return fields[totalFields - 1];
    }

    public void Log()
    {
        string line1 = "";
        string line3 = "";

        for (int i = 0; i < 6; i++)
            line1 += fields[i] + ", ";

        string line2 = fields[13] + "            " + fields[6];

        for (int i = 12; i > 6; i--)
            line3 += fields[i] + ", ";

        Debug.Log(line3);
        Debug.Log(line2);
        Debug.Log(line1);
    }
}
