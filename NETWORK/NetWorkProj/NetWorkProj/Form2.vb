Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Random
Public Class Form2
    Public lc As ListenClass
    Public acceptThread As Thread
    Public clientThread As Thread
    Public tcpListener As TcpListener
    Public tcpClient As TcpClient
    Private rnd As New Random
    Public networkStream As NetworkStream
    Delegate Sub setForm(ByVal tmpStr As String)
    Public ImAsking As Boolean = False
    Public Num(16) As Integer
    Public UnConceal(16) As Integer
    Public Btn(16) As Button
    Public CNum As Integer
    Public YourTurn As Boolean
    Private MyScore As Integer
    Private ReceiveScore As Integer
    Private NotYet As Boolean = False
    Dim Clik As Integer         '確認按幾次
    Public Quet(5) As String
    Public QuetCount As Integer
    Public Answer As Integer
    'Move Form2's sub   「移動視窗」
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
            If Form1.CheckBox1.Checked = True Then
                Dim msg As Byte() = Encoding.Default.GetBytes("endg")
                lc.networkStream.Write(msg, 0, msg.Length)
            Else
                Dim msg As Byte() = Encoding.Default.GetBytes("endg")
                networkStream.Write(msg, 0, msg.Length)
            End If
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
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i = 0 To 15
            Btn(i) = Me.Controls("Button" & i + 3)
        Next

        If Form1.CheckBox1.Checked Then         '如果是HOST端
            Button19.Visible = True

            Label1.Text &= "(Host)"
            Button20.Visible = True
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
                Dim serverIP As IPAddress = IPAddress.Parse(Form1.ComboBox1.Text)
                Dim serverhost As New IPEndPoint(serverIP, 888)
                tcpListener = New TcpListener(serverhost)
                tcpListener.Start(10)
                Console.WriteLine("Server is Listening")
                lc = New ListenClass(tcpListener, Me)
                acceptThread = New Thread(AddressOf lc.ServerThreadProc)
                acceptThread.Start()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK)
            End Try
        Else        '不是HOST端
            YourTurn = False
            Label1.Text &= "(Client)"
            Me.Text = Label1.Text
            Try
                Dim serverIP As IPAddress = IPAddress.Parse(Form1.ComboBox1.Text)   'record ip
                Dim serverhost As New IPEndPoint(serverIP, 888) 'set port
                tcpClient = New TcpClient()
                tcpClient.Connect(serverhost)
                networkStream = tcpClient.GetStream
                Console.WriteLine("Server is connected")
                clientThread = New Thread(AddressOf receiveThreadProc)
                clientThread.Start()
                Console.WriteLine("等待對手中")
            Catch ex As Exception
                MsgBox("請確認Host端是否開啟")
                Me.Close()
            End Try

        End If
    End Sub
    Private Sub receiveThreadProc() 'receive message from server
        Try
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do
                rcvBytes = networkStream.Read(bytes, 0, bytes.Length)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Console.WriteLine(tmpStr)
                Select Case tmpStr.Substring(0, 4)  '分析收到訊息
                    Case "Butn"     '載入host端傳來的按鈕號碼
                        InputNum(tmpStr(4))
                    Case "Chos"
                        ChosBtn(tmpStr.Substring(4)) '讀取Host端傳來的數字
                    Case "Turn"
                        SetLabel5("Turn")       '換我了
                        YourTurn = True
                    Case "Rset"     'Reset Game
                        Reset("")
                    Case "Mesg"     '訊息
                        setTextBox1(tmpStr.Substring(4))
                    Case "Quet"     '收問題
                        Quet(QuetCount) = tmpStr.Substring(4)
                        QuetCount += 1
                        If QuetCount >= 5 Then
                            QuetCount = 0
                        End If
                    Case "AskY"     '收完問題後開啟的F3
                        showF3("")
                    Case "Answ"     '收答案
                        Answer = tmpStr.Substring(4)
                    Case "reAn"     '收答案
                        CheckAns(tmpStr.Substring(4))
                    Case "Lo22"     '鎖定B22
                        LockB22("Lock")
                    Case "Un22"     '解鎖B22
                        LockB22("Unlock")
                    Case "SysM"
                        setTextBox1SysMsg(tmpStr.Substring(4))
                   
                End Select
                Console.WriteLine("received message from Server successfully")

            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
    '收取Host端的所有數字排版
    Private Sub InputNum(ByVal NumB As String)
        If Button1.InvokeRequired Then
            Dim d As New setForm(AddressOf InputNum)
            Invoke(d, NumB)
        Else
            Me.Controls("Button" & CNum + 3).Text = NumB

            Num(CNum) = NumB
            CNum += 1
            If CNum >= 16 Then
                CNum = 0
                Dim t As New Thread(AddressOf Count3s)
                t.Start()
            End If
        End If

    End Sub
    '關閉表單
    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not clientThread Is Nothing Then
            clientThread.Abort()
        End If
        If Form1.CheckBox1.Checked Then
            If Not lc.networkStream Is Nothing Then
                lc.networkStream.Close()
            End If

            If Not tcpListener Is Nothing Then
                tcpListener.Stop()
            End If
        End If
        If Not acceptThread Is Nothing Then
            acceptThread.Abort()
        End If
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Try
            swapNum()       '隨機數字排版
            YourTurn = True
            Button19.Enabled = False
        Catch ex As Exception
            Button19.Enabled = True
            YourTurn = False
        End Try


    End Sub
    Private Sub swapNum()
        Dim i As Integer = 1
        Dim i2 As Integer
        Dim ij1 As Integer = 0
        While i2 < 16
            Num(i2) = i

            If i2 Mod 2 = 1 Then
                i += 1
            End If
            i2 += 1
        End While
        Dim ChangeFreq As Integer
        Dim swapX, swapY, tmp As Integer
        While ChangeFreq < 10000
            swapX = rnd.Next(0, 16)
            swapY = rnd.Next(0, 16)
            If swapX <> swapY Then
                tmp = Num(swapX)
                Num(swapX) = Num(swapY)
                Num(swapY) = tmp
                ChangeFreq += 1
            End If
        End While

        For i = 0 To 15

            Me.Controls("Button" & i + 3).Text = Num(i)

            Thread.Sleep(2)
            Try
                Dim msg As Byte() = Encoding.Default.GetBytes("Butn" & Num(i))
                lc.networkStream.Write(msg, 0, msg.Length)

            Catch ex As Exception

            End Try
        Next
        Dim t As New Thread(AddressOf Count3s)
        t.Start()
    End Sub
    Private Sub Count3s()   '數三秒的Thread
        For i = 3 To 0 Step -1
            Thread.Sleep(1000)
            SetLabel5("Secc" & i)
        Next
        SetLabel5("Secc" & 0)
        Thread.Sleep(300)
        SetLabel5("spac")
        For i = 0 To 15
            ConcealBtnN(i + 3)
        Next
    End Sub
    Private Sub ConcealBtnN(ByVal tmpStr As String) '3秒後隱藏數字
        If Button3.InvokeRequired Then
            Dim d As New setForm(AddressOf ConcealBtnN)
            Invoke(d, tmpStr)
        Else
            Me.Controls("Button" & tmpStr).Text = ""
        End If
    End Sub
    Private Sub LockAllB(ByVal tmpStr As String) '鎖定所有按鈕 (沒用在此程式碼中)
        If Button3.InvokeRequired Then
            Dim d As New setForm(AddressOf LockAllB)
            Invoke(d, tmpStr)
        Else
            For i = 3 To 18
                Me.Controls("Button" & tmpStr).Enabled = False
            Next
        End If
    End Sub
    Public Sub SetLabel5(ByVal tmpStr As String)    '設定Label5
        If Label5.InvokeRequired Then
            Dim d As New setForm(AddressOf SetLabel5)
            Invoke(d, tmpStr)
        Else
            Select Case tmpStr.Substring(0, 4)
                Case "Fini"     '遊戲結束用
                    Label5.Text = ""
                    Label5.Left = 134
                    Label5.Text = "Game Over"
                Case "Secc"     '倒數秒數用
                    Label5.Left = 134
                    Label5.Text = "Remember : " & tmpStr.Substring(4) & "sec"
                Case "spac"     '空白
                    Label5.Left = 134
                    Label5.Text = ""
                Case "Turn"     '換人 + 聊天室顯示
                    Label5.Text = "Your Turn"
                    setTextBox1SysMsg("System:Your Turn")
                Case "ChTu"     '換人  但聊天室不顯示
                    Label5.Text = "Your Turn"
                Case "NotU"     '換朋友
                    Label5.Text = "Friend's Turn"


            End Select
        End If
    End Sub
    Private Sub SetLabel3(ByVal tmpStr As String)   'Set Friend's Score
        If Label3.InvokeRequired Then
            Dim d As New setForm(AddressOf SetLabel3)
            Invoke(d, tmpStr)
        Else
            Label3.Text = "Friend:" & tmpStr & "Point"
        End If
    End Sub
    Private Sub SetLabel2(ByVal tmpStr As String)   'Set Your Score
        If Label2.InvokeRequired Then
            Dim d As New setForm(AddressOf SetLabel2)
            Invoke(d, tmpStr)
        Else
            Label2.Text = "Your:" & tmpStr & "Point"
        End If
    End Sub
    Private Sub CheckFinish(ByVal tmpStr As String) 'Check Game over yet
        If Button1.InvokeRequired Then
            Dim d As New setForm(AddressOf CheckFinish)
            Invoke(d, tmpStr)
        Else
            NotYet = True
            For i = 3 To 18
                If Me.Controls("Button" & i).Enabled = True Then
                    NotYet = False
                    Exit Sub
                End If
            Next

            SetLabel5("Fini")
            If MyScore > ReceiveScore Then
                Label5.Text &= "(YouWin)"
            ElseIf MyScore < ReceiveScore Then
                Label5.Text &= "(YouLose)"
            Else
                Label5.Text &= "(Tie)"
            End If
            Button20.Enabled = True
            Console.WriteLine("GameOver")
        End If
    End Sub
    Public Sub ChosBtn(ByVal tmpStr As String)      '收到的數字與判斷是否相同
        If Button3.InvokeRequired Then
            Dim d As New setForm(AddressOf ChosBtn)
            Invoke(d, tmpStr)
        Else
            Static Dim x As Integer
            Static Dim y As Integer
            If Clik = 0 Then

                x = tmpStr
                Me.Controls("Button" & x + 3).Text = Num(x)
                Me.Controls("Button" & x + 3).Enabled = False
                Clik += 1
            Else
                SetLabel5("NotU")
                y = tmpStr
                Me.Controls("Button" & y + 3).Text = Num(y)
                Me.Controls("Button" & y + 3).Enabled = False
                Thread.Sleep(500)
                If Me.Controls("Button" & x + 3).Text = Me.Controls("Button" & y + 3).Text Then
                    Me.Controls("Button" & y + 3).Enabled = False
                    Me.Controls("Button" & x + 3).Enabled = False
                    Me.Controls("Button" & x + 3).BackColor = Color.Red
                    Me.Controls("Button" & y + 3).BackColor = Color.Red
                    ReceiveScore += 1
                    SetLabel3(ReceiveScore)
                    CheckFinish("Lose")

                Else
                    SetLabel5("ChTu")
                    Me.Controls("Button" & y + 3).Enabled = True
                    Me.Controls("Button" & x + 3).Enabled = True
                    Me.Controls("Button" & y + 3).Text = ""
                    Me.Controls("Button" & x + 3).Text = ""
                End If
                Clik = 0
            End If
        End If
    End Sub
    Private Sub Button_Click(sender As Object, e As EventArgs) Handles Button3.Click, Button4.Click, Button5.Click, Button6.Click, Button7.Click, Button8.Click, Button9.Click, Button10.Click, Button11.Click, Button12.Click, Button13.Click, Button14.Click, Button15.Click, Button16.Click, Button17.Click, Button18.Click
        If YourTurn = True Then
            Static Dim x As Integer
            Static Dim y As Integer
            Static Dim xTag As Integer
            Static Dim yTag As Integer

            Btn(CType(sender, Button).Tag).Text = Num(CType(sender, Button).Tag)
            Btn(CType(sender, Button).Tag).Enabled = False
            Dim msg As Byte() = Encoding.Default.GetBytes("Chos" & CType(sender, Button).Tag)
            If Clik = 0 Then
                x = Btn(CType(sender, Button).Tag).Text
                xTag = CType(sender, Button).Tag
                Console.WriteLine("Clicked" & Clik)
                Clik += 1
                If Form1.CheckBox1.Checked Then
                    lc.networkStream.Write(msg, 0, msg.Length)
                Else
                    networkStream.Write(msg, 0, msg.Length)
                End If
            Else
                Console.WriteLine("Clicked2")
                y = Btn(CType(sender, Button).Tag).Text
                yTag = CType(sender, Button).Tag
                Thread.Sleep(200)
                Console.WriteLine("x:" & x & "y:" & y)
                If Form1.CheckBox1.Checked Then
                    lc.networkStream.Write(msg, 0, msg.Length)
                Else
                    networkStream.Write(msg, 0, msg.Length)
                End If
                If x = y Then
                    SetLabel5("ChTu")
                    UnConceal(xTag) = 1
                    UnConceal(yTag) = 1
                    Btn(xTag).Enabled = False
                    Btn(yTag).Enabled = False
                    Btn(xTag).BackColor = Color.Blue
                    Btn(yTag).BackColor = Color.Blue
                    Clik = 0
                    MyScore += 1
                    SetLabel2(MyScore)
                    CheckFinish("Win")

                Else
                    SetLabel5("NotU")
                    Btn(xTag).Text = ""
                    Btn(yTag).Text = ""
                    Btn(xTag).Enabled = True
                    Btn(yTag).Enabled = True
                    Clik = 0
                    YourTurn = False
                    msg = Encoding.Default.GetBytes("Turn")
                    If Form1.CheckBox1.Checked Then
                        lc.networkStream.Write(msg, 0, msg.Length)
                        Console.WriteLine("sendYourTern")
                    Else
                        networkStream.Write(msg, 0, msg.Length)
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click 'reset
        Try
            Dim msg As Byte() = Encoding.Default.GetBytes("Rset")
            lc.networkStream.Write(msg, 0, msg.Length)
            Reset("")
            Thread.Sleep(500)
            Button19_Click(sender, e)
            Button20.Enabled = False
        Catch ex As Exception
            Button20.Enabled = True
        End Try

    End Sub
    Private Sub Reset(ByVal tmpStr As String)   'reset
        If Button3.InvokeRequired Then
            Dim d As New setForm(AddressOf Reset)
            Invoke(d, tmpStr)
        End If
        For i = 3 To 18
            Me.Controls("Button" & i).Enabled = True
            Me.Controls("Button" & i).BackColor = Color.Honeydew
            Me.Controls("Button" & i).Text = ""
            MyScore = 0
            ReceiveScore = 0
            SetLabel2(MyScore)
            SetLabel3(ReceiveScore)
        Next
        SetLabel5("spac")
    End Sub
    Public Sub setTextBox1(ByVal tmpStr As String)  '收聊天
        If TextBox1.InvokeRequired Then
            Dim d As New setForm(AddressOf setTextBox1)
            Invoke(d, tmpStr)
        Else
            TextBox1.Text &= "Friend:" & tmpStr & vbNewLine
            TextBox1.SelectionStart = TextBox1.TextLength
            TextBox1.SelectionLength = 0
            TextBox1.ScrollToCaret()
        End If
    End Sub
    Public Sub setTextBox1SysMsg(ByVal tmpStr As String)    '聊天
        If TextBox1.InvokeRequired Then
            Dim d As New setForm(AddressOf setTextBox1SysMsg)
            Invoke(d, tmpStr)
        Else
            TextBox1.Text &= tmpStr & vbNewLine
            TextBox1.SelectionStart = TextBox1.TextLength
            TextBox1.SelectionLength = 0
            TextBox1.ScrollToCaret()
        End If
    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        TextBox1.Text &= "你:" & TextBox2.Text & vbNewLine
        Dim msg As Byte() = Encoding.Default.GetBytes("Mesg" & TextBox2.Text)
        If Form1.CheckBox1.Checked Then
            lc.networkStream.Write(msg, 0, msg.Length)
        Else
            networkStream.Write(msg, 0, msg.Length)
        End If
        TextBox2.Text = ""
        TextBox1.SelectionStart = TextBox1.TextLength   '將textbox自動下拉
        TextBox1.SelectionLength = 0
        TextBox1.ScrollToCaret()


    End Sub

    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                If TextBox2.Text <> "" Then
                    TextBox1.Text &= "你:" & TextBox2.Text & vbNewLine
                    Dim msg As Byte() = Encoding.Default.GetBytes("Mesg" & TextBox2.Text)
                    If Form1.CheckBox1.Checked Then
                        lc.networkStream.Write(msg, 0, msg.Length)
                    Else
                        networkStream.Write(msg, 0, msg.Length)
                    End If
                    TextBox2.Text = ""
                    TextBox1.SelectionStart = TextBox1.TextLength
                    TextBox1.SelectionLength = 0 '將textbox自動下拉
                    TextBox1.ScrollToCaret()
                End If
                
        End Select
    End Sub


    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        ImAsking = True
        Dim msg As Byte() = Encoding.Default.GetBytes("Lo22")
        If Form1.CheckBox1.Checked Then
            lc.networkStream.Write(msg, 0, msg.Length)
        Else
            networkStream.Write(msg, 0, msg.Length)
        End If
        Form3.Show()
    End Sub
    Public Sub LockB22(ByVal tmpStr As String)
        If Button22.InvokeRequired Then
            Dim d As New setForm(AddressOf LockB22)
            Invoke(d, tmpStr)
        Else
            Select Case tmpStr
                Case "Unlock"
                    Button22.Enabled = True
                Case "Lock"
                    Button22.Enabled = False
                    setTextBox1SysMsg("System:你的朋友正在設定問題...")
            End Select

        End If
    End Sub
    Public Sub showF3(ByVal tmpStr As String)
        If Me.InvokeRequired Then
            Dim d As New setForm(AddressOf showF3)
            Invoke(d, tmpStr)
        Else
            Form3.Show()
        End If
    End Sub
    Public Sub CheckAns(ByVal reAns As Integer) '確認答案正確
        Dim msg As Byte()
        If reAns = Answer Then
            setTextBox1SysMsg("System:Answer is Currect!!")
            msg = Encoding.Default.GetBytes("SysM" & "System:Answer is Currect!!")
        Else
            setTextBox1SysMsg("System:Answer is WRONG!!")
            msg = Encoding.Default.GetBytes("SysM" & "System:Answer is WRONG!!")
        End If
        Console.WriteLine("RE:" & reAns & "ANS:" & Answer)
    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click

        showF4("")
    End Sub
    Public Sub showF4(ByVal tmpStr As String)
        If Me.InvokeRequired Then
            Dim d As New setForm(AddressOf showF4)
            Invoke(d, tmpStr)
        Else
            Form4.Show()
        End If
    End Sub

End Class
Public Class ListenClass
    Private f2 As Form2
    Delegate Sub setTextDel(ByVal tmpStr As String)
    Private tcpListener As TcpListener
    Private tcpClient As TcpClient
    Public networkStream As NetworkStream
    Public Sub New(ByVal tmpSocket As TcpListener, ByVal tmpForm1 As Form2)
        Me.tcpListener = tmpSocket
        f2 = tmpForm1
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
                Select Case tmpStr.Substring(0, 4)          '收到的訊息
                    Case "Chos"
                        f2.ChosBtn(tmpStr.Substring(4))
                    Case "Turn"
                        f2.YourTurn = True
                        f2.SetLabel5("Turn")
                    Case "Mesg"
                        f2.setTextBox1(tmpStr.Substring(4))
                    Case "Quet"
                        f2.Quet(f2.QuetCount) = tmpStr.Substring(4)
                        f2.QuetCount += 1
                        If f2.QuetCount >= 5 Then
                            f2.QuetCount = 0
                        End If
                    Case "AskY"
                        f2.showF3("")
                    Case "Answ"
                        f2.Answer = tmpStr.Substring(4)
                    Case "reAn"
                        f2.CheckAns(tmpStr.Substring(4))
                    Case "Lo22"
                        f2.LockB22("Lock")
                    Case "Un22"
                        f2.LockB22("Unlock")
                    Case "SysM"
                        f2.setTextBox1SysMsg(tmpStr.Substring(4))
                    
                End Select
                Console.WriteLine("received message from client succefully ")
            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0
        Catch ex As Exception
        End Try
    End Sub
End Class
