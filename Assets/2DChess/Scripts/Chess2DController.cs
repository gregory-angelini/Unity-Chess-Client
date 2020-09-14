using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessCore;
using UnityEditor;
using System;
using PopUp;
using TMPro;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Chess2DController : MonoBehaviour
{
    public class ResultArgs : EventArgs
    {
        public ChessCore.Color player;
        public bool check;
        public bool checkmate;
        public bool stalemate;
    }

    public event EventHandler<ResultArgs> OnMoveResult;

    public Chess Chess { get; private set; }
    public static Chess2DController Instance;
    //[SerializeField] string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
    SynchronizationContext mainSyncContext;
    float refreshHz = 2f;// in seconds
    public bool ShowTips = false;
    [SerializeField] TextMeshProUGUI bottomPlayerName;
    [SerializeField] TextMeshProUGUI topPlayerName;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mainSyncContext = SynchronizationContext.Current;// Context of main thread
    }

    void Start()
    {
        DragAndDropController.Instance.OnStartDragFigure += OnStartDragFigure;
        DragAndDropController.Instance.OnEndDragFigure += OnEndDragFigure;

        Chess = new Chess(ClientController.Instance.GameInfo.FEN);
        Board2DBuilder.Instance.Build(Chess);

        //System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

        if (Chess.GetMoveColor().ToString() == ClientController.Instance.PlayerColor)
        {
            ShowLegalFigures();
        }

        // adapt the board to a player color
        if (ClientController.Instance.PlayerColor == "black")
            Board2DBuilder.Instance.FlipBoard();

        // show player names 
        bottomPlayerName.text = ClientController.Instance.PlayerInfo.playerName;
       
        ClientController.Instance.GetPlayer(GetOpponentColor(), result =>
        {
            topPlayerName.text = result.playerName;
        });

        if((ClientController.Instance.PlayerColor == "black" && Chess.GetMoveColor() == ChessCore.Color.white) ||
           (ClientController.Instance.PlayerColor == "white" && Chess.GetMoveColor() == ChessCore.Color.black))
        InvokeRepeating("RefreshGame", refreshHz, refreshHz);
    }

    public void OnResignButton()
    {
        ClientController.Instance.Resign((result) =>
        {
            Resign(GetOpponentColor());
        });
    }

    void Resign(string playerName)
    {
        Debug.Log($"{Chess.GetMoveColor()} resigned");
        CancelInvoke("RefreshGame");

        MessagePopUp popUp = GuiController.Instance.ShowMessage() as MessagePopUp;

        popUp.Message = playerName + " win";

        popUp.AddButton("Restart", () =>
        {
            popUp.CloseWindow();
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });
    }


    string GetOpponentColor()
    {
        if (ClientController.Instance.PlayerColor == "white")
            return "black";
        
        if (ClientController.Instance.PlayerColor == "black")
            return "white";

        return "";
    }

    IEnumerator Checkmate(string player)
    {
        Debug.Log($"{Chess.GetMoveColor()} player in checkmate");
        CancelInvoke("RefreshGame");

        MessagePopUp popUp = GuiController.Instance.ShowMessage() as MessagePopUp;
        
        popUp.Message = player + " win";

        popUp.AddButton("Restart", () =>
        {
            popUp.CloseWindow();
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });

        yield return new WaitForClosing(popUp); // conditions for stopping the coroutine
    }

    IEnumerator Stalemate()
    {
        Debug.Log($"{Chess.GetMoveColor()} player in stalemate");
        CancelInvoke("RefreshGame");

        MessagePopUp popUp = GuiController.Instance.ShowMessage() as MessagePopUp;

        popUp.Message = "Draw";

        popUp.AddButton("Restart", () =>
        {
            popUp.CloseWindow();
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        });

        yield return new WaitForClosing(popUp); // conditions for stopping the coroutine
    }


    void RefreshGame()
    {
        Debug.Log("RefreshGame");

        ClientController.Instance.UpdateGameState((result) =>
        {
            if (!string.IsNullOrEmpty(result.lastMoveColor) && result.lastMoveColor != ClientController.Instance.PlayerColor)// our turn begins
            {
                Chess = new Chess(ClientController.Instance.GameState.FEN);
                Board2DBuilder.Instance.UpdateBoard();

                Board2DBuilder.Instance.HideMoves();
                ShowLegalFigures(); 
                ShowMove(result.lastMove);
                
                CancelInvoke("RefreshGame");
            }
            else
            {
                Board2DBuilder.Instance.HideMoves();
                ShowMove(ClientController.Instance.GameState.lastMove);
            }

            if (result.status == "completed")
            {
                if (result.result == "checkmate")
                    StartCoroutine(Checkmate(ClientController.Instance.GameState.lastMoveColor));
                else if (result.result == "stalemate")
                    StartCoroutine(Stalemate());
                else if (result.result == "resign")
                    Resign(GetOpponentColor());
            }
            else
            {
                if (Chess.IsCheck())// TODO: server must make this decision
                    Debug.Log($"{Chess.GetMoveColor()} player in check");
            }
        });
    }

    void ShowLegalFigures()
    {
        if (!ShowTips) 
            return;

        Board2DBuilder.Instance.HideMoves();
        int x, y;

        foreach(string move in Chess.YieldAllMoves())
        {
            Chess.SquareNameToSquarePos(move.Substring(1, 2), out x, out y);
            Board2DBuilder.Instance.ShowLegalSquare(x, y, true);
        }
    }

    void ShowMove(string fenMove)
    {
        if (string.IsNullOrEmpty(fenMove)) 
            return;

        if (fenMove == "resign") // TODO: remove this (we store 'result' in fenMove)
            return;

        string from = fenMove.Substring(1, 2);
        string to = fenMove.Substring(3, 2);
        int x1, y1, x2, y2;

        Chess.SquareNameToSquarePos(from, out x1, out y1);
        Chess.SquareNameToSquarePos(to, out x2, out y2);

        Board2DBuilder.Instance.ShowLegalSquare(x1, y1, true);
        Board2DBuilder.Instance.ShowLegalSquare(x2, y2, true);
    }

    void ShowLegalMoves(Figure figure, int figureX, int figureY)
    {
        if (!ShowTips) return;
        //Debug.Log($"{figure} in pos [{figureX}, {figureY}]");

        Board2DBuilder.Instance.HideMoves();
        int x, y;

        foreach (string move in Chess.YieldAllMoves())
        {
            if ((char)figure == move.Substring(0, 1)[0])// the same figure
            {
                Chess.SquareNameToSquarePos(move.Substring(1, 2), out x, out y);
               
                if (figureX == x && figureY == y)// the same position
                {
                    Chess.SquareNameToSquarePos(move.Substring(3, 2), out x, out y);

                    if (Chess.FigureAt(x, y) != Figure.none)
                    {
                        Board2DBuilder.Instance.ShowLegalSquare(x, y, false);
                    }
                    else
                    {
                        int epX, epY;
                        Chess.SquareNameToSquarePos(Chess.GetEnPassant(), out epX, out epY);
                        bool enPassant = x == epX && y == epY && (figure == Figure.whitePawn || figure == Figure.blackPawn);
                        Board2DBuilder.Instance.ShowLegalSquare(x, y, !enPassant);
                    }
                }
            }
        }
    }



    void OnDestroy()
    {
        DragAndDropController.Instance.OnStartDragFigure -= OnStartDragFigure;
        DragAndDropController.Instance.OnEndDragFigure -= OnEndDragFigure;
    }

    void OnStartDragFigure(object source, DragAndDropController.DragArgs args)
    {
        Figure2D figure = args.draggedObject.GetComponent<Figure2D>();
        ShowLegalMoves(figure.Id, figure.XBoardPos, figure.YBoardPos);
    }

    void OnEndDragFigure(object source, DragAndDropController.DragArgs args)
    {
        StartCoroutine(OnEndDragFigure(args));
    }

    IEnumerator OnEndDragFigure(DragAndDropController.DragArgs args)
    {
        if (args.result)
        {
            Debug.Log($"End drag: { args.fenMove }");

            // here we can handle figure promotion by modifying fenMove
            Figure draggedFigure = Chess.FigureAt((int)args.startDragPos.x, (int)args.startDragPos.y);
            if (draggedFigure == Figure.whitePawn || draggedFigure == Figure.blackPawn)
            {
                bool promotion = (int)args.endDragPos.y == 7 || (int)args.endDragPos.y == 0;

                if (promotion)
                {
                    FigurePromotionPopUp popUp = GuiController.Instance.ShowFigurePromotion() as FigurePromotionPopUp;
                    popUp.Run(Chess.GetMoveColor(), (Figure figure) =>
                    {
                        args.fenMove += (char)figure;
                        Debug.Log("fenMove:" + args.fenMove);
                    });
                    yield return new WaitForClosing(popUp); // conditions for stopping the coroutine
                }
            }

            ClientController.Instance.SendMove(args.fenMove, (result) =>
            {
                Chess = new Chess(result.FEN);
                Board2DBuilder.Instance.UpdateBoard();
                Debug.Log($"new state: {Chess.fen}");

                ResultArgs resultArgs = new ResultArgs();
                resultArgs.player = Chess.GetMoveColor();
                resultArgs.checkmate = Chess.IsCheckmate();
                resultArgs.check = Chess.IsCheck();
                resultArgs.stalemate = Chess.IsStalemate();

                /*if (resultArgs.checkmate)
                    Debug.Log($"{Chess.GetMoveColor()} player in checkmate");
                else if (resultArgs.stalemate)
                    Debug.Log($"{Chess.GetMoveColor()} player in stalemate");
                else if (resultArgs.check)
                    Debug.Log($"{Chess.GetMoveColor()} player in check");
                */

                Board2DBuilder.Instance.HideMoves();
                ShowMove(result.lastMove);

                InvokeRepeating("RefreshGame", refreshHz, refreshHz);// the opponent's turn begins
                
                OnMoveResult?.Invoke(this, resultArgs);
            });
        }
        else // illegal move
        {
            Board2DBuilder.Instance.HideMoves();
            if (Chess.GetMoveColor().ToString() == ClientController.Instance.PlayerColor)
            {
                ShowLegalFigures();
            }
            ShowMove(ClientController.Instance.GameState.lastMove);
        }
    }
}
