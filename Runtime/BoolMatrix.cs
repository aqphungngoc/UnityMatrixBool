using UnityEngine;

namespace Qutility.Type
{
    [System.Serializable]
    public class BoolMatrix
    {
        [SerializeField] bool isYup;
        [SerializeField] byte[] matrix;
        [SerializeField] int size;

        public BoolMatrix(int matrixSize)
        {
            isYup = false;
            size = matrixSize;
            // Each byte can store 8 boolean values
            matrix = new byte[(matrixSize * matrixSize + 7) / 8];
        }

        public bool IsDisplayAsScreenCoordinate => isYup;
        public bool this[int row, int col]
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

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    target[i, j] = this[i, j];
                }
            }

            return target;
        }
    }
}