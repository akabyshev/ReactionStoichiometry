namespace ReactionStoichiometry;

using Rationals;

internal static class RationalMatrixMath
{
    public static Int32 GetRank(Rational[,] source)
    {
        var matrix = (Rational[,])source.Clone();

        var rowCount = matrix.GetLength(0);
        var colCount = matrix.GetLength(1);

        var result = 0;

        for (var c = 0; c < colCount; c++)
        {
            var pivotRow = result;
            while (pivotRow < rowCount && matrix[pivotRow, c] == 0)
            {
                pivotRow++;
            }

            if (pivotRow >= rowCount) continue;

            if (pivotRow != result)
                for (var j = 0; j < colCount; j++)
                {
                    (matrix[result, j], matrix[pivotRow, j]) = (matrix[pivotRow, j], matrix[result, j]);
                }

            for (var i = 0; i < rowCount; i++)
            {
                if (i == result) continue;

                var factor = matrix[i, c] / matrix[result, c];
                for (var j = c; j < colCount; j++)
                {
                    matrix[i, j] -= factor * matrix[result, j];
                }
            }

            result++;
        }

        return result;
    }

    public static Rational GetDeterminant(Rational[,] matrix)
    {
        var size = matrix.GetLength(0);

        switch (size)
        {
            case 1: return matrix[0, 0];
            case 2: return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        Rational determinant = 0;
        for (var col = 0; col < size; col++)
        {
            Rational subDeterminant;
            var sign = col % 2 == 0 ? 1 : -1;

            var subMatrix = new Rational[size - 1, size - 1];
            for (var i = 1; i < size; i++)
            {
                var subMatrixCol = 0;
                for (var j = 0; j < size; j++)
                {
                    if (j == col) continue;
                    subMatrix[i - 1, subMatrixCol] = matrix[i, j];
                    subMatrixCol++;
                }
            }

            subDeterminant = GetDeterminant(subMatrix);
            determinant += sign * matrix[0, col] * subDeterminant;
        }

        return determinant;
    }

    public static Rational[,] GetInverse(Rational[,] matrix)
    {
        var n = matrix.GetLength(0);
        if (n != matrix.GetLength(1)) throw new ArgumentException("Augmented matrix can't be inverted");

        var augmentedMatrix = new Rational[n, 2 * n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                augmentedMatrix[i, j] = matrix[i, j];
                augmentedMatrix[i, j + n] = i == j ? Rational.One : Rational.Zero;
            }
        }


        for (var i = 0; i < n; i++)
        {
            var pivot = augmentedMatrix[i, i];

            if (pivot == 0) throw new ArgumentException("Singular matrix");

            for (var j = 0; j < 2 * n; j++)
            {
                augmentedMatrix[i, j] /= pivot;
            }

            for (var k = 0; k < n; k++)
            {
                if (k == i) continue;
                var factor = augmentedMatrix[k, i];
                for (var j = 0; j < 2 * n; j++)
                {
                    augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                }
            }
        }

        // Extract the right half of the augmented matrix
        var result = new Rational[n, n];
        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                result[r, c] = augmentedMatrix[r, c + n];
            }
        }
        return result;
    }

    public static Rational[,] GetReduced(Rational[,] source)
    {
        var matrix = (Rational[,])source.Clone();

        var rowCount = matrix.GetLength(0);
        var columnCount = matrix.GetLength(1);

        var leadColumnIndex = 0;
        for (var r = 0; r < rowCount; r++)
        {
            if (columnCount <= leadColumnIndex) break;

            var i = r;

            while (matrix[i, leadColumnIndex].IsZero)
            {
                i++;

                if (i < rowCount) continue;

                i = r;

                if (leadColumnIndex < columnCount - 1)
                    leadColumnIndex++;
                else
                    break;
            }

            if (i != r)
                for (var c = 0; c < columnCount; c++)
                {
                    (matrix[r, c], matrix[i, c]) = (matrix[i, c], matrix[r, c]);
                }

            var div = matrix[r, leadColumnIndex];
            if (!div.IsZero)
                for (var c = 0; c < columnCount; c++)
                {
                    matrix[r, c] = Rational.Divide(matrix[r, c], div);
                }

            for (var r2 = 0; r2 < rowCount; r2++)
            {
                if (r2 == r) continue;

                var sub = matrix[r2, leadColumnIndex];
                for (var c2 = 0; c2 < columnCount; c2++)
                {
                    matrix[r2, c2] = Rational.Subtract(matrix[r2, c2], Rational.Multiply(sub, matrix[r, c2]));
                }
            }

            leadColumnIndex++;
        }

        return Utils.WithoutTrailingZeroRows(matrix);
    }
}
