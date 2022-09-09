using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    private GameObject reference;

    public Sprite attackMovePlate;

    private int matrixX;
    private int matrixY;

    private bool attack;

    private bool judgeEnPassant = false;

    // castling
    GameObject whiteKing = null;
    GameObject blackKing = null;
    GameObject whiteKingSideRook = null;
    GameObject whiteQueenSideRook = null;
    GameObject blackKingSideRook = null;
    GameObject blackQueenSideRook = null;
    private bool castling = false;

    // drag
    private bool overlap = false;
    private bool over = false;

    // enPassant
    private bool enPassant = false;

    // record
    private string preFile;


    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack == true)
           gameObject.GetComponent<SpriteRenderer>().sprite = attackMovePlate;

        SetPieceForCastling();
    }

    private void OnMouseDown()
    {
        SetPiece();
    }

    #region drag
    private void Update()
    {
        if (reference.GetComponent<Dragger>().set == true && overlap == true)
            SetPiece();
    }

    private void OnMouseOver()
    {
        over = true;
    }

    private void OnMouseExit()
    {
        over = false;
    }

    private void OnTriggerStay2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Chesspiece") && over == true)
        {
            reference.GetComponent<Dragger>().setPiece = true;
            overlap = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider2D)
    {
           if (collider2D.CompareTag("Chesspiece"))
        {
            reference.GetComponent<Dragger>().setPiece = false;
            overlap = false;
        }
    }
    #endregion

    public void SetAttack(bool set)
    {
        attack = set;
    }

    public void SetPiece()
    {
        if (attack == true)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name == "WhiteKing") controller.GetComponent<Game>().Winner("black");
            if (cp.name == "BlackKing") controller.GetComponent<Game>().Winner("white");

            Destroy(cp);
        }

        EnPassant();
        PawnPromote();
        Castling(reference);

        preFile = File(reference.GetComponent<Piece>().GetXBoard());

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Piece>().GetXBoard(), reference.GetComponent<Piece>().GetYBoard());

        reference.GetComponent<Piece>().SetXBoard(matrixX);
        reference.GetComponent<Piece>().SetYBoard(matrixY);
        reference.GetComponent<Piece>().SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        reference.GetComponent<Piece>().SetSoundOn(true);

        reference.GetComponent<Piece>().DestroyMovePlates();

        controller.GetComponent<Game>().NextTurn();

        // simulation
        controller.GetComponent<Game>().Synchronization();
        JudgeCheck();
        controller.GetComponent<Game>().MateSimulation();

        // record
        if (castling == false)
            Debug.Log(InitialPieceCharacter(reference.name) + Take(attack) + File(matrixX) + Rank(matrixY));
        else
        {
            if (this.transform.position.x == 6)
                Debug.Log("o-o");
            else if (this.transform.position.x == 2)
                Debug.Log("o-o-o");
        }

        Reset();
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    #region EnPassant
    private void EnPassant()
    {
        // white can EnPassant
        if (judgeEnPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "black" && matrixY < 5)
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
                GameObject rcp = controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY);
                if (rcp.name == "WhitePawn")
                {
                    rcp.GetComponent<Piece>().canLeftSideEnPassant = true;
                }
            }
        }

        // black can EnPassant
        if (judgeEnPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "white" && matrixY < 2)
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
                GameObject rcp = controller.GetComponent<Game>().GetPosition(matrixX - 1, matrixY);
                if (rcp.name == "BlackPawn")
                {
                    rcp.GetComponent<Piece>().canLeftSideEnPassant = true;
                }
            }
        }

        // white EnPassant
        if (reference.GetComponent<Piece>().enPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "white")
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY - 1);

            if (reference != cp)
               Destroy(cp);

            // reset canEnPassant
            if (reference.GetComponent<Piece>().canRightSideEnPassant == true)
               reference.GetComponent<Piece>().canRightSideEnPassant = false;
            if (reference.GetComponent<Piece>().canLeftSideEnPassant == true)
               reference.GetComponent<Piece>().canLeftSideEnPassant = false;

            enPassant = true;
        }

        // black EnPassant
        if (reference.GetComponent<Piece>().enPassant == true && controller.GetComponent<Game>().GetCurrentPlayer() == "black")
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY + 1);

            if (reference != cp)
               Destroy(cp);

            // reset canEnPassant
            if (reference.GetComponent<Piece>().canRightSideEnPassant == true)
               reference.GetComponent<Piece>().canRightSideEnPassant = false;
            if (reference.GetComponent<Piece>().canLeftSideEnPassant == true)
               reference.GetComponent<Piece>().canLeftSideEnPassant = false;

            enPassant = true;
        }
    }

    public void SetJudgeEnPassant(bool set)
    {
        judgeEnPassant = set;
    }
    #endregion


    private void PawnPromote()
    {
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white" && matrixY == 7 && reference.name == "WhitePawn")
        {
            Destroy(reference);
            controller.GetComponent<Game>().PawnPromoted(matrixX, matrixY);
        }
        else if (controller.GetComponent<Game>().GetCurrentPlayer() == "Black" && matrixY == 0 && reference.name == "BlackPawn")
        {
            Destroy(reference);
            controller.GetComponent<Game>().PawnPromoted(matrixX, matrixY);
        }
    }


    #region castling
    private void Castling(GameObject obj)
    {
        ActCastling(obj);
        CanCastling(obj);
    }

    private void ActCastling(GameObject piece)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        
        // white castling
        if (piece.name == "WhiteKing" && this.transform.position.x == 6 && controller.GetComponent<Game>().canWhiteKingSideCastling == true)
        {
            Destroy(controller.GetComponent<Game>().GetPosition(7, 0));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("WhiteRook", 5, 0));
            castling = true;
        }
        else if (piece.name == "WhiteKing" && this.transform.position.x == 2 && controller.GetComponent<Game>().canWhiteQueenSideCastling == true)
        {
            Destroy(controller.GetComponent<Game>().GetPosition(0, 0));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("WhiteRook", 3, 0));
            castling = true;
        }
        // black castling
        if (piece.name == "BlackKing" && this.transform.position.x == 6 && controller.GetComponent<Game>().canBlackKingSideCastling == true)
        {
            Destroy(controller.GetComponent<Game>().GetPosition(7, 7));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("BlackRook", 5, 7));
            castling = true;
        }
        else if (piece.name == "BlackKing" && this.transform.position.x == 2 && controller.GetComponent<Game>().canBlackQueenSideCastling == true)
        {
            Destroy(controller.GetComponent<Game>().GetPosition(0, 7));
            controller.GetComponent<Game>().SetPosition(controller.GetComponent<Game>().Create("BlackRook", 3, 7));
            castling = true;
        }
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
        else if (piece == whiteKingSideRook)
        {
            cp.canWhiteKingSideCastling = false;
        }
        else if (piece == whiteQueenSideRook)
        {
            cp.canWhiteQueenSideCastling = false;
        }
        // black
        if (piece == blackKing)
        {
            cp.canBlackKingSideCastling = false;
            cp.canBlackQueenSideCastling = false;
        }
        else if (piece == blackKingSideRook)
        {
            cp.canBlackKingSideCastling = false;
        }
        else if (piece == blackQueenSideRook)
        {
            cp.canBlackQueenSideCastling = false;
        }
    }

    private void SetPieceForCastling()
    {
        whiteKing = controller.GetComponent<Game>().GetPosition(4, 0);
        whiteKingSideRook = controller.GetComponent<Game>().GetPosition(7, 0);
        whiteQueenSideRook = controller.GetComponent<Game>().GetPosition(0, 0);
        blackKing = controller.GetComponent<Game>().GetPosition(4, 7);
        blackKingSideRook = controller.GetComponent<Game>().GetPosition(7, 7);
        blackQueenSideRook = controller.GetComponent<Game>().GetPosition(0, 7);
    }
    #endregion

    #region record
    private string InitialPieceCharacter(string name)
    {
        if (name == "WhitePawn" || name == "BlackPawn")
        {
            if (attack == true || enPassant == true)
            {
                return preFile;
            }
            else
            {
                return null;
            }
        }
        else if (name == "WhiteBishop" || name == "BlackBishop")
        {
            return "B";
        }
        else if (name == "WhiteKnight" || name == "BlackKnight")
        {
            return "N";
        }
        else if (name == "WhiteRook" || name == "BlackRook")
        {
            return "R";
        }
        else if (name == "WhiteQueen" || name == "BlackQueen")
        {
            return "Q";
        }
        else if (name == "WhiteKing" || name == "BlackKing")
        {
            return "K";
        }
        else
            return null;
    }

    private string Take(bool attack)
    {
        if (attack == true || enPassant == true)
            return "x";
        else 
            return null;
    }

    private string File(int x)
    {
        string[] file = {"a", "b", "c", "d", "e", "f", "g", "h"};
        return file[x];
    }

    private int Rank(int y)
    {
        return y + 1;
    }
    #endregion

    private void JudgeCheck()
    {
        reference.GetComponent<Piece>().Simulation(reference);
        if (controller.GetComponent<Game>().GetSimCheck() == true)
        {
            controller.GetComponent<Game>().SetCheck(true);
            Debug.Log("check");
        }
    }

    private void Reset()
    {
        controller.GetComponent<Game>().SetSimCheck(false);
        controller.GetComponent<Game>().SetCheck(false);
        reference.GetComponent<Dragger>().set = false;
        overlap = false;
        enPassant = false;
    }
}
