using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FillTheSafe.Gameplay
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private int height;
        [SerializeField]
        private int length;
        [SerializeField]
        private int width;
        [SerializeField]
        private float cellWidth;
        [SerializeField]
        private float cellHeight;
        [Space(10)]
        [SerializeField]
        private bool debug;
        [SerializeField]
        private GameObject debugPrefab;

        private Dictionary<float, List<Cell>> dicGridCells = new Dictionary<float, List<Cell>>();
        private List<Cell> firstLevelCells;
        private float[] dicKeys;

        private void Awake()
        {
            var start = cellHeight / 2f;
            var end = height * cellHeight;

            for (float i = start; i < end; i += cellHeight)
            {
                var grid = GenerateGrid(cellWidth, i, length, width);
                dicGridCells.Add(i, grid);
            }

            dicKeys = dicGridCells.Keys.ToArray();

            for (int i = 0; i < dicKeys.Length - 1; i++)
            {
                foreach (var cell in dicGridCells[dicKeys[i]])
                {
                    var upperCellCenter = cell.Center.ChangeY(cell.Center.y + cellHeight);

                    foreach (var upperCell in dicGridCells[dicKeys[i + 1]])
                    {
                        if (upperCellCenter == upperCell.Center)
                        {
                            cell.SetUpperCell(upperCell);
                            break;
                        }
                    }
                }
            }

            firstLevelCells = dicGridCells[dicKeys[0]];

            if (debug)
            {
                for (int i = 0; i < dicKeys.Length; i++)
                {
                    var cells = dicGridCells[dicKeys[i]];

                    for (int y = 0; y < cells.Count; y++)
                    {
                        var debugObject = Instantiate(debugPrefab);
                        debugObject.transform.parent = transform.parent;
                        debugObject.transform.localPosition = cells[y].Center;
                        debugObject.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        public bool GetClosestPositionToObject(Vector3 position, out Vector3 closestPosition)
        {
            var correctPosition = transform.parent.InverseTransformPoint(position);
            var cell = GetClosestPosition(firstLevelCells, correctPosition, cellWidth);

            if (cell == null)
            {
                closestPosition = Vector3.zero;
                return false;
            }

            closestPosition = cell.TakeCellPosition();
            return true;
        }

        private Cell GetClosestPosition(List<Cell> cells, Vector3 position, float cellWidth)
        {
            Dictionary<float, Cell> dicCells = new Dictionary<float, Cell>();
            float radius = cellWidth / 2;

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].Locked && cells[i].WithObject == false)
                {
                    continue;
                }

                var center = cells[i].Center + transform.localPosition;
                float distance = Vector3.Distance(center, position);

                if (distance <= radius)
                {
                    dicCells.Add(distance, cells[i]);
                }
            }

            if (dicCells.Count == 0)
            {
                return null;
            }

            var key = dicCells.Keys.OrderBy(x => x).ToArray()[0];
            var cell = dicCells[key];

            while (cell.WithObject)
            {
                cell = cell.UpperCell;

                if (cell == null)
                {
                    break;
                }
            }

            return cell;
        }

        private List<Cell> GenerateGrid(float cellWidth, float cellHeight, float length, float width)
        {
            var dicRowsCells = new Dictionary<int, List<Cell>>();
            var listCells = new List<Cell>();

            float gridWidth = width * cellWidth;
            float gridLength = length * cellWidth;
            float halfCellWidth = cellWidth / 2f;
            int row = 0;
            int column;

            var startCenter = new Vector3(-(gridWidth / 2f - halfCellWidth), cellHeight, gridLength / 2f - halfCellWidth);

            for (float z = 0f; z < gridLength; z += cellWidth, row++)
            {
                var rowCells = new List<Cell>();

                column = 0;
                for (float x = 0f; x < gridWidth; x += cellWidth, column++)
                {
                    var step = new Vector3(x, 0f, -z);
                    var cell = new Cell(startCenter + step);

                    if (column != 0)
                    {
                        var wrongCenter = cell.Center.ChangeX(cell.Center.x - halfCellWidth);
                        var wrongCell = new Cell(wrongCenter);

                        wrongCell.AddNeighbor(cell);
                        wrongCell.AddNeighbor(rowCells[column - 1]);

                        cell.AddNeighbor(wrongCell);
                        rowCells[column - 1].AddNeighbor(wrongCell);

                        rowCells.Add(wrongCell);
                    }

                    rowCells.Add(cell);
                }

                dicRowsCells.Add(row, rowCells);

                if (row == 0)
                {
                    continue;
                }

                var prevRowCells = dicRowsCells[row - 1];
                var middleRowCells = new List<Cell>();

                for (column = 0; column < rowCells.Count; column++)
                {
                    var middleCellCenter = rowCells[column].Center;
                    var middleCell = new Cell(middleCellCenter.ChangeZ(middleCellCenter.z + halfCellWidth));

                    middleCell.AddNeighbor(prevRowCells[column]);
                    middleCell.AddNeighbor(rowCells[column]);

                    prevRowCells[column].AddNeighbor(middleCell);
                    rowCells[column].AddNeighbor(middleCell);

                    middleRowCells.Add(middleCell);

                    if (column == 0)
                    {
                        continue;
                    }

                    middleCell.AddNeighbor(prevRowCells[column - 1]);
                    middleCell.AddNeighbor(rowCells[column - 1]);
                    middleCell.AddNeighbor(middleRowCells[column - 1]);

                    prevRowCells[column].AddNeighbor(middleRowCells[column - 1]);
                    rowCells[column].AddNeighbor(middleRowCells[column - 1]);

                    prevRowCells[column - 1].AddNeighbor(middleCell);
                    rowCells[column - 1].AddNeighbor(middleCell);

                    middleRowCells[column - 1].AddNeighbor(middleCell);
                    middleRowCells[column - 1].AddNeighbor(prevRowCells[column]);
                    middleRowCells[column - 1].AddNeighbor(rowCells[column]);
                }

                listCells.AddRange(middleRowCells);
            }

            foreach (var value in dicRowsCells.Values)
            {
                listCells.AddRange(value);
            }

            return listCells;
        }
    }
}
