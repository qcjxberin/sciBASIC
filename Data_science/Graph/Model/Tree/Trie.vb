﻿Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 朴素字典树（Trie）
''' </summary>
Public Class Trie

    Public ReadOnly Property Root As New CharacterNode("*")

    ''' <summary>
    ''' 建立字典树
    ''' </summary>
    ''' <param name="word"></param>
    ''' <returns></returns>
    Public Function Add(word As String) As Trie
        Dim chars As New Pointer(Of Char)(word.SafeQuery)
        Dim child As CharacterNode = Root
        Dim c As Char
        Dim [next] As CharacterNode = Nothing

        If chars = 0 Then
            Return Me
        End If

        Do While Not chars.EndRead
            c = ++chars

            If child.Childs.ContainsKey(c) Then
                [next] = child(c)
            Else
                [next] = New CharacterNode(c) With {
                    .Parent = child
                }
                child.Childs.Add(c, [next])
            End If

            child = [next]
        Loop

        [next].Ends += 1

        Return Me
    End Function

    Private Function FindByPrefix(chars As Pointer(Of Char)) As (child As CharacterNode, success As Boolean)
        Dim child As CharacterNode = Root
        Dim c As Char
        Dim [next] As CharacterNode = Nothing

        Do While Not chars.EndRead
            c = ++chars

            If child.Childs.ContainsKey(c) Then
                [next] = child(c)
            Else
                Return (child, False)
            End If

            child = [next]
        Loop

        Return (child, True)
    End Function

    ''' <summary>
    ''' populate words by a given prefix string.
    ''' </summary>
    ''' <param name="prefix">The prefix string</param>
    ''' <returns></returns>
    Public Iterator Function PopulateWordsByPrefix(prefix As String) As IEnumerable(Of String)
        Dim chars As New Pointer(Of Char)(prefix.SafeQuery)

        With FindByPrefix(chars)
            If .success Then
                ' 将所有的子节点所构成的单词返回
                For Each last As Char() In Populate(.child, chars.RawBuffer.AsList)
                    Yield New String(last.ToArray)
                Next
            Else
                ' 不存在，则返回空集合
                Return
            End If
        End With
    End Function

    Private Iterator Function Populate(child As CharacterNode, prefix As List(Of Char)) As IEnumerable(Of IEnumerable(Of Char))
        If child.Ends > 0 Then
            ' 其自身也算
            Yield prefix.JoinIterates(child.Character)
        End If

        For Each [next] As CharacterNode In child.Childs.Values
            If [next].Ends > 0 Then
                ' 其自身也算
                Yield prefix.JoinIterates([next].Character)
            End If

            Dim nextPrefix As New List(Of Char)(prefix.JoinIterates([next].Character))

            For Each pop In Populate([next], nextPrefix)
                Yield pop
            Next
        Next
    End Function

    ''' <summary>
    ''' 查找某一个单词或者前缀是否在这颗字典树之中
    ''' </summary>
    ''' <param name="word"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Contains(word As String) As Boolean
        Return Count(word) > 0
    End Function

    ''' <summary>
    ''' 获取某一个单词/前缀在字典树之中的计数
    ''' </summary>
    ''' <param name="word"></param>
    ''' <returns></returns>
    Public Function Count(word As String) As Integer
        Dim chars As New Pointer(Of Char)(word.SafeQuery)

        If chars = 0 Then
            Return 0
        Else
            With FindByPrefix(chars)
                If .success Then
                    Return .child.Ends
                Else
                    Return 0
                End If
            End With
        End If
    End Function
End Class

''' <summary>
''' 在字典树之中，一个字母构成一个节点
''' </summary>
Public Class CharacterNode : Inherits AbstractTree(Of CharacterNode, Char)

    ''' <summary>
    ''' 以这个字符结束的单词的数目
    ''' </summary>
    ''' <returns></returns>
    Public Property Ends As Integer
    Public Property Character As Char

    Sub New(c As Char)
        Call MyBase.New(qualDeli:="")

        Me.Childs = New Dictionary(Of Char, CharacterNode)
        Me.Character = c
        Me.Label = c
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(c As CharacterNode) As Char
        Return c.Character
    End Operator
End Class