Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.IO

Public Class Run
    Dim trSendMessage As Thread
    Dim trClose As Thread
    Dim t = Form1.ComboBox1.SelectedItem
    Sub closeRun()
        Dim host As String = t
        Dim port As Integer = 63000
        Try
            Dim tcpCli As New TcpClient(host, port)
            Dim ns As NetworkStream = tcpCli.GetStream

            ' Send data to the server.
            Dim sw As New StreamWriter(ns)
            sw.WriteLine("Close")
            sw.WriteLine(TextBox1.Text)
            'End If
            sw.Flush()

            ' Receive and display data.
            Dim sr As New StreamReader(ns)
            Dim result As String = sr.ReadLine()
            If result = "###OK###" Then
                MsgBox("Operation Performed!!!", MsgBoxStyle.Information, "Accepted by client")
            End If
            'MsgBox(result)
            sr.Close()
            sw.Close()
            ns.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub SendMessage()
        Dim host As String = t
        Dim port As Integer = 63000
        Try
            Dim tcpCli As New TcpClient(host, port)
            Dim ns As NetworkStream = tcpCli.GetStream

            ' Send data to the server.
            Dim sw As New StreamWriter(ns)
            sw.WriteLine("RUN")
            sw.WriteLine(TextBox1.Text)
            sw.Flush()

            ' Receive and display data.
            Dim sr As New StreamReader(ns)
            Dim result As String = sr.ReadLine()
            If result = "###OK###" Then
                MsgBox("Operation Performed!!!", MsgBoxStyle.Information, "Accepted by client")
            End If
            'MsgBox(result)
            sr.Close()
            sw.Close()
            ns.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        trSendMessage = New Thread(AddressOf SendMessage)
        trSendMessage.Start()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        trClose = New Thread(AddressOf closeRun)
        trClose.Start()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        End
    End Sub
End Class