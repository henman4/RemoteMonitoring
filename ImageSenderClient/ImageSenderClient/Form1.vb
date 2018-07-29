
Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Text
Imports System.IO

Public Class Form1

    Dim trlisten As Thread
    Dim trShutdown As Thread
    Dim trClose As Thread
    Dim trReboot As Thread
    Dim trLogOff As Thread
    Dim trRun As Thread
    Dim r As String

    Const ConnectionPort As Int16 = 9876 'Connection Port Number
    Const RequestPort As Int16 = 6789 'Request Port Number

    Dim ServerIp As String

    Dim NetStream As NetworkStream
    Dim myReader As BinaryReader
    Dim myWriter As BinaryWriter

    Dim Look4Request As Thread = Nothing


    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.NotifyIcon1.Visible = False
        Me.NotifyIcon1.Dispose()
        System.Environment.Exit(System.Environment.ExitCode) ' Informs to the Server When it is Closed
    End Sub


    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'Stores the ImageReceiver Address To Connect
        ServerIp = InputBox("Enter the Image Receiver IPAddress", "Image Receiver IPAddress", "127.0.0.1").Trim
        If ServerIp = "" Then
            MsgBox("Image Receiver IP Can't be Empty", MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical)
            End
        End If

        'Informs the Image Receiver That it is Connected
        Try
            Dim myClient As New TcpClient
            myClient.Connect(ServerIp, ConnectionPort)
            myClient.Close()
        Catch ex As Exception
            MsgBox("Please Start the Receiver", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly)

            Me.NotifyIcon1.Dispose()
            End

        End Try
       


        'Creates a Thread To Listen 4 the Request of the Receiver 
        Look4Request = New Thread(New ThreadStart(AddressOf WaitForRequest))
        Look4Request.Start()


    End Sub


    Sub WaitForRequest()

        'Gets the Receiver Address
        Dim ServerAddress As IPAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList(0)
        Dim myListener As New TcpListener(ServerAddress, RequestPort)
        myListener.Start()

        'If Connected Gets the Client Stream
        Try
            While (True)
                Try
                    Dim myClient As TcpClient = myListener.AcceptTcpClient
                    NetStream = myClient.GetStream
                    Send_Screen_Shot() 'Sends the Screen Shot of the Desktop
                    myClient.Close()
                Catch ex As Exception
                    Exit While
                End Try

            End While

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            myReader.Close()
            NetStream.Close()
        End Try

    End Sub

    Sub Send_Screen_Shot()


        Dim FileName As String = Environment.CurrentDirectory & "\Mohiudeen_Screen.bmp"

        ScreenCapture.CurrentScreen() 'Capture the Current Screen

        If File.Exists(FileName) Then
            File.Delete(FileName)
        End If

        ScreenCapture.oBitMap.Save(FileName) 'Saves it to a File
        ScreenCapture.oBitMap = Nothing

        'Then,Sends the File to the Image Receiver
        Using FStreams As New FileStream(FileName, FileMode.Open)

            Dim buffer(1024 - 1) As Byte

            Do While True
                Dim bytesRead As Integer = FStreams.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then Exit Do
                Me.NetStream.Write(buffer, 0, bytesRead)
                NetStream.Flush()
            Loop

            FStreams.Close()
            NetStream.Close()
        End Using
        'Finally Closes


    End Sub
    Sub shutdown()
        Dim t As Single
        Dim objWMIService, objComputer As Object
        'Now get some privileges
        objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")
        For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")

            t = objComputer.Win32Shutdown(8 + 4, 0)

            If t <> 0 Then
                'Error occurred!!!
            Else
                'Shutdown your system
            End If
        Next
    End Sub
    Sub CloseRun()
        Try
            Dim pProcess() As Process = System.Diagnostics.Process.GetProcessesByName(r)

            For Each p As Process In pProcess
                p.Kill()
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
            ' MsgBox("Cannot find  Please make sure that the file exists.", MsgBoxStyle.Critical, "Error")
        End Try

    End Sub

    Sub Run()
        Try

            Process.Start(r)
            My.Settings.Run = r
            My.Settings.Save()
            My.Settings.Reload()
        Catch ex As Exception
            MsgBox("Cannot find  Please make sure that the file exists.", MsgBoxStyle.Critical, "Error")
        End Try

    End Sub
    Sub reboot()
        Dim t As Single
        Dim objWMIService, objComputer As Object
        'Now get some privileges
        objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")
        For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")

            t = objComputer.Win32Shutdown(2 + 4, 0)

            If t <> 0 Then
                'Error occurred!!!
            Else
                'Reboot your system
            End If
        Next
    End Sub
    Sub logoff()
        Dim t As Single
        Dim objWMIService, objComputer As Object
        'Now get some privileges
        objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")
        For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")

            t = objComputer.Win32Shutdown(0, 0)

            If t <> 0 Then
                'Error occurred!!!
            Else
                'LogOff your system
            End If
        Next
    End Sub
    Sub ListenToServer()
        'Try

        Dim LISTENING As Boolean
        Dim localhostAddress As IPAddress = ipAddress.Parse(ipAddress.ToString)
        Dim port As Integer = 63000      '' PORT ADDRESS
        ''''''''''' making socket tcpList ''''''''''''''''
        Dim tcpList As New TcpListener(localhostAddress, port)
        Try
            tcpList.Start()
            LISTENING = True

            Do While LISTENING

                Do While tcpList.Pending = False And LISTENING = True
                    ' Yield the CPU for a while.
                    Thread.Sleep(10)
                Loop
                If Not LISTENING Then Exit Do

                Dim tcpCli As TcpClient = tcpList.AcceptTcpClient()
                'ListBox1.Items.Add("Data from " & "128.10.20.63")
                Dim ns As NetworkStream = tcpCli.GetStream
                Dim sr As New StreamReader(ns)
                ''''''''' get data from client '''''''''''''''
                Dim receivedData As String = sr.ReadLine()
                If receivedData = "###SHUTDOWN###" Then
                    trShutdown = New Thread(AddressOf shutdown)
                    trShutdown.Start()
                End If
                If receivedData = "###REBOOT###" Then
                    trReboot = New Thread(AddressOf reboot)
                    trReboot.Start()
                End If
                If receivedData = "###LOGOFF###" Then
                    trLogOff = New Thread(AddressOf logoff)
                    trLogOff.Start()
                End If

                If receivedData = "RUN" Then
                    r = sr.ReadLine()
                    trRun = New Thread(AddressOf Run)
                    trRun.Start()
                End If

                If receivedData = "Close" Then
                    r = sr.ReadLine()
                    trClose = New Thread(AddressOf CloseRun)
                    trClose.Start()
                End If


                Dim returnedData As String = "###OK###" '& " From Server"
                Dim sw As New StreamWriter(ns)
                sw.WriteLine(returnedData)

                sw.Flush()
                sr.Close()
                sw.Close()
                ns.Close()
                tcpCli.Close()
            Loop
            tcpList.Stop()
        Catch ex As Exception
            'error
            LISTENING = False
        End Try
    End Sub
    Dim ipAddress As IPAddress = Dns.Resolve(Dns.GetHostName()).AddressList(0)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        NotifyIcon1.Icon = Me.Icon
        trlisten = New Thread(AddressOf ListenToServer)
        trlisten.Start()
    End Sub



    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click
        Me.Show()
    End Sub
End Class
