using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    private string player;

    public Sprite WhitePawn, WhiteBishop, WhiteKnight, WhiteRook ,WhiteQueen, WhiteKing;
    public Sprite BlackPawn, BlackBishop, BlackKnight, BlackRook, BlackQueen, BlackKing;

    private bool soundOn = false;
    private AudioSource moveSound;

    // enPassant
    public bool canRightSideEnPassant = false;
    public bool canLeftSideEnPassant = false;
    public bool enPassant = false;

    // simulation
    private GameObject SimPiece;
    private int xSimBoard = -1;
    private int ySimBoard = -1;


    private void Start()
    {
        moveSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (soundOn == true)
        {
            moveSound.PlayOneShot(moveSound.clip);
            soundOn = false;
        }
    }


    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();
        SetRenderer();
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    private void SetRenderer()
    {
        switch (this.name)
        {
            case "WhitePawn": this.GetComponent<SpriteRenderer>().sprite = WhitePawn; player = "white"; break;
            case "WhiteBishop": this.GetComponent<SpriteRenderer>().sprite = WhiteBishop; player = "white"; break;
            case "WhiteKnight": this.GetComponent<SpriteRenderer>().sprite = WhiteKnight; player = "white"; break;
            case "WhiteRook": this.GetComponent<SpriteRenderer>().sprite = WhiteRook; player = "white"; break;
            case "WhiteQueen": this.GetComponent<SpriteRenderer>().sprite = WhiteQueen; player = "white"; break;
            case "WhiteKing": this.GetComponent<SpriteRenderer>().sprite = WhiteKing; player = "white"; break;

            case "BlackPawn": this.GetComponent<SpriteRenderer>().sprite = BlackPawn; player = "black"; break;
            case "BlackBishop": this.GetComponent<SpriteRenderer>().sprite = BlackBishop; player = "black"; break;
            case "BlackKnight": this.GetComponent<SpriteRenderer>().sprite = BlackKnight; player = "black"; break;
            case "BlackRook": this.GetComponent<SpriteRenderer>().sprite = BlackRook; player = "black"; break;
            case "BlackQueen": this.GetComponent<SpriteRenderer>().sprite = BlackQueen; player = "black"; break;
            case "BlackKing": this.GetComponent<SpriteRenderer>().sprite = BlackKing; player = "black"; break;
        }
    }

    private void OnMouseDown()
    {
        string currentPlayer = controller.GetComponent<Game>().GetCurrentPlayer();

        if (currentPlayer == player)
        {
            controller.GetComponent<Game>().ClearSimulationBoard();
            DestroyMovePlates();
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");

        for (int i = 0; i < movePlates.Length; i++)
            Destroy(movePlates[i]);
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "WhitePawn":
                PawnMovePlate(xBoard, yBoard + 1);
                EnPassantMovePlate(xBoard, yBoard);
                break;
            case "BlackPawn":
                PawnMovePlate(xBoard, yBoard - 1);
                EnPassantMovePlate(xBoard, yBoard);
                break;

            case "WhiteBishop":
            case "BlackBishop":
                LineMovePlate( 1, 1);
                LineMovePlate( 1,-1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1,-1);
                break;

            case "WhiteKnight":
            case "BlackKnight":
                LMovePlate();
                break;

            case "WhiteRook":
            case "BlackRook":
                LineMovePlate( 1, 0);
                LineMovePlate( 0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate( 0,-1);
                break;

            case "WhiteQueen":
            case "BlackQueen":
                LineMovePlate( 1, 0);
                LineMovePlate( 0, 1);
                LineMovePlate( 1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate( 0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(-1, 1);
                LineMovePlate( 1,-1);
                break;              

            case "WhiteKing":
            case "BlackKing":
                SurroundMovePlate();
                KingSideCastlingMovePlate();
                QueenSideCastlingMovePlate();
                break;
        }
    }


    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        PrepareSimulation(x, y);

        if (sc.GetPosition(x, y) == null)
        {
            sc.SetSimulation(SimPiece);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x, y);

            sc.SetSimCheck(false);
        }

        // attack
        if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
            sc.GetPosition(x + 1, y).GetComponent<Piece>().player != player)
        {
            AttackSimulation(x + 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateAttackSpawn(x + 1, y);

            sc.SetSimCheck(false);
        }

        if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
            sc.GetPosition(x - 1, y).GetComponent<Piece>().player != player)
        {
             AttackSimulation(x - 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateAttackSpawn(x - 1, y);

            sc.SetSimCheck(false);
        }   

        // only Initial move
        if (player == "white" && y == 2 && sc.GetPosition(x, y) == null && sc.GetPosition(x, y + 1) == null)
        {
            sc.SetSimulation(SimPiece);
            sc.SetSimulationEmpty(x, y);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateIniPawnMoveSpawn(x, y + 1);

            sc.SetSimCheck(false);
        }
        if (player == "black" && y == 5 && sc.GetPosition(x, y) == null && sc.GetPosition(x, y - 1) == null)
        {
            sc.SetSimulation(SimPiece);
            sc.SetSimulationEmpty(x, y);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateIniPawnMoveSpawn(x, y - 1);

            sc.SetSimCheck(false);
        }
    }

    public void EnPassantMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        PrepareSimulation(x, y);

        if (canRightSideEnPassant == true && player == "white")
        {
            AttackSimulation(x - 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x - 1, y + 1);

            sc.SetSimCheck(false);
            enPassant = true;
        }
        if (canLeftSideEnPassant == true && player == "white")
        {
            AttackSimulation(x + 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x + 1, y + 1);

            sc.SetSimCheck(false);
            enPassant = true;
        }

        if (canRightSideEnPassant == true && player == "black")
        {
            AttackSimulation(x - 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x - 1, y - 1);

            sc.SetSimCheck(false);
            enPassant = true;
        }
        if (canLeftSideEnPassant == true && player == "black")
        {
            AttackSimulation(x + 1, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x + 1, y - 1);

            sc.SetSimCheck(false);
            enPassant = true;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        PrepareSimulation(x, y);

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            if (sc.PositionOnSimBoard(xSimBoard, ySimBoard))
                sc.SetSimulation(SimPiece);

            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(x, y);
                    
            sc.SetSimulationEmpty(x, y);

            x += xIncrement;
            y += yIncrement;

            if (sc.PositionOnSimBoard(x, y))
                SetSimXY(x, y);

            sc.SetSimCheck(false);
        }

        // attack
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Piece>().player != player)
        {
            AttackSimulation(x, y);

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateAttackSpawn(x, y);

            sc.SetSimCheck(false);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard + 0, yBoard + 1);
        PointMovePlate(xBoard + 0, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        PrepareSimulation(x, y);

        if (sc.PositionOnBoard(x, y))
        {
            sc.SetSimulation(SimPiece);

            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                sc.OpponentPiecesSimulation();

                if (sc.GetSimCheck() == false)
                    if (sc.IniSimulation() == true)
                        sc.SetCheckmate(false);
                    else if (sc.IniSimulation() == false)
                        MovePlateSpawn(x, y);

                sc.SetSimCheck(false);
            }
            else if (cp.GetComponent<Piece>().player != player)
            {
                AttackSimulation(x, y);

                if (sc.GetSimCheck() == false)
                    if (sc.IniSimulation() == true)
                        sc.SetCheckmate(false);
                    else if (sc.IniSimulation() == false)
                        MovePlateAttackSpawn(x, y);

                sc.SetSimCheck(false);
            }
        }
      
    }

    public void KingSideCastlingMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        if (player == "white" && sc.GetPosition(5, 0) == null && sc.GetPosition(6, 0) == null && sc.canWhiteKingSideCastling == true)
        {
            PrepareSimulation(6, 0);
            sc.SetSimulation(SimPiece);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                    if (sc.IniSimulation() == true)
                        sc.SetCheckmate(false);
                    else if (sc.IniSimulation() == false)
                        MovePlateSpawn(6, 0);

            sc.SetSimCheck(false);      
        }
        else if (player == "black" && sc.GetPosition(5, 7) == null && sc.GetPosition(6, 7) == null && sc.canBlackKingSideCastling == true)
        {
            PrepareSimulation(6, 7);
            sc.SetSimulation(SimPiece);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                    if (sc.IniSimulation() == true)
                        sc.SetCheckmate(false);
                    else if (sc.IniSimulation() == false)
                        MovePlateSpawn(6, 7);

            sc.SetSimCheck(false);
        }
    }

    public void QueenSideCastlingMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        if (player == "white" && sc.GetPosition(1, 0) == null && sc.GetPosition(2, 0) == null && sc.canWhiteQueenSideCastling == true)
        {
            PrepareSimulation(1, 0);
            sc.SetSimulation(SimPiece);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(1, 0);

            sc.SetSimCheck(false);                       
        }
        else if (player == "black" && sc.GetPosition(1, 7) == null && sc.GetPosition(2, 7) == null && sc.canBlackQueenSideCastling == true)
        {
            PrepareSimulation(1, 7);
            sc.SetSimulation(SimPiece);
            sc.OpponentPiecesSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(1, 7);

            sc.SetSimCheck(false);               
        }
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
        mpScript.SetAttack(true);
    }

    public void MovePlateIniPawnMoveSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
        mpScript.SetJudgeEnPassant(true);
    }

    public void SetSoundOn(bool set)
    {
        soundOn = set;
    }


    #region simulation
    public void SetSimXY(int x, int y)
    {
        xSimBoard = x;
        ySimBoard = y;
    }

    public int GetXSimBoard()
    {
        return xSimBoard;
    }

    public int GetYSimBoard()
    {
        return ySimBoard;
    }

    private void PrepareSimulation(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        for (int x0 = 0; x0 < 8; x0++)
            for (int y0 = 0; y0 < 8; y0++)
                if (sc.GetPosition(x0, y0) != null && sc.PositionOnBoard(x0, y0))
                    sc.IniSetSimulation(sc.GetPosition(x0, y0));
                else
                    sc.SetSimulationEmpty(x0, y0);

        SimPiece = sc.GetPosition(xBoard, yBoard);

        sc.SetSimulationEmpty(xBoard, yBoard);

        if (sc.PositionOnSimBoard(x, y))
            SetSimXY(x, y);
    }

    private void AttackSimulation(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        sc.SetSimAttackPiece(sc.GetPosition(x, y));
        sc.SetSimulationEmpty(x, y);
        sc.SetSimulation(SimPiece);
        sc.OpponentPiecesSimulation();
    }

    public void Simulation(GameObject piece)
    {
        switch (piece.name)
        {
            case "WhitePawn":
                SimulationPawnMove(xBoard, yBoard + 1);
                break;
            case "BlackPawn":
                SimulationPawnMove(xBoard, yBoard - 1);
                break;

            case "WhiteBishop":
            case "BlackBishop":
                SimulationLineMove( 1, 1);
                SimulationLineMove( 1,-1);
                SimulationLineMove(-1, 1);
                SimulationLineMove(-1,-1);
                break;

            case "WhiteKnight":
            case "BlackKnight":
                SimulationLMove();
                break;

            case "WhiteRook":
            case "BlackRook":
                SimulationLineMove( 1, 0);
                SimulationLineMove( 0, 1);
                SimulationLineMove(-1, 0);
                SimulationLineMove( 0,-1);
                break;

            case "WhiteQueen":
            case "BlackQueen":
                SimulationLineMove( 1, 0);
                SimulationLineMove( 0, 1);
                SimulationLineMove( 1, 1);
                SimulationLineMove(-1, 0);
                SimulationLineMove( 0,-1);
                SimulationLineMove(-1,-1);
                SimulationLineMove(-1, 1);
                SimulationLineMove( 1,-1);
                break;              

            case "WhiteKing":
            case "BlackKing":
                SimulationSurroundMove();
                break;
        }
    }

    public void SimulationPawnMove(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        // attack
        if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
            sc.GetPosition(x + 1, y).GetComponent<Piece>().player != player)
        {
            GameObject cp = sc.GetPosition(x + 1, y);
            if (cp.name == "WhiteKing" || cp.name == "BlackKing")
            {
                Debug.Log("simulation check");
                sc.SetSimCheck(true);
            }
        }

        if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
            sc.GetPosition(x - 1, y).GetComponent<Piece>().player != player)
        {
            GameObject cp = sc.GetPosition(x - 1, y);
            if (cp.name == "WhiteKing" || cp.name == "BlackKing")
            {
                Debug.Log("simulation check");
                sc.SetSimCheck(true);
            }
        }   
    }

    public void SimulationLineMove(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetSimulation(x, y) == null)
        {
            x += xIncrement;
            y += yIncrement;
        }

        // attack
        if (sc.PositionOnBoard(x, y) && sc.GetSimulation(x, y).GetComponent<Piece>().player != player)
        {
            GameObject cp = sc.GetSimulation(x, y);
            if (cp.name == "WhiteKing" || cp.name == "BlackKing")
            {
                Debug.Log("simulation check");
                sc.SetSimCheck(true);
            }
        }
    }

    public void SimulationLMove()
    {
        SimulationPointMove(xBoard + 1, yBoard + 2);
        SimulationPointMove(xBoard - 1, yBoard + 2);
        SimulationPointMove(xBoard + 2, yBoard + 1);
        SimulationPointMove(xBoard + 2, yBoard - 1);
        SimulationPointMove(xBoard + 1, yBoard - 2);
        SimulationPointMove(xBoard - 1, yBoard - 2);
        SimulationPointMove(xBoard - 2, yBoard + 1);
        SimulationPointMove(xBoard - 2, yBoard - 1);
    }

    public void SimulationSurroundMove()
    {
        SimulationPointMove(xBoard + 0, yBoard + 1);
        SimulationPointMove(xBoard + 0, yBoard - 1);
        SimulationPointMove(xBoard - 1, yBoard - 1);
        SimulationPointMove(xBoard - 1, yBoard + 0);
        SimulationPointMove(xBoard - 1, yBoard + 1);
        SimulationPointMove(xBoard + 1, yBoard - 1);
        SimulationPointMove(xBoard + 1, yBoard + 0);
        SimulationPointMove(xBoard + 1, yBoard + 1);
    }

    public void SimulationPointMove(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp != null && cp.GetComponent<Piece>().player != player)
            {
                if (cp.name == "WhiteKing" || cp.name == "BlackKing")
                {
                    Debug.Log("simulation check");
                    sc.SetSimCheck(true);
                }
            }
        }
    }
    #endregion
}
