Imports System

Module Program
    Sub Main(args As String())
        Dim eq As String

        If args.Length > 0 Then
            eq = args(0)
        Else
            Console.WriteLine("Equation?")
            eq = Console.ReadLine()
        End If
    End Sub
End Module
