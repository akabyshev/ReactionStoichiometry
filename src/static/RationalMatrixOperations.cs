namespace ReactionStoichiometry;

using System.Diagnostics;
using Rationals;

internal static class RationalMatrixOperations
{
    internal static void TrimAndGetCanonicalForms(ref Rational[,] matrix)
    {
        var indexLastRowToCopy = matrix.RowCount() - 1;
        while (indexLastRowToCopy >= 0 && matrix.Row(indexLastRowToCopy).All(predicate: static r => r == 0))
            indexLastRowToCopy--;

        if (indexLastRowToCopy == -1)
            throw new InvalidOperationException(message: "All-zeroes matrix");

        var newArray = new Rational[indexLastRowToCopy + 1, matrix.ColumnCount()];
        for (var r = 0; r < newArray.RowCount(); r++)
            for (var c = 0; c < newArray.ColumnCount(); c++)
                newArray[r, c] = matrix[r, c].CanonicalForm;

        matrix = newArray;
    }

    // ReSharper disable once InconsistentNaming
    internal static void TurnIntoREF(this Rational[,] matrix)
    {
        // Gaussian elimination
        var leadColumnIndex = 0;
        for (var r = 0; r < matrix.RowCount(); r++)
        {
            if (leadColumnIndex >= matrix.ColumnCount())
                break;

            var i = r;
            while (matrix[i, leadColumnIndex].IsZero)
            {
                i++;

                if (i != matrix.RowCount())
                    continue;

                i = r;

                if (leadColumnIndex >= matrix.ColumnCount() - 1)
                    break;

                leadColumnIndex++;
            }

            for (var c = 0; c < matrix.ColumnCount(); c++)
                (matrix[r, c], matrix[i, c]) = (matrix[i, c], matrix[r, c]);

            var div = matrix[r, leadColumnIndex];
            if (div != 0)
                for (var c = 0; c < matrix.ColumnCount(); c++)
                    matrix[r, c] = (matrix[r, c] / div).CanonicalForm;

            Parallel.For(fromInclusive: 0
                       , matrix.RowCount()
                       , k =>
                         {
                             // ReSharper disable twice AccessToModifiedClosure
                             if (k == r)
                                 return;
                             var factor = matrix[k, leadColumnIndex];
                             for (var c = 0; c < matrix.ColumnCount(); c++)
                                 matrix[k, c] = (matrix[k, c] - factor * matrix[r, c]).CanonicalForm;
                         });
            leadColumnIndex++;
        }
    }

    internal static Rational[,] GetInverse(Rational[,] matrix)
    {
        var size = matrix.RowCount();
        if (size != matrix.ColumnCount())
            throw new ArgumentException(message: "Non-square matrix");

        var augmentedMatrix = new Rational[size, 2 * size];
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                augmentedMatrix[i, j] = matrix[i, j];
                augmentedMatrix[i, j + size] = i == j ? 1 : 0;
            }
        }

        augmentedMatrix.TurnIntoREF();

        var leftHalf = new Rational[size, size];
        var rightHalf = new Rational[size, size];
        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                leftHalf[r, c] = augmentedMatrix[r, c];
                rightHalf[r, c] = augmentedMatrix[r, c + size];
            }
        }

        try
        {
            BalancerException.ThrowIf(!leftHalf.IsIdentityMatrix(), message: "Singular matrix");
            return rightHalf;
        }
        catch
        {
            Debug.WriteLine($"Exception at {nameof(GetInverse)}");
            Debug.WriteLine(augmentedMatrix.Readable(nameof(augmentedMatrix)));
            throw;
        }
    }
}
