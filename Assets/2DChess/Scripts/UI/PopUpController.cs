using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PopUp
{
    public class PopUpController : MonoBehaviour
    {
        [SerializeField] bool CanBeEscaped = true;
        public delegate void ActionType(PopUpController popUp);// определяем сигнатуру методов-действий, которые могут использоваться в качестве реакции на событие  
        public event ActionType OnWindowOpen; // runs after opening
        public event ActionType OnWindowClose; // runs after closing

        bool isDestroyed = false;
        public bool IsVisible { get; private set; } = false;


        public void CloseWindow()
        {
            SetControlActivity(false);
            //GetComponent<GuiFader_v2>().FadeOut(0, () => { CloseHandler(); });
            CloseHandler();
        }

        void CloseHandler()
        {
            if (isDestroyed)
                return;

            //if (SoundMasterController.Instance != null)
            //    SoundMasterController.Instance.SoundPlayPopUpHide();

            IsVisible = false;

            OnWindowClose?.Invoke(this);
        }

        // Runs after creating window but before it becomes visible
        // Play open sound. Fade in. Set window control activity. Run open delegate. 
        public void ShowWindow()
        {
            //if (SoundMasterController.Instance != null)
            //  SoundMasterController.Instance.SoundPlayPopUpShow(0.2f, null);

            //GetComponent<GuiFader_v2>().FadeIn(0, () =>
            //{
            SetControlActivity(true);

            IsVisible = true;

            OnWindowOpen?.Invoke(this);
            //});
        }

        void Update()
        {
            if (CanBeEscaped && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }
        }

        void OnDestroy()
        {
            isDestroyed = true;
        }

        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();

            if (buttons == null)
                return;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }
    }
}