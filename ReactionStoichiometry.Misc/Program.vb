Imports System.IO

Module Program
    Sub Main()
        SerializationDemo()
    End Sub

    Private Sub SerializationDemo()
        Const equation = "CO+CO2+H2=CH4+H2O"

        Dim balancer1 As New BalancerGeneralized(equation)
        balancer1.Run()
        File.WriteAllText("balancer1.json", balancer1.ToString(Balancer.OutputFormat.Json))

        Dim balancer2 As New BalancerInverseBased(equation)
        balancer2.Run()
        File.WriteAllText("balancer2.json", balancer2.ToString(Balancer.OutputFormat.Json))
    End Sub
End Module
