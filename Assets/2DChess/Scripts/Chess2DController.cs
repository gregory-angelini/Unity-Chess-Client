using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessCore;
using UnityEditor;
using System;
using PopUp;

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
        ShowLegalFigures();

        // TODO: need remove this
        //System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

        //InvokeRepeating("RefreshGame", refreshHz, refreshHz);
    }

    async void RefreshGame()
    {
        /*
        await ClientController.Instance.Client.GetGame(ClientController.Instance.GameInfo.gameID, (result) =>
        {
            mainSyncContext.Post(s =>// runs the following code on the main thread
            {
                Debug.Log("RefreshGame");
                ClientController.Instance.GameState = result;
                Chess = new Chess(ClientController.Instance.GameState.FEN);

                Board2DBuilder.Instance.UpdateBoard();
                ShowLegalFigures();
            }, null);
        });
        */
    }

    void ShowLegalFigures()
    {
        HideMoves();
        int x, y;

        foreach(string move in Chess.YieldAllMoves())
        {
            Chess.SquareNameToSquarePos(move.Substring(1, 2), out x, out y);
            ShowLegalSquare(x, y, true);
        }
    }
    public void ShowLegalMoves(Figure figure, int figureX, int figureY)
    {
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

                ShowLegalFigures();
                OnMoveResult?.Invoke(this, resultArgs);
            });
        }
        else // illegal move
            ShowLegalFigures();
    }
}
