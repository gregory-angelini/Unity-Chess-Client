using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopUp
{
    public class GuiController : MonoBehaviour
    {
        Queue<PopUpController> popUps = new Queue<PopUpController>();
        [SerializeField] Canvas popup;
        [Header("Popup prefabs", order = 1)]
        [SerializeField] PopUpController figurePromotionPrefab;
        public static GuiController Instance;


        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
                Instance = this;
        }

        public PopUpController ShowFigurePromotion()
        {
            PopUpController controller = CreatePopUp(figurePromotionPrefab);

            controller.GetComponent<FigurePromotionPopUp>().Init(
                Chess2DController.Instance.Chess.GetCurrentPlayerColor(),
                () =>
                {
                    controller.CloseWindow();
                },
                () =>
                {
                    controller.CloseWindow();
                },
                () =>
                {
                    controller.CloseWindow();
                },
                () =>
                {
                    controller.CloseWindow();
                });

            controller.ShowWindow();
            return controller;
        }

        PopUpController CreatePopUp(PopUpController prefab)
        {
            if (prefab == null) 
                throw new System.IndexOutOfRangeException("CreatePopUp: the prefab you want to instantiate is null");

            PopUpController popUp = Instantiate(prefab);

            if (popUp == null)
                throw new System.IndexOutOfRangeException("CreatePopUp: cannot create the prefab");

            popUp.GetComponent<RectTransform>().SetParent(popup.transform, false);
            popUp.SetControlActivity(false);

            popUp.OnWindowOpen += OnWindowOpen;
            popUp.OnWindowClose += OnWindowClose;
            return popUp;
        }

        void OnWindowOpen(PopUpController popUp)
        {
            popUps.Enqueue(popUp);
        }

        void OnWindowClose(PopUpController popUp)
        {
            popUp = popUps.Dequeue();
            Destroy(popUp.gameObject);
        }

        public bool IsNoPopUp()
        {
            return popUps.Count > 0;
        }

        public void CloseAllPopUps()
        {
            foreach (var it in popUps)
                it.CloseWindow();
        }
    }
}

