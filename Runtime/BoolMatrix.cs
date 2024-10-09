using UnityEngine;

namespace Qutility.Type
{
    [System.Serializable]
    public class BoolMatrix
    {
        [SerializeField] bool isYup;
        [SerializeField] byte[] matrix;
        [SerializeField] int size;
        public int Size => size;
        public bool IsDisplayAsScreenCoordinate => isYup;
        public BoolMatrix(int matrixSize)
        {
            isYup = false;
            size = matrixSize;
            // Each byte can store 8 boolean values
            matrix = new byte[(matrixSize * matrixSize + 7) / 8];
        }

        public bool this[int col, int row]
        {
            get => (matrix[(row * size + col) / 8] & (1 << ((row * size + col) % 8))) != 0;
            set
            {
                if (value)
                    matrix[(row * size + col) / 8] |= (byte)(1 << ((row * size + col) % 8));
                else
                    matrix[(row * size + col) / 8] &= (byte)~(1 << ((row * size + col) % 8));
            }
        }

        public bool[,] ToMatrix()
        {
            bool[,] target = new bool[size, size];

            for (int col = 0; col < size; col++)
            {
                for (int row = 0; row < size; row++)
                {
                    target[col, row] = this[col, row];
                }
            }

            return target;
        }
    }
}