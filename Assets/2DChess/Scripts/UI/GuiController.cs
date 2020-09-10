using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopUp
{
    public class GuiController : MonoBehaviour
    {
        Queue<BasicPopUp> popUps = new Queue<BasicPopUp>();
        [SerializeField] Canvas popup;
        [Header("Popup prefabs", order = 1)]
        [SerializeField] BasicPopUp figurePromotionPrefab;
        [SerializeField] BasicPopUp messagePrefab;
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

        public BasicPopUp ShowFigurePromotion()
        {
            BasicPopUp controller = CreatePopUp(figurePromotionPrefab);
            controller.ShowWindow();
            return controller;
        }

        public BasicPopUp ShowMessage()
        {
            BasicPopUp controller = CreatePopUp(messagePrefab);
            controller.ShowWindow();
            return controller;
        }

        BasicPopUp CreatePopUp(BasicPopUp prefab)
        {
            if (prefab == null) 
                throw new System.IndexOutOfRangeException("CreatePopUp: the prefab you want to instantiate is null");

            BasicPopUp popUp = Instantiate(prefab);

            if (popUp == null)
                throw new System.IndexOutOfRangeException("CreatePopUp: cannot create the prefab");

            popUp.GetComponent<RectTransform>().SetParent(popup.transform, false);
            popUp.SetControlActivity(false);

            popUp.OnWindowOpen += OnWindowOpen;
            popUp.OnWindowClose += OnWindowClose;
            return popUp;
        }

        void OnWindowOpen(BasicPopUp popUp)
        {
            popUps.Enqueue(popUp);
        }

        void OnWindowClose(BasicPopUp popUp)
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

