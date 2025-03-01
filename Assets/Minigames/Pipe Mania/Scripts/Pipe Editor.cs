using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PipeEditor : MonoBehaviour
{
    Vector3 oldPos = Vector2.zero;

    [System.Serializable]
    public class Pieces
    {
        public Sprite Normal;
        public Sprite Broken;
        public Sprite Fixed;
    }

    [SerializeField] Pieces[] IPieces;
    [SerializeField] Pieces[] Corner;
    [SerializeField] Pieces[] TPieces;
    [SerializeField] Pieces[] Crosses;

    public Sprite Broken, Fixed;

    private void Awake()
    {
        SpriteRenderer SP = GetComponent<SpriteRenderer>();
        string sprite = SP.sprite.name;

        if (sprite.Contains("I Piece"))
        {
            int randomIndex = Random.Range(0, IPieces.Length);

            SP.sprite = IPieces[randomIndex].Normal;
            Broken = IPieces[randomIndex].Broken;
            Fixed = IPieces[randomIndex].Fixed;

            transform.localScale = new Vector3(transform.localScale.x * RandomPos(), transform.localScale.y * RandomPos(), 1); // Pos/Neg scale in both axises
        }
        else if (sprite.Contains("Corner"))
        {
            int randomIndex = Random.Range(0, Corner.Length);

            SP.sprite = Corner[randomIndex].Normal;
            Broken = Corner[randomIndex].Broken;
            Fixed = Corner[randomIndex].Fixed;
        }
        else if (sprite.Contains("T Piece"))
        {
            int randomIndex = Random.Range(0, TPieces.Length);

            SP.sprite = TPieces[randomIndex].Normal;
            Broken = TPieces[randomIndex].Broken;
            Fixed = TPieces[randomIndex].Fixed;

            transform.localScale = new Vector3(transform.localScale.x * RandomPos(), transform.localScale.y, 1); // Pos/Neg scale only horizontally
        }
        else if (sprite.Contains("Cross"))
        {
            int randomIndex = Random.Range(0, Crosses.Length);
            
            SP.sprite = Crosses[randomIndex].Normal;

            Broken = Crosses[randomIndex].Broken;
            Fixed = Crosses[randomIndex].Fixed;

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

// Displays only the relevent varaibles; IPieces when its the IPiece

#if UNITY_EDITOR // It will not build otherwise
[CustomEditor(typeof(PipeEditor))]
class PipeEditorInspector : Editor
{
    SerializedProperty IPieces;
    SerializedProperty Corner;
    SerializedProperty TPieces;
    SerializedProperty Crosses;

    void OnEnable()
    {
        IPieces = serializedObject.FindProperty("IPieces");
        Corner = serializedObject.FindProperty("Corner");
        TPieces = serializedObject.FindProperty("TPieces");
        Crosses = serializedObject.FindProperty("Crosses");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("Piece Specific Sprites");


        string Sprite = this.target.name.ToLower();


        if (Sprite.Contains("i piece"))
        {
            EditorGUILayout.PropertyField(IPieces, true);
            IPieces.serializedObject.ApplyModifiedProperties();
        }
        else if (Sprite.Contains("corner"))
        {
            EditorGUILayout.PropertyField(Corner, true);
            Corner.serializedObject.ApplyModifiedProperties();
        }
        else if (Sprite.Contains("t piece"))
        {
            EditorGUILayout.PropertyField(TPieces, true);
            TPieces.serializedObject.ApplyModifiedProperties();
        }
        else if (Sprite.Contains("cross"))
        {
            EditorGUILayout.PropertyField(Crosses, true);
            IPieces.serializedObject.ApplyModifiedProperties();
        }
        else EditorGUILayout.LabelField("No Sprites to Change or Add");

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif