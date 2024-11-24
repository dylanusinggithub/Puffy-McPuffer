using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField, Range(0, 10)]
    float objectRange = 6;

    enum ObjectType { Obstacle, Collectable }

    #region Tree Settings
    [Header("Tree")]
    [SerializeField, Range(0, 10)]
    float TreeCooldown = 3;
    float TreeCooldownRemining;

    [SerializeField, Range(0, 20)]
    float TreeSpeed = 5;

    [SerializeField, Range(0, 1)]
    float TreeVariation = 1;

    [SerializeField]
    GameObject TreeObject;

    [SerializeField]
    ObjectType TreeType;
    #endregion
    
    #region create Settings
    [Header("Create")]
    [SerializeField, Range(0, 10)]
    float createCooldown = 3;
    float createCooldownRemining;

    [SerializeField, Range(0, 20)]
    float createSpeed = 5;

    [SerializeField, Range(0, 1)]
    float createVariation = 1;

    [SerializeField]
    GameObject createObject;

    [SerializeField]
    ObjectType createType;
    #endregion

    #region Wave Settings
    [Header("Wave")]
    [SerializeField, Range(0, 10)]
    float waveCooldown = 3;
    float waveCooldownRemining;

    [SerializeField, Range(0, 20)]
    float waveSpeed = 5;

    [SerializeField, Range(0, 1)]
    float waveVariation = 1;

    [SerializeField]
    GameObject waveObject;

    [SerializeField]
    ObjectType waveType;
    #endregion

    void Start()
    {
        TreeCooldownRemining = TreeCooldown;
    }


    // Update is called once per frame
    void Update()
    {
        if (TreeCooldownRemining < 0)
        {
            TreeCooldownRemining = TreeCooldown;
            GameObject Tree = SpawnObject(TreeObject);

            Vector3 Scale = Tree.transform.localScale;
            Tree.transform.localScale = new Vector3(Scale.x * Random.Range(TreeVariation, 1), Scale.y, Scale.z);
            Tree.gameObject.tag = TreeType.ToString();
        }
        else TreeCooldownRemining -= Time.deltaTime; 

        if (createCooldownRemining < 0)
        {
            createCooldownRemining = createCooldown;
            GameObject create = SpawnObject(createObject);

            Vector3 Scale = create.transform.localScale;
            float scaleOffset = Random.Range(createVariation, 1);

            create.transform.localScale = new Vector3(Scale.x * scaleOffset, Scale.y * scaleOffset, Scale.z);
            create.gameObject.tag = createType.ToString();
        }
        else createCooldownRemining -= Time.deltaTime;
        
        if (waveCooldownRemining < 0)
        {
            waveCooldownRemining = waveCooldown;
            GameObject wave = SpawnObject(waveObject);

            Vector3 Scale = wave.transform.localScale;
            wave.transform.localScale = new Vector3(Scale.x * Random.Range(waveVariation, 1), Scale.y, Scale.z);
            wave.gameObject.tag = waveType.ToString();
        }
        else waveCooldownRemining -= Time.deltaTime;
    }

    GameObject SpawnObject(GameObject Object)
    {
        Vector2 ObjectSize = Object.GetComponent<Renderer>().bounds.size;
        Vector3 pos = new Vector3(Random.Range(-objectRange + ObjectSize.x / 2, objectRange - ObjectSize.x / 2), 20, 0);
        GameObject GOObject = Instantiate(Object, pos, Quaternion.identity);

        GOObject.AddComponent<Rigidbody2D>().gravityScale = 0;
        GOObject.AddComponent<BoxCollider2D>().isTrigger = true;

        GOObject.AddComponent<DestroyOffScreen>();
        GOObject.AddComponent<ObjectCollision>();

        GOObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -TreeSpeed);


        return GOObject;
    }
}
