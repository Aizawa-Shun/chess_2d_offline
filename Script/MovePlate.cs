using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    public Sprite attackMovePlate;

    // For Castling
    GameObject whiteKing = null;
    GameObject blackKing = null;
    GameObject whiteKingsideRook = null;
    GameObject whiteQueensideRook = null;
    GameObject blackKingsideRook = null;
    GameObject blackQueensideRook = null;
    private bool castling = false;

    GameObject reference = null;

    // Board Positions
    int matrixX;
    int matrixY;

    // false:movement, true:attacking
    private bool attack = false;

    // Piece on moveplate
    private bool overlap = false;
    private bool over = false;

    private bool enpassant = false;

    
    void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    public void Start()
    {
        if (attack)
           gameObject.GetComponent<SpriteRenderer>().sprite = attackMovePlate;

        SetPiecesForCastling();
    }

    private void SetPiecesForCastling()
    {
        whiteKing = controller.GetComponent<Game>().GetPosition(4, 0);
        whiteKingsideRook = controller.GetComponent<Game>().GetPosition(7, 0);
        whiteQueensideRook = controller.GetComponent<Game>().GetPosition(0, 0);
        blackKing = controller.GetComponent<Game>().GetPosition(4, 15);
        blackKingsideRook = controller.GetComponent<Game>().GetPosition(7, 15);
        blackQueensideRook = controller.GetComponent<Game>().GetPosition(0, 15);
    }

    void OnMouseDown()
    {
        SetPiece();
    }

    void Update()
    {
        if (reference.GetComponent<Dragger>().set == true && overlap == true)
            SetPiece();
    }

    void OnMouseOver()
    {
        over = true;
    }

    void OnMouseExit()
    {
        over = false;
    }

    public void SetAttack(bool set)
    {
        attack = set;
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Chesspiece") && over == true)
        {
            reference.GetComponent<Dragger>().setPiece = true;
            overlap = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Chesspiece"))
        {
            reference.GetComponent<Dragger>().setPiece = false;
            overlap = false;
        }
    }    

    public void SetPiece()
    {   
        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name == "WhiteKing") controller.GetComponent<Game>().Winner("black");
            if (cp.name == "BlackKing") controller.GetComponent<Game>().Winner("White");

            Destroy(cp);
        }
        
        PawnPromote();
        EnPassant();        
        Castling(reference);

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Piece>().GetXBoard(), reference.GetComponent<Piece>().GetYBoard());

        reference.GetComponent<Piece>().SetXBoard(matrixX);
        reference.GetComponent<Piece>().SetYBoard(matrixY);
        reference.GetComponent<Piece>().SetCoords(); 

        controller.GetComponent<Game>().SetPosition(reference);


        reference.GetComponent<Piece>().DestroyMovePlates();

        reference.GetComponent<Piece>().SetSoundOn(true);

        controller.GetComponent<Game>().SetNextTurn();

        JudgeAttackOpponentKing();

        controller.GetComponent<Game>().CheckmateSimulation();

        controller.GetComponent<Game>().Checkmate();
        controller.GetComponent<Game>().Stalemate();

        // Record
        if (castling == false)
            Debug.Log(InitialPieceCharacter(reference.name) + Take(attack) + Rank(matrixX) + File(matrixY));
        else
            castling = false;

        Reset();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }

    private void JudgeAttackOpponentKing()
    {
        reference.GetComponent<Piece>().Simulation(reference);
        if (controller.GetComponent<Game>().GetSimCheck())
            controller.GetComponent<Game>().setCheck(true);
    }

    #region Castling
    private void Castling(GameObject obj)
    {
        ActCastling(obj);
        CanCastling(obj);
    }

    private void CanCastling(GameObject piece)
    {
        Game cp = controller.GetComponent<Game>();

        // white
        if (piece == whiteKing)
        {
            cp.canWhiteKingSideCastling = false;
            cp.canWhiteQueenSideCastling = false;
        }
        if (piece == whiteKingsideRook)
        {
            cp.canWhiteKingSideCastling = false;
        }
        if (piece == whiteQueensideRook)
        {
            cp.canWhiteQueenSideCastling = false;
        }

        // black
        if (piece == blackKing)
        {
            cp.canBlackKingSideCastling = false;
            cp.canBlackQueenSideCastling = false;
        }
        if (piece == blackKingsideRook)
        {
            cp.canBlackKingSideCastling = false;
        }
        if (piece == blackQueensideRook)
        {
            cp.canBlackQueenSideCastling = false;
        }
    }

    private void ActCastling(GameObject piece)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (piece.name == "WhiteKing" && this.transform.position.x == 6 && controller.GetComponent<Game>().canWhiteKingSideCastling)  // white kingside
        {
            Debug.Log("o-o");
            Destroy(controller.GetComponent<Game>().GetPosition(7, 0));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("WhiteRook", 5, 0));
            castling = true;
        }
        else if (piece.name == "WhiteKing" && this.transform.position.x == 2 && controller.GetComponent<Game>().canWhiteQueenSideCastling) // white queenside
        {
            Debug.Log("o-o-o");
            Destroy(controller.GetComponent<Game>().GetPosition(0, 0));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("WhiteRook", 3, 0));
            castling = true;
        }
        else if (piece.name == "BlackKing" && this.transform.position.x == 6 && controller.GetComponent<Game>().canBlackKingSideCastling) // black kingside
        {
            Debug.Log("o-o");
            Destroy(controller.GetComponent<Game>().GetPosition(7, 15));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("BlackRook", 5, 15));
            castling = true;
        }
        else if (piece.name == "BlackKing" && this.transform.position.x == 2 && controller.GetComponent<Game>().canBlackQueenSideCastling) // black queenside
        {
            Debug.Log("o-o-o");
            Destroy(controller.GetComponent<Game>().GetPosition(0, 15));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("BlackRook", 0, 15));
            castling = true;
        }
    }
    #endregion

    private void PawnPromote()
    {
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white" && matrixY == 15 && reference.name == "WhitePawn")
        {
            Destroy(reference);
            controller.GetComponent<Game>().PawnPromoted(matrixX, matrixY);
        }
        else if (controller.GetComponent<Game>().GetCurrentPlayer() == "black" && matrixY == 0 && reference.name == "BlackPawn")
        {
            Destroy(reference);
            controller.GetComponent<Game>().PawnPromoted(matrixX, matrixY);
        }
    }

    private void EnPassant()
    {
        // White can EnPassant
        if (reference.GetComponent<Piece>().judgeEnPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "black" && matrixY < 13)
        {
            if (controller.GetComponent<Game>().PositionOnBoard(matrixX + 1, matrixY) == true && controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY) != null)
            {
                GameObject rcp = controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY);
                if (rcp.name == "WhitePawn")
                {
                    rcp.GetComponent<Piece>().canRightSideEnPassant = true;
                }
            }

            if (controller.GetComponent<Game>().PositionOnBoard(matrixX - 1, matrixY) == true && controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY) != null)
            {
                GameObject lcp = controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY);
                if (lcp.name == "WhitePawn")
                {
                    lcp.GetComponent<Piece>().canLeftSideEnPassant = true;
                }
            }
        }
        // Black can EnPassant
        if (reference.GetComponent<Piece>().judgeEnPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "white" && matrixY > 2)
        {
            if (controller.GetComponent<Game>().PositionOnBoard(matrixX + 1, matrixY) == true && controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY) != null)
            {
                GameObject rcp = controller.GetComponent<Game>().GetPosition(matrixX + 1, matrixY);
                if (rcp.name == "BlackPawn")
                {
                    rcp.GetComponent<Piece>().canRightSideEnPassant = true;
                }
            }

            if (controller.GetComponent<Game>().PositionOnBoard(matrixX - 1, matrixY) == true && controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY) != null)
            {
                GameObject lcp = controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY);
                if (lcp.name == "BlackPawn")
                {
                    lcp.GetComponent<Piece>().canLeftSideEnPassant = true;
                }
            }
        }

        // White EnPassant
        if (reference.GetComponent<Piece>().enPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "white")
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY - 1);

            if (reference != cp)
                Destroy(cp);

            // Reset canEnPassant
            if (reference.GetComponent<Piece>().canRightSideEnPassant == true)
                reference.GetComponent<Piece>().canRightSideEnPassant = false;
            if (reference.GetComponent<Piece>().canLeftSideEnPassant == true)
                reference.GetComponent<Piece>().canLeftSideEnPassant = false;

            enpassant = true;
        }
        // black EnPassant
        if (reference.GetComponent<Piece>().enPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "black")
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY + 1);

            if (reference != cp)
                Destroy(cp);

            // Reset canEnPassant
            if (reference.GetComponent<Piece>().canRightSideEnPassant == true)
                reference.GetComponent<Piece>().canRightSideEnPassant = false;
            if (reference.GetComponent<Piece>().canLeftSideEnPassant == true)
                reference.GetComponent<Piece>().canLeftSideEnPassant = false;

             enpassant = true;
        }
    }

    #region Record
    private string Rank(int x)
    {
        string[] file = {"a","b","c","d","e","f","g","h"};
        return file[x];
    }

    private int File(int y)
    {
        return y + 1;
    }

    private string InitialPieceCharacter(string pieceName)
    {
        if (pieceName == "WhitePawn" || pieceName == "BlackPawn")
        {
            return "";
        }
        if (pieceName == "WhiteBishop" || pieceName == "BlackBishop")
        {
            return "B";
        }
        if (pieceName == "WhiteKnight" || pieceName == "BlackKnight")
        {
            return "K";
        }
        if (pieceName == "WhiteRook" || pieceName == "BlackRook")
        {
            return "R";
        }
        if (pieceName == "WhiteQueen" || pieceName == "BlackQueen")
        {
            return "Q";
        }
        if (pieceName == "WhiteKing" || pieceName == "BlackKing")
        {
            return "K";
        }
        else
        {
            return null;
        }
    }

    private string Take(bool attack)
    {
        if (attack == true || enpassant == true)
        {
            enpassant = false;
            return "x";
        }

        else
            return null;
    }
    #endregion

    private void Reset()
    {
        controller.GetComponent<Game>().SetSimCheck(false);
        controller.GetComponent<Game>().setCheck(false);
        reference.GetComponent<Dragger>().set = false;
        overlap = false;   
    }
}