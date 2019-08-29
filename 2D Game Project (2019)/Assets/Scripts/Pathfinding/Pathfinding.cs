using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    private Grid grid;
    //public Transform startPosition;
    //public Transform targetPosition;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    //private void Update()
    //{
    //    FindPath(startPosition.position, targetPosition.position);
    //}

    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;


        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {

            Heap<Node> OpenSet = new Heap<Node>(grid.MaxSize);
            //List<Node> OpenSet= new List<Node>();
            HashSet<Node> ClosedSet = new HashSet<Node>();

            OpenSet.Add(startNode);
            //print("Starting Position: " + startNode.gridX + " , " + startNode.gridY);
            //print("Target Position: " + targetNode.gridX + " , " + targetNode.gridY);

            while (OpenSet.Count > 0)
            {
                Node currentNode = OpenSet.RemoveFirst();
                //Node currentNode = OpenSet[0];

                //for (int i = 1; i < OpenSet.Count; i++)
                //{
                //    if (OpenSet[i].fCost < currentNode.fCost || OpenSet[i].fCost == currentNode.fCost && OpenSet[i].hCost < currentNode.hCost)
                //    {
                //        //print("Found better node! Old Node position was: " + currentNode.position + " New Node position is: " + OpenList[i].position);
                //        currentNode = OpenSet[i];
                //    }
                //}

                //OpenSet.Remove(currentNode);
                ClosedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found in: " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbourNode in grid.GetNeighbouringNodes(currentNode))
                {
                    if (!neighbourNode.walkable || ClosedSet.Contains(neighbourNode))
                    {
                        continue;
                    }

                    int moveCost = currentNode.gCost + GetManhattenDistance(currentNode, neighbourNode) + neighbourNode.movementPenalty;

                    if (moveCost < neighbourNode.gCost || !OpenSet.Contains(neighbourNode))
                    {
                        neighbourNode.gCost = moveCost;
                        neighbourNode.hCost = GetManhattenDistance(neighbourNode, targetNode);
                        neighbourNode.parent = currentNode;

                        if (!OpenSet.Contains(neighbourNode))
                        {
                            OpenSet.Add(neighbourNode);
                        }
                        else
                        {
                            OpenSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
        {
            waypoints = GetFinalPath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private Vector2[] GetFinalPath(Node startingNode, Node endNode)
    {      
        List<Node> FinalPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startingNode)
        {
            FinalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2[] waypoints = SimplifyPath(FinalPath);
        Array.Reverse(waypoints);
        grid.finalPath = FinalPath;
        return waypoints;
    }

    private Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            if (newDirection != oldDirection)
            {
                waypoints.Add(path[i].position);
            }
        }
        return waypoints.ToArray();
    }

    private int GetManhattenDistance(Node nodeA, Node nodeB)
    {
        int x = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int y = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return x + y;
    }
}
