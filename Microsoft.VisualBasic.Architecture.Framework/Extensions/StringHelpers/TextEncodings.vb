﻿Imports System.Runtime.CompilerServices

Public Module TextEncodings

    Public Enum Encodings As Byte
        ASCII = 0
        Unicode
        UTF7
        UTF8
        UTF32
        GB2312
    End Enum

    Public ReadOnly Property TextEncodings As IReadOnlyDictionary(Of Encodings, System.Text.Encoding) =
        New Dictionary(Of Encodings, System.Text.Encoding) From {
 _
        {Encodings.ASCII, System.Text.Encoding.ASCII},
        {Encodings.GB2312, System.Text.Encoding.GetEncoding("GB2312")},
        {Encodings.Unicode, System.Text.Encoding.Unicode},
        {Encodings.UTF7, System.Text.Encoding.UTF7},
        {Encodings.UTF32, System.Text.Encoding.UTF32},
        {Encodings.UTF8, System.Text.Encoding.UTF8}
    }

    <Extension> Public Function GetEncodings(value As Encodings) As System.Text.Encoding
        If TextEncodings.ContainsKey(value) Then
            Return TextEncodings(value)
        Else
            Return System.Text.Encoding.UTF8
        End If
    End Function

    Public Function GetEncodings(value As System.Text.Encoding) As TextEncodings.Encodings
        Dim Name As String = value.ToString.Split("."c).Last

        Select Case Name
            Case NameOf(Encodings.ASCII) : Return Encodings.ASCII
            Case NameOf(Encodings.GB2312) : Return Encodings.GB2312
            Case NameOf(Encodings.Unicode) : Return Encodings.Unicode
            Case NameOf(Encodings.UTF32) : Return Encodings.UTF32
            Case NameOf(Encodings.UTF7) : Return Encodings.UTF7
            Case NameOf(Encodings.UTF8) : Return Encodings.UTF8
            Case Else
                Return Encodings.UTF8
        End Select
    End Function
End Module
