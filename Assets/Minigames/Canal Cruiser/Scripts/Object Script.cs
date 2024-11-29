using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    [SerializeField]
    bool randomiseScale;

    [SerializeField, Range(-0.5f, 0.5f)]
    float startSpeed = 0;

    private void OnValidate()
    {
        if (randomiseScale)
        {
            transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), transform.localScale.y, transform.localScale.z);
            randomiseScale = false;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector2(0, startSpeed));   // don't ask why it's reversed, idk why
    }
}