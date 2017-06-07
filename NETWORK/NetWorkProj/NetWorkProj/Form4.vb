Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Random
Public Class Form4
    Dim lc As f4ListenClass
    Public acceptThread As Thread
    Public clientThread As Thread
    Public ballmove As Thread
    Public tcpListener As TcpListener
    Public tcpClient As TcpClient
    Dim networkStream As NetworkStream
    Delegate Sub setpointdel(ByVal tmpstr As String)
    Delegate Sub setballpointdel()
    Dim loc As Point
    Dim locx As Integer
    Dim rr As New Random()
    Property dx As Double
    Property dy As Double
    Dim delay As Integer
    Property start As Boolean
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

    Private Sub Form2_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If Form1.CheckBox1.Checked = True Then
            If e.KeyCode = Keys.Left Then
                Label4.Left -= 10
                If Label4.Left <= 0 Then
                    Label4.Left = 0
                End If
            ElseIf e.KeyCode = Keys.Right Then
                Label4.Left += 10
                If Label4.Left + Label4.Width >= Me.Width Then
                    Label4.Left = Me.Width - Label4.Width
                End If
            End If
            Dim msg As Byte() = Encoding.Default.GetBytes("bslc" & Label4.Left)
            lc.networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
        Else
            If e.KeyCode = Keys.Left Then
                Label5.Left -= 10
                If Label5.Left <= 0 Then
                    Label5.Left = 0
                End If
            ElseIf e.KeyCode = Keys.Right Then
                Label5.Left += 10
                If Label5.Left + Label5.Width >= Me.Width Then
                    Label5.Left = Me.Width - Label5.Width
                End If

            End If
            Dim msg As Byte() = Encoding.Default.GetBytes("bslc" & Label5.Left)
            networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
        End If

    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dx = 0
        dy = 0
        start = False
        delay = 1
        If Form1.CheckBox1.Checked Then         '如果是HOST端
            Label1.Text &= "(Host)"
            Me.Text = Label1.Text
            Try

                dx = rr.Next(1, 10)
                If rr.Next(0, 2) = 1 Then
                    dx = dx * -1
                End If
                dy = rr.Next(1, 10)
                If rr.Next(0, 2) = 1 Then
                    dy = dy * -1
                End If
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
                Dim serverhost As New IPEndPoint(serverIP, 877)
                tcpListener = New TcpListener(serverhost)
                tcpListener.Start(10)
                Console.WriteLine("Server is Listening")
                lc = New f4ListenClass(tcpListener, Me)
                acceptThread = New Thread(AddressOf lc.ServerThreadProc)
                acceptThread.Start()
                Console.WriteLine("Server got client")
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK)
            End Try
        Else        '不是HOST端
            Label1.Text &= "(Client)"
            Me.Text = Label1.Text
            Try
                Dim serverIP As IPAddress = IPAddress.Parse(Form1.ComboBox1.Text)   'record ip
                Dim serverhost As New IPEndPoint(serverIP, 877) 'set port
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
        ballmove = New Thread(AddressOf ball)
        ballmove.Start()
        Label6.BackColor = Color.MediumSpringGreen
        Dim m_path As System.Drawing.Drawing2D.GraphicsPath
        m_path = New System.Drawing.Drawing2D.GraphicsPath(Drawing.Drawing2D.FillMode.Winding)
        m_path.AddEllipse(1, 1, 20, 20)
        Label6.Region() = New Region(m_path)

    End Sub

    'Client
    Public Sub setpoint4(ByVal tmpstr As String)
        If Label4.InvokeRequired = True Then
            Dim d As New setpointdel(AddressOf setpoint4)
            Invoke(d, tmpstr)
        Else
            Try
                Label4.Left = tmpstr
            Catch ex As Exception

            End Try
        End If
    End Sub
    Public Sub setpoint5(ByVal tmpstr As String)
        If Label5.InvokeRequired = True Then
            Dim d As New setpointdel(AddressOf setpoint5)
            Invoke(d, tmpstr)
        Else
            Try
                Label5.Left = tmpstr
            Catch ex As Exception

            End Try

        End If
    End Sub
    Public Sub restart(ByVal tmpstr As String)
        If Label4.InvokeRequired = True Then
            Dim d As New setpointdel(AddressOf restart)
            Invoke(d, tmpstr)
        Else
            Label8.Visible = True
            Label9.Visible = True
            Label10.Visible = True
            Label11.Visible = True
        End If
    End Sub
    Public Sub winlose(ByVal tmpstr As String)
        If Label4.InvokeRequired = True Then
            Dim d As New setpointdel(AddressOf winlose)
            Invoke(d, tmpstr)
        Else
            If Form1.CheckBox1.Checked = True Then
                If tmpstr = "You Lose" Then
                    Label7.Text = "You Win"
                    Label7.Visible = True
                    Label8.Visible = True
                    Label9.Visible = True
                    Label10.Visible = True
                    Label11.Visible = True
                Else
                    Label7.Text = "You Lose"
                    Label7.Visible = True
                    Label8.Visible = True
                    Label9.Visible = True
                    Label10.Visible = True
                    Label11.Visible = True
                End If
            Else
                If tmpstr = "You Lose" Then
                    Label7.Text = "You Win"
                    Label7.Visible = True
                Else
                    Label7.Text = "You Lose"
                    Label7.Visible = True
                End If
            End If

        End If
    End Sub

    Private Sub ball()
        While True
            If start = True Then
                setballpoint()
                Thread.Sleep(30)
            End If
        End While
    End Sub
    Public Sub setballpoint()

        If Label6.InvokeRequired = True Then
            Dim d As New setballpointdel(AddressOf setballpoint)
            Invoke(d)
        Else
            Dim tmp As Integer
            tmp = delay
            For i = 0 To tmp Step 2
                Label6.Left += dx
                Label6.Top += dy
                If Label6.Left < 0 Then
                    Label6.Left -= dx
                    dx *= -1

                ElseIf Label6.Left + 20 > Me.Width Then
                    Label6.Left -= dx
                    dx *= -1

                End If
                If (Label6.Top + 20 >= Label4.Top) And (Label6.Top + 20 <= Label4.Top + Label4.Height) Then
                    If Label6.Left + 20 > Label4.Left And (Label6.Left) <= (Label4.Left + Label4.Width) Then
                        Label6.Top -= dy
                        Label6.Left += dx
                        dy *= -1
                        hspeed()
                    End If
                End If
                If (Label6.Top > Label5.Top) And (Label6.Top <= Label5.Top + Label5.Height) Then
                    If Label6.Left + 20 > Label5.Left And (Label6.Left) <= (Label5.Left + Label5.Width) Then
                        Label6.Top -= dy
                        Label6.Left += dx
                        dy *= -1
                        hspeed()
                    End If
                End If
                If Label6.Top < 0 Then
                    Label6.Top -= dy
                    dy *= -1
                    hspeed()
                ElseIf Label6.Top + 20 > Me.Height Then
                    Label6.Top -= dy
                    dy *= -1
                    hspeed()
                End If
                winornot()
            Next

        End If
    End Sub
    Public Sub hspeed()
        If delay <= 20 Then
            delay += 1
        End If
    End Sub
    Public Sub winornot()
        If Label6.Top + 20 > Label4.Top + Label4.Height Then
            If Form1.CheckBox1.Checked = True Then
                Label7.Text = "You Lose"
                Label7.Visible = True
                Label8.Visible = True
                Label9.Visible = True
                Label10.Visible = True
                Label11.Visible = True
                Dim msg As Byte() = Encoding.Default.GetBytes("wanl" & Label7.Text)
                lc.networkStream.Write(msg, 0, msg.Length)
                Thread.Sleep(10)
            Else
                Label7.Text = "You Win"
                Label7.Visible = True
                Dim msg As Byte() = Encoding.Default.GetBytes("wanl" & Label7.Text)
                networkStream.Write(msg, 0, msg.Length)
                Thread.Sleep(10)

            End If
            ballmove.Abort()
        ElseIf Label6.Top + 20 < Label5.Top Then
            If Form1.CheckBox1.Checked = False Then
                Label7.Text = "You Lose"
                Label7.Visible = True
                Dim msg As Byte() = Encoding.Default.GetBytes("wanl" & Label7.Text)
                networkStream.Write(msg, 0, msg.Length)
                Thread.Sleep(10)

            Else
                Label7.Text = "You Win"
                Label7.Visible = True
                Label8.Visible = True
                Label9.Visible = True
                Label10.Visible = True
                Label11.Visible = True
                Dim msg As Byte() = Encoding.Default.GetBytes("wanl" & Label7.Text)
                lc.networkStream.Write(msg, 0, msg.Length)
                Thread.Sleep(10)
            End If
            ballmove.Abort()
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
                Select Case tmpStr.Substring(0, 4)
                    Case "bslc"
                        setpoint4(tmpStr.Substring(4))
                    Case "bast"
                        If tmpStr.Substring(4, 1) = "x" Then
                            dx = tmpStr.Substring(5)
                        Else
                            dy = tmpStr.Substring(5)
                            Try
                                Dim msg As Byte() = Encoding.Default.GetBytes("star")
                                networkStream.Write(msg, 0, msg.Length)
                                Thread.Sleep(60)
                                start = True
                                Console.WriteLine("Start")
                            Catch ex As Exception

                            End Try

                        End If
                    Case "wanl"
                        winlose(tmpStr.Substring(4))
                        If Not ballmove Is Nothing Then
                            ballmove.Abort()
                        End If
                    Case "endg"
                        MessageBox.Show("對方離開了")
                        Me.Close()
                    Case "rest"
                        restart(tmpStr)

                End Select

                Console.WriteLine("received message from Server successfully")

            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub


    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not ballmove Is Nothing Then
            ballmove.Abort()
        End If
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



    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
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
            Me.Close()
        End Try
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Label2_MouseDown(sender As Object, e As MouseEventArgs) Handles Label2.MouseDown
        Label2.BackColor = Form1.Button1.FlatAppearance.MouseDownBackColor
    End Sub

    Private Sub Label2_MouseLeave(sender As Object, e As EventArgs) Handles Label2.MouseLeave
        Label2.BackColor = Form1.Button1.BackColor
    End Sub

    Private Sub Label2_MouseMove(sender As Object, e As MouseEventArgs) Handles Label2.MouseMove
        Label2.BackColor = Form1.Button1.FlatAppearance.MouseOverBackColor

    End Sub

    Private Sub Label3_MouseDown(sender As Object, e As MouseEventArgs) Handles Label3.MouseDown
        Label3.BackColor = Form1.Button1.FlatAppearance.MouseDownBackColor
    End Sub

    Private Sub Label3_MouseLeave(sender As Object, e As EventArgs) Handles Label3.MouseLeave
        Label3.BackColor = Form1.Button1.BackColor
    End Sub

    Private Sub Label3_MouseMove(sender As Object, e As MouseEventArgs) Handles Label3.MouseMove
        Label3.BackColor = Form1.Button1.FlatAppearance.MouseOverBackColor
    End Sub

    Private Sub Label4_MouseDown(sender As Object, e As MouseEventArgs) Handles Label4.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left And Form1.CheckBox1.Checked = True Then
            locx = e.Location.X
        End If
    End Sub

    Private Sub Label4_MouseMove(sender As Object, e As MouseEventArgs) Handles Label4.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left And Form1.CheckBox1.Checked = True Then
            Label4.Left += e.Location.X - locx
            Dim msg As Byte() = Encoding.Default.GetBytes("bslc" & Label4.Left)
            lc.networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(10)
        End If
    End Sub
    Private Sub Label5_MouseDown(sender As Object, e As MouseEventArgs) Handles Label5.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left And Form1.CheckBox1.Checked = False Then
            locx = e.Location.X
        End If
    End Sub

    Private Sub Label5_MouseMove(sender As Object, e As MouseEventArgs) Handles Label5.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left And Form1.CheckBox1.Checked = False Then
            Label5.Left += e.Location.X - locx
            Dim msg As Byte() = Encoding.Default.GetBytes("bslc" & Label5.Left)
            networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(10)
        End If
    End Sub

    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click
        Label7.Visible = False
        Label8.Visible = False
        Label9.Visible = False
        Label10.Visible = False
        Label11.Visible = False
        Label6.Left = 136
        Label6.Top = 251
        dx = 0
        dy = 0
        start = False
        delay = 1
        ballmove = New Thread(AddressOf ball)
        ballmove.Start()
        If Form1.CheckBox1.Checked = True Then
            dx = rr.Next(1, 10)
            If rr.Next(0, 2) = 1 Then
                dx = dx * -1
            End If
            dy = rr.Next(1, 10)
            If rr.Next(0, 2) = 1 Then
                dy = dy * -1
            End If
            Dim msg As Byte() = Encoding.Default.GetBytes("rest")
            lc.networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
        Else
            Dim msg As Byte() = Encoding.Default.GetBytes("rest")
            networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
        End If
    End Sub

    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click
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

    Private Sub Label9_MouseMove(sender As Object, e As MouseEventArgs) Handles Label9.MouseMove
        Label9.BackColor = Color.Gray
    End Sub

    Private Sub Label9_MouseLeave(sender As Object, e As EventArgs) Handles Label9.MouseLeave
        Label9.BackColor = Color.Honeydew
    End Sub

    Private Sub Label10_MouseLeave(sender As Object, e As EventArgs) Handles Label10.MouseLeave
        Label10.BackColor = Color.Honeydew

    End Sub

    Private Sub Label10_MouseMove(sender As Object, e As MouseEventArgs) Handles Label10.MouseMove
        Label10.BackColor = Color.Gray
    End Sub
