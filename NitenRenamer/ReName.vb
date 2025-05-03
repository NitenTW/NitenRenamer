Public Class ReName
    '產生亂數的型態。數字或英文
    Public Enum RndType
        Number = 10
        English = 52
        Both = 62
    End Enum

    '重新命名
    Public Function GetNewName(ByVal oldName As String, ByVal newName As String) As String
        Return newName
    End Function

    '取代檔案部份名稱
    Public Function GetNewName(ByVal oldName As String, ByVal oldString As String, ByVal newString As String) As String
        Return oldName.Replace(oldString, newString)
    End Function

    '日期
    Public Function GetDate(ByVal d As Date) As String
        Return Format(d, "yyyyMMdd")
    End Function

    '序號
    Public Function GetSerial(ByVal index As Decimal, ByVal digits As Decimal, ByVal switch As Boolean) As String
        Dim result As String = index.ToString

        '補足位數
        If switch Then
            result = 10 ^ digits + result
            result = Strings.Right(result, digits)
        End If

        Return result
    End Function

    Public Function GetRnd(ByVal max As Decimal, ByVal state As RndType) As String
        Dim result As String = String.Empty

        Select Case state
            Case RndType.Number
                For i As Integer = 0 To max - 1
                    result &= MakeRndValue(RndType.Number)
                Next

            Case RndType.English
                For i As Integer = 0 To max - 1
                    result &= GetRndEnglish()
                Next

            Case RndType.Both
                For i As Integer = 0 To max - 1
                    result &= GetRndBoth()
                Next
        End Select

        Return result
    End Function

    '產生亂數值
    Private Function MakeRndValue(ByVal tmpRndType As RndType) As UInteger
        Randomize()
        Return Fix(Rnd() * tmpRndType)
    End Function

    '產生亂數英文字
    Private Function GetRndEnglish() As Char
        Dim result As Char
        Dim tmpRnd As Integer = MakeRndValue(RndType.English)

        Select Case tmpRnd
            Case < 26
                result = Chr(tmpRnd + 65)

            Case >= 26
                result = Chr(tmpRnd + 71)
        End Select

        Return result
    End Function

    '產生亂數英數字
    Private Function GetRndBoth() As Char
        Dim result As Char
        Dim tmpRnd As UInteger = MakeRndValue(RndType.Both)

        Select Case tmpRnd
            Case < 10
                result = Chr(tmpRnd + 48)

            Case 10 To 35
                result = Chr(tmpRnd + 55)

            Case > 35
                result = Chr(tmpRnd + 61)
        End Select

        Return result
    End Function
End Class