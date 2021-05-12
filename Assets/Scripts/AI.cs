using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatesTreeNode
{
    public Board board;
    public List<StatesTreeNode> children = new List<StatesTreeNode>();
    public int[] moves;
    private int score = 0;
    private bool scoreSet = false;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            scoreSet = true;
        }
    }

    public bool ScoreSet
    {
        get { return scoreSet; }
        private set { scoreSet = value; }
    }

    public StatesTreeNode(Board board, int[] newMoves)
    {
        this.board = board;
        moves = (int[])newMoves.Clone();
    }
}

public class AI : Player
{
    StatesTreeNode currentTree;
    int deep;
    int mode;
    int heuristic;

    public AI(bool plr, GameServer gs, int mode, int heuristic, int deep) : base(plr, gs)
    {
        this.deep = deep;
        this.mode = mode;
        this.heuristic = heuristic;
        
        //currentTree = GenerateStatesTreeInit(gameServer.board, deep);
    }

    public int RateBoardByScore(Board board) // heuristic = 0
    {
        return board.getPlayerScore(player) - board.getPlayerScore(!player);
    }

    public int RateBoardByAttackOppurtinity(Board board) // heuristic = 1
    {
        int score = RateBoardByScore(board);
        if (board.lastMoveWasAttack)
            return score + 1000;
        else
            return score;
    }

    public int RateBoardByWinningOppurtinity(Board board) // heuristic = 2
    {
        if (board.isFinished())
        {
            board.finishGame();
            if (RateBoardByScore(board) > 0)
                return 10000;
            else if (RateBoardByScore(board) < 0)
                return -10000;
        }

        return RateBoardByScore(board);
    }

    public int RateBoardByWinningOppurtinityWithAttackFocus(Board board) // heuristic = 3
    {
        if (board.isFinished())
        {
            board.finishGame();
            if (RateBoardByScore(board) > 0)
                return 10000;
            else if (RateBoardByScore(board) < 0)
                return -10000;
        }

        return RateBoardByAttackOppurtinity(board);
    }

    protected override void setMoves(bool m)
    {
        movesAllowed = m;
        if(m) ChooseMove();
    }
    void Computations()
    {
        Board newBoard = gameServer.board;
        currentTree = GenerateStatesTreeInit(newBoard, deep);
        int[] moves;
        gameServer.stopwatch.Start();
        if (mode == 0)
            moves = Minimax(currentTree);
        else
            moves = Alfabeta(currentTree, -1000000, 1000000);

        //Debug.Log(gameServer.stopwatch.ElapsedMilliseconds);
        MakeMove(moves);
        
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        Computations();
    }

    void ChooseMove()
    {
        if (MovesAllowed && gameServer.gameIsPlayed)
        {
            gameServer.StartCoroutine(Wait());
            //Computations();
        }
    }

    private List<StatesTreeNode> ConsolidateNodes(StatesTreeNode node, List<int> movesSoFar)
    {
        var possibleMoves = node.board.GetPossibleMoves();
        List<StatesTreeNode> nodesToConsolidate = new List<StatesTreeNode>();

        foreach (int move in possibleMoves)
        {
            Board brd = node.board.Clone();
            bool lastPlr = brd.currentPlayer;
            bool plr = brd.MakeMove(move);
            movesSoFar.Add(move);
            StatesTreeNode newNode = new StatesTreeNode(brd, movesSoFar.ToArray());
            if (plr == lastPlr && !brd.isFinished())
            {
                nodesToConsolidate.AddRange(ConsolidateNodes(newNode, movesSoFar));
            }
            else
            {
                nodesToConsolidate.Add(newNode);
            }
            movesSoFar.RemoveAt(movesSoFar.Count - 1);
        }

        return nodesToConsolidate;
    }

