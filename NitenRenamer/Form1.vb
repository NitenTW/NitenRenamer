Imports System.ComponentModel
Imports System.IO
Imports NitenRenamer.ReName

Public Class Form1
#Region "宣告變數、物件和結構"
    Private Enum FormatType
        File
        Directory
    End Enum

    Private fileSourcePath As New ArrayList
#End Region

#Region "主控制項"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitState()
    End Sub

    '名稱
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Select Case ToolStripButton1.Checked
            Case True
                RadioButton1.Enabled = False
                RadioButton2.Enabled = False
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                TextBox1.Enabled = False
                TextBox2.Enabled = False
                TextBox3.Enabled = False
                ToolStripButton1.Checked = False
                TabControl1.SelectedTab = TabPage1
            Case False
                RadioButton1.Enabled = True
                RadioButton1.Checked = True
                TextBox1.Enabled = True
                RadioButton2.Enabled = True
                ToolStripButton1.Checked = True
                TabControl1.SelectedTab = TabPage2
        End Select
    End Sub

    '日期
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Select Case ToolStripButton2.Checked
            Case True
                MonthCalendar1.Enabled = False
                ToolStripButton2.Checked = False
                TabControl1.SelectedTab = TabPage1
            Case False
                MonthCalendar1.Enabled = True
                ToolStripButton2.Checked = True
                TabControl1.SelectedTab = TabPage3
        End Select
    End Sub

    '序號
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Select Case ToolStripButton3.Checked
            Case True
                NumericUpDown1.Enabled = False
                NumericUpDown3.Enabled = False
                ToolStripButton3.Checked = False
                CheckBox3.Enabled = False
                CheckBox3.Checked = False
                TabControl1.SelectedTab = TabPage1
            Case False
                NumericUpDown1.Enabled = True
                ToolStripButton3.Checked = True
                CheckBox3.Enabled = True
                TabControl1.SelectedTab = TabPage4
        End Select
    End Sub

    '亂數
    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Select Case ToolStripButton4.Checked
            Case True
                NumericUpDown2.Enabled = False
                ToolStripButton4.Checked = False
                CheckBox1.Enabled = False
                CheckBox2.Enabled = False
                CheckBox1.Checked = False
                CheckBox2.Checked = False
                TabControl1.SelectedTab = TabPage1
            Case False
                NumericUpDown2.Enabled = True
                ToolStripButton4.Checked = True
                CheckBox1.Enabled = True
                CheckBox2.Enabled = True
                CheckBox1.Checked = True
                CheckBox2.Checked = True
                TabControl1.SelectedTab = TabPage5
        End Select
    End Sub

    '拖曳效果
    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop
        '拖曳檔案至ListView控制項
        Dim sFile() As String = CType(e.Data.GetData(DataFormats.FileDrop), Array)

        For Each i In sFile
            Select Case CheckPath(i) '判斷是目錄還是檔案
                Case FormatType.File
                    If CheckFilePath(i) Then
                        ListBox1.Items.Add(Path.GetFileName(i))
                        fileSourcePath.Add(i)
                    End If


                Case FormatType.Directory
                    For Each foundFile As String In My.Computer.FileSystem.GetFiles(i)
                        If CheckFilePath(foundFile) Then
                            ListBox1.Items.Add(Path.GetFileName(foundFile))
                            fileSourcePath.Add(foundFile)
                        End If
                    Next
            End Select
        Next
    End Sub

    '按下執行鈕
    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        '沒開任何命名選項則離開副程式
        If ToolStripButton1.Checked = False And ToolStripButton2.Checked = False And ToolStripButton3.Checked = False And ToolStripButton4.Checked = False Then
            Exit Sub
        End If

        '沒新增檔案到ListBox1則離開副程式
        If ListBox1.Items.Count = 0 Then
            Exit Sub
        End If

        If ToolStripButton1.Checked AndAlso RadioButton2.Checked Then
            If TextBox2.Text = "" Then
                MsgBox(My.Resources.Lang.replaceString)
                Exit Sub
            End If
        End If

        If ToolStripButton4.Checked Then
            If Not CheckBox1.Checked And Not CheckBox2.Checked Then
                MsgBox(My.Resources.Lang.check)
                Exit Sub
            End If
        End If

        ToolStripProgressBar1.Value = 0
        DisabledControls()
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    '背景作業
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim tmpFileName As String
        Dim tmpExtension As String
        Dim m_RenameValue As String
        Dim rndState As RndType = GetRndState()

        For i As Integer = 0 To ListBox1.Items.Count - 1
            tmpExtension = GetFileExtension(0)   '取出副檔名
            m_RenameValue = ListBox1.Items(i)
            tmpFileName = GetNewName(m_RenameValue, i, rndState)
            CheckFile(tmpFileName, tmpExtension, i) '重新命名
            BackgroundWorker1.ReportProgress(i * 100 / (ListBox1.Items.Count - 1))
        Next
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MsgBox(My.Resources.Lang.finish)
        EnabledControlsd()
        InitState()
        ResetState()
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ToolStripProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            TextBox1.Enabled = True
            TextBox2.Enabled = False
            TextBox3.Enabled = False
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            TextBox1.Enabled = False
            TextBox2.Enabled = True
            TextBox3.Enabled = True
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        InitState()
        ResetState()
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            NumericUpDown3.Enabled = True
        Else
            NumericUpDown3.Enabled = False
        End If
    End Sub
#End Region

#Region "自訂函式、副程式"
    ''' <summary>
    ''' 驗證路徑是目錄或檔案
    ''' </summary>
    ''' <param name="tmpPath">路徑</param>
    ''' <returns>目錄傳回Directory，檔案傳回File</returns>
    Private Function CheckPath(ByVal tmpPath As String) As FormatType
        Dim result As FormatType

        If File.Exists(tmpPath) Then
            result = FormatType.File
        ElseIf Directory.Exists(tmpPath) Then
            result = FormatType.Directory
        Else
            Throw New Exception(My.Resources.Lang.unknownError)
        End If

        Return result
    End Function

    ''' <summary>
    ''' 初始狀態
    ''' </summary>
    Private Sub InitState()
        RadioButton1.Enabled = False
        RadioButton2.Enabled = False
        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        MonthCalendar1.Enabled = False
        NumericUpDown1.Enabled = False
        NumericUpDown2.Enabled = False
        NumericUpDown3.Enabled = False
        CheckBox1.Enabled = False
        CheckBox2.Enabled = False
        CheckBox3.Enabled = False
        ToolStripProgressBar1.Value = 0
    End Sub

    ''' <summary>
    ''' 重置狀態
    ''' </summary>
    Private Sub ResetState()
        ToolStripButton1.Checked = False
        ToolStripButton2.Checked = False
        ToolStripButton3.Checked = False
        ToolStripButton4.Checked = False
        CheckBox1.Checked = False
        CheckBox2.Checked = False
        CheckBox3.Checked = False
        RadioButton1.Checked = False
        RadioButton2.Enabled = False
        ListBox1.Items.Clear()
        fileSourcePath.Clear()
        NumericUpDown1.Value = 0
        NumericUpDown2.Value = 8
        NumericUpDown3.Value = 1
    End Sub

    '運算結束，開啟控制項
    Private Sub EnabledControlsd()
        ToolStripButton1.Enabled = True
        ToolStripButton2.Enabled = True
        ToolStripButton3.Enabled = True
        ToolStripButton4.Enabled = True
        ToolStripButton6.Enabled = True
    End Sub

    '運算時，關閉控制項
    Private Sub DisabledControls()
        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        MonthCalendar1.Enabled = False
        NumericUpDown1.Enabled = False
        NumericUpDown2.Enabled = False
        NumericUpDown3.Enabled = False
        CheckBox1.Enabled = False
        CheckBox2.Enabled = False
        CheckBox3.Enabled = False
        ToolStripButton1.Enabled = False
        ToolStripButton2.Enabled = False
        ToolStripButton3.Enabled = False
        ToolStripButton4.Enabled = False
        ToolStripButton6.Enabled = False
    End Sub

    Private Function GetNewName(ByVal oldName As String, ByVal index As Integer, ByVal rnsState As RndType) As String
        Dim result As String = String.Empty
        Dim rename As New ReName

        '名稱
        If ToolStripButton1.Checked Then
            If RadioButton1.Checked Then
                result &= rename.GetNewName(oldName, TextBox1.Text)
            End If
            If RadioButton2.Checked Then
                result &= Path.GetFileNameWithoutExtension(rename.GetNewName(oldName, TextBox2.Text, TextBox3.Text))
            End If
        End If

        '日期
        If ToolStripButton2.Checked Then
            result &= rename.GetDate(MonthCalendar1.SelectionStart)
        End If

        '序號
        If ToolStripButton3.Checked Then
            result &= rename.GetSerial(index + NumericUpDown1.Value, NumericUpDown3.Value, CheckBox3.Checked)
        End If

        '亂數
        If ToolStripButton4.Checked Then
            result &= rename.GetRnd(NumericUpDown2.Value, GetRndState)
        End If

        Return result
    End Function

    Private Function GetRndState() As RndType
        Dim result As RndType

        If CheckBox1.Checked And Not CheckBox2.Checked Then
            result = RndType.Number
        ElseIf Not CheckBox1.Checked And CheckBox2.Checked Then
            result = RndType.English
        ElseIf CheckBox1.Checked And CheckBox2.Checked Then
            result = RndType.Both
        End If

        Return result
    End Function

    ''' <summary>
    ''' 取得副檔名
    ''' </summary>
    ''' <param name="tmpValue">m_FilePath的索引編號</param>
    ''' <returns>傳回副檔名字串</returns>
    Private Function GetFileExtension(ByVal tmpValue As Integer) As String
        Return Path.GetExtension(ListBox1.Items(tmpValue))
    End Function

    '驗證檔案是否存在,重新命名
    Private Sub CheckFile(ByVal tmpFileName As String, ByVal tmpExtension As String, ByVal index As Integer)
        Dim count As Integer = 0
        Dim replaceName As String = tmpFileName
        Dim tmpDirectory As String = Path.GetDirectoryName(fileSourcePath(index))

        Do While File.Exists(tmpDirectory & "\" & tmpFileName & tmpExtension)
            count += 1
            tmpFileName = replaceName & "(" & count & ")"
        Loop

        My.Computer.FileSystem.RenameFile(fileSourcePath(index), tmpFileName & tmpExtension)
    End Sub

    ''' <summary>
    ''' 確認 m_FilePath 內的路徑是否相同
    ''' </summary>
    ''' <param name="parmString">待確認值</param>
    ''' <returns>有相同傳回 False，無則傳回 True</returns>
    Private Function CheckFilePath(ByVal parmString As String) As Boolean
        For Each j In fileSourcePath
            If j = parmString Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Sub DelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DelToolStripMenuItem.Click
        If ListBox1.SelectedIndex = -1 Then
            Exit Sub
        End If

        Dim index As Integer = ListBox1.SelectedIndex

        ListBox1.Items.RemoveAt(index)
        fileSourcePath.RemoveAt(index)
    End Sub
#End Region
End Class