Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO
Public Class Form1
    Dim clientSocket As Socket
    Delegate Sub setTextDel(ByVal tmpStr As String)
    Dim clientThread As Thread
    Dim ClientOut As String
    Dim ServerOut As String
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

    Private Sub receiveThreadProc()
        Try
            Dim bytes(1024) As Byte
            Dim rcvBytes As Integer
            Dim tmpStr As String
            Do

                rcvBytes = clientSocket.Receive(bytes, 0, bytes.Length, SocketFlags.None)
                tmpStr = Encoding.Default.GetString(bytes, 0, rcvBytes)
                Select Case tmpStr.Substring(0, 4)
                    Case "Sysm"
                        setLabel(tmpStr.Substring(4)) '同步倒數
                    Case "Fini"
                        setLabel("出拳時間結束") '結束出拳
                        Select Case ServerOut
                            Case "剪刀"
                                Select Case ClientOut
                                    Case "剪刀"
                                        setLabel("平手")
                                        pin += 1
                                    Case "石頭"
                                        setLabel("你贏了")
                                        win += 1
                                    Case "布"
                                        setLabel("你輸了")
                                        lose += 1
                                End Select
                            Case "石頭"
                                Select Case ClientOut
                                    Case "剪刀"
                                        setLabel("你輸了")
                                        lose += 1
                                    Case "石頭"
                                        setLabel("平手")
                                        pin += 1
                                    Case "布"
                                        setLabel("你贏了")
                                        win += 1
                                End Select
                            Case "布"
                                Select Case ClientOut
                                    Case "布"
                                        setLabel("平手")
                                        pin += 1
                                    Case "剪刀"
                                        setLabel("你贏了")
                                        win += 1
                                    Case "石頭"
                                        setLabel("你輸了")
                                        lose += 1
                                End Select

                        End Select

                        setServerPic(ServerOut)
                        setLabel7("")
                        'Select Case ServerOut
                        '    Case "布"
                        '        PictureBox2.Image = My.Resources.布
                        '    Case "剪刀"
                        '        PictureBox2.Image = My.Resources.剪刀
                        '    Case "石頭"
                        '        PictureBox2.Image = My.Resources.石頭
                        'End Select
                    Case "out:"
                        ServerOut = tmpStr.Substring(4)
                    Case "Mesg"
                        setText(tmpStr.Substring(4))
                    Case "Rdoc"
                        setRadio(tmpStr.Substring(4))

                End Select


            Loop While clientSocket.Available <> 0 Or rcvBytes <> 0

        Catch ex As Exception

        End Try
    End Sub
    Private Sub setServerPic(ByVal ServerOut As String)
        If PictureBox2.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setServerPic)
            Invoke(d, ServerOut)
        Else
            Select Case ServerOut
                Case "布"
                    PictureBox2.Image = My.Resources.布
                Case "剪刀"
                    PictureBox2.Image = My.Resources.剪刀
                Case "石頭"
                    PictureBox2.Image = My.Resources.石頭
            End Select
        End If
        
    End Sub
    Private Sub setLabel(ByVal tmpStr As String)
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
    Private Sub setText(ByVal tmpStr As String)
        If TextBox2.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setText)
            Invoke(d, tmpStr)
        Else
            TextBox2.Text = tmpStr
        End If
    End Sub
    Private Sub setRadio(ByVal tmpStr As String)
        If RadioButton1.InvokeRequired = True Then
            Dim d As New setTextDel(AddressOf setRadio)
            Invoke(d, tmpStr)
        Else
            Select Case tmpStr
                Case "1"
                    RadioButton1.Checked = True
                Case "2"
                    RadioButton2.Checked = True
                Case "3"
                    RadioButton3.Checked = True
            End Select
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try

            clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Dim serverIP As IPAddress = IPAddress.Parse(ComboBox1.Text)
            Dim serverhost As New IPEndPoint(serverIP, TextBox1.Text)
            clientSocket.Connect(serverhost)
            Console.WriteLine("Server is connected")
            clientThread = New Thread(AddressOf receiveThreadProc)
            clientThread.Start()
            setLabel("等待對手中")
            Dim msg As Byte() = Encoding.Default.GetBytes("RADY")
            clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
            PictureBox2.Image = Nothing
        Catch ex As Exception
            MsgBox("請確認是否已Connect或另一段是否開啟")
        End Try

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not clientThread Is Nothing Then
            clientThread.Abort()
        End If

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try

            PictureBox1.Image = My.Resources.剪刀
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:剪刀")
            ClientOut = "剪刀"
            clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            PictureBox1.Image = My.Resources.石頭
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:石頭")
            ClientOut = "石頭"
            clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            PictureBox1.Image = My.Resources.布
            Dim msg As Byte()
            msg = Encoding.Default.GetBytes("out:布")
            ClientOut = "布"
            clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
        Catch ex As Exception

        End Try
        
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim msg As Byte()
        msg = Encoding.Default.GetBytes("Mesg" & TextBox2.Text)
        clientSocket.Send(msg, 0, msg.Length, SocketFlags.None)
    End Sub

    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label7.Click

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged
        If RadioButton1.Checked = True Then

        ElseIf RadioButton2.Checked = True Then

        ElseIf RadioButton3.Checked = True Then

        End If
    End Sub
End Class
