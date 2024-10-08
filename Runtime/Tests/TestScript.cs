using NUnit.Framework;
using Qutility.Type;
using UnityEngine;

public class TestScript
{
    [Test]
    public void TestScriptSimplePasses()
    {
        // Initialize the BoolMatrix with a size of 4
        BoolMatrix boolMatrix = new BoolMatrix(4);

        // Set some initial values
        boolMatrix[0, 0] = true;
        boolMatrix[1, 1] = true;
        boolMatrix[2, 2] = true;
        boolMatrix[3, 3] = true;

        // Assert the initial values
        Assert.IsTrue(boolMatrix[0, 0], "Expected (0,0) to be true");
        Assert.IsTrue(boolMatrix[1, 1], "Expected (1,1) to be true");
        Assert.IsTrue(boolMatrix[2, 2], "Expected (2,2) to be true");
        Assert.IsTrue(boolMatrix[3, 3], "Expected (3,3) to be true");

        // Check some initial false values
        Assert.IsFalse(boolMatrix[0, 1], "Expected (0,1) to be false");
        Assert.IsFalse(boolMatrix[3, 2], "Expected (3,2) to be false");

        // Print the original matrix
        Debug.Log("Original BoolMatrix:");
        PrintMatrix(boolMatrix.ToMatrix());

        // Modify some values
        boolMatrix[0, 1] = false; // Setting (0, 1) to false
        boolMatrix[3, 2] = false; // Setting (3, 2) to false

        // Assert modified values
        Assert.IsFalse(boolMatrix[0, 1], "Expected (0,1) to be false after modification");
        Assert.IsFalse(boolMatrix[3, 2], "Expected (3,2) to be false after modification");

        // Print the modified matrix
        Debug.Log("Modified BoolMatrix:");
        PrintMatrix(boolMatrix.ToMatrix());

        // Additional assertions after modification
        Assert.IsTrue(boolMatrix[0, 0], "Expected (0,0) to remain true after modification");
        Assert.IsTrue(boolMatrix[1, 1], "Expected (1,1) to remain true after modification");
        Assert.IsTrue(boolMatrix[2, 2], "Expected (2,2) to remain true after modification");
        Assert.IsTrue(boolMatrix[3, 3], "Expected (3,3) to remain true after modification");
    }

    private void PrintMatrix(bool[,] matrix)
    {
        int size = matrix.GetLength(0);
        for (int i = 0; i < size; i++)
        {
            string row = "";
            for (int j = 0; j < size; j++)
            {
                row += matrix[i, j] ? "1 " : "0 ";
            }

            Debug.Log(row);
        }
    }
}