using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PipeGeneration : MonoBehaviour
{
    [SerializeField] Vector2Int GridArea;
    [SerializeField] Vector2Int GridSize;

    List<Vector2Int> Obstacles = new List<Vector2Int>();
    [SerializeField] int ObstaclesCount;

    [SerializeField] Sprite ObstacleSprite, StartPipe, EndPipe;

    [SerializeField] GameObject[] VerticalPipes;
    [SerializeField] GameObject[] HorizontalPipes;

    [SerializeField] Sprite test;

    [Header("Movement Cost")]
    [SerializeField] float movementCostX;
    [SerializeField] float movementCostY;

    Node[,] NodeGraph;

    void Start()
    {
        int GridX = GridArea.x;
        int GridY = GridArea.y;

        // Set Start & End Position
        int StartPos = Random.Range(0, GridArea.y);
        int EndPos = Random.Range(0, GridArea.y);

        CreatePipe(-1, StartPos, StartPipe);
        CreatePipe(GridX, EndPos, EndPipe);

        int i = 0;
        while (Obstacles.Count < ObstaclesCount)
        {
            // Can't use VectorInt directly in lists???
            int x = Random.Range(0, GridX);
            int y = Random.Range(0, GridY);

            if (x == 0 && y == StartPos || x == GridY - 1 && y == EndPos) return; // In front of pipes

            Obstacles.Add(new Vector2Int(x, y));
            CreatePipe(Obstacles[i].x, Obstacles[i].y, ObstacleSprite);

            i++;
        }

        GenerateNodeGraph();

        List<Node> Path = ComputePathtoTarget(StartPos, EndPos);

        foreach (Node pipe in Path) CreatePipe(pipe.x, pipe.y, test);
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

    List<Node> GetFoundPath(Node targetNode)
    {
        if (targetNode == null) Debug.LogError("Invalid Path!");

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

    void CreatePipe(int x, int y, Sprite Sprite)
    {
        GameObject Obj;

        // Names objects depending on their sprite (switch statement didn't work :'( )
        if(Sprite == ObstacleSprite) Obj = new GameObject("Obstacle, X: " + x + ", Y: " + y);
        else if(Sprite == EndPipe) Obj = new GameObject("End, X: " + x + ", Y: " + y);
        else if(Sprite == StartPipe) Obj = new GameObject("Start, X: " + x + ", Y: " + y);
        else Obj = new GameObject("X: " + x + ", Y: " + y);

        Obj.transform.parent = transform;
        Obj.transform.position = new Vector2(x, y);
        Obj.transform.localScale = Vector3.one;

        Obj.AddComponent<Image>();
        Obj.GetComponent<Image>().sprite = Sprite;
    }
}
