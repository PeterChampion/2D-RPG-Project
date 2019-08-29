using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Debug/Editor Variables
    [SerializeField] private bool showWalkableNodes;
    [SerializeField] private bool showUnwalkableNodes;
    [SerializeField] private bool showCalculatedPath;


    //public Transform startPosition;
    public LayerMask obstacleLayers; // Layers that will obstruct pathfinding/nodes
    public Vector2 gridSize; // Size of the 2D grid
    public float nodeRadius; // Radius of each individual node within the grid
    public TerrainType[] walkableRegions;
    public float distanceBetweenNodes; // Distance that nodes have between eachother within the grid

    private Node[,] grid; // Node array that stores all nodes within the grid
    public List<Node> finalPath; // List of nodes which are used for the final path calculated

    private float nodeDiamater; 
    private int gridSizeX; // Size of X axis of the grid
    private int gridSizeY; // Size of Y axis of the grid

    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    private void Awake()
    {
        nodeDiamater = nodeRadius * 2; // Calculate diamater based on radius
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiamater); // Caclulate size of X axis based on the gridSize divided by diamater
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiamater); // Caclulate size of Y axis based on the gridSize divided by diamater
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // Create new node array used as the grid
        Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * gridSize.x / 2 - Vector2.up * gridSize.y / 2; // Calculate the bottom left position of the grid

        // For loop that cycles through every position of the grid/array
        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector2 worldPoint = bottomLeft + Vector2.right * (x * nodeDiamater + nodeRadius) + Vector2.up * (y * nodeDiamater + nodeRadius); // Calculate the position the node is in the world
                bool walkable = true; // Flag as not being obstructed by default

                // CircleCast at the position of the grid to check if there are any obstacles
                if (Physics2D.CircleCast(worldPoint, nodeRadius, Vector2.zero, 0.5f, obstacleLayers))
                {
                    walkable = false; // Flag as being obstructed if an obstacle was overalpping the circlecast
                }

                int movementPenalty = 0;
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty); // Create a new node at the current position of the grid, assigning its values
            }
        }
    }

    public Node NodeFromWorldPosition(Vector2 worldPosition)
    {
        float xPoint = ((worldPosition.x + gridSize.x / 2) / gridSize.x);
        float yPoint = ((worldPosition.y + gridSize.y / 2) / gridSize.y);

        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);

        return grid[x, y];
    }

    public List<Node> GetNeighbouringNodes(Node node)
    {
        List<Node> NeighbouringNodes = new List<Node>();
        int xCheck;
        int yCheck;

        // Right Side
        xCheck = node.gridX + 1;
        yCheck = node.gridY;

        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                NeighbouringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        // Left Side
        xCheck = node.gridX - 1;
        yCheck = node.gridY;

        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                NeighbouringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        // Top Side
        xCheck = node.gridX;
        yCheck = node.gridY + 1;

        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                NeighbouringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        // Bottom Side
        xCheck = node.gridX;
        yCheck = node.gridY - 1;

        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                NeighbouringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        return NeighbouringNodes;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridSize.x, gridSize.y));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                if (node.walkable && showWalkableNodes) // If the node is walkable...
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawCube(node.position, Vector3.one * (nodeDiamater - distanceBetweenNodes));
                }
                else if (!node.walkable && showUnwalkableNodes) // Otherwise if the node is not walkable...
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(node.position, Vector3.one * (nodeDiamater - distanceBetweenNodes));
                }

                if (finalPath != null)
                {
                    if (finalPath.Contains(node) && showCalculatedPath)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(node.position, Vector3.one * (nodeDiamater - distanceBetweenNodes));
                    }
                }
            }       
        }
    }

    private void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x < kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeY; x++)
        {
            for (int y = -kernelExtents; y < kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;
            }
        }
    }
}

[System.Serializable] public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}
