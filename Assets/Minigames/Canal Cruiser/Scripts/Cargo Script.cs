using UnityEngine;

public class CargoScript : MonoBehaviour
{
    [SerializeField]
    Transform PuffyAnchor;

    [SerializeField, Range(0, 2)]
    float maxLength;

    // Start is called before the first frame update

    void OnValidate() // Updates whilst editing outside of playmode
    {
        Transform Anchor = PuffyAnchor.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform cargo = transform.GetChild(i);

            cargo.GetComponent<LineRenderer>().SetPosition(0, Anchor.position);
            cargo.GetComponent<LineRenderer>().SetPosition(1, cargo.position);

            cargo.GetComponent<DistanceJoint2D>().distance = maxLength;
            cargo.GetComponent<DistanceJoint2D>().connectedAnchor = Anchor.position;

            Anchor = cargo;
        }
    }

    void FixedUpdate()
    {
        Transform Anchor = PuffyAnchor.transform;

        for(int i = 0; i < transform.childCount; i++)
        {
            Transform cargo = transform.GetChild(i);

            cargo.GetComponent<LineRenderer>().SetPosition(0, Anchor.position);
            cargo.GetComponent<LineRenderer>().SetPosition(1, cargo.position);

            cargo.GetComponent<DistanceJoint2D>().distance = maxLength;
            cargo.GetComponent<DistanceJoint2D>().connectedAnchor = Anchor.position;

            Anchor = cargo;
        }
    }
}
