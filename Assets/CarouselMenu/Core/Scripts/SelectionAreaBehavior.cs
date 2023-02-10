using UnityEngine;
using TMPro;
using System.Collections;

namespace CarouselMenu
{
    public class SelectionAreaBehavior : MonoBehaviour
    {
        /// <summary>
        /// Handles what happens when a prefab is in the selection area. Scale is handled here for emphasis.
        /// </summary>

        [SerializeField] private CarouselMenuController carouselMenuController;
        [SerializeField] private GameObject displayParent;
        [SerializeField] private TextMeshPro displayText;
        [SerializeField] private float scaleDuration = 0.05f;

        //Coroutine definitions
        private IEnumerator ie_rescale;
        private IEnumerator IE_Rescale
        {
            get { return ie_rescale; }
            set
            {
                if (ie_rescale == null)
                {
                    ie_rescale = value;
                }
                else
                {
                    StopCoroutine(ie_rescale);
                    ie_rescale = null;
                    ie_rescale = value;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //Do this on exit from menu selection

            displayText.text = "";
            other.transform.localScale = Vector3.one * 0.5f;
            IE_Rescale = changeObjectSize(other.transform, Vector3.one * 0.75f);
            StartCoroutine(IE_Rescale);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Do this on enter from menu selection

            carouselMenuController.SetSelectedObject(other.gameObject);
            displayText.text = other.name;

            IE_Rescale = changeObjectSize(other.transform, Vector3.one);
            StartCoroutine(IE_Rescale);
        }

        private IEnumerator changeObjectSize(Transform transform, Vector3 newScale)
        {
            Vector3 currentScale = transform.localScale;
            float elapsedTime = 0;

            while (elapsedTime < scaleDuration)
            {
                transform.localScale = Vector3.Lerp(currentScale, newScale, (elapsedTime / scaleDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Make sure we got there
            transform.localScale = newScale;
            yield return null;
        }
    }
}