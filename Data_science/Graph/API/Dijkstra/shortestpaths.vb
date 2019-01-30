Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports number = System.Double

Namespace Layouts.Cola.shortestpaths

    Public Class Neighbour

        Public id As Double
        Public distance As Double

        Public Sub New(id As Double, distance As Double)
            Me.id = id
            Me.distance = distance
        End Sub
    End Class

    Public Class Node

        Public id As Double

        Public Sub New(id As Double)
            Me.neighbours = New Neighbour() {}
        End Sub
        Public neighbours As Neighbour()
        Private d As Double
        Private prev As Node
        Private q As PairingHeap(Of Node)
    End Class

    Class QueueEntry

        Public node As Node
        Public prev As QueueEntry
        Public d As Double

        Public Sub New(node As Node, prev As QueueEntry, d As Double)
            Me.node = node
            Me.prev = prev
            Me.d = d
        End Sub
    End Class

    '*
    ' * calculates all-pairs shortest paths or shortest paths from a single node
    ' * @class Calculator
    ' * @constructor
    ' * @param n {number} number of nodes
    ' * @param es {Edge[]} array of edges
    ' 

    Class Calculator(Of Link)
        Private neighbours As Node()
        Public n As Integer
        Public es As Link()

        Private Sub New(n As Double, es As Link(), getSourceIndex As Func(Of Link, number), getTargetIndex As Func(Of Link, number), getLength As Func(Of Link, number))
            Me.neighbours = New Node(Me.n - 1) {}
            Dim i = Me.n
            While System.Math.Max(Interlocked.Decrement(i), i + 1)
                Me.neighbours(i) = New Node(i)
            End While

            i = Me.es.Length
            While System.Math.Max(Interlocked.Decrement(i), i + 1)
                Dim e = Me.es(i)
                Dim u = getSourceIndex(e)
                Dim v = getTargetIndex(e)
                Dim d = getLength(e)
                Me.neighbours(u).neighbours.Add(New Neighbour(v, d))
                Me.neighbours(v).neighbours.Add(New Neighbour(u, d))
            End While
        End Sub

        '*
        '     * compute shortest paths for graph over n nodes with edges an array of source/target pairs
        '     * edges may optionally have a length attribute.  1 is the default.
        '     * Uses Johnson's algorithm.
        '     *
        '     * @method DistanceMatrix
        '     * @return the distance matrix
        '     

        Private Function DistanceMatrix() As Double()()
            Dim D = New Double(Me.n)() {}
            For i As Integer = 0 To Me.n - 1
                D(i) = Me.dijkstraNeighbours(i)
            Next
            Return D
        End Function

        '*
        '     * get shortest paths from a specified start node
        '     * @method DistancesFromNode
        '     * @param start node index
        '     * @return array of path lengths
        '     

        Private Function DistancesFromNode(start As Double) As Double()
            Return Me.dijkstraNeighbours(start)
        End Function

        Private Function PathFromNodeToNode(start As Double, [end] As Double) As Double()
            Return Me.dijkstraNeighbours(start, [end])
        End Function

        ' find shortest path from start to end, with the opportunity at
        ' each edge traversal to compute a custom cost based on the
        ' previous edge.  For example, to penalise bends.
        Private Function PathFromNodeToNodeWithPrevCost(start As Integer, [end] As Double, prevCost As Func(Of number, number, number, number)) As Double()
            Dim q = New PriorityQueue(Of QueueEntry)(Function(a, b) a.d <= b.d)
            Dim u = Me.neighbours(start)
            Dim qu = New QueueEntry(u, Nothing, 0)
            Dim visitedFrom = New Object() {}
            q.push(qu)
            While Not q.empty()
                qu = q.pop()
                u = qu.node
                If u.id = [end] Then
                    Exit While
                End If
                Dim i = u.neighbours.Length
                While System.Math.Max(System.Threading.Interlocked.Decrement(i), i + 1)
                    Dim neighbour = u.neighbours(i)

                    Dim v = Me.neighbours(neighbour.id)

                    ' don't double back
                    If qu.prev AndAlso v.id = qu.prev.node.id Then
                        Continue While
                    End If

                    ' don't retraverse an edge if it has already been explored
                    ' from a lower cost route
                    Dim viduid = v.id + ","c + u.id
                    If visitedFrom.IndexOf(viduid) > -1 AndAlso visitedFrom(viduid) <= qu.d Then
                        Continue While
                    End If

                    Dim cc = If(qu.prev, prevCost(qu.prev.node.id, u.id, v.id), 0)
                    Dim t = qu.d + neighbour.distance + cc

                    ' store cost of this traversal
                    visitedFrom(viduid) = t
                    q.push(New QueueEntry(v, qu, t))
                End While
            End While
            Dim path As Double() = {}
            While qu.prev
                qu = qu.prev
                path.push(qu.node.id)
            End While
            Return path
        End Function

        Private Function dijkstraNeighbours(start As Double, Optional dest As Double = -1) As Double()
            Dim q = New PriorityQueue(Of Node)(Function(a, b) a.d <= b.d)
            Dim i As Integer = Me.neighbours.Length
            Dim d As Double() = New Array(i)

            While System.Math.Max(System.Threading.Interlocked.Decrement(i), i + 1)
                Dim node = Me.neighbours(i)
                node.d = If(i = start, 0, number.POSITIVE_INFINITY)
                node.q = q.push(node)
            End While
            While Not q.empty()
                ' console.log(q.toString(function (u) { return u.id + "=" + (u.d === Number.POSITIVE_INFINITY ? "\u221E" : u.d.toFixed(2) )}));
                Dim u = q.pop()
                d(u.id) = u.d
                If u.id = dest Then
                    Dim path As Double = {}
                    Dim v = u
                    While v.prev IsNot Nothing
                        path.push(v.prev.id)
                        v = v.prev
                    End While
                    Return path
                End If
                i = u.neighbours.Length
                While System.Math.Max(System.Threading.Interlocked.Decrement(i), i + 1)
                    Dim neighbour = u.neighbours(i)
                    Dim v = Me.neighbours(neighbour.id)
                    Dim t = u.d + neighbour.distance
                    If u.d <> number.MAX_VALUE AndAlso v.d > t Then
                        v.d = t
                        v.prev = u
                        q.reduceKey(v.q, v, Function(e, q) InlineAssignHelper(e.q, q))
                    End If
                End While
            End While
            Return d
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace