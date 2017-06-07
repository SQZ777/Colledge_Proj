Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Random
Public Class Form4
    Dim lc As ListenClass4
    Dim GameB(25) As Button         '每一個格子(按鈕)
    Dim ifApearYou As Thread
    Public acceptThread As Thread
    Public clientThread As Thread
    'Public clientSocket As Socket
    'Public ServerSocket As Socket
    Public tcpClient As TcpClient
    Dim networkStream As NetworkStream
    Public tcpListener As TcpListener
    Public BlockGameB(25) As Integer   '已被選取的格子
    Public Winline As Integer          '已有幾條線
    Public NeedLine As Integer      '需要幾條線才能贏
    Public GAMEMODE As Integer
    Public GameOver As Boolean = False
    Delegate Sub setForm(ByVal tmpStr As String)
    'Move Form2's sub
    Dim loc As Point
    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            loc = e.Location
        End If
    End Sub
    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += e.Location - loc
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Me.Close()
        Catch ex As Exception

        End Try

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) Handles Label1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            loc = e.Location
        End If
    End Sub
    Private Sub Label1_MouseMove(sender As Object, e As MouseEventArgs) Handles Label1.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += e.Location - loc
        End If
    End Sub
    'End Move Form2's sub
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim i As Integer
        For i = 0 To 24
            GameB(i) = Me.Controls("Button" & i + 4)
        Next
        If Form1.RadioButton1.Checked Then
            GAMEMODE = 3
        ElseIf Form1.RadioButton2.Checked Then
            GAMEMODE = 4
        ElseIf Form1.RadioButton3.Checked Then
            GAMEMODE = 5
        End If
        If Form1.CheckBox1.Checked Then
            Label1.Text &= "(Host)"
            Me.Text = Label1.Text
            Try
                Dim hostName As String
                hostName = Dns.GetHostName
                Dim listIP() As IPAddress
                listIP = Dns.GetHostEntry(hostName).AddressList
                Dim idx As Integer
                For i = 0 To listIP.Length - 1
                    Form1.ComboBox1.Items.Add(listIP(i))
                    If listIP(i).ToString.IndexOf(".") <> -1 Then
                        idx = i
                    End If
                Next
                Form1.ComboBox1.SelectedIndex = idx
                'ServerSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                Dim serverIP As IPAddress = IPAddress.Parse(Form1.ComboBox1.Text)
                Dim serverhost As New IPEndPoint(serverIP, 888)
                'ServerSocket.Bind(serverhost)
                'ServerSocket.Listen(10)
                Console.WriteLine("Server is Listening")
                tcpListener = New TcpListener(serverhost)
                tcpListener.Start(10)
                lc = New ListenClass4(tcpListener, Me)
                acceptThread = New Thread(AddressOf lc.ServerThreadProc)
                acceptThread.Start()
                Console.WriteLine("Server got client")
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK)
            End Try
        Else
            Label1.Text &= "(Client)"
            Me.Text = Label1.Text
            Try
                'clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                Dim serverIP As IPAddress = IPAddress.Parse(Form1.ComboBox1.Text)   'record ip
                Dim serverhost As New IPEndPoint(serverIP, 888) 'set port
                'clientSocket.Connect(serverhost)
                tcpClient = New TcpClient()
                tcpClient.Connect(serverhost)
                networkStream = tcpClient.GetStream
                Console.WriteLine("Server is connected")
                clientThread = New Thread(AddressOf receiveThreadProc)
                clientThread.Start()
                Console.WriteLine("等待對手中")
                Dim msg As Byte() = Encoding.Default.GetBytes("RADY")
                networkStream.Write(msg, 0, msg.Length)
                'clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
            Catch ex As Exception
                MsgBox("請確認Host端是否開啟")
                Me.Close()
            End Try
            Button3.Visible = False
            Button33.Visible = False
            Label5.Visible = True
        End If
        If Form1.CheckBox1.Checked Then
            NeedLine = Form1.ComboBox2.Text
            Console.WriteLine("set NeedLine is:" & NeedLine)
            setLabel4(NeedLine)
        Else
            LockOrUnlockB1to15("Lock")
        End If
        ifApearYou = New Thread(AddressOf IPAY)
        ifApearYou.Start()
    End Sub
    Private Sub IPAY()
        Try
            While Not Label2.Text.Contains("You")
                Console.WriteLine("偵測中")
            End While
            LockOrUnlockB1to15("Lock")
            LockOrUnlockB1to15("UnlcokGameOver")
            GameOver = True
        Catch ex As Exception

        End Try

    End Sub
    Private Sub LockOrUnlockB1to15(ByVal tmpStr As String)
        If Button4.InvokeRequired Then
            Dim d As New setForm(AddressOf LockOrUnlockB1to15)
            Invoke(d, tmpStr)
        End If
        If tmpStr = "Lock" Then
            For i = 0 To 24
                GameB(i).Enabled = False
            Next
        ElseIf tmpStr = "Unlock" Then
            For i = 0 To 24
                GameB(i).Enabled = True
            Next
        ElseIf tmpStr = "ResetText" Then
            For i = 0 To 24
                GameB(i).Text = ""
                GameB(i).Font = New Font(GameB(i).Font, FontStyle.Regular)
            Next
        ElseIf tmpStr = "Lock3" Then
            Button3.Enabled = False
        ElseIf tmpStr = "Unlock3" Then
            Button3.Enabled = True
        ElseIf tmpStr = "UnlcokGameOver" Then
            Button33.Enabled = True
        End If

    End Sub
    Private Sub receiveThreadProc() 'receive message from server
        Try
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do
                'rcvBytes = clientSocket.Receive(bytes, 0, bytes.Length, SocketFlags.None)
                rcvBytes = networkStream.Read(bytes, 0, bytes.Length)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Console.WriteLine(tmpStr)
                Select Case tmpStr.Substring(0, 4)
                    Case "Strt"
                        FormChange(tmpStr.Substring(4))
                    Case "NumB"
                        setLabel3("Please Choose number")
                        Console.WriteLine("recNumb+" & tmpStr.Substring(4))
                        CanceledBlock("")
                        setGameNumber(tmpStr.Substring(4))
                        setLabel2("目前有" & checkLine() & "條連線")
                        Dim msg As Byte() = Encoding.Default.GetBytes("NowL" & Winline)
                        networkStream.Write(msg, 0, msg.Length)
                        'clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
                        If checkWin(Winline) Then
                            setLabel2("You Win")
                        End If
                    Case "NowL"
                        If checkWin(tmpStr.Substring(4)) Then
                            If GameOver = False Then
                                setLabel2("You Lose")
                            End If

                            setLabel5("True")
                        End If
                    Case "NedL"
                        NeedLine = tmpStr.Substring(4)
                        setLabel4(NeedLine)
                        Console.WriteLine("set NeedLine is:" & NeedLine)
                    Case "RADY"
                        setLabel5("false")
                        LockOrUnlockB1to15("Lock")
                        SwapAllButton()
                    Case "GGGG"

                        GGSMIIDA()
                End Select
                Console.WriteLine("received message from Server successfully")

            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
    Private Sub FormChange(ByVal tmpstr As String)
        If Me.InvokeRequired Then
            Dim d As New setForm(AddressOf FormChange)
            Invoke(d, tmpstr)
        Else
            If tmpstr = 3 Then
                Form3.Show()
                Me.Close()
            ElseIf tmpstr = 4 Then
                Form2.Show()
                Me.Close()
            End If
        End If
    End Sub
    Public Function checkLine()
        Dim i, j As Integer
        Dim countOneLine As Integer
        Dim HowmanyLine As Integer
        For i = 0 To 20 Step 5
            j = i
            countOneLine = 0
            While j < i + 5                 '水平線
                If GameB(j).Enabled = False Then '如果有一個沒被圈走則沒有線
                    countOneLine += 1
                End If
                j += 1
            End While
            If countOneLine = 5 Then         '如果都被圈走則成為一條線 線+1
                HowmanyLine += 1
            End If
        Next
        For i = 0 To 4
            j = i
            countOneLine = 0
            While j <= i + 20                    '垂直線
                If GameB(j).Enabled = False Then '如果有一個沒被圈走則沒有線 線+1
                    countOneLine += 1
                End If
                j += 5
            End While
            If countOneLine = 5 Then         '如果都被圈走則成為一條線 線+1
                HowmanyLine += 1
            End If
        Next
        countOneLine = 0
        For i = 0 To 24 Step 6      '左往右下斜線
            If GameB(i).Enabled = False Then '如果有一個沒被圈走則沒有線 線+1
                countOneLine += 1
            End If
        Next
        If countOneLine = 5 Then         '如果都被圈走則成為一條線 線+1
            HowmanyLine += 1
        End If
        countOneLine = 0
        For i = 4 To 20 Step 4      '右往左斜線
            If GameB(i).Enabled = False Then '如果有一個沒被圈走則沒有線 線+1
                countOneLine += 1
            End If
        Next
        If countOneLine = 5 Then         '如果都被圈走則成為一條線 線+1
            HowmanyLine += 1
        End If
        If Winline < HowmanyLine Then
            Winline = HowmanyLine
        End If
        Return Winline      '回傳現有的線
    End Function
    Public Function checkWin(ByVal tmpStr As String)
        If tmpStr < NeedLine Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Sub setGameNumber(ByVal tmpStr As String)
        If Button2.InvokeRequired Then
            Dim d As New setForm(AddressOf setGameNumber)
            Invoke(d, tmpStr)
        Else
            For i = 0 To 24
                If GameB(i).Text = tmpStr Then
                    GameB(i).Font = New Font(GameB(i).Font, FontStyle.Strikeout)
                    Console.WriteLine(GameB(i).Text & "StrikeOut Setting Successfully")
                    GameB(i).Enabled = False
                    BlockGameB(i) = 1
                End If
            Next
            Console.Write("set gameNumber completed")
        End If
    End Sub
    Public Sub setLabel2(ByVal tmpStr As String)
        Console.Write("setting Label2 completed")
        If Label2.InvokeRequired Then
            Dim d As New setForm(AddressOf setLabel2)
            Invoke(d, tmpStr)
        Else
            Label2.Text = tmpStr
            Console.Write("set Label2 completed")
        End If
    End Sub
    Public Sub setLabel3(ByVal tmpStr As String)
        If Label3.InvokeRequired Then
            Dim d As New setForm(AddressOf setLabel3)
            Invoke(d, tmpStr)
        Else
            If tmpStr = "Please Choose number" Then
                Label3.Text = tmpStr
                Label3.Left = 54

            ElseIf tmpStr = "Waiting" Then
                Label3.Text = tmpStr
                Label3.Left = 122
            End If
        End If
    End Sub
    Public Sub setLabel4(ByVal tmpStr As String)
        If Label4.InvokeRequired Then
            Dim d As New setForm(AddressOf setLabel4)
            Invoke(d, tmpStr)
        Else
            Label4.Text = "You need to connect " & tmpStr & " line to win"
        End If
    End Sub
    Public Sub setLabel5(ByVal tmpStr As String)
        If Label5.InvokeRequired Then
            Dim d As New setForm(AddressOf setLabel5)
            Invoke(d, tmpStr)
        Else
            If tmpStr = "True" Then
                Label5.Visible = True
            Else
                Label5.Visible = False
            End If

        End If
    End Sub
    Public Sub CanceledBlock(ByVal tmpStr As String)
        Console.WriteLine("in canceled")
        If Button1.InvokeRequired Then
            Dim d As New setForm(AddressOf CanceledBlock)
            Invoke(d, tmpStr)
        Else
            For i = 0 To 24
                Console.Write(BlockGameB(i) & " ")
                If BlockGameB(i) = 0 Then
                    GameB(i).Enabled = True
                End If
            Next
        End If
    End Sub
    'create random number
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        SwapAllButton()
        If Form1.CheckBox1.Checked Then
            setLabel3("Please Choose number")
        Else
            setLabel3("Waiting")
        End If
        LockOrUnlockB1to15("Lock3")
        NeedLine = Form1.ComboBox2.Text
        If Form1.CheckBox1.Checked Then         '傳送需要幾條線
            Try
                Dim msg As Byte() = Encoding.Default.GetBytes("NedL" & Form1.ComboBox2.Text)
                lc.networkStream.Write(msg, 0, msg.Length)
                msg = Encoding.Default.GetBytes("RADY")
                lc.networkStream.Write(msg, 0, msg.Length)
                Console.WriteLine("Sendded Rady")
            Catch ex As Exception
                LockOrUnlockB1to15("Unlock3")
            End Try
        End If
    End Sub
    Public Sub SwapAllButton()

        Dim swapX, swapY, tmp As Integer
        Dim i As Integer
        Dim Num(25) As Integer
        Dim rnd As New Random
        For i = 0 To 24
            Num(i) = i + 1
            GameB(i) = Me.Controls("Button" & i + 4)
        Next
        Thread.Sleep(10)
        While i < 10000
            swapX = rnd.Next(0, 25)
            swapY = rnd.Next(0, 25)
            If swapX <> swapY Then
                tmp = Num(swapX)
                Num(swapX) = Num(swapY)
                Num(swapY) = tmp
                i += 1
            End If
        End While
        For i = 0 To 24
            GameB(i).Text = Num(i)
        Next
    End Sub
    'finish create random number
    Private Sub GameButton_Click(sender As Object, e As EventArgs) Handles Button4.Click, Button5.Click, Button6.Click, Button7.Click, Button8.Click, Button9.Click, Button10.Click, Button11.Click, Button12.Click, Button13.Click, Button14.Click, Button15.Click, Button16.Click, Button17.Click, Button18.Click, Button19.Click, Button20.Click, Button21.Click, Button22.Click, Button23.Click, Button24.Click, Button25.Click, Button26.Click, Button27.Click, Button28.Click
        Dim GBNum As String
        GBNum = CType(sender, Button).Text
        CType(sender, Button).Font = New Font(CType(sender, Button).Font, FontStyle.Strikeout)
        Console.WriteLine("Set Strikeout successfully")
        BlockGameB(CType(sender, Button).Tag) = 1   '紀錄被按過的按鈕
        CType(sender, Button).Enabled = False
        setLabel2("目前有" & checkLine() & "條連線")
        If Form1.CheckBox1.Checked Then
            Try
                Dim msg As Byte() = Encoding.Default.GetBytes("NumB" & GBNum)
                lc.networkStream.Write(msg, 0, msg.Length)
                SendNowLine()

            Catch ex As Exception
            End Try
        Else
            Try
                Dim msg As Byte() = Encoding.Default.GetBytes("NumB" & GBNum)
                networkStream.Write(msg, 0, msg.Length)
                SendNowLine()
            Catch ex As Exception

            End Try

        End If
        For i = 0 To 24
            GameB(i).Enabled = False
            setLabel3("Waiting")
        Next
        If Winline >= NeedLine Then
            setLabel2("You Win")
        End If
        Console.WriteLine("sended")
    End Sub
    Public Sub SendNowLine()
        Dim msg As Byte() = Encoding.Default.GetBytes("NowL" & Winline)
        If Form1.CheckBox1.Checked Then
            lc.networkStream.Write(msg, 0, msg.Length)
        Else
            networkStream.Write(msg, 0, msg.Length)
        End If
    End Sub
    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'If Not ServerSocket Is Nothing Then
        '    ServerSocket.Close()
        'End If
        'If Not clientSocket Is Nothing Then
        '    clientSocket.Close()
        'End If
        If Not clientThread Is Nothing Then
            clientThread.Abort()
        End If
        If Not acceptThread Is Nothing Then
            acceptThread.Abort()
        End If
        If Form1.CheckBox1.Checked Then
            'If Not lc.clientSocket Is Nothing Then
            '    lc.clientSocket.Close()
            'End If
            'If Not lc.serverSocket Is Nothing Then
            '    lc.serverSocket.Close()
            'End If
            If Not lc.networkStream Is Nothing Then
                lc.networkStream.Close()
            End If
        End If
        If Not tcpListener Is Nothing Then
            tcpListener.Stop()
        End If
        If Not ifApearYou Is Nothing Then
            ifApearYou.Abort()
        End If
        For i = 1 To 3
            Form1.Controls("RadioButton" & i).Enabled = True
        Next
        For i = 1 To 2
            Form1.Controls("ComboBox" & i).Enabled = True
        Next
        For i = 3 To 4
            Form1.Controls("Button" & i).Enabled = True
        Next
        Form1.CheckBox1.Enabled = True
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button33.Click
        LockOrUnlockB1to15("Unlock3")
        GGSMIIDA()
    End Sub
    Public Sub GGSMIIDA()
        If GameOver Then
            Try
                Dim msg As Byte() = Encoding.Default.GetBytes("GGGG")
                If Form1.CheckBox1.Checked Then
                    lc.networkStream.Write(msg, 0, msg.Length)
                Else
                    networkStream.Write(msg, 0, msg.Length)
                End If
            Catch ex As Exception

            End Try
            Button33.Enabled = False
            LockOrUnlockB1to15("Unlock")
            LockOrUnlockB1to15("ResetText")
            Winline = 0
            For i = 0 To 24
                BlockGameB(i) = 0
            Next
            setLabel2("目前有0條連線")
            GameOver = False
            ifApearYou = New Thread(AddressOf IPAY)
            ifApearYou.Start()
        End If
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click

    End Sub
End Class
Public Class ListenClass4
    Private f4 As Form4
    Public ClientOut As String '紀錄Client出的拳
    Delegate Sub setTextDel(ByVal tmpStr As String)
    'Public clientSocket As Socket
    'Public serverSocket As Socket
    Private tcpListener As TcpListener
    Private tcpClient As TcpClient
    Public networkStream As NetworkStream
    Public Sub New(ByVal tmpSocket As TcpListener, ByVal tmpForm1 As Form4)
        Me.tcpListener = tmpSocket
        f4 = tmpForm1
    End Sub
    Public Sub ServerThreadProc()
        Do While True
            Try
                tcpClient = tcpListener.AcceptTcpClient()
                networkStream = tcpClient.GetStream
                'clientSocket = serverSocket.Accept
                Dim t As New Thread(AddressOf receiveThreadProc)
                t.Start()
            Catch ex As Exception

            End Try
        Loop
    End Sub
    'receive from Client
    Private Sub receiveThreadProc()
        Try
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do
                'rcvBytes = clientSocket.Receive(bytes, 0, bytes.Length, SocketFlags.None)
                rcvBytes = networkStream.Read(bytes, 0, bytes.Length)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Console.WriteLine(tmpStr)
                Select Case tmpStr.Substring(0, 4)
                    Case "RADY"
                        Dim msg As Byte() = Encoding.Default.GetBytes("Strt" & f4.GAMEMODE)
                        networkStream.Write(msg, 0, msg.Length)
                        'clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
                        Console.WriteLine("sendMode")
                    Case "NumB"
                        f4.setLabel3("Please Choose number")
                        Console.WriteLine("recNumb+" & tmpStr.Substring(4))
                        f4.CanceledBlock("")
                        f4.setGameNumber(tmpStr.Substring(4))
                        f4.setLabel2("目前有" & f4.checkLine() & "條連線")
                        Console.WriteLine("set completely ")
                        Dim msg As Byte() = Encoding.Default.GetBytes("NowL" & f4.Winline)
                        networkStream.Write(msg, 0, msg.Length)
                        'clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
                        If f4.checkWin(f4.Winline) Then
                            f4.setLabel2("You Win")
                        End If
                    Case "NowL"
                        If f4.checkWin(tmpStr.Substring(4)) Then
                            If f4.GameOver = False Then
                                f4.setLabel2("You Lose")
                            End If
                        End If
                    Case "GGGG"
                        f4.GGSMIIDA()
                End Select
                Console.WriteLine("received message from client succefully ")

            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
End Class