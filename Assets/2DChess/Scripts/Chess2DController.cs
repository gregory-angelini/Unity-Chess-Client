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
    [SerializeField] Transform HUDParent;
    [SerializeField] HighlightSquare squarePrefab;
    HighlightSquare[,] squares = new HighlightSquare[8, 8];
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

    async void Start()
    {
        DragAndDropController.Instance.OnStartDragFigure += OnStartDragFigure;
        DragAndDropController.Instance.OnEndDragFigure += OnEndDragFigure;

        CreateSquares();

        Chess = new Chess(ClientController.Instance.GameInfo.FEN);
        Board2DBuilder.Instance.Build(Chess);


        //System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

        if (Chess.GetCurrentPlayerColor().ToString() == ClientController.Instance.PlayerColor)
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

        InvokeRepeating("RefreshGame", refreshHz, refreshHz); // we have to do it in case of reconnection (we'are white and made the last move)
    }

    string GetOpponentColor()
    {
        if (ClientController.Instance.PlayerColor == "white")
            return "black";
        
        if (ClientController.Instance.PlayerColor == "black")
            return "white";

        return "";
    }
    void RefreshGame()
    {
        //Debug.Log("RefreshGame");

        ClientController.Instance.UpdateGameState((result) =>
        {
            if (!string.IsNullOrEmpty(result.lastMoveColor) && result.lastMoveColor != ClientController.Instance.PlayerColor)// our turn begins
            {
                Chess = new Chess(ClientController.Instance.GameState.FEN);
                Board2DBuilder.Instance.UpdateBoard();

                HideMoves();
                ShowLegalFigures();
                ShowMove(result.lastMove);

                CancelInvoke("RefreshGame");
            }
            else
            {
                HideMoves();
                ShowMove(ClientController.Instance.GameState.lastMove);
            }
        });
    }

    void ShowLegalFigures()
    {
        if (!ShowTips) 
            return;

        HideMoves();
        int x, y;

        foreach(string move in Chess.YieldAllMoves())
        {
            Chess.SquareNameToSquarePos(move.Substring(1, 2), out x, out y);
            ShowLegalSquare(x, y, true);
        }
    }

    void ShowMove(string fenMove)
    {
        if (string.IsNullOrEmpty(fenMove)) 
            return;

        string from = fenMove.Substring(1, 2);
        string to = fenMove.Substring(3, 2);
        int x1, y1, x2, y2;

        Chess.SquareNameToSquarePos(from, out x1, out y1);
        Chess.SquareNameToSquarePos(to, out x2, out y2);

        ShowLegalSquare(x1, y1, true);
        ShowLegalSquare(x2, y2, true);
    }

    void ShowLegalMoves(Figure figure, int figureX, int figureY)
    {
        if (!ShowTips) return;
        //Debug.Log($"{figure} in pos [{figureX}, {figureY}]");

        HideMoves();
        int x, y;

        foreach (string move in Chess.YieldAllMoves())
        {
            if ((char)figure == move.Substring(0, 1)[0])// the same figure
            {
                Chess.SquareNameToSquarePos(move.Substring(1, 2), out x, out y);
               
                if (figureX == x && figureY == y)// the same position
                {
                    Chess.SquareNameToSquarePos(move.Substring(3, 2), out x, out y);

                    if (Chess.FigureAt(x, y) != Figure.none)// target square is not empty
                    {
                        ShowLegalSquare(x, y, false);
                    }
                    else
                    {
                        bool enPassant = false;
                        int epX, epY;
                        if (Chess.IsEnPassant(out epX, out epY))
                        {
                            enPassant = x == epX && y == epY;
                        }
                        ShowLegalSquare(x, y, !enPassant);
                    }
                }
            }
        }
    }

    void CreateSquares()
    {
        HighlightSquare square;

        for (int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                square = Instantiate(squarePrefab);
                square.transform.SetParent(HUDParent, false);

                square.Hide();

                Vector3 pos = Board2DBuilder.Instance.BoardStartPos + new Vector2(x * Board2DBuilder.Instance.SquareSize.x, y * Board2DBuilder.Instance.SquareSize.y);
                square.SetWorldPosition(pos);
                squares[x, y] = square;
            }
        }
    }
     
    void ShowLegalSquare(int x, int y, bool emptyOrEnemy)
    {
        if (emptyOrEnemy) 
            squares[x, y].ShowEmptySquare();
        else 
            squares[x, y].ShowEnemySquare();
    }
     
    void HideMoves()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                squares[x, y].Hide();
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
                    popUp.Run(Chess.GetCurrentPlayerColor(), (Figure figure) =>
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
                resultArgs.player = Chess.GetCurrentPlayerColor();
                resultArgs.checkmate = Chess.IsCheckmate();
                resultArgs.check = Chess.IsCheck();
                resultArgs.stalemate = Chess.IsStalemate();

                if (resultArgs.checkmate)
                    Debug.Log($"{Chess.GetCurrentPlayerColor()} player in checkmate");
                else if (resultArgs.stalemate)
                    Debug.Log($"{Chess.GetCurrentPlayerColor()} player in stalemate");
                else if (resultArgs.check)
                    Debug.Log($"{Chess.GetCurrentPlayerColor()} player in check");

                HideMoves();
                ShowMove(result.lastMove);

                InvokeRepeating("RefreshGame", refreshHz, refreshHz);// the opponent's turn begins
                OnMoveResult?.Invoke(this, resultArgs);
            });
        }
        else // illegal move
        {
            HideMoves();
            if (Chess.GetCurrentPlayerColor().ToString() == ClientController.Instance.PlayerColor)
            {
                ShowLegalFigures();
            }
            ShowMove(ClientController.Instance.GameState.lastMove);
        }
    }
}
