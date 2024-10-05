
using UnityEngine;

namespace Qutility.Type
{
    [System.Serializable]
    public class BoolMatrix
    {
        public const char trueC = '1';
        public const char falseC = '0';

        public static char BoolChar(bool value) => value ? trueC : falseC;

        public string[] matrix;

        public BoolMatrix(int matrixSize)
        {
            matrix = new string[matrixSize];
            for (int i = 0; i < matrixSize; i++)
            {
                matrix[i] = new string(falseC, matrixSize);
            }
        }

        public bool this[int y, int x]
        {
            get => matrix[y][x] == trueC;
            set
            {
                char[] charArray = matrix[y].ToCharArray();
                charArray[x] = BoolChar(value);
                // Convert the char array back to a string
                matrix[y] = new string(charArray);
            }
        }

        public int GetLength(int demension)
        {
            if (demension > 1) return 0;
            if (demension == 1) return matrix[0].Length;
            return matrix.Length;
        }

        public bool[,] ToMatrix()
        {
            int rows = GetLength(0);
            int cols = GetLength(1);

            bool[,] target = new bool[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    target[i, j] = matrix[i][j] == trueC;
                }
            }

            return target;
        }
    }
}

