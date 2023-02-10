using TMPro;
using UnityEngine;

public class MenuObjectController : MonoBehaviour
{
    /// <summary>
    /// DEMO: Example controller script to generate some public variables that can be used for the demo.
    /// </summary>
    /// 

    [SerializeField] private TextMeshPro prefabText;
    [SerializeField] private Transform arrowQuad;
    [SerializeField] private Renderer scrollType;
    [SerializeField] private Sprite continuousSprite;
    [SerializeField] private Sprite nonContinuousSprite;

    public int NumberOfItemsToSpawn { get; private set; }
    public bool Above { get; private set; }
    public bool Scroll { get; private set; }

    private void OnEnable()
    {
        //Set some random values

        NumberOfItemsToSpawn = Random.Range(1, 200);
        prefabText.text = NumberOfItemsToSpawn.ToString();
        Above = randomBoolean();
        if (!Above)
        {
            arrowQuad.Rotate(new Vector3(0, 0, 180));
        }
        Scroll = randomBoolean();
        if (Scroll)
        {
            scrollType.material.mainTexture = continuousSprite.texture;
        }
    }

    private bool randomBoolean()
    {
        if (Random.value >= 0.5f)
        {
            return true;
        }
        return false;
    }
}
