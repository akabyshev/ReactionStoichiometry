Imports System
Imports System.IO
Imports System.Xml.Serialization

Module Program
    Sub Main()
        Ser()
        ' DetailedMultilineToMarkdown("D:\Solutions\ReactionStoichiometry\batchdata\DetailedMultiline-BalancerGeneralized.txt")
        ' Console.WriteLine("Hello World!")
    End Sub

    Private Sub DetailedMultilineToMarkdown(ByVal filePath As String)
        Using reader As New StreamReader(filePath)
            While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()

            End While
        End Using
    End Sub

    Private Sub Ser()
        Dim balancer As New BalancerGeneralized("H2+O2=H2O")

        balancer.Run()

        ' Create an XmlSerializer for YourClass
        Dim serializer As New XmlSerializer(GetType(BalancerGeneralized))

        ' Serialize the instance to a file
        Using fs As New FileStream("yourfile.xml", FileMode.Create)
            serializer.Serialize(fs, balancer)
        End Using

        Console.WriteLine("Serialization complete.")
    End Sub
End Module
