using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piece : MonoBehaviour
{
    // Reference
    public GameObject controller;
    public GameObject movePlate;

    // Simulation
    private GameObject SimPiece; 

    // Positions
    private int xBoard = -1;
    private int yBoard = -1;

    // Simulation Positions
    private int xSimBoard = -1;
    private int ySimBoard = -1;

    // Variable to keep track of "black" or "white" player
    private string player;

    // Reference for all the sprites that the chesspiece can be
    public Sprite WhitePawn, WhiteBishop, WhiteKnight, WhiteRook, WhiteQueen, WhiteKing;
    public Sprite BlackPawn, BlackBishop, BlackKnight, BlackRook, BlackQueen, BlackKing;
    
    // Sound
    private AudioSource moveSound;
    public bool soundOn = false;

    // Enpassant
    public bool canRightSideEnPassant = false;
    public bool canLeftSideEnPassant = false;
    public bool judgeEnPassant = false;
    public bool enPassant = false;


    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        // Take the instantiated location and adjust the transform
        SetCoords();
        // Set Renderer and team coler
        SetRenderer();
    }

    private void SetRenderer()
    {
        switch (this.name)
        {
            case "WhitePawn": this.GetComponent<SpriteRenderer>().sprite = WhitePawn; player = "white";  break;
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

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
        }
    }

    public void SetCoords() 
    {
        float x = xBoard;
        float y = yBoard;

        this.transform.position = new Vector3(x, y, -1.0f);       
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public int GetXSimBoard()
    {
        return xSimBoard;
    }

    public int GetYSimBoard()
    {
        return ySimBoard;
    }

    public void SetSimXY(int x, int y)
    {
        xSimBoard = x;
        ySimBoard = y;
    }

    public string GetTeam()
    {
        return player;
    }

    private void OnMouseDown()
    {   
        string currentPlayer = controller.GetComponent<Game>().GetCurrentPlayer();

        if (!controller.GetComponent<Game>().IsGameOver() && currentPlayer == player && controller.GetComponent<Game>().TurnManager(player))
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
            // Pawn movement
            case "WhitePawn":
                PawnMovePlate(xBoard, yBoard + 1);
                EnPassantMovePlate(xBoard, yBoard);
                break;
            case "BlackPawn":
                PawnMovePlate(xBoard, yBoard - 1);
                EnPassantMovePlate(xBoard, yBoard);
                break;

            // Bishop movement
            case "WhiteBishop":
            case "BlackBishop":
                LineMovePlate( 1, 1);
                LineMovePlate( 1,-1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1,-1);
                break;

            // Knight movement
            case "WhiteKnight":
            case "BlackKnight":
                LMovePlate();
                break;

            // Rook movement
            case "WhiteRook":
            case "BlackRook":
                LineMovePlate( 1, 0);
                LineMovePlate( 0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate( 0,-1);
                break;  

            // Queen movement
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

            // King movement
            case "WhiteKing":
            case "BlackKing":
                SurroundMovePlate();
                KingSideCastlingMoveplate();
                QueenSideCastlingMovePlate();
                break;
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        // Get Component
        Game sc = controller.GetComponent<Game>();

        // Copy the current board to the simulation board
        // Set SimPiece
        PrepareSimulation(x, y);

        if (sc.PositionOnBoard(x, y))
        {
            // Only first move
            if (player == "white" && y == 2)
            {
                while (sc.GetPosition(x, y) == null && y < 5)
                {
                    sc.SetSimulation(SimPiece);
                    sc.OpponentPieceSimulation();

                    if (sc.GetSimCheck() == false)
                        if (sc.IniSimulation() == true)
                            sc.SetCheckmate(false);
                        else if (sc.IniSimulation() == false)
                            MovePlateSpawn(x, y);
                            
                    sc.SetSimulationEmpty(x, y);

                    y++;
                    judgeEnPassant = true;

                    SetSimXY(x, y);
                    sc.SetSimCheck(false);
                }
            }

            if (player == "black" && y == 13)
            {
                while (sc.GetPosition(x, y) == null && y > 10)
                {
                    sc.SetSimulation(SimPiece);
                    sc.OpponentPieceSimulation();

                    if (sc.GetSimCheck() == false)
                        if (sc.IniSimulation() == true)
                            sc.SetCheckmate(false);
                        else if (sc.IniSimulation() == false)
                            MovePlateSpawn(x, y);

                    sc.SetSimulationEmpty(x, y);
                    
                    y--;
                    judgeEnPassant = true;

                    SetSimXY(x, y);
                    sc.SetSimCheck(false);
                }
            }

            if (sc.GetPosition(x, y) == null)
            {
                sc.SetSimulation(SimPiece);
                sc.OpponentPieceSimulation();

                        if (sc.GetSimCheck() == false)
                            if (sc.IniSimulation() == true)
                                sc.SetCheckmate(false);
                            else if (sc.IniSimulation() == false)
                                MovePlateSpawn(x, y);

                sc.SetSimulationEmpty(x, y);
                sc.SetSimCheck(false);
            }

            if (player == "white" && y != 2)
                y = yBoard + 1;
            else if (player == "black" && y != 13)
                y = yBoard - 1;

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
        } 
    }

    public void EnPassantMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        PrepareSimulation(x, y);
    
        // white
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

        // black
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

            sc.OpponentPieceSimulation();
            
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

        // Attack
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
                sc.OpponentPieceSimulation();

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
    
    public void KingSideCastlingMoveplate()
    {
        Game sc = controller.GetComponent<Game>(); 

        if (player == "white" && sc.GetPosition(5, 0) == null && sc.GetPosition(6, 0) == null && sc.canWhiteKingSideCastling == true) 
        {
            PrepareSimulation(6, 0);
            sc.SetSimulation(SimPiece);
            sc.OpponentPieceSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(6, 0);

            sc.SetSimCheck(false);       
        }

        if (player == "black" && sc.GetPosition(5, 15) == null && sc.GetPosition(6, 15) == null && sc.canBlackKingSideCastling == true)
        {
            PrepareSimulation(6, 15);
            sc.SetSimulation(SimPiece);
            sc.OpponentPieceSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(6, 15);

            sc.SetSimCheck(false);
        }
    }

    public void QueenSideCastlingMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        if (player == "white" && sc.GetPosition(1, 0) == null && sc.GetPosition(2, 0) == null && sc.GetPosition(3, 0) == null && sc.canWhiteQueenSideCastling == true) 
        {
            PrepareSimulation(2, 0);
            sc.SetSimulation(SimPiece);
            sc.OpponentPieceSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(2, 0);

            sc.SetSimCheck(false);
        }

        if (player == "black" && sc.GetPosition(1, 15) == null && sc.GetPosition(2, 15) == null && sc.GetPosition(3, 15) == null && sc.canBlackQueenSideCastling == true)
        {   
            PrepareSimulation(2, 15);
            sc.SetSimulation(SimPiece);
            sc.OpponentPieceSimulation();

            if (sc.GetSimCheck() == false)
                if (sc.IniSimulation() == true)
                    sc.SetCheckmate(false);
                else if (sc.IniSimulation() == false)
                    MovePlateSpawn(2, 15);

            sc.SetSimCheck(false);
        }
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

    private void PrepareSimulation(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        // Copy the current board to the simulation board
        for (int x0 = 0; x0 < 8; x0++)
            for (int y0 = 0; y0 < 16; y0++)
                if (sc.GetPosition(x0, y0) != null && sc.PositionOnBoard(x0, y0))
                    sc.IniSetSimulation(sc.GetPosition(x0, y0));
                else
                    sc.SetSimulationEmpty(x0, y0);

        SimPiece = sc.GetPosition(xBoard, yBoard);

        // Deleate current position on the simulation board
        sc.SetSimulationEmpty(xBoard, yBoard);

        if (sc.PositionOnSimBoard(x ,y))
        {
            xSimBoard = x;
            ySimBoard = y;
        }
    }

    private void AttackSimulation(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        sc.SetSimAttackPiece(sc.GetPosition(x, y));
        SetSimXY(x, y);
        sc.SetSimulationEmpty(x, y);
        sc.SetSimulation(SimPiece);
        sc.OpponentPieceSimulation();
    }

    public void Simulation(GameObject piece)
    {
        switch (piece.name)
        {
            // Pawn movement simulation
            case "WhitePawn":
                SimulationPawnMove(xBoard, yBoard + 1);
                break;
            case "BlackPawn":
                SimulationPawnMove(xBoard, yBoard - 1);
                break;

            // Bishop movement simulation
            case "WhiteBishop":
            case "BlackBishop":
                SimulationLineMove( 1, 1);
                SimulationLineMove( 1,-1);
                SimulationLineMove(-1, 1);
                SimulationLineMove(-1,-1);
                break;

            // Knight movement simulation
            case "WhiteKnight":
            case "BlackKnight":
                SimulationLMove();
                break;

            // Rook movement simulation
            case "WhiteRook":
            case "BlackRook":
                SimulationLineMove( 1, 0);
                SimulationLineMove( 0, 1);
                SimulationLineMove(-1, 0);
                SimulationLineMove( 0,-1);
                break;  

            // Queen movement simulation
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

            // King movement simulation
            case "WhiteKing":
            case "BlackKing":
                SimulationSurroundMove();
                break;
        }
    }

    private void SimulationPawnMove(int x, int y)
    {
        // Get Component
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            // Only first move
            if (player == "white" && y == 2)
            {
                while (sc.GetPosition(x, y) == null && y < 5)
                    {
                        y++;
                    }
            }

            if (player == "black" && y == 13)
            {
                while (sc.GetPosition(x, y) == null && y > 10)
                    {
                        y--;
                    }
            }

            if (player == "white" && y != 2)
            {
                y = yBoard + 1;
            }
            else if (player == "black" && y != 13)
            {
                y = yBoard - 1;
            }

            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Piece>().player != player)
            {
                GameObject cp = controller.GetComponent<Game>().GetPosition(x + 1, y);

                if (cp.name == "WhiteKing" || cp.name == "BlackKing")
                {
                    Debug.Log("simulate check");
                    sc.SetSimCheck(true);
                }
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Piece>().player != player)
            {
                GameObject cp = controller.GetComponent<Game>().GetPosition(x - 1, y);

                if (cp.name == "WhiteKing" || cp.name == "BlackKing")
                {
                    Debug.Log("simulate check");
                    sc.SetSimCheck(true);
                }
            }
        } 
    }

    private void SimulationLineMove(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetSimulation(x, y) == null)
        {
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetSimulation(x, y).GetComponent<Piece>().player != player)
        {
            GameObject cp = controller.GetComponent<Game>().GetSimulation(x, y);

            if (cp.name == sc.GetMyKing().name)
            {
                Debug.Log("simulate check");
                sc.SetSimCheck(true);
            }
        }
    }

    private void SimulationLMove()
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

    private void SimulationPointMove(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp != null && cp.GetComponent<Piece>().player != player)
            {
                if (cp.name == "WhiteKing" || cp.name == "BlackKing")
                {
                    Debug.Log("simulate check");
                    sc.SetSimCheck(true);
                }
            }
        }
    }

    public void SetSoundOn(bool set)
    {
        soundOn = set;
    }
}