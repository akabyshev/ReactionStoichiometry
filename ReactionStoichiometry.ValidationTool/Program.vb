Imports System.Numerics
Imports System.Text.RegularExpressions
Imports Rationals

Module Program
    Sub Main(args As String())
        Dim equationString As String

        If args.Length > 0 Then
            equationString = args(0)
        Else
            Console.WriteLine("Equation?")
            equationString = Console.ReadLine()
        End If

        If equationString = "" Then
            equationString =
                $"2·C6H5COOH + 15·O2 = 14·CO2 + 6·H2O"
        End If

        If Not IsReactionEquationBalanced(equationString) Then
            Environment.ExitCode = - 1
        End If
    End Sub

    Private Function IsReactionEquationBalanced(eq As String)
        Dim parts = eq.Split("="c, "+"c)
        Dim border = eq.Split("="c)(0).Split("+"c).Length
        Dim coefficients = New List(Of BigInteger)()
        For i As Integer = LBound(parts) To UBound(parts) Step 1
            Dim part As String = parts(i)
            Dim cAsString As String = IIf(part.Contains(GlobalConstants.MULTIPLICATION_SYMBOL),
                                          part.Split(GlobalConstants.MULTIPLICATION_SYMBOL)(0), "1")
            Dim cAsRational As Rational = Rational.ParseDecimal(cAsString)*
                                          IIf(i < border, New Rational(- 1), New Rational(1))
            If (cAsRational.Denominator <> 1) Then
                Throw New FormatException
            End If
            coefficients.Add(cAsRational.Numerator)
        Next

        Dim skeletal As String = Regex.Replace(eq, "\d+·", "")
        Console.WriteLine("Skeletal:" + skeletal)
        Console.WriteLine("Solution:" + eq)

        Dim equation As New ChemicalReactionEquation(skeletal)
        Dim verdict = equation.Validate(coefficients.ToArray())
        Console.WriteLine()
        Console.WriteLine(IIf(verdict, "Solution OK", "IT IS NOT BALANCED"))
        Console.WriteLine()
        Console.WriteLine()

        IsReactionEquationBalanced = verdict
    End Function
End Module
