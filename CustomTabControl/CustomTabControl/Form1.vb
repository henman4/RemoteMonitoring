Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System.IO

Public Class Form1
    Dim t As String
    Dim trSendMessage As Thread
    Dim mReader As BinaryReader
    Dim mWriter As BinaryWriter = Nothing
    Const ListenPort As Int16 = 9876
    Const RequestPort As Int16 = 6789
    Shared NoofClients As Int16 = 0 'Stores the Number of Image Sender Connected
    Dim a As String
    Sub SendMessage()
        Dim host As String = t
        Dim port As Integer = 63000
        Try
            Dim tcpCli As New TcpClient(host, port)
            Dim ns As NetworkStream = tcpCli.GetStream

            ' Send data to the server.
            Dim sw As New StreamWriter(ns)
            If a = "ShutDown" Then
                sw.WriteLine("###SHUTDOWN###")
            End If
            If a = "Reboot" Then
                sw.WriteLine("###REBOOT###")
            End If
            If a = "Logoff" Then
                sw.WriteLine("###LOGOFF###")
            End If
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



    Sub ListenAlways()

        'Listen 4 InComming Connections
        Dim MyListener As TcpListener
        Dim MyIp As IPAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList(0)
        MyListener = New TcpListener(MyIp, ListenPort)
        MyListener.Start()

        Try
            While (True)

                Application.DoEvents()

                Dim TempClient As TcpClient = MyListener.AcceptTcpClient
                'Accept the Client

                NoofClients += 1

                Dim myEndPoint As IPEndPoint = TempClient.Client.RemoteEndPoint
                AddToComboBox(myEndPoint.Address.ToString)
                'Stores it Address in the List Box With a Delegate
                TempClient.Close()
                'Closes the Client

            End While


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Public Delegate Sub AddItemDelegate(ByVal AddThis As String)

    Sub AddToComboBox(ByVal AddThis As String)
        'A Delegate to Store the address of the incomming connection in a listbox
        If Me.ComboBox1.InvokeRequired = True Then
            Dim NewAdd As New AddItemDelegate(AddressOf AddToComboBox)
            ComboBox1.Invoke(NewAdd, New Object() {AddThis})
        Else
            ComboBox1.Items.Add(AddThis)
        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TabControlClass1.SizeMode = TabSizeMode.Normal
        Dim ListenThread As New Thread(New ThreadStart(AddressOf ListenAlways))
        ListenThread.Start()
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        'Me.Hide()
        Form2.Show()
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        Me.Hide()
        Form3.Show()
    End Sub


    Private Sub PictureBox5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox5.Click
        t = ComboBox1.SelectedItem
        a = "Logoff"
        trSendMessage = New Thread(AddressOf SendMessage)
        trSendMessage.Start()

    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        t = ComboBox1.SelectedItem
        a = "ShutDown"
        trSendMessage = New Thread(AddressOf SendMessage)
        trSendMessage.Start()

    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        t = ComboBox1.SelectedItem
        a = "Reboot"
        trSendMessage = New Thread(AddressOf SendMessage)
        trSendMessage.Start()
    End Sub

    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
        t = ComboBox1.SelectedItem
        Run.Show()

    End Sub
End Class
