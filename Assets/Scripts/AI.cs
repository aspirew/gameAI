using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatesTreeNode
{
    public Board board;
    public List<StatesTreeNode> children = new List<StatesTreeNode>();
    public int[] moves;
    public int? score = null;

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

    public AI(bool plr, GameServer gs, int mode, int deep) : base(plr, gs)
    {
        this.deep = deep;
        this.mode = mode;
        //currentTree = GenerateStatesTreeInit(gameServer.board, deep);
    }

    public int RateBoardByScore(Board board)
    {
        return board.getPlayerScore(player) - board.getPlayerScore(!player);
    }

    protected override void setMoves(bool m)
    {
        base.setMoves(m);
        if(m) ChooseMove();
    }
    void Computations()
    {
/*        await Task.Run(() =>
        {*/
            Board newBoard = gameServer.board;
            currentTree = GenerateStatesTreeInit(newBoard, deep);
            int[] moves = Minimax(currentTree);
            MakeMove(moves);
        //});
        
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

    override public void MakeMove(int[] moves)
    {
        if (MovesAllowed && gameServer.gameIsPlayed)
        {
            gameServer.ReceiveMove(moves);
        }

/*        Board newBoard = StaticData.gameServer.board;
        currentTree = GenerateStatesTreeInit(newBoard, deep); // asynchronous*/
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
            if (plr == lastPlr)
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
        {
            tree.score = RateBoardByScore(tree.board);
            return;
        }

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
            return tree.moves;
        }

        int[] moves = new int[0];
        if (tree.board.currentPlayer == player)
        {
            foreach (var child in tree.children)
            {
                Minimax(child);
                if (tree.score == null || tree.score < child.score)
                {
                    tree.score = child.score;
                    moves = child.moves;
                }
            }
        }
        else
        {
            foreach (var child in tree.children)
            {
                Minimax(child);
                if (tree.score == null || tree.score > child.score)
                {
                    tree.score = child.score;
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
        Debug.Log("SCORE: " + tree.score);
        tree.board.Log();

        foreach (var child in tree.children)
        {
            LogTree(child);
        }
    }
}
