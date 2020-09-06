using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessClient;
using System.Threading;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;


public class ClientController : MonoBehaviour
{
    [SerializeField] string host = "http://localhost:44334/api/";


    [HideInInspector]
    public string PlayerColor { get; private set; } = "";

    Client Client;

    [HideInInspector]
    public GameInfo GameInfo { get; private set; }

    [HideInInspector]
    public GameState GameState { get; private set; } 

    [HideInInspector]
    public PlayerInfo PlayerInfo { get; private set; }


    SynchronizationContext mainSyncContext;
    public static ClientController Instance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        mainSyncContext = SynchronizationContext.Current;// Context of main thread
    }

    void Start()
    {
        Client = new Client(host);
        
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        Player player = new Player() { GUID = deviceId, Name = "testname1" };

        AuthenticatePlayer(player, (result) =>
        {
            PlayerInfo = result;
            LobbyController.Instance.SetPlayerName(result.playerName);
        });
    }

    async void AuthenticatePlayer(Player player, Action<PlayerInfo> callback)
    {
        await Client.GetPlayer(player, (result) =>
        {
            mainSyncContext.Post(s =>// runs the following code on the main thread
            {
                Debug.Log(JsonConvert.SerializeObject(result));
                callback?.Invoke(result);
            }, null);
        });
    }

    public async void StartNewGame(RequestedGame game, Action<GameInfo> callback)
    {
        await Client.FindGame(game, (result) =>
        {
            mainSyncContext.Post(s =>// runs the following code on the main thread
            {
                PlayerColor = game.playerColor;
                GameInfo = result;
                Debug.Log(JsonConvert.SerializeObject(result));
                callback?.Invoke(result);
            }, null);
        });
    }

    public async void UpdateGameState(Action<GameState> callback)
    {
        await Client.GetGame(GameInfo.gameID, (result) =>
        {
            mainSyncContext.Post(s =>// runs the following code on the main thread
            {
                GameState = result;
                
                if(GameState.status == "play")
                {
                    Debug.Log(JsonConvert.SerializeObject(result));
                    callback?.Invoke(result);
                }
            }, null);
        });
    }

    public async void SendMove(string fenMove, Action<GameState> callback)
    {
        MoveInfo moveInfo = new MoveInfo() { gameID = GameInfo.gameID, fenMove = fenMove, playerID = PlayerInfo.playerID };

        await Client.SendMove(moveInfo, (result) =>
        {
            mainSyncContext.Post(s =>// runs the following code on the main thread
            {
                GameState = result;
                Debug.Log(JsonConvert.SerializeObject(result));
                callback?.Invoke(result);
            }, null);
        });
    }
}
