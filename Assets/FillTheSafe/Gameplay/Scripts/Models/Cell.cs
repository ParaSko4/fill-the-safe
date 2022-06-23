using System.Collections.Generic;
using UnityEngine;

namespace FillTheSafe.Gameplay
{
    public class Cell
    {
        private List<Cell> neighbors = new List<Cell>();
        private Cell upperCell;

        public Cell UpperCell => upperCell;
        public Vector3 Center { get; private set; }
        public bool Locked { get; private set; }
        public bool WithObject { get; private set; }

        public Cell(Vector3 center)
        {
            Center = center;
        }

        public void AddNeighbor(Cell neighbor)
        {
            neighbors.Add(neighbor);
        }

        public void SetUpperCell(Cell upperCell)
        {
            this.upperCell = upperCell;
        }

        public Vector3 TakeCellPosition()
        {
            Locked = true;
            WithObject = true;

            foreach (var neighbor in neighbors)
            {
                neighbor.Locked = true;
            }

            return Center;
        }
    }
}