End Class
Public Class f4ListenClass
    Private f4 As Form4

    Private tcpListener As TcpListener
    Private tcpClient As TcpClient
    Public networkStream As NetworkStream
    Public Sub New(ByVal tmpSocket As TcpListener, ByVal tmpForm4 As Form4)
        Me.tcpListener = tmpSocket
        f4 = tmpForm4

    End Sub
    Public Sub ServerThreadProc()
        Do While True
            Try
                tcpClient = tcpListener.AcceptTcpClient()
                networkStream = tcpClient.GetStream
                Dim t As New Thread(AddressOf receiveThreadProc)
                t.Start()
            Catch ex As Exception

            End Try
        Loop
    End Sub
    'receive from Client
    Private Sub receiveThreadProc()
        Try
            Dim msg As Byte() = Encoding.Default.GetBytes("bastx" & f4.dx)
            networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
            msg = Encoding.Default.GetBytes("basty" & f4.dy)
            networkStream.Write(msg, 0, msg.Length)
            Thread.Sleep(5)
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do
                rcvBytes = networkStream.Read(bytes, 0, bytes.Length)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Console.WriteLine(tmpStr)
                Select Case tmpStr.Substring(0, 4)
                    Case "bslc"
                        f4.setpoint5(tmpStr.Substring(4))
                    Case "star"
                        Console.WriteLine("START IS HERE")
                        f4.start = True
                        Console.WriteLine("Start")
                    Case "endg"
                        MessageBox.Show("對方離開了")
                        f4.Close()
                    Case "rest"
                        msg = Encoding.Default.GetBytes("bastx" & f4.dx)
                        networkStream.Write(msg, 0, msg.Length)
                        Thread.Sleep(5)
                        msg = Encoding.Default.GetBytes("basty" & f4.dy)
                        networkStream.Write(msg, 0, msg.Length)
                        Thread.Sleep(5)
                    Case "wanl"
                        f4.winlose(tmpStr.Substring(4))
                        If Not f4.ballmove Is Nothing Then
                            f4.ballmove.Abort()
                        End If

                End Select
                Console.WriteLine("received message from client succefully ")

            Loop While networkStream.DataAvailable = True Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
End Class
