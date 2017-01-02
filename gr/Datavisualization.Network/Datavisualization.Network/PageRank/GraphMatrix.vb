﻿Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class GraphMatrix

    Dim indices As New Dictionary(Of String, List(Of Integer))
    Dim nodes As FileStream.Node()
    Dim edges As NetworkEdge()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="net"></param>
    ''' <param name="skipCount">
    ''' 对于文本处理的时候，textrank的这部分数据可能会比较有用，这个时候这里可以设置为False.
    ''' </param>
    Sub New(net As FileStream.Network, Optional skipCount As Boolean = True)
        nodes = net.Nodes
        edges = net.Edges

        Dim index As New IndexOf(Of String)(
            nodes.Select(Function(x) x.Identifier))

        For Each node As FileStream.Node In nodes
            Call indices.Add(
                node.Identifier,
                New List(Of Integer))
        Next

        For Each edge As NetworkEdge In edges
            Call indices(edge.FromNode) _
                .Add(index(edge.ToNode))
        Next

        If Not skipCount Then

            ' 对于文本处理的时候，textrank的这部分数据可能会比较有用
            Dim counts As New Dictionary(Of String, (Edge As NetworkEdge, C As int))
            Dim uid$

            For Each edge As NetworkEdge In edges
                uid = edge.GetDirectedGuid

                If Not counts.ContainsKey(Uid) Then
                    Call counts.Add(uid, (edge, 1))
                Else
                    counts(uid).C.value += 1
                End If
            Next

            ' 统计计数完毕之后再重新赋值
            For Each edge As NetworkEdge In edges
                uid = edge.GetDirectedGuid
                edge.Properties.Add("c", counts(uid).C)
            Next
        End If

        For Each k In indices.Keys.ToArray
            indices(k) = indices(k).Distinct.ToList
        Next
    End Sub

    Sub New(g As NetworkGraph)
        Call Me.New(g.Tabular)
    End Sub

    ''' <summary>
    ''' Save network
    ''' </summary>
    ''' <param name="DIR$"></param>
    Public Sub Save(DIR$)
        Call GetNetwork.Save(DIR)
    End Sub

    Public Function GetNetwork() As FileStream.Network
        Return New FileStream.Network With {
            .Nodes = nodes,
            .Edges = edges
        }
    End Function

    Public Function TranslateVector(v#(), Optional reorder As Boolean = False) As Dictionary(Of String, Double)
        If Not reorder Then
            Return nodes _
                .SeqIterator _
                .ToDictionary(Function(n) (+n).Identifier,
                              Function(i) v(i))
        Else
            Dim orders As SeqValue(Of Double)() = v _
                .SeqIterator _
                .OrderByDescending(Function(x) x.value) _
                .ToArray

            Return orders.ToDictionary(
                Function(i) nodes(i).Identifier,
                Function(value) +value)
        End If
    End Function

    Public Overrides Function ToString() As String
        Return indices.GetJson
    End Function

    Public Shared Narrowing Operator CType(gm As GraphMatrix) As List(Of Integer)()
        Return gm.nodes.ToArray(Function(k) gm.indices(k.Identifier))
    End Operator
End Class
