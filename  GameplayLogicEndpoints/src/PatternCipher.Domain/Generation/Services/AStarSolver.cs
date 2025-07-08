using System.Diagnostics.CodeAnalysis;
using PatternCipher.Domain.Generation.Interfaces;
using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Domain.Puzzles.Entities;
using PatternCipher.Domain.Puzzles.ValueObjects;
using PatternCipher.Shared.Models;
using PatternCipher.Shared.Models.Goals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternCipher.Domain.Generation.Services
{
    /// <summary>
    /// Implements a solvability validator using the A* pathfinding algorithm to find an optimal solution path.
    /// </summary>
    public sealed class AStarSolver : ISolvabilityValidator
    {
        /// <summary>
        /// Attempts to find an optimal solution for a given puzzle using the A* algorithm.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="solution">When this method returns, contains the found solution path if successful; otherwise, null.</param>
        /// <returns>True if a solution was found; otherwise, false.</returns>
        public bool TryFindSolution(Puzzle puzzle, [MaybeNullWhen(false)] out SolutionPath? solution)
        {
            solution = null;

            var openSet = new List<Node>();
            var closedSet = new HashSet<string>();

            var initialGrid = puzzle.GetGridState();
            var initialHeuristic = CalculateHeuristic(initialGrid, puzzle.Goal);
            var startNode = new Node(initialGrid, null, null, 0, initialHeuristic);
            
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Find node with lowest F-cost in open set
                var currentNode = openSet.OrderBy(n => n.F_Cost).ThenBy(n => n.H_Cost).First();

                // If goal is reached
                if (currentNode.H_Cost == 0)
                {
                    solution = ReconstructPath(currentNode);
                    return true;
                }

                openSet.Remove(currentNode);
                closedSet.Add(GetGridStateHash(currentNode.State));

                var neighborNodes = GetNeighborNodes(currentNode, puzzle.Goal);

                foreach (var neighborNode in neighborNodes)
                {
                    if (closedSet.Contains(GetGridStateHash(neighborNode.State)))
                    {
                        continue;
                    }

                    var existingNode = openSet.FirstOrDefault(n => GetGridStateHash(n.State) == GetGridStateHash(neighborNode.State));
                    if (existingNode == null)
                    {
                        openSet.Add(neighborNode);
                    }
                    else if (neighborNode.G_Cost < existingNode.G_Cost)
                    {
                        // Found a better path to this existing node, so we'd normally update it.
                        // For simplicity in this list-based A*, we can just remove the old and add the new.
                        openSet.Remove(existingNode);
                        openSet.Add(neighborNode);
                    }
                }
            }

            // No solution found
            return false;
        }

        /// <summary>
        /// Generates all possible successor nodes from a parent node by applying valid moves.
        /// </summary>
        private IEnumerable<Node> GetNeighborNodes(Node parentNode, IPuzzleGoal goal)
        {
            var neighbors = new List<Node>();
            var grid = parentNode.State;

            // Generate neighbors for swap-based puzzles
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    var pos1 = new GridPosition(r, c);
                    var tile1 = grid.GetTileAt(pos1);
                    if (tile1.IsLocked) continue;

                    // Consider swap with right neighbor
                    if (c + 1 < grid.Columns)
                    {
                        var pos2 = new GridPosition(r, c + 1);
                        var tile2 = grid.GetTileAt(pos2);
                        if (!tile2.IsLocked)
                        {
                            var move = new Move(Shared.Enums.MoveType.Swap, pos1, pos2);
                            var newGrid = grid.DeepCopy();
                            newGrid.SwapTiles(pos1, pos2);
                            var h = CalculateHeuristic(newGrid, goal);
                            neighbors.Add(new Node(newGrid, parentNode, move, parentNode.G_Cost + 1, h));
                        }
                    }

                    // Consider swap with bottom neighbor
                    if (r + 1 < grid.Rows)
                    {
                        var pos2 = new GridPosition(r + 1, c);
                        var tile2 = grid.GetTileAt(pos2);
                        if (!tile2.IsLocked)
                        {
                            var move = new Move(Shared.Enums.MoveType.Swap, pos1, pos2);
                            var newGrid = grid.DeepCopy();
                            newGrid.SwapTiles(pos1, pos2);
                            var h = CalculateHeuristic(newGrid, goal);
                            neighbors.Add(new Node(newGrid, parentNode, move, parentNode.G_Cost + 1, h));
                        }
                    }
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Calculates the heuristic (estimated cost to goal) for a given grid state.
        /// </summary>
        private int CalculateHeuristic(Grid currentGrid, IPuzzleGoal goal)
        {
            if (goal is DirectMatchGoal directMatchGoal)
            {
                var targetGrid = directMatchGoal.TargetGrid;
                int totalDistance = 0;

                // Create a map of where each symbol SHOULD be for fast lookup.
                var targetPositions = new Dictionary<int, GridPosition>();
                 for (int r = 0; r < targetGrid.Rows; r++)
                {
                    for (int c = 0; c < targetGrid.Columns; c++)
                    {
                        var pos = new GridPosition(r, c);
                        targetPositions[targetGrid.GetTileAt(pos).Symbol.Id] = pos;
                    }
                }

                // Calculate Manhattan distance for each tile from its target position.
                for (int r = 0; r < currentGrid.Rows; r++)
                {
                    for (int c = 0; c < currentGrid.Columns; c++)
                    {
                        var currentPos = new GridPosition(r, c);
                        var currentTile = currentGrid.GetTileAt(currentPos);
                        
                        // If the tile is in the wrong spot, add its distance to the target.
                        var targetPos = targetPositions[currentTile.Symbol.Id];
                        if (currentPos != targetPos)
                        {
                             totalDistance += Math.Abs(currentPos.Row - targetPos.Row) + Math.Abs(currentPos.Column - targetPos.Column);
                        }
                    }
                }
                // The heuristic is the sum of distances divided by 2, because each swap can reduce the total distance by at most 2.
                return totalDistance / 2;
            }

            if (goal is RuleBasedGoal ruleBasedGoal)
            {
                // Heuristic is the number of unsatisfied rules.
                return ruleBasedGoal.Rules.Count(rule => !rule.IsSatisfiedBy(currentGrid));
            }

            return 0;
        }
        
        /// <summary>
        /// Reconstructs the solution path by backtracking from the goal node to the start node.
        /// </summary>
        private SolutionPath ReconstructPath(Node goalNode)
        {
            var moves = new List<Move>();
            var currentNode = goalNode;
            while (currentNode.Parent != null)
            {
                if(currentNode.Move.HasValue)
                {
                    moves.Add(currentNode.Move.Value);
                }
                currentNode = currentNode.Parent;
            }
            moves.Reverse();
            return new SolutionPath(moves);
        }

        /// <summary>
        /// Creates a unique, reproducible string hash for a grid state to use in the closed set.
        /// </summary>
        private string GetGridStateHash(Grid grid)
        {
            var sb = new StringBuilder();
            for(int r = 0; r < grid.Rows; r++)
            {
                for(int c = 0; c < grid.Columns; c++)
                {
                    sb.Append(grid.GetTileAt(new GridPosition(r, c)).Symbol.Id);
                    sb.Append(',');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Represents a node in the A* search space.
        /// </summary>
        private sealed class Node
        {
            public Grid State { get; }
            public Node? Parent { get; }
            public Move? Move { get; }
            public int G_Cost { get; } // Cost from start
            public int H_Cost { get; } // Heuristic cost to end
            public int F_Cost => G_Cost + H_Cost; // Total estimated cost

            public Node(Grid state, Node? parent, Move? move, int gCost, int hCost)
            {
                State = state;
                Parent = parent;
                Move = move;
                G_Cost = gCost;
                H_Cost = hCost;
            }
        }
    }
}