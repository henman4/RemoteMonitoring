Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System.IO

Public Class Form3

    Dim mReader As BinaryReader
    Dim mWriter As BinaryWriter = Nothing
    Const ListenPort As Int16 = 9876
    Const RequestPort As Int16 = 6789
    Shared NoofClients As Int16 = 0 'Stores the Number of Image Sender Connected


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
                AddToListBox(myEndPoint.Address.ToString)
                'Stores it Address in the List Box With a Delegate
                TempClient.Close()
                'Closes the Client

            End While


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Public Delegate Sub AddItemDelegate(ByVal AddThis As String)

    Sub AddToListBox(ByVal AddThis As String)
        'A Delegate to Store the address of the incomming connection in a listbox
        If Me.ListBox1.InvokeRequired = True Then
            Dim NewAdd As New AddItemDelegate(AddressOf AddToListBox)
            ListBox1.Invoke(NewAdd, New Object() {AddThis})
        Else
            ListBox1.Items.Add(AddThis)
        End If

    End Sub


    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim ListenThread As New Thread(New ThreadStart(AddressOf ListenAlways))
        ListenThread.Start()

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex < 0 Then Exit Sub

        Dim FilName As String = Environment.CurrentDirectory & "\" & "Mohy_Screen_shot.bmp"
        Dim fStram As New FileStream(FilName, FileMode.Create)
        'Creates the File Where we are going to receive the File From the Sender

        Dim ImageSenderAddress As IPAddress = Dns.GetHostAddresses(ListBox1.SelectedItem)(0)
        'Gets the IPAddress Where i have to Connect

        Dim ClientToSee As TcpClient
        Try
            ClientToSee = New TcpClient()
            ClientToSee.Connect(ImageSenderAddress, RequestPort)
            'Connects to the Image Sender

        Catch ex As Exception
            MsgBox("Sorry !!!,Please ReStart the Sender")
            ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
            fStram.Close()
            Exit Sub
        End Try
        Try

            Dim NStream As NetworkStream = New NetworkStream(ClientToSee.Client)
            'Gets the Stream to Communicate

            mReader = New BinaryReader(NStream)
            Dim buffer(1024 - 1) As Byte
            Do While True
                Dim bytesRead As Integer = mReader.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then Exit Do
                fStram.Write(buffer, 0, bytesRead)
                fStram.Flush()
            Loop

            'Gets the Screen Shot and Closes the Stream
            fStram.Close()
            fStram.Dispose()
            fStram = Nothing
            ClientToSee.Close()

            'Finally Showing the Screen Shot in the Picture Box
            Dim fs As New System.IO.FileStream(FilName, IO.FileMode.Open, IO.FileAccess.Read)
            Dim imgCurrentPhoto As Image = Image.FromStream(fs)
            PictureBox1.Image = imgCurrentPhoto
            fs.Dispose()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If ListBox1.SelectedIndex < 0 Then Exit Sub

        Dim FilName As String = Environment.CurrentDirectory & "\" & "Mohy_Screen_shot.bmp"
        Dim fStram As New FileStream(FilName, FileMode.Create)
        'Creates the File Where we are going to receive the File From the Sender

        Dim ImageSenderAddress As IPAddress = Dns.GetHostAddresses(ListBox1.SelectedItem)(0)
        'Gets the IPAddress Where i have to Connect

        Dim ClientToSee As TcpClient
        Try
            ClientToSee = New TcpClient()
            ClientToSee.Connect(ImageSenderAddress, RequestPort)
            'Connects to the Image Sender

        Catch ex As Exception
            MsgBox("Sorry !!!,Please ReStart the Sender")
            ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
            fStram.Close()
            Exit Sub
        End Try
        Try

            Dim NStream As NetworkStream = New NetworkStream(ClientToSee.Client)
            'Gets the Stream to Communicate

            mReader = New BinaryReader(NStream)
            Dim buffer(1024 - 1) As Byte
            Do While True
                Dim bytesRead As Integer = mReader.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then Exit Do
                fStram.Write(buffer, 0, bytesRead)
                fStram.Flush()
            Loop

            'Gets the Screen Shot and Closes the Stream
            fStram.Close()
            fStram.Dispose()
            fStram = Nothing
            ClientToSee.Close()

            'Finally Showing the Screen Shot in the Picture Box
            Dim fs As New System.IO.FileStream(FilName, IO.FileMode.Open, IO.FileAccess.Read)
            Dim imgCurrentPhoto As Image = Image.FromStream(fs)
            PictureBox1.Image = imgCurrentPhoto
            fs.Dispose()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Timer1.Stop()
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Me.WindowState = FormWindowState.Maximized
        PictureBox1.Dock = DockStyle.Fill
    End Sub

    Private Sub PictureBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.DoubleClick
        Me.WindowState = FormWindowState.Normal
        PictureBox1.Dock = DockStyle.None
    End Sub
End Class