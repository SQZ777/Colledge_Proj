Imports System.IO
Imports System.Net
Imports System.Text
Public Class Form1
    'Move Form1's sub   「移動視窗」
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
    'End Move Form1's sub
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
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not Form2.clientThread Is Nothing Then
            Form2.clientThread.Abort()
        End If
        If Not Form2.acceptThread Is Nothing Then
            Form2.acceptThread.Abort()
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form2.Show()
    End Sub
End Class