using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class Game : MonoBehaviour
{
    private AudioSource moveSound;
    
    public GameObject Chesspiece;

    // Position and team for each chesspiece
    private GameObject[,] positions = new GameObject[8, 16];
    private GameObject[] playerWhite = new GameObject[16];
    private GameObject[] playerBlack = new GameObject[16];

    // Simulation positions
    private GameObject[,] simulation = new GameObject[8, 16];

    private string currentPlayer = "white";
    public bool gameOver = false;
    public bool draw = false;
    private bool nextTurn = false;
    private bool check = false;

    // EnPassant positions
    public int ex;
    public int ey;

    // Castling
    public bool canWhiteKingSideCastling = true;
    public bool canWhiteQueenSideCastling = true;
    public bool canBlackKingSideCastling = true;
    public bool canBlackQueenSideCastling = true;

    // Simulation
    private bool simCheck = false;
    private bool checkmate = false;
    private bool iniSimulation = false;
    private GameObject attackSim;

    // Synchronize previous coords
    private int previousX;
    private int previousY;

    // Synchronize castling
    private string castling;


    void Awake() 
    {
        moveSound = GetComponent<AudioSource>();

        CreateWhitePieces();
        CreateBlackPieces();
    }

    private void CreateWhitePieces() 
    {
        playerWhite = new GameObject[] {
            Create("WhiteRook", 0, 0), Create("WhiteKnight", 1, 0), Create("WhiteBishop", 2, 0), Create("WhiteQueen", 3, 0),
            Create("WhiteKing", 4, 0), Create("WhiteBishop", 5, 0), Create("WhiteKnight", 6, 0), Create("WhiteRook", 7, 0),
            Create("WhitePawn", 0, 1), Create("WhitePawn", 1, 1), Create("WhitePawn", 2, 1), Create("WhitePawn", 3, 1),
            Create("WhitePawn", 4, 1), Create("WhitePawn", 5, 1), Create("WhitePawn", 6, 1), Create("WhitePawn", 7, 1) };
        
        // Set white piece positions on the position board
        for (int i = 0; i < playerWhite.Length; i++)
            SetPosition(playerWhite[i]);
    }

    private void CreateBlackPieces() 
    {
        playerBlack = new GameObject[] {
            Create("BlackRook", 0, 15), Create("BlackKnight", 1, 15), Create("BlackBishop", 2, 15), Create("BlackQueen", 3, 15),
            Create("BlackKing", 4, 15), Create("BlackBishop", 5, 15), Create("BlackKnight", 6, 15), Create("BlackRook", 7, 15),
            Create("BlackPawn", 0, 14), Create("BlackPawn", 1, 14), Create("BlackPawn", 2, 14), Create("BlackPawn", 3, 14),
            Create("BlackPawn", 4, 14), Create("BlackPawn", 5, 14), Create("BlackPawn", 6, 14), Create("BlackPawn", 7, 14) };

        // Set black piece positions on the position board
        for (int i = 0; i < playerBlack.Length; i++)
            SetPosition(playerBlack[i]);  
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj;

        obj = Instantiate(Chesspiece, new Vector3(0,0,-1), Quaternion.identity);

        Piece cm = obj.GetComponent<Piece>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }

        if (nextTurn == true)
            NextTurn();
    }


    #region Positions
    public void SetPosition(GameObject obj)
    {
        Piece cm = obj.GetComponent<Piece>();

        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
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

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }
    #endregion


    #region Simulation
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

    public void OpponentPieceSimulation()
    {
        if (currentPlayer == "white")
            BlackPiecesSimulation();
        if (currentPlayer == "black")
            WhitePiecesSimulation();
    }

    public void WhitePiecesSimulation()
    {
        for (int i = 0; i < playerWhite.Length; i++)
        {
            if (playerWhite[i] != null)
            {
                Piece ps = playerWhite[i].GetComponent<Piece>();
                if (playerWhite[i] != attackSim)
                    ps.Simulation(playerWhite[i]);
            }
        }
        attackSim = null;
    }

    public void BlackPiecesSimulation()
    {
        for (int i = 0; i < playerBlack.Length; i++)
        {
            if (playerBlack[i] != null)
            {
                Piece ps = playerBlack[i].GetComponent<Piece>();
                if (playerBlack[i] != attackSim)
                    ps.Simulation(playerBlack[i]);
            }
        }
        attackSim = null;
    }

    public GameObject GetSimPiece(GameObject piece)
    {
        GameObject simPiece = null;
        
        for (int x = 0; x < 8; x++) 
            for (int y = 0; y < 16; y++) 
                if (GetPosition(x, y) != null)
                    if (GetPosition(x, y).name == piece.name)
                        simPiece = GetPosition(x, y);

        return simPiece;
    }

    public GameObject GetMyKing()
    {
        GameObject myKing = null;

        for (int x = 0; x < 8; x++) 
            for (int y = 0; y < 16; y++) 
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
            for (int y = 0; y < 16; y++) 
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
                    Piece ps = playerWhite[i].GetComponent<Piece>();
                    ps.InitiateMovePlates();
                }
            }
            if (currentPlayer == "black")
            {
                if (playerBlack[i] != null)
                {
                    Piece ps = playerBlack[i].GetComponent<Piece>();
                    ps.InitiateMovePlates();                 
                }
            }
        }

        iniSimulation = false;
    }

    public bool IniSimulation()
    {
        return iniSimulation;
    }
    #endregion

 
    #region Turn
    public bool TurnManager(string player)
    {
        if (player == "white")
            return true;
        else if (player == "black")
            return true;
        else 
            return false;
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
    #endregion

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

    public void Winner(string playerWinner)
    {
        gameOver = true;
    }

    public void Draw()
    {
        gameOver = true;
        draw = true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void setCheck(bool set)
    {
        check = set;
    }

    public bool GetCheck()
    {
        return check;
    }

    public void SetCheckmate(bool set)
    {
        checkmate = set;
    }

    public void Checkmate()
    {
        if (checkmate && check == true)
        {
            Debug.Log("checkmate"); 
            if (GetCurrentPlayer() == "white") Winner("black");
            if (GetCurrentPlayer() == "black") Winner("white");
        }
    }

    public void Stalemate()
    {
        if (checkmate && check == false)
        {
            Debug.Log("stalemate"); 
            Draw();
        }
    }
}