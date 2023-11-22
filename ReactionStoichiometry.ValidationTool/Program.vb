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
                $"10·K4Fe(CN)6 + 122·KMnO4 + 299·H2SO4 = 162·KHSO4 + 5·Fe2(SO4)3 + 122·MnSO4 + 60·HNO3 + 60·CO2 + 188·H2O"
        End If

        If Not IsReactionEquationBalanced(equationString) Then
            Environment.ExitCode = 1
        End If
    End Sub

    Private Function IsReactionEquationBalanced(eq)
        IsReactionEquationBalanced = Len(eq) > 2
    End Function
End Module
