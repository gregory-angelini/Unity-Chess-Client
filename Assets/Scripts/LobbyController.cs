using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using ChessClient;
using Newtonsoft.Json;


public class LobbyController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerColor;
    public static LobbyController Instance;
    [SerializeField] TMP_InputField playerName;
    public string PlayerName { get => playerName.text; set => playerName.text = value; }


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playerName.characterLimit = 20;
    }

    public void OnPlayButton()
    {
        if (playerName.text.Length > 0 && playerColor.text.Length > 0)// TODO: show ui message to inform the player
        {
            ClientController.Instance.AuthenticatePlayer(PlayerName, () =>
            {
                PlayerName = ClientController.Instance.PlayerInfo.playerName;

                RequestedGame game = new RequestedGame();
                game.playerID = ClientController.Instance.PlayerInfo.playerID;
                game.playerColor = playerColor.text.ToLower();

                // TODO: update player name on server side

                ClientController.Instance.StartNewGame(game, (result) =>
                {
                    SceneManager.LoadScene("WaitingScreen", LoadSceneMode.Single);
                });
            });
        }
    }

    public void OnWhiteButton()
    {
        playerColor.text = "WHITE";
        playerColor.color = Color.white;
    }

    public void OnBlackButton()
    {
        playerColor.text = "BLACK";
        playerColor.color = Color.black;
    }
}
