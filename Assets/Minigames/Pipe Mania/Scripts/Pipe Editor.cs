using UnityEngine;

[ExecuteInEditMode]
public class PipeEditor : MonoBehaviour
{
    Vector3 oldPos = Vector2.zero;

    [SerializeField] Sprite[] IPieces;
    [SerializeField] Sprite[] Corner;
    [SerializeField] Sprite[] TPieces;
    [SerializeField] Sprite[] Crosses;

    private void Awake()
    {
        SpriteRenderer SP = GetComponent<SpriteRenderer>();
        string sprite = SP.sprite.name;

        if (sprite.Contains("I Piece"))
        {
            SP.sprite = IPieces[Random.Range(0, IPieces.Length)];
            transform.localScale = new Vector3(transform.localScale.x * RandomPos(), transform.localScale.y * RandomPos(), 1); // Pos/Neg scale in both axises
        }
        else if (sprite.Contains("Corner"))
        {
            SP.sprite = Corner[Random.Range(0, Corner.Length)];
        }
        else if (sprite.Contains("TPiece"))
        {
            SP.sprite = TPieces[Random.Range(0, TPieces.Length)];
            transform.localScale = new Vector3(transform.localScale.x * RandomPos(), 1, 1); // Pos/Neg scale only horizontally
        }
        else if (sprite.Contains("Cross"))
        {
            SP.sprite = Crosses[Random.Range(0, Crosses.Length)];

            // Pos/Neg scale in both axises & rotate in any direction
            transform.localScale *= RandomPos();
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
        }
        else if (sprite.Contains("End") || sprite.Contains("Start")) { /* Do nothing :)*/ }
        else Debug.LogWarning("Invalid Sprite " + SP.sprite + ". Won't be able to do cool variations on it :(");

        // Removes this script
        if (Application.isPlaying)
        {
            // Makes it automatically rotates to a mutliple of 90
            if (!sprite.Contains("End") && !sprite.Contains("Start"))
            {
                transform.eulerAngles = new Vector3(0, 0, Mathf.RoundToInt(transform.eulerAngles.z / 90) * 90);
                gameObject.AddComponent<PipeController>();
            }

            Destroy(gameObject.GetComponent<PipeEditor>());
        }
    }

    int RandomPos()
    {
        return Random.Range(0, 2) * 2 - 1;
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            if (oldPos != transform.position)
            {
                float GridScale = 1.2f; // 6x5 area
                Vector2 GridPos = new Vector2(transform.position.x - (GridScale / 2), transform.position.y - (GridScale / 2));

                transform.localScale = Vector3.one * GridScale;

                Vector2 Magnetude = new Vector2(Mathf.RoundToInt(GridPos.x / GridScale), Mathf.RoundToInt(GridPos.y / GridScale));
                transform.position = Magnetude * GridScale - new Vector2(0.5f, 0.2f); // minus 0.5 is because the play area is even

                oldPos = transform.position;
            }
        }
    }
}