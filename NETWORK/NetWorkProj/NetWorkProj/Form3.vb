Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Random
Public Class Form3
    Delegate Sub setForm(ByVal tmpStr As String)
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

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Form2.ImAsking = False Then
            Dim t As New Thread(AddressOf Threadlabel)
            t.Start()
            Button19.Text = "SendAnswer"
        Else
            Label2.Visible = False
            Label3.Visible = False
            Label4.Visible = False
            Label2.SendToBack()

            For i = 1 To 5
                Me.Controls("TextBox" & i).Visible = True
                Me.Controls("Label" & i + 4).Visible = True
            Next
            For i = 5 To 8
                Me.Controls("RadioButton" & i).Visible = True
                Me.Controls("RadioButton" & i).SendToBack()
            Next
            Button19.Text = "SetQuetion"
            
        End If
    End Sub
    Private Sub Threadlabel()
        For i = 213 To 145 Step -1
            movelabel2(i)
            Thread.Sleep(20)
            If i = 179 Then
                Thread.Sleep(400)
            End If
        Next

        LoadPanel("")
    End Sub
    Private Sub LoadPanel(ByVal tmpStr As String)
        If Panel2.InvokeRequired Then
            Dim d As New setForm(AddressOf LoadPanel)
            Invoke(d, tmpStr)
        Else
            Panel2.Left = 8
            Panel2.Visible = True
            LoadQuetion("")
        End If
    End Sub
    Private Sub LoadQuetion(ByVal tmpStr As String)
        If RadioButton1.InvokeRequired Then
            Dim d As New setForm(AddressOf LoadQuetion)
            Invoke(d, tmpStr)
        Else
            LoadQuetionTitle("")
            For i = 1 To 4
                Panel2.Controls("RadioButton" & i).Text = Form2.Quet(i)
            Next

        End If
    End Sub
    Private Sub LoadQuetionTitle(ByVal tmpStr As String)
        If Label1.InvokeRequired Then
            Dim d As New setForm(AddressOf LoadQuetionTitle)
            Invoke(d, tmpStr)
        Else
                Panel2.Controls("Label10").Text &= Form2.Quet(0)
        End If
    End Sub
    Private Sub movelabel2(ByVal tmpStr As String)
        If Label2.InvokeRequired Then
            Dim d As New setForm(AddressOf movelabel2)
            Invoke(d, tmpStr)
        End If
        Label2.Top = tmpStr

    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Dim msg As Byte()
        Dim Rb As RadioButton
        If Form2.ImAsking = False Then
            Dim reAns As Integer
            For i = 1 To 4
                Rb = Panel2.Controls("RadioButton" & i)
                If Rb.Checked Then
                    reAns = i
                    Exit For
                End If
            Next
            msg = Encoding.Default.GetBytes("reAn" & reAns)
            If Form1.CheckBox1.Checked Then
                Form2.lc.networkStream.Write(msg, 0, msg.Length)
            Else
                Form2.networkStream.Write(msg, 0, msg.Length)
            End If
            Form2.CheckAns(reAns)
            Form2.LockB22("Unlock")

        Else
            For i = 1 To 5
                msg = Encoding.Default.GetBytes("Quet" & Me.Controls("TextBox" & i).Text)
                Thread.Sleep(3)
                
                    If Form1.CheckBox1.Checked Then
                        Form2.lc.networkStream.Write(msg, 0, msg.Length)
                    Else
                        Form2.networkStream.Write(msg, 0, msg.Length)
                    End If

            Next

            Dim Ans As Integer
            For i = 5 To 8
                Rb = Me.Controls("RadioButton" & i)
                If Rb.Checked Then
                    Ans = i - 4
                    Exit For
                End If
            Next
            Form2.Answer = Ans
            msg = Encoding.Default.GetBytes("Answ" & Ans)
            If Form1.CheckBox1.Checked Then
                Form2.lc.networkStream.Write(msg, 0, msg.Length)
            Else
                Form2.networkStream.Write(msg, 0, msg.Length)
            End If
            Thread.Sleep(5)
            msg = Encoding.Default.GetBytes("AskY")
            If Form1.CheckBox1.Checked Then
                Form2.lc.networkStream.Write(msg, 0, msg.Length)
            Else
                Form2.networkStream.Write(msg, 0, msg.Length)
            End If
            Thread.Sleep(5)

            msg = Encoding.Default.GetBytes("Mesg" & "問題設好囉~")
            Form2.TextBox1.Text &= "你:問題設好囉~" & vbNewLine
            If Form1.CheckBox1.Checked Then
                Form2.lc.networkStream.Write(msg, 0, msg.Length)
            Else
                Form2.networkStream.Write(msg, 0, msg.Length)
            End If
            Form2.TextBox1.SelectionStart = TextBox1.TextLength
            Form2.TextBox1.SelectionLength = 0
            Form2.TextBox1.ScrollToCaret()
        End If
        Form2.ImAsking = False
        
        Me.Close()
    End Sub

End Class