using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingController : MonoBehaviour
{
    public static WaitingController Instance;
    float refreshHz = 2f;// in seconds


    void Start()
    {
        InvokeRepeating("RefreshGameStatus", refreshHz, refreshHz);
    }

    void RefreshGameStatus()
    {
        //Debug.Log("RefreshGameStatus");
        ClientController.Instance.UpdateGameState((result) =>
        {
            if (result.status == "play")
                SceneManager.LoadScene("2DBoard", LoadSceneMode.Single);
        });
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
