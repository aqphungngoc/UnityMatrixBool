using System;
using UnityEngine;

namespace Qutility.Type
{
    [Serializable]
    public class Bool2DArray
    {
        [SerializeField] bool isYup;
        [SerializeField] byte[] matrix;
        [SerializeField] int rowCount, columnCount;
        public int RowCount => rowCount;
        public int ColumnCount => columnCount;
        public bool IsDisplayAsScreenCoordinate => isYup;
        public Bool2DArray(int _rowCount, int _columnCount)
        {
            isYup = false;
            rowCount = _rowCount;
            columnCount = _columnCount;
            // Each byte can store 8 boolean values
            matrix = new byte[(rowCount * columnCount + 7) / 8];
        }

        public bool this[int col, int row]
        {
            get => (matrix[(row * columnCount + col) / 8] & (1 << ((row * columnCount + col) % 8))) != 0;
            set
            {
                if (value)
                    matrix[(row * columnCount + col) / 8] |= (byte)(1 << ((row * columnCount + col) % 8));
                else
                    matrix[(row * columnCount + col) / 8] &= (byte)~(1 << ((row * columnCount + col) % 8));
            }
        }

        public bool[,] ToMatrix()
        {
            bool[,] target = new bool[columnCount, rowCount];

            for (int col = 0; col < columnCount; col++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    target[col, row] = this[col, row];
                }
            }

            return target;
        }
    }
}