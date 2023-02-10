using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CarouselMenu
{
    public class CarouselMenuController : MonoBehaviour
    {
        /// <summary>
        /// Main script to handle all menu object generation, positioning and movement.
        /// </summary>


        [SerializeField] private GameObject menuPrefab;
        [SerializeField] private GameObject selectionAreaPrefab;
        [SerializeField] private Transform contentParent;
        [SerializeField] private float defaultDistance = 10;
        [SerializeField] private float dragSensitivity = 1f;
        [SerializeField] private TextMeshPro displayText;
        [SerializeField] private Color menuActiveColor;
        [SerializeField] private float slowDownTime = 0.3f;
        [SerializeField] private bool isContinuous;

        private Dictionary<int, GameObject> prefabsSpawned = new Dictionary<int, GameObject>();
        private Camera mainCamera;
        private Vector3 lastMousePosition;
        private bool slowDown = false;
        private Vector3 screenPoint;
        private Vector3 offset;
        private Vector3 targetPosition;
        private float yVelocity = 0.0f;
        private bool active;
        private float singleAngle;
        private float angle;

        public GameObject SelectedObject { get; private set; }
        public CarouselParentController CarouselParentController { get; private set; }
        public int CarouselMenuIndex { get; private set; }


        void OnEnable()
        {
            //Cache the camera position
            mainCamera = Camera.main;
        }

        private float xMouseDelta
        {
            //Getter for mouse velocity
            get
            {
                return Input.mousePosition.x - lastMousePosition.x;
            }
        }

        public void SetContinuous(bool continuousScolling)
        {
            isContinuous = continuousScolling;
        }

        public void SetMenuIndex(int menuIndex)
        {
            CarouselMenuIndex = menuIndex;
        }

        public void SetSelectedObject(GameObject selectedObject)
        {
            SelectedObject = selectedObject;
        }

        public void InitiateMenu(int numberOfObjects, CarouselParentController cpc)
        {
            //Set the parent class
            CarouselParentController = cpc;

            //Always set continuous to false if the number of objects is small
            if (numberOfObjects < 2 * defaultDistance) isContinuous = false;

            dragSensitivity = dragSensitivity / numberOfObjects;
            float radiusOfMenu = defaultDistance;
            float zOffset = 0;

            singleAngle = 180 / (float)numberOfObjects;

            //Reset if menu is complete circle
            if (isContinuous)
            {
                singleAngle = 360 / (float)numberOfObjects;
            }
            
            //Resize circle to get objects in approximately the same lateral position in the screen space
            if (numberOfObjects > defaultDistance)
            {

                float defaultIncrementAngle = 180 - (180 / defaultDistance);
                float adjustedIncrementAngle = 180 / (float)numberOfObjects;

                if (isContinuous)
                {
                    defaultIncrementAngle = 180 - (180 / defaultDistance);
                    adjustedIncrementAngle = 360 / (float)numberOfObjects;
                }

                radiusOfMenu = (defaultDistance * Mathf.Sin(defaultIncrementAngle * Mathf.Deg2Rad)) / Mathf.Sin(adjustedIncrementAngle * Mathf.Deg2Rad);
                zOffset = defaultDistance - radiusOfMenu;
            }

            //Instantiate and position the menu objects
            for (int i = 0; i < numberOfObjects; i++)
            {
                GameObject newMenuPrefab = Instantiate(menuPrefab, contentParent);
                newMenuPrefab.name = (i + 1) + " / " + numberOfObjects;
                if (numberOfObjects < defaultDistance)
                {
                    newMenuPrefab.transform.localPosition = getRadialPosition((int)defaultDistance, i, radiusOfMenu);
                }
                else
                {
                    newMenuPrefab.transform.localPosition = getRadialPosition(numberOfObjects, i, radiusOfMenu);
                }

                //Orient the objects to the camera
                newMenuPrefab.transform.LookAt(mainCamera.transform);
                if (i == 0)
                {
                    selectionAreaPrefab.transform.SetPositionAndRotation(newMenuPrefab.transform.position, newMenuPrefab.transform.rotation);
                }

                //Add to a dictionary of prefabs
                prefabsSpawned.Add(i, newMenuPrefab);
            }

            //Offset the menu to accomodate larger numbers of menu objects
            transform.localPosition = new Vector3(0, 0, zOffset);
        }

        private void Update()
        {
            if (active)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //start dragging

                    slowDown = false;
                    screenPoint = Input.mousePosition;
                    offset = contentParent.localEulerAngles;
                }

                if (Input.GetMouseButton(0))
                {
                    //drag the menu and capture the mouse speed;

                    lastMousePosition = Input.mousePosition;
                    
                    float difference = (Input.mousePosition.x - screenPoint.x);
                    contentParent.localEulerAngles = offset + new Vector3(0, difference * dragSensitivity, 0);
                }


                if (Input.GetMouseButtonUp(0))
                {
                    //Set parameters for slowdown and begin the slowdown

                    if (contentParent.localEulerAngles.y < 90 && !isContinuous)
                    {
                        targetPosition = Vector3.zero;
                    }
                    else if (contentParent.localEulerAngles.y < 360 - angle && !isContinuous)
                    {
                        targetPosition = new Vector3(0, 360 - angle, 0);
                    }
                    else if (contentParent.localEulerAngles.y > angle && contentParent.localEulerAngles.y < 360 && !isContinuous)
                    {
                        targetPosition = calculatePoistionToRotateTo(-xMouseDelta, contentParent.localEulerAngles.y);
                    }else
                    {
                        targetPosition = calculatePoistionToRotateTo(-xMouseDelta, contentParent.localEulerAngles.y);
                    }

                    slowDown = true;
                }
            }

            if (slowDown)
            {
                //slowdown the menu to a menu object

                float yAngle = Mathf.SmoothDampAngle(contentParent.localEulerAngles.y, targetPosition.y, ref yVelocity, slowDownTime);
                contentParent.localEulerAngles = new Vector3(0, yAngle, 0);
            }
        }

        private Vector3 getRadialPosition(int numberOfPositions, int positionInCircle, float radius)
        {
            //Calculates the radial position of menu objects

            angle = (180 / (float)numberOfPositions) * (float)positionInCircle;
            if (isContinuous)
            {
                angle = (360 / (float)numberOfPositions) * (float)positionInCircle;
            }
            float zPos = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float xPos = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            return new Vector3(xPos, 0, zPos);
        }

        public GameObject GetSpawnedMenuPrefab(int id)
        {

            //Generic method to return a spawned prefab

            GameObject outObject;
            if (prefabsSpawned.TryGetValue(id, out outObject))
            {
                return outObject;
            }
            else
            {
                return null;
            }
        }

        public void ToggleMenuActive(bool isActive)
        {

            //Toggle the menu to be scrollable

            active = isActive;
            if (isActive)
            {
                displayText.color = menuActiveColor;
            }
            else
            {
                displayText.color = Color.black;
            }
        }

        private Vector3 calculatePoistionToRotateTo(float velocity, float currentRotation)
        {
            //Method to calculate where to send the menu during slowdow, based on mouse velocity

            velocity = ((velocity * Mathf.PI) / prefabsSpawned.Count) * 10;  //Scaled so menus slowdown equally
            float targetYRot = Mathf.Round((currentRotation - velocity) / singleAngle) * singleAngle;

            if (isContinuous)
            {
                Vector2 newposition = new Vector2(Mathf.Cos(targetYRot * Mathf.Deg2Rad), Mathf.Sin(targetYRot * Mathf.Deg2Rad));
                targetYRot = Mathf.Atan2(newposition.y, newposition.x) * Mathf.Rad2Deg;
            }

            if (targetYRot >= 360 && !isContinuous)
            {
                targetYRot = 359.9f;  //handles an annoying edge case. TODO: handle this better.
            }
            else if (targetYRot >= 360 && isContinuous)
            {
                targetYRot = targetYRot - 360;
            }
            else if (targetYRot < 360 - angle && !isContinuous)
            {
                targetYRot = 360 - angle;
            }
            else if (targetYRot < 0 && isContinuous)
            {
                targetYRot = 360 + targetYRot;
            }

            Vector3 target = new Vector3(0, targetYRot, 0);
            return target;
        }

    }
}
