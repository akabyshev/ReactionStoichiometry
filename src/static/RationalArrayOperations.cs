namespace ReactionStoichiometry;

using Rationals;

internal static class RationalArrayOperations
{
    internal static Rational[,] GetSpecialForm(Rational[,] source)
    {
        var result = (Rational[,])source.Clone();

        var rowCount = result.GetLength(dimension: 0);
        var columnCount = result.GetLength(dimension: 1);

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

        Normalize(ref result);
        return result;
    }

    internal static Rational[,] GetInverse(Rational[,] matrix)
    {
        // todo: see sticky notes
        var n = matrix.GetLength(dimension: 0);
        if (n != matrix.GetLength(dimension: 1)) throw new ArgumentException(message: "Non-square matrix passed to method");

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

            BalancerException.ThrowIf(pivot == 0, message: "Singular matrix");

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

    internal static void Normalize(ref Rational[,] array)
    {
        var indexLastCopiedRow = array.GetLength(dimension: 0) - 1;
        while (indexLastCopiedRow >= 0)
        {
            var foundNonZero = false;
            for (var c = 0; c < array.GetLength(dimension: 1); c++)
            {
                if (array[indexLastCopiedRow, c].IsZero) continue;
                foundNonZero = true;
                break;
            }
            if (foundNonZero) break;
            indexLastCopiedRow--;
        }

        if (indexLastCopiedRow < 0) throw new InvalidOperationException(message: "All-zeroes matrix");

        var newArray = new Rational[indexLastCopiedRow + 1, array.GetLength(dimension: 1)];
        for (var r = 0; r < newArray.GetLength(dimension: 0); r++)
        {
            for (var c = 0; c < newArray.GetLength(dimension: 1); c++)
            {
                newArray[r, c] = array[r, c].CanonicalForm;
            }
        }

        array = newArray;
    }
}
