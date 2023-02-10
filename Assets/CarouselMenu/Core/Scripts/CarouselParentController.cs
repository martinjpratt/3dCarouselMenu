using System.Collections.Generic;
using UnityEngine;

namespace CarouselMenu
{
    public class CarouselParentController : MonoBehaviour
    {
        [SerializeField] private GameObject carouselMenuPrefab;
        [SerializeField] private CarouselMenuController carouselMenuController;

        private List<CarouselMenuController> carouselMenuControllers = new List<CarouselMenuController>();
        private int screenHeight;
        private int screenIncrement;
        private int currentMenuIndex;
        private Camera mainCamera;

        private void OnEnable()
        {
            //Cache these once
            mainCamera = Camera.main;
            screenHeight = Screen.height;

            //DEMO: Instantiate a menu
            InitiateNewCarousel(10, 0);
        }

        public void InitiateNewCarousel(int numberOfMenuItemsToSpawn, int menuToGenerateFrom, bool instantiateAbove = false, bool continuous = false)
        {

            //Create a new carousel if non exist already
            if (carouselMenuControllers.Count == 0)
            {
                GameObject newCarouselMenu = Instantiate(carouselMenuPrefab, transform);
                CarouselMenuController tempCarouselMenuController = newCarouselMenu.GetComponent<CarouselMenuController>();
                carouselMenuControllers.Add(tempCarouselMenuController);

                //Make continuous if necessary
                if (continuous)
                {
                    tempCarouselMenuController.SetContinuous(true);
                }

                //Create the menu
                tempCarouselMenuController.InitiateMenu(numberOfMenuItemsToSpawn, this);
                currentMenuIndex = 0;
                carouselMenuController = carouselMenuControllers[currentMenuIndex];
                carouselMenuController.ToggleMenuActive(true);
            }
            else
            {
                //Otherwise make a new carousel

                GameObject newCarouselMenu = Instantiate(carouselMenuPrefab, transform);
                CarouselMenuController tempCarouselMenuController = newCarouselMenu.GetComponent<CarouselMenuController>();

                //Make continuous if necessary
                if (continuous)
                {
                    tempCarouselMenuController.SetContinuous(true);
                }

                //Create the menu
                tempCarouselMenuController.SetMenuIndex(carouselMenuControllers.Count);
                tempCarouselMenuController.InitiateMenu(numberOfMenuItemsToSpawn, this);
                
                //move the menu above or below in the menu list
                if (instantiateAbove)
                {
                    carouselMenuControllers.Insert(menuToGenerateFrom + 1, tempCarouselMenuController);   
                }
                else
                {
                    carouselMenuControllers.Insert(menuToGenerateFrom, tempCarouselMenuController);
                }
            }

            //Set the active areas of the screen and arrange the menus
            screenIncrement = screenHeight / carouselMenuControllers.Count;
            arrangeMenus();
        }

        private void arrangeMenus()
        {
            //Arrange the menus based on position in list and space in the screen area

            for (int i = 0; i < carouselMenuControllers.Count; i++)
            {
                carouselMenuControllers[i].name = i.ToString();
                carouselMenuControllers[i].SetMenuIndex(i);
                carouselMenuControllers[i].transform.SetSiblingIndex(i);

                float yOffset = mainCamera.ScreenToWorldPoint(new Vector3(0, (i * screenIncrement) + (screenIncrement / 2), 4.5f)).y;  //Hardcoded and set close to the distance of the carousels. TODO: Handle scale of carousels better.
                carouselMenuControllers[i].transform.position = new Vector3(carouselMenuControllers[i].transform.position.x, yOffset, carouselMenuControllers[i].transform.position.z);
            }
        }

        void Update()
        {
            //Set a menu as active based on screen position of mouse
            CheckMousePosition();
        }

        public void CheckMousePosition()
        {
            int checkMousePosition = (int)Mathf.Floor(Input.mousePosition.y / screenIncrement);
            if (checkMousePosition != currentMenuIndex)
            {
                if (carouselMenuController != null) carouselMenuController.ToggleMenuActive(false);
                currentMenuIndex = checkMousePosition;
                if (currentMenuIndex > -1 && currentMenuIndex < carouselMenuControllers.Count)
                {
                    carouselMenuController = carouselMenuControllers[currentMenuIndex];
                    carouselMenuController.ToggleMenuActive(true);
                }
            }
        }
    }
}
