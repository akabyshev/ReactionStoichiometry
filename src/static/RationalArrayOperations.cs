namespace ReactionStoichiometry;

using Rationals;

internal static class RationalArrayOperations
{
    internal static Rational[,] GetRowEchelonForm(Rational[,] source)
    {
        var result = (Rational[,])source.Clone();

        var rowCount = result.RowCount();
        var columnCount = result.ColumnCount();

        var leadColumnIndex = 0;
        for (var r = 0; r < rowCount; r++)
        {
            if (columnCount <= leadColumnIndex) break;

            var i = r;

            while (result[i, leadColumnIndex].IsZero)
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
                    (result[r, c], result[i, c]) = (result[i, c], result[r, c]);
                }

            var div = result[r, leadColumnIndex];
            if (!div.IsZero)
                for (var c = 0; c < columnCount; c++)
                {
                    result[r, c] /= div;
                }

            for (var r2 = 0; r2 < rowCount; r2++)
            {
                if (r2 == r) continue;

                var sub = result[r2, leadColumnIndex];
                for (var c2 = 0; c2 < columnCount; c2++)
                {
                    result[r2, c2] -= sub * result[r, c2];
                }
            }

            leadColumnIndex++;
        }

        TrimAndGetCanonicalForms(ref result);
        return result;
    }

    internal static Rational[,] GetInverse(Rational[,] matrix)
    {
        var n = matrix.RowCount();
        if (n != matrix.ColumnCount()) throw new ArgumentException(message: "Non-square matrix");

        var augmentedMatrix = new Rational[n, 2 * n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                augmentedMatrix[i, j] = matrix[i, j];
                augmentedMatrix[i, j + n] = i == j ? 1 : 0;
            }
        }

        for (var i = 0; i < n; i++)
        {
            var pivot = augmentedMatrix[i, i];

            if (pivot == 0)
            {
                BalancerException.ThrowIf(GetDeterminant(matrix) != 0, message: "Assertion failed");
                BalancerException.ThrowIf(condition: true, message: "Singular matrix");
            }

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

    private static Rational GetDeterminant(Rational[,] matrix)
    {
        var n = matrix.RowCount();
        if (n != matrix.ColumnCount()) throw new ArgumentException(message: "Non-square matrix");

        switch (n)
        {
            case 1: return matrix[0, 0];
            case 2: return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        Rational determinant = 0;
        for (var col = 0; col < n; col++)
        {
            Rational subDeterminant;
            var sign = col % 2 == 0 ? 1 : -1;

            var subMatrix = new Rational[n - 1, n - 1];
            for (var i = 1; i < n; i++)
            {
                var subMatrixCol = 0;
                for (var j = 0; j < n; j++)
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

    internal static void TrimAndGetCanonicalForms(ref Rational[,] matrix)
    {
        var indexLastRowToCopy = matrix.RowCount() - 1;
        while (indexLastRowToCopy >= 0 && matrix.Row(indexLastRowToCopy).All(static r => r == 0))
        {
            indexLastRowToCopy--;
        }

        if (indexLastRowToCopy == -1) throw new InvalidOperationException(message: "All-zeroes matrix");

        var newArray = new Rational[indexLastRowToCopy + 1, matrix.ColumnCount()];
        for (var r = 0; r < newArray.RowCount(); r++)
        {
            for (var c = 0; c < newArray.ColumnCount(); c++)
            {
                newArray[r, c] = matrix[r, c].CanonicalForm;
            }
        }

        matrix = newArray;
    }
}
