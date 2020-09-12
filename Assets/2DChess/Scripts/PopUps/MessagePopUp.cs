using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopUp;
using System;
using TMPro;
using UnityEngine.UI;


public class MessagePopUp : BasicPopUp
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Button buttonPrefab;
    [SerializeField] Transform parent;

    public string Message
    {
        get 
        { 
            if (message != null) 
                return message.text; 
            else 
                return string.Empty; 
        }
        set 
        { 
            if (message != null) 
                message.text = value; 
        }
    }

    public void AddButton(string name, Action callback)
    {
        Button button = Instantiate(buttonPrefab);

        TextMeshProUGUI buttonName = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonName.text = name;

        button.transform.SetParent(parent, false);

        button.onClick.AddListener(() =>
        {
            callback?.Invoke();
        });
    }
}
