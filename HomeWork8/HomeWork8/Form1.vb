Imports System.IO
Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Public Class Form1
    Dim loc As Point
    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown 'wait to click down
        If e.Button = Windows.Forms.MouseButtons.Left Then
            loc = e.Location
        End If
    End Sub
    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove    'move form
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += e.Location - loc
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click   'end
        End
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click   'minimize form
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) Handles Label1.MouseDown 'wait to click down
        If e.Button = Windows.Forms.MouseButtons.Left Then
            loc = e.Location
        End If
    End Sub
    Private Sub Label1_MouseMove(sender As Object, e As MouseEventArgs) Handles Label1.MouseMove    'move form
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += e.Location - loc
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'load title
        Me.Text = Label1.Text
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click   'get ip
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

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            For i = 2 To 4
                Me.Controls("Label" & i).Visible = True
            Next
            'Me.Controls("RadioButton" & 2).Visible = True 'OUT TO BUG GROUP
            For i = 1 To 3
                Me.Controls("RadioButton" & i).Visible = True
            Next
            ComboBox2.Visible = True
        Else
            For i = 2 To 4
                Me.Controls("Label" & i).Visible = False
            Next
            For i = 1 To 3
                Me.Controls("RadioButton" & i).Visible = False
            Next
            ComboBox2.Visible = False
        End If
       
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If RadioButton1.Checked Then
            If ComboBox2.Text < 6 Then
                pre_stargame()
                Form3.Show()
                Exit Sub
            End If
        ElseIf RadioButton2.Checked Then
            If ComboBox2.Text < 8 Then
                pre_stargame()
                Form2.Show()
                Exit Sub
            End If
        ElseIf RadioButton3.Checked Then
            If ComboBox2.Text < 10 Then
                pre_stargame()
                Form4.Show()
                Exit Sub
            End If
        End If
        MessageBox.Show("連線數請選擇小於盤子的邊長*2")    '可再創一個form 製作Message Box
    End Sub
    Private Sub pre_stargame()
        For i = 1 To 3
            Me.Controls("RadioButton" & i).Enabled = False
        Next
        For i = 1 To 2
            Me.Controls("ComboBox" & i).Enabled = False
        Next
        For i = 3 To 4
            Me.Controls("Button" & i).Enabled = False
        Next
        CheckBox1.Enabled = False

    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'If Not Form2.ServerSocket Is Nothing Then
        '    Form2.ServerSocket.Close()
        'End If
        'If Not Form2.clientSocket Is Nothing Then
        '    Form2.clientSocket.Close()
        'End If
        If Not Form2.clientThread Is Nothing Then
            Form2.clientThread.Abort()
        End If
        If Not Form2.acceptThread Is Nothing Then
            Form2.acceptThread.Abort()
        End If
        If Not Form3.clientThread Is Nothing Then
            Form3.clientThread.Abort()
        End If
        If Not Form3.acceptThread Is Nothing Then
            Form3.acceptThread.Abort()
        End If
        If Not Form4.clientThread Is Nothing Then
            Form4.clientThread.Abort()
        End If
        If Not Form4.acceptThread Is Nothing Then
            Form4.acceptThread.Abort()
        End If

    End Sub

End Class