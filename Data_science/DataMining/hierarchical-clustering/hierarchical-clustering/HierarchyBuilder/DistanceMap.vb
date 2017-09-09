﻿#Region "Microsoft.VisualBasic::238eccb892af3f4ace656cfc34d160df, ..\sciBASIC#\Data_science\DataMining\hierarchical-clustering\hierarchical-clustering\HierarchyBuilder\DistanceMap.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Hierarchy


    ''' <summary>
    ''' Container for linkages
    ''' with the minimal methods needed in the package
    ''' Created by Alexandre Masselot on 7/18/14.
    ''' </summary>
    Public Class DistanceMap

        Private pairTable As Dictionary(Of String, Item)
        Private data As PriorityQueueTable(Of Item)

        Private Class Item : Implements IComparable, IComparable(Of Item), IReadOnlyId

            Private ReadOnly outerInstance As DistanceMap

            Friend ReadOnly pair As HierarchyTreeNode
            Friend removed As Boolean = False

            Public ReadOnly Property hash As String Implements IReadOnlyId.Identity

            Friend Sub New(outerInstance As DistanceMap, p As HierarchyTreeNode)
                Me.outerInstance = outerInstance
                pair = p
                hash = outerInstance.hashCodePair(p)
            End Sub

            Public Function compareTo(o As Item) As Integer Implements IComparable(Of Item).CompareTo
                Return pair.compareTo(o.pair)
            End Function

            Public Overrides Function ToString() As String
                Return hash
            End Function

            Private Function __compareTo(obj As Object) As Integer Implements IComparable.CompareTo
                Return compareTo(obj)
            End Function
        End Class

        Public Sub New()
            data = New PriorityQueue(Of Item)
            pairTable = New Dictionary(Of String, Item)
        End Sub

        Public Function list() As IList(Of HierarchyTreeNode)
            Dim l As IList(Of HierarchyTreeNode) = New List(Of HierarchyTreeNode)
            For Each clusterPair As Item In data
                l.Add(clusterPair.pair)
            Next
            Return l
        End Function

        Public Function findByCodePair(c1 As Cluster, c2 As Cluster) As HierarchyTreeNode
            Dim inCode As String = hashCodePair(c1, c2)
            Return pairTable(inCode).pair
        End Function

        Public Function RemoveFirst() As HierarchyTreeNode
            Dim poll As Item = data.Dequeue

            Do While poll IsNot Nothing AndAlso poll.removed
                poll = data.Dequeue
            Loop

            If poll Is Nothing Then
                Return Nothing
            Else
                With poll.pair
                    Call pairTable.Remove(poll.hash)
                    Return .ref
                End With
            End If
        End Function

        Public Function Remove(link As HierarchyTreeNode) As Boolean
            Dim ___remove As Item = pairTable.RemoveAndGet(hashCodePair(link))
            If ___remove Is Nothing Then
                Return False
            End If
            ___remove.removed = True
            data.Remove(___remove)
            Return True
        End Function

        Public Function Add(link As HierarchyTreeNode) As Boolean
            Dim e As New Item(Me, link)

            If pairTable.ContainsKey(e.hash) Then
                Dim existingItem As Item = pairTable(e.hash)
                Console.Error.WriteLine("hashCode = " & existingItem.hash & " adding redundant link:" & link.ToString & " (exist:" & existingItem.ToString & ")")
                Return False
            Else
                pairTable(e.hash) = e
                data.Enqueue(e)
                Return True
            End If
        End Function

        ''' <summary>
        ''' Peak into the minimum distance
        ''' @return
        ''' </summary>
        Public Function minDist() As Double
            Dim peek As Item = data.Peek()
            If peek IsNot Nothing Then
                Return peek.pair.LinkageDistance
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Compute some kind of unique ID for a given cluster pair. </summary>
        ''' <returns> The ID </returns>
        Friend Function hashCodePair(link As HierarchyTreeNode) As String
            Return hashCodePair(link.lCluster(), link.rCluster())
        End Function

        Friend Function hashCodePair(lCluster As Cluster, rCluster As Cluster) As String
            Return hashCodePairNames(lCluster.Name, rCluster.Name)
        End Function

        Friend Shared Function hashCodePairNames(lName As String, rName As String) As String
            If lName.CompareTo(rName) < 0 Then
                Return lName & "~~~" & rName 'getlCluster().hashCode() + 31 * (getrCluster().hashCode());
            Else
                Return rName & "~~~" & lName 'return getrCluster().hashCode() + 31 * (getlCluster().hashCode());
            End If
        End Function

        Public Overloads Function ToString() As String
            Return data.ToString()
        End Function
    End Class
End Namespace
