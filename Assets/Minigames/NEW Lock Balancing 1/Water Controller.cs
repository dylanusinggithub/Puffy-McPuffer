using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour
{
    #region Water Movement
    [HideInInspector] public float perlinStepSizeX = 1;
    [HideInInspector] public float strengthX = 6;

    [HideInInspector] public float boatTransformX;

    float perlinStepSizeY = 1;
    float strengthY = 0.5f;
    float boatTransformY;

    float strengthR = 1;
    float boatRotation;

    int perlinX = 0;
    int perlinY = 50;

    #endregion

    #region Water Height    
    float waterMaxHeight = 2.5f;
    float waterMinHeight = 3.5f;
    float waterOffset = 1;

    [HideInInspector] public float waterHeight;
    float waterPecentage = 0;

    GameObject waterObject;
    GameObject background;

    #endregion

    AnimationScript AS;
    NEWLockBalancing LB;
    GameObject Puffy;

    private void Start()
    {
        AS = GetComponent<AnimationScript>();
        LB = GetComponent<NEWLockBalancing>();

        waterHeight = -waterMinHeight;

        waterObject = GameObject.Find("WaterSimple");
        waterObject.transform.localScale = new Vector2(waterObject.transform.localScale.x, waterOffset + waterHeight + waterMinHeight);

        Puffy = GameObject.Find("Player");
        Puffy.transform.position = new Vector2(Puffy.transform.position.x, -waterMinHeight);
    }

    void OnValidate() // Only activated outside of playmode
    {
        Puffy = GameObject.Find("Player");
        Puffy.transform.position = new Vector2(Puffy.transform.position.x, -waterMinHeight);

        waterObject = GameObject.Find("WaterSimple");
        waterObject.transform.localScale = new Vector2(waterObject.transform.localScale.x, waterOffset + waterHeight + waterMinHeight);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (perlinX == 100 / perlinStepSizeX || AS.enabled) perlinX = 0;
        else perlinX++;

        if (perlinY == 100 / perlinStepSizeY) perlinY = 0;
        else perlinY++;

        boatTransformX = Mathf.PerlinNoise1D(((float)perlinX / 100) * perlinStepSizeX) - 0.5f;
        boatTransformY = Mathf.PerlinNoise1D(((float)perlinY / 100) * perlinStepSizeY) - 0.5f;

        if (LB.state == NEWLockBalancing.GameState.Play) boatTransformX *= strengthX;
        else boatTransformX = Puffy.transform.position.x; // stops puffy from being moved around during cutscene but still bobs up and down

        boatTransformY *= strengthY;

        boatRotation = -Puffy.transform.position.x * ((float)strengthR / 100);

        Puffy.transform.position = new Vector2(boatTransformX , boatTransformY + waterHeight);
        Puffy.transform.eulerAngles += new Vector3(0, 0, boatRotation);
    }

    // get called from Puffy Controller
    public IEnumerator changeHeight(bool increase)
    {
        float waterHeightSeconds = 1;
        int waterSmoothness = 100;
        if (increase)
        {
            for (int i = 0; i < waterSmoothness; i++)
            {
                yield return new WaitForSeconds(waterHeightSeconds / waterSmoothness);
                waterPecentage += waterHeightSeconds / waterSmoothness;

                waterHeight = Mathf.Lerp(-waterMinHeight, waterMaxHeight, waterPecentage / LB.createCompletion);
                float waterObjectScale = Mathf.Lerp(0, waterMaxHeight + waterMinHeight, waterPecentage / LB.createCompletion) + waterOffset;

                waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterObjectScale, 1);
            }
        }
        else
        {
            for (int i = waterSmoothness; i > 0; i--)
            {
                yield return new WaitForSeconds(waterHeightSeconds / waterSmoothness);
                waterPecentage -= waterHeightSeconds / waterSmoothness;

                waterHeight = Mathf.Lerp(-waterMinHeight, waterMaxHeight, waterPecentage / LB.createCompletion);
                float waterObjectScale = Mathf.Lerp(0, waterMaxHeight + waterMinHeight, waterPecentage / LB.createCompletion) + waterOffset;

                waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterObjectScale, 1);
            }
        }
    }
}
