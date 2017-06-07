Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Public Class Form1
    Dim serverSocket As Socket
    Dim acceptThread As Thread
    Public ServerOut As String
    Dim lc As ListenClass
    Delegate Sub setTextDel(ByVal tmpStr As String)
    Dim win As Integer
    Dim lose As Integer
    Dim pin As Integer
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim hostName As String

        hostName = Dns.GetHostName
        Dim listIP() As IPAddress
        listIP = Dns.GetHostEntry(hostName).AddressList
        Dim i As Integer
        Dim idx As Integer
        For i = 0 To listIP.Length - 1
            ComboBox1.Items.Add(listIP(i))
            If listIP(i).ToString.IndexOf(".") <> -1 Then
                idx = i
            End If
        Next
        ComboBox1.SelectedIndex = idx
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not lc.clientSocket Is Nothing Then
            lc.clientSocket.Close()
        End If
        If Not lc.serverSocket Is Nothing Then
            lc.serverSocket.Close()
        End If
        If Not acceptThread Is Nothing Then
            acceptThread.Abort()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim hostName As String

            hostName = Dns.GetHostName
            Dim listIP() As IPAddress
            listIP = Dns.GetHostEntry(hostName).AddressList
            Dim i As Integer
            Dim idx As Integer
            For i = 0 To listIP.Length - 1
                ComboBox1.Items.Add(listIP(i))
                If listIP(i).ToString.IndexOf(".") <> -1 Then
                    idx = i
                End If
            Next
            ComboBox1.SelectedIndex = idx
            serverSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Dim serverIP As IPAddress = IPAddress.Parse(ComboBox1.Text)
            Dim serverhost As New IPEndPoint(serverIP, TextBox1.Text)
            serverSocket.Bind(serverhost)
            serverSocket.Listen(10)
            Console.WriteLine("Server is Listening")
            lc = New ListenClass(serverSocket, Me)
            acceptThread = New Thread(AddressOf lc.ServerThreadProc)
            acceptThread.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK)
        End Try

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim count As Integer = 5

        If Label4.Text = "對手準備好了" Then
            Dim t As New Thread(AddressOf changeLabel)
            t.Start()
            PictureBox2.Image = Nothing
        End If

    End Sub
    Private Sub changeLabel()
        Dim msg As Byte()
        Dim count As Integer = 5
        Do While count <> -1
            msg = Encoding.Default.GetBytes("Sysm" & count)
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
            setLabel(count)
            Thread.Sleep(1000)
            Console.WriteLine(count)
            count -= 1
        Loop
        msg = Encoding.Default.GetBytes("Fini")
        lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        setLabel("出拳時間結束")
         Select ServerOut
            Case "剪刀"
                Select Case lc.ClientOut
                    Case "剪刀"
                        setLabel("平手")
                        pin += 1
                    Case "石頭"
                        setLabel("你輸了")
                        lose += 1
                    Case "布"
                        setLabel("你贏了")
                        win += 1
                End Select
            Case "石頭"
                Select Case lc.ClientOut
                    Case "剪刀"
                        setLabel("你贏了")
                        win += 1
                    Case "石頭"
                        setLabel("平手")
                        pin += 1
                    Case "布"
                        setLabel("你輸了")
                        lose += 1
                End Select
            Case "布"
                Select Case lc.ClientOut
                    Case "布"
                        setLabel("平手")
                        pin += 1
                    Case "剪刀"
                        setLabel("你輸了")
                        lose += 1
                    Case "石頭"
                        setLabel("你贏了")
                        win += 1
                End Select

        End Select
        setLabel7("")
        Select Case lc.ClientOut
            Case "布"
                PictureBox2.Image = My.Resources.布
            Case "剪刀"
                PictureBox2.Image = My.Resources.剪刀
            Case "石頭"
                PictureBox2.Image = My.Resources.石頭
        End Select
    End Sub
    Public Sub setLabel(ByVal tmpStr As String)
        If Label4.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setLabel)
            Invoke(d, tmpStr)
        Else
            Label4.Text = tmpStr
        End If
    End Sub
    Private Sub setLabel7(ByVal tmpStr As String)
        Console.WriteLine("test")
        If Label7.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setLabel7)
            Invoke(d, tmpStr)
        Else
            Label7.Text = "勝:" & win & "     敗:" & lose & "    平手:" & pin
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            PictureBox1.Image = My.Resources.剪刀
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:剪刀")
            ServerOut = "剪刀"
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            PictureBox1.Image = My.Resources.石頭
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:石頭")
            ServerOut = "石頭"
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            PictureBox1.Image = My.Resources.布
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:布")
            ServerOut = "布"
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim msg As Byte()
        msg = Encoding.Default.GetBytes("Mesg" & TextBox2.Text)
        lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.Click
        If RadioButton3.Checked = True Then
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("Rdoc" & 3)
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.Click
        If RadioButton2.Checked = True Then
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("Rdoc" & 2)
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.Click
        If RadioButton1.Checked Then
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("Rdoc" & 1)
            lc.clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        End If
    End Sub
End Class
Public Class ListenClass
    Private f1 As Form1
    Public ClientOut As String '紀錄Client出的拳
    Delegate Sub setTextDel(ByVal tmpStr As String)
    Public clientSocket As Socket
    Public serverSocket As Socket
    Public Sub New(ByVal tmpSocket As Socket, ByVal tmpForm1 As Form1)
        serverSocket = tmpSocket
        f1 = tmpForm1
    End Sub
    Public Sub ServerThreadProc()
        Do While True
            Try
                clientSocket = serverSocket.Accept
                Dim t As New Thread(AddressOf receiveThreadProc)
                t.Start()
            Catch ex As Exception

            End Try
        Loop
    End Sub
    Private Sub receiveThreadProc()
        Try
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do
                rcvBytes = clientSocket.Receive(bytes, 0, bytes.Length, SocketFlags.None)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Console.WriteLine(tmpStr)
                Select Case tmpStr.Substring(0, 4)

                    Case "RADY"
                        setLabel("對手準備好了")
                    Case "out:"
                        ClientOut = tmpStr.Substring(4)
                    Case "Mesg"
                        setText(tmpStr.Substring(4))

                End Select


            Loop While clientSocket.Available <> 0 Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
    Private Sub setText(ByVal tmpStr As String)
        If f1.TextBox2.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setText)
            f1.Invoke(d, tmpStr)
        Else
            f1.TextBox2.Text = tmpStr
        End If
    End Sub
    Public Sub setLabel(ByVal tmpStr As String)
        If f1.Label4.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setLabel)
            f1.Invoke(d, tmpStr)
        Else
            f1.Label4.Text = tmpStr
        End If
    End Sub
End Class


