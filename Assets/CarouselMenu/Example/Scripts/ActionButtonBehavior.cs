using UnityEngine;
using CarouselMenu;

public class ActionButtonBehavior : MonoBehaviour
{
    [SerializeField] private CarouselMenuController carouselMenuController;
    [SerializeField] private Renderer prefabRenderer;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highLightColor;

    private bool canClick = false;

    private void OnMouseUp()
    {
        //DEMO: this is the function called on action button click
        if (canClick)
        {
            MenuObjectController moc = carouselMenuController.SelectedObject.GetComponent<MenuObjectController>();  //Normally I would have a better reference than GetComponent, but as these varaiables are randomly generated at runtime this seems the only option.
            int numberOfObjects = moc.NumberOfItemsToSpawn;
            bool above = moc.Above;
            bool scroll = moc.Scroll;
            carouselMenuController.CarouselParentController.InitiateNewCarousel(numberOfObjects, carouselMenuController.CarouselMenuIndex, above, scroll);
        }
    }

    private void OnMouseEnter()
    {
        //Set action button UX on mouse enter

        canClick = true;
        prefabRenderer.material.color = highLightColor;
        prefabRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    private void OnMouseExit()
    {
        //Set action button UX on mouse exit

        canClick = false;
        prefabRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        prefabRenderer.material.color = defaultColor;
    }
}
