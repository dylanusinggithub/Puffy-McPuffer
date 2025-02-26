using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PipeGeneration : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip waterLeakSound;

    [SerializeField] Vector2Int GridArea;

    List<Vector2Int> Obstacles = new List<Vector2Int>();
    [SerializeField] int ObstaclesCount;

    [SerializeField] Sprite ObstacleSprite, StartPipe, EndPipe;

    [SerializeField] Sprite[] CornerPiece;
    [SerializeField] Sprite[] IPieces;
    GameObject PipeParent;

    [Header("Movement Cost")]
    [SerializeField] float movementCostX;
    [SerializeField] float movementCostY;

    Node[,] NodeGraph;

    void Start()
    {
        GenerateLayout();
        PlayLeakingSound();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If no AudioSource is attached to GameObject, add one
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space)) GenerateLayout();
    }

    void PlayLeakingSound()
    {
        if (waterLeakSound == null) return;

        audioSource.clip = waterLeakSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    void StopLeakingSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    bool IsAnyPipeLeaking()
    {
        // Logic to check if any pipes are still leaking (modify based on how leaks are tracked)
        return false;
    }

    public void CheckLeakAndStopSound()
    {
        if (!IsAnyPipeLeaking()) // If no leaks remain
        {
            StopLeakingSound();
            bool isLeaking = false;
        }
    }

    void GenerateLayout()
    {
        // Removes previous layout
        Obstacles.Clear();
        foreach (Transform child in GetComponentInChildren<Transform>()) Destroy(child.gameObject);

        // Pipes holder for easy prefabing
        PipeParent = new GameObject("Pipes");
        PipeParent.transform.parent = transform;
        PipeParent.transform.localScale = Vector3.one; // It's not centred normally?


        // Can't use VectorInt directly in lists???
        int GridX = GridArea.x;
        int GridY = GridArea.y;

        // Set Start & End Position
        int StartPos = Random.Range(0, GridY);
        int EndPos = Random.Range(0, GridY);

        CreatePipe(-1, StartPos, StartPipe);
        CreatePipe(GridX, EndPos, EndPipe);

        int i = 0, Attempts = 0;
        while (Obstacles.Count < ObstaclesCount && Attempts < 1000) 
        {
            Attempts++;

            int x = Random.Range(0, GridX);
            int y = Random.Range(0, GridY);

            bool isValid = true;
            if (x == 0 && y == StartPos || x == (GridX - 1) && y == EndPos) isValid = false; // In front of pipes
            else
            {
                foreach (Vector2Int point in Obstacles)
                {
                    float dist = (point - new Vector2Int(x, y)).magnitude;

                    if (dist < 1.42f && dist > 1) isValid = false; // Diagonal
                    else if (dist == 0) isValid = false; // Obstacle already there
                }
            }

            if (!isValid) continue;
            Obstacles.Add(new Vector2Int(x, y));

            i++;
        }

        // While loops my detested </3s
        if (Attempts == 1000) Debug.LogError("Too many obstacles! Only " + Obstacles.Count + " were generated");


        GenerateNodeGraph();
        List<Node> Path = ComputePathtoTarget(StartPos, EndPos);


        Path.Add(new Node(GridX, EndPos));
        Vector2Int previousPipe = new Vector2Int(-1, StartPos);
        for (i = 0; i < Path.Count - 1; i++)
        {
            if (Path[i].x != previousPipe.x) // Horizontal
            {
                // If the next pipe doesn't change height then it's straight
                if (Path[i + 1].y == Path[i].y)
                {
                    GameObject pipe = CreatePipe(Path[i].x, Path[i].y, IPieces[Random.Range(0, IPieces.Length)]);
                    pipe.transform.Rotate(new Vector3(0, 0, 90));

                    // adds some variation by mirroring the pipes
                    pipe.transform.localScale *= new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1);
                }
                else
                {
                    GameObject pipe = CreatePipe(Path[i].x, Path[i].y, CornerPiece[Random.Range(0, CornerPiece.Length)]);

                    // if the next pipe faces downwards then rotate the pipe to connect it
                    if ((Path[i].y - Path[i + 1].y) == 1)
                    {
                        if ((Path[i].x - Path[i + 1].x) == 0) pipe.transform.Rotate(0, 0, 90);
                        else pipe.transform.Rotate(0, 0, 180);
                    }
                    if ((Path[i].x - Path[i + 1].x) == 1) pipe.transform.Rotate(0, 0, -90);
                }
            }
            else
            {
                // If the previous, current & next pipes are all in the same X
                if (Path[i + 1].x == Path[i].x && previousPipe.x == Path[i].x)
                {
                    GameObject pipe = CreatePipe(Path[i].x, Path[i].y, IPieces[Random.Range(0, IPieces.Length)]);
                    pipe.transform.localScale *= new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1);
                }
                // If the next pipe is further down
                else if (Path[i + 1].y < previousPipe.y)
                {
                    CreatePipe(Path[i].x, Path[i].y, CornerPiece[Random.Range(0, CornerPiece.Length)]).transform.Rotate(0, 0, 270);
                }
                else
                {
                    CreatePipe(Path[i].x, Path[i].y, CornerPiece[Random.Range(0, CornerPiece.Length)]).transform.Rotate(0, 0, 180);
                }
            }
            previousPipe = new Vector2Int(Path[i].x, Path[i].y);
        }

        // Tries to fit the layout into the play area (880px)
        PipeParent.transform.localScale = Vector3.one * ((8.8f - (10f / GridX))/ GridX);
    }

    void GenerateNodeGraph()
    {
        // Pregenerate NodeGraph
        NodeGraph = new Node[GridArea.x, GridArea.y];
        for (int x = 0; x < GridArea.x; x++)
        {
            for (int y = 0; y < GridArea.y; y++)
            {
                NodeGraph[x, y] = new Node(x, y);
            }
        }

        for (int x = 0; x < GridArea.x; x++)
        {
            for (int y = 0; y < GridArea.y; y++)
            {
                Node node = NodeGraph[x, y];

                if (CheckIfObstacles(node.x, node.y))
                {
                    node.neighbourCosts = new float[0];
                    node.neighbours = new Node[0];

                    NodeGraph[x, y] = node;
                }

                int neighbourCount = 0;
                for (int i = 0; i < 4; i++)
                {
                    // Checks the direct neighbours if they're valid in a plus pattern
                    switch (i)
                    {
                        case 0: if (CheckIfValid(x + 1, y, x, y)) neighbourCount++;
                            break;

                        case 1: if (CheckIfValid(x - 1, y, x, y)) neighbourCount++;
                            break;

                        case 2: if (CheckIfValid(x, y + 1, x, y)) neighbourCount++;
                            break;

                        case 3: if (CheckIfValid(x, y - 1, x, y)) neighbourCount++;
                            break;
                    }
                }

                node.neighbourCosts = new float[neighbourCount];
                node.neighbours = new Node[neighbourCount];

                int neighbourIndex = 0;
                for (int i = 0; i < 4; i++)
                {
                    // Checks the direct neighbours if they're valid in a plus pattern
                    switch (i)
                    {
                        case 0:
                            {
                                if (!CheckIfValid(x + 1, y, x, y)) continue;

                                node.neighbours[neighbourIndex] = NodeGraph[x + 1, y];
                                node.neighbourCosts[neighbourIndex] = CalculateNodeCost(x + 1, x);

                                neighbourIndex++;
                            }
                            break;
                        case 1:
                            {
                                if (!CheckIfValid(x - 1, y, x, y)) continue;

                                node.neighbours[neighbourIndex] = NodeGraph[x - 1, y];
                                node.neighbourCosts[neighbourIndex] = CalculateNodeCost(x - 1, x);

                                neighbourIndex++;
                            }
                            break;
                        case 2:
                            {
                                if (!CheckIfValid(x, y + 1, x, y)) continue;

                                node.neighbours[neighbourIndex] = NodeGraph[x, y + 1];
                                node.neighbourCosts[neighbourIndex] = CalculateNodeCost(x, x); // if they're the same then it's vertical

                                neighbourIndex++;

                            }
                            break;
                        case 3:
                            {
                                if (!CheckIfValid(x, y - 1, x, y)) continue;

                                node.neighbours[neighbourIndex] = NodeGraph[x, y - 1];
                                node.neighbourCosts[neighbourIndex] = CalculateNodeCost(x, x); // if they're the same then it's vertical

                                neighbourIndex++;
                            }
                            break;
                    }
                }

                NodeGraph[x, y] = node;
            }
        }
    }

    bool CheckIfValid(int neighbourX, int neighbourY, int x, int y)
    {
        if (neighbourX < 0 || neighbourX >= GridArea.x) return false;
        else if (neighbourY < 0 || neighbourY >= GridArea.y) return false;
        else if (neighbourX == x && neighbourY == y) return false; // Is centre node / is current node
        else if (CheckIfObstacles(neighbourX, neighbourY)) return false; // if its a Obstacles

        return true;
    }

    bool CheckIfObstacles(int nodeX, int nodeY)
    {
        // If its a obstacle then make it intraversable
        foreach (Vector2Int point in Obstacles)
        {
            if (point.x == nodeX && point.y == nodeY) return true;
        }
        return false;
    }

    float CalculateNodeCost(int neighX, int startX)
    {
        // Find the distance between nodes
        int xCost = Mathf.Abs(neighX - startX);

        if (xCost > 0) return movementCostX;
        else return movementCostY; 
    }

    List<Node> ComputePathtoTarget(int startY, int endY)
    {
        List<Node> discoveredNodes = new List<Node>();
        List<Node> openList = new List<Node>();

        openList.Add(NodeGraph[0, startY]);

        Node targetNode = NodeGraph[GridArea.x - 1, endY];

        while (openList.Count > 0)
        {
            openList.Sort();

            Node currentNode = openList[0];
            openList.RemoveAt(0);
            currentNode.onClosedList = true;

            if (currentNode == targetNode) return GetFoundPath(targetNode);

            Node[] neighbours = currentNode.neighbours;
            int neighboursCount = neighbours.Length;
            for (int neighbourIndex = 0; neighbourIndex < neighboursCount; ++neighbourIndex)
            {
                Node currentNeighbour = neighbours[neighbourIndex];

                if (currentNeighbour.onClosedList) continue;

                // Euclidean Distance Heuristic
                float distX = (transform.position.x - currentNeighbour.x) * movementCostX;
                float distY = (transform.position.y - currentNeighbour.y) * movementCostY;

                float hCost = Mathf.Sqrt((float)((distX * distX) + (distY * distY)));

                float gCost = currentNode.g + currentNode.neighbourCosts[neighbourIndex];
                float fCost = gCost + hCost;

                if (fCost <= currentNeighbour.f || currentNeighbour.g == 0)
                {
                    currentNeighbour.g = gCost;
                    currentNeighbour.h = hCost;
                    currentNeighbour.f = fCost;
                    currentNeighbour.parent = currentNode;
                }

                if (!currentNeighbour.onOpenList)
                {
                    currentNeighbour.onOpenList = true;
                    openList.Add(currentNeighbour);
                }
            }
        }
        return GetFoundPath(null);
    }

    /* please ignore this List<Node> SecondaryPath(Node fish)
    {
        List<Node> fishy = new List<Node>();
        return fishy;
    }*/

    List<Node> GetFoundPath(Node targetNode)
    {
        // If invalid show why
        if (targetNode == null)
        {
            foreach(Vector2Int point in Obstacles) CreatePipe(point.x, point.y, ObstacleSprite);
        }

        List<Node> foundPath = new List<Node>();
        if (targetNode != null)
        {
            foundPath.Add(targetNode);

            while (targetNode.parent != null)
            {
                foundPath.Add(targetNode.parent);
                targetNode = targetNode.parent;
            }

            // Reverse the path so the start node is at index 0
            foundPath.Reverse();

        }
        return foundPath;
    }

    GameObject CreatePipe(int x, int y, Sprite Sprite)
    {
        GameObject Obj = new GameObject("X: " + x + ", Y: " + y);
        Obj.transform.parent = PipeParent.transform;

        Obj.transform.position = new Vector2(x, y);
        Obj.transform.position -= new Vector3(GridArea.x - 1, GridArea.y - 1)/2; // Centres it

        Obj.transform.localScale = Vector3.one * 1.08f;

        Obj.AddComponent<Image>();
        Obj.GetComponent<Image>().sprite = Sprite;

        return Obj;
    }
}
