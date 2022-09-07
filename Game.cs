using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerWhite = new GameObject[16];
    private GameObject[] playerBlack = new GameObject[16];

    private bool nextTurn = false;
    private string currentPlayer = "white";
    public bool gameOver = false;

    // castling
    public bool canWhiteKingSideCastling = true;
    public bool canWhiteQueenSideCastling = true;
    public bool canBlackKingSideCastling = true;
    public bool canBlackQueenSideCastling = true;

    private AudioSource moveSound;

    // simulation
    private GameObject[,] simulation = new GameObject[8, 8];
    private GameObject attackSim;
    private bool simCheck = false;
    private bool checkmate = false;
    private bool iniSimulation = false;
    private bool check = false;

    // Start is called before the first frame update
    void Start()
    {
        CreateWhitePieces();
        CreateBlackPieces();

        moveSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (nextTurn == true)
            NextTurn();

        if (gameOver == true)
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    private void CreateWhitePieces()
    {
        playerWhite = new GameObject[] {
            Create("WhiteRook", 0, 0), Create("WhiteKnight", 1, 0), Create("WhiteBishop", 2, 0), Create("WhiteQueen", 3, 0), 
            Create("WhiteKing", 4, 0), Create("WhiteBishop", 5, 0), Create("WhiteKnight", 6, 0), Create("WhiteRook", 7, 0), 
            Create("WhitePawn", 0, 1), Create("WhitePawn", 1, 1), Create("WhitePawn", 2, 1), Create("WhitePawn", 3, 1), 
            Create("WhitePawn", 4, 1), Create("WhitePawn", 5, 1), Create("WhitePawn", 6, 1), Create("WhitePawn", 7, 1) };

        for (int i = 0; i < playerWhite.Length; i++)
            SetPosition(playerWhite[i]);

    }

    private void CreateBlackPieces()
    {
        playerBlack = new GameObject[] {
            Create("BlackRook", 0, 7), Create("BlackKnight", 1, 7), Create("BlackBishop", 2, 7), Create("BlackQueen", 3, 7), 
            Create("BlackKing", 4, 7), Create("BlackBishop", 5, 7), Create("BlackKnight", 6, 7), Create("BlackRook", 7, 7), 
            Create("BlackPawn", 0, 6), Create("BlackPawn", 1, 6), Create("BlackPawn", 2, 6), Create("BlackPawn", 3, 6), 
            Create("BlackPawn", 4, 6), Create("BlackPawn", 5, 6), Create("BlackPawn", 6, 6), Create("BlackPawn", 7, 6) };

        for (int i = 0; i < playerBlack.Length; i++)
            SetPosition(playerBlack[i]);
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj;

        obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);

        Piece cm = obj.GetComponent<Piece>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();

        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Piece cm = obj.GetComponent<Piece>();

        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
            currentPlayer = "black";
        else
            currentPlayer = "white";

        nextTurn = false;
    }  

    public void SetNextTurn()
    {
        nextTurn = true;
    }  

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void Winner(string winner)
    {
        gameOver = true;
        Debug.Log($"{winner} win");
    } 

    public void Draw()
    {
        gameOver = true;
        Debug.Log("draw");
    }

    public void PawnPromoted(int x, int y)
    {
        if (currentPlayer == "white")
        {
            SetPosition(Create("WhiteQueen", x, y));
            moveSound.PlayOneShot(moveSound.clip);
        }
        else if (currentPlayer == "black")
        {
            SetPosition(Create("BlackQueen", x, y));
            moveSound.PlayOneShot(moveSound.clip);
        }
    }

    #region simulation
    public void IniSetSimulation(GameObject sim)
    {
        Piece cm = sim.GetComponent<Piece>();

        if (PositionOnSimBoard(cm.GetXBoard(), cm.GetYBoard()))
            simulation[cm.GetXBoard(), cm.GetYBoard()] = sim;
    }

    public void SetSimulation(GameObject sim)
    {
        Piece cm = sim.GetComponent<Piece>();

        if (PositionOnSimBoard(cm.GetXSimBoard(), cm.GetYSimBoard()))
            simulation[cm.GetXSimBoard(), cm.GetYSimBoard()] = sim;
    }

    public void SetSimulationEmpty(int x, int y)
    {
        simulation[x, y] = null;
    }

    public GameObject GetSimulation(int x, int y)
    {
        return simulation[x, y];
    }

    public void SetSimAttackPiece(GameObject piece)
    {
        attackSim = piece;
    }

    public void OpponentPiecesSimulation()
    {
        if (currentPlayer == "white")
            BlackPiecesSimulation();
        else if (currentPlayer == "black")
            WhitePiecesSimulation();
    }

    public void WhitePiecesSimulation()
    {
        for (int i = 0; i < playerWhite.Length; i ++)
            if (playerWhite[i] != null)
            {
                Piece cm = playerWhite[i].GetComponent<Piece>();
                if (playerWhite[i] != attackSim)
                   cm.Simulation(playerWhite[i]);
            }
        attackSim = null;
    }

    public void BlackPiecesSimulation()
    {
        for (int i = 0; i < playerBlack.Length; i ++)
            if (playerBlack[i] != null)
            {
                Piece cm = playerBlack[i].GetComponent<Piece>();
                if (playerBlack[i] != attackSim)
                   cm.Simulation(playerBlack[i]);
            }
        attackSim = null;
    }

    public GameObject GetMyKing()
    {
        GameObject myKing = null;

        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (GetPosition(x, y) != null)
                    if (currentPlayer == "white" && GetPosition(x, y).name == "WhiteKing")
                        myKing = GetPosition(x, y);
                    else if (currentPlayer == "black" && GetPosition(x, y).name == "BlackKing")
                        myKing = GetPosition(x, y);

        return myKing;
    }

    public void ClearSimulationBoard()
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (GetPosition(x, y) != null)
                    SetSimulationEmpty(x, y);
    }

    public void SetSimCheck(bool judge)
    {
        simCheck = judge;
    }

    public bool GetSimCheck()
    {
        return simCheck;
    }

    public bool PositionOnSimBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= simulation.GetLength(0) || y >= simulation.GetLength(1)) return false;
        return true;
    }

    public void CheckmateSimulation()
    {
        iniSimulation = true;
        checkmate = true;

        for (int i = 0; i < 16; i++)
        {
            if (currentPlayer == "white")
            {
                if (playerWhite[i] != null)
                {
                    Piece cm = playerWhite[i].GetComponent<Piece>();
                    cm.InitiateMovePlates();
                }
            }
            if (currentPlayer == "black")
            {
                if (playerBlack[i] != null)
                {
                    Piece cm = playerBlack[i].GetComponent<Piece>();
                    cm.InitiateMovePlates();
                }
            }
        }

        iniSimulation = false;
    }

    public bool IniSimulation()
    {
        return iniSimulation;
    }

    public void SetCheckmate(bool set)
    {
        checkmate = set;
    }

    public void SetCheck(bool set)
    {
        check = set;
    }

    public bool GetCheck()
    {
        return check;
    }

    public void Checkmate()
    {
        if (checkmate == true && check == true)
        {
            Debug.Log("checkmate");
            if (currentPlayer == "white") Winner("white");
            if (currentPlayer == "black") Winner("black");
        }
    }

    public void Stalemate()
    {
        if (checkmate == true && check == false)
        {
            Debug.Log("stalemate");
            Draw();
        }
    }
    #endregion
}
