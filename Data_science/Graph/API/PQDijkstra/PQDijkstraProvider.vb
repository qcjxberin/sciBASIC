﻿#Region "Microsoft.VisualBasic::6954a42314c3147cc56cf03beb920f4d, ..\sciBASIC#\Data_science\Graph\API\PQDijkstra\PQDijkstraProvider.vb"

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

Namespace Dijkstra.PQDijkstra

    Public MustInherit Class PQDijkstraProvider

        Dim dijkstra As DijkstraFast

        ''' <summary>
        ''' get costs. If there is no connection, then cost is maximum.(»ñÈ¡)
        ''' </summary>
        ''' <param name="start"></param>
        ''' <param name="finish"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function getInternodeTraversalCost(start As Integer, finish As Integer) As Single
        ''' <summary>
        ''' »ñÈ¡ÓëÄ¿±ê½ÚµãÖ±½ÓÏàÁÚµÄËùÓÐµÄ½ÚµãµÄ±àºÅ
        ''' </summary>
        ''' <param name="startingNode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function GetNearbyNodes(startingNode As Integer) As IEnumerable(Of Integer)

        ''' <summary>
        ''' ÍøÂçÖ®ÖÐµÄ½ÚµãµÄ×ÜÊýÄ¿
        ''' </summary>
        ''' <param name="totalNodes"></param>
        ''' <remarks></remarks>
        Sub New(totalNodes As Integer)
            dijkstra = New DijkstraFast(totalNodes, New DijkstraFast.InternodeTraversalCost(AddressOf getInternodeTraversalCost), New DijkstraFast.NearbyNodesHint(AddressOf GetNearbyNodes))
        End Sub

        Public Function Compute(start As Integer, ends As Integer) As Integer()
            Dim minPath As Integer() = dijkstra.GetMinimumPath(start, ends)
            Return minPath
        End Function
    End Class
End Namespace