    private void GenerateStatesTree(StatesTreeNode tree, int threshold)
    {

        var possibleMoves = tree.board.GetPossibleMoves();

        if (threshold < 1 || possibleMoves.Count < 1)
            return;

        foreach (int move in possibleMoves)
        {
            var movesSoFar = new int[] { move };
            Board newBoard = tree.board.Clone();
            bool lastPlr = newBoard.currentPlayer;
            bool plr = newBoard.MakeMove(move);
            StatesTreeNode treeNode = new StatesTreeNode(newBoard, movesSoFar);

            if (plr == lastPlr && !newBoard.isFinished())
            {
                var nodesToAdd = ConsolidateNodes(treeNode, movesSoFar.ToList());
                foreach (var nodeToAdd in nodesToAdd)
                {
                    GenerateStatesTree(nodeToAdd, threshold - 1);
                    tree.children.Add(nodeToAdd);
                }
            }
            else
            {
                GenerateStatesTree(treeNode, threshold - 1);
                tree.children.Add(treeNode);
            }
        }
    }

    public StatesTreeNode GenerateStatesTreeInit(Board board, int threshold)
    {
        StatesTreeNode tree = new StatesTreeNode(board, new int[1]);
        GenerateStatesTree(tree, threshold); 
        return tree;
    }

    private int[] Minimax(StatesTreeNode tree)
    {
        if (tree.children.Count < 1) {
            if (heuristic == 0)
                tree.Score = RateBoardByScore(tree.board);
            else if (heuristic == 1)
                tree.Score = RateBoardByAttackOppurtinity(tree.board);
            else if (heuristic == 2)
                tree.Score = RateBoardByWinningOppurtinity(tree.board);
            else if (heuristic == 3)
                tree.Score = RateBoardByWinningOppurtinityWithAttackFocus(tree.board);
            else
                tree.Score = 0;
            return tree.moves;
        }

        int[] moves = new int[0];
        if (tree.board.currentPlayer == player)
        {
            foreach (var child in tree.children)
            {
                Minimax(child);
                if (!tree.ScoreSet || tree.Score < child.Score)
                {
                    tree.Score = child.Score;
                    moves = child.moves;
                }
            }
        }
        else
        {
            foreach (var child in tree.children)
            {
                Minimax(child);
                if (!tree.ScoreSet || tree.Score > child.Score)
                {
                    tree.Score = child.Score;
                    moves = child.moves;
                }
            }
        }
        return moves;
    }


    private int[] Alfabeta(StatesTreeNode tree, int alfa, int beta)
    {
        if (tree.children.Count < 1)
        {
            if (heuristic == 0)
                tree.Score = RateBoardByScore(tree.board);
            else if (heuristic == 1)
                tree.Score = RateBoardByAttackOppurtinity(tree.board);
            else if (heuristic == 2)
                tree.Score = RateBoardByWinningOppurtinity(tree.board);
            else if (heuristic == 3)
                tree.Score = RateBoardByWinningOppurtinityWithAttackFocus(tree.board);
            else
                tree.Score = 0;
            return tree.moves;
        }

        int[] moves = new int[0];
        if (tree.board.currentPlayer == player)
        {
            foreach (var child in tree.children)
            {
                Alfabeta(child, alfa, beta);
                alfa = Mathf.Max(alfa, child.Score);
                if (alfa >= beta)
                {
                    tree.Score = 1000000;
                    return tree.moves;
                }
                if (!tree.ScoreSet || tree.Score < child.Score)
                {
                    tree.Score = child.Score;
                    moves = child.moves;
                }
            }
        }
        else
        {
            foreach (var child in tree.children)
            {
                Alfabeta(child, alfa, beta);
                beta = Mathf.Min(beta, child.Score);
                if (alfa >= beta)
                {
                    tree.Score = -1000000;
                    return tree.moves;
                }
                if (!tree.ScoreSet || tree.Score > child.Score)
                {
                    tree.Score = child.Score;
                    moves = child.moves;
                }
            }
        }
        return moves;
    }

    public void LogTree(StatesTreeNode tree)
    {

        string movv = "";
        foreach (var move in tree.moves)
        {
            movv += move + ", ";
        }
        Debug.Log("MOVES: " + movv);
        Debug.Log("SCORE: " + tree.Score);
        tree.board.Log();

        foreach (var child in tree.children)
        {
            LogTree(child);
        }
    }
}
