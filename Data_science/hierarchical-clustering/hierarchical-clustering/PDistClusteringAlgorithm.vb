Imports System.Collections.Generic
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.Hierarchy

'
'*****************************************************************************
' Copyright 2013 Lars Behnke
' 
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
' 
'   http://www.apache.org/licenses/LICENSE-2.0
' 
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' *****************************************************************************
'

Public Class PDistClusteringAlgorithm
    Implements ClusteringAlgorithm

    Public Function performClustering(distances As Double()(), clusterNames As String(), linkageStrategy As LinkageStrategy) As Cluster Implements ClusteringAlgorithm.performClustering

        ' Argument checks 
        If distances Is Nothing OrElse distances.Length = 0 Then Throw New System.ArgumentException("Invalid distance matrix")
        If distances(0).Length <> clusterNames.Length * (clusterNames.Length - 1) \ 2 Then Throw New System.ArgumentException("Invalid cluster name array")
        If linkageStrategy Is Nothing Then Throw New System.ArgumentException("Undefined linkage strategy")

        ' Setup model 
        Dim clusters As IList(Of Cluster) = createClusters(clusterNames)
        Dim linkages As DistanceMap = createLinkages(distances, clusters)

        ' Process 
        Dim builder As New HierarchyBuilder(clusters, linkages)
        Do While Not builder.TreeComplete
            builder.agglomerate(linkageStrategy)
        Loop

        Return builder.RootCluster
    End Function

    Public Function performFlatClustering(distances As Double()(), clusterNames As String(), linkageStrategy As LinkageStrategy, threshold As Double) As IList(Of Cluster) Implements ClusteringAlgorithm.performFlatClustering

        ' Argument checks 
        If distances Is Nothing OrElse distances.Length = 0 Then Throw New System.ArgumentException("Invalid distance matrix")
        If distances(0).Length <> clusterNames.Length * (clusterNames.Length - 1) \ 2 Then Throw New System.ArgumentException("Invalid cluster name array")
        If linkageStrategy Is Nothing Then Throw New System.ArgumentException("Undefined linkage strategy")

        ' Setup model 
        Dim clusters As IList(Of Cluster) = createClusters(clusterNames)
        Dim linkages As DistanceMap = createLinkages(distances, clusters)

        ' Process 
        Dim builder As New HierarchyBuilder(clusters, linkages)
        Return builder.flatAgg(linkageStrategy, threshold)
    End Function

    Public Function performWeightedClustering(distances As Double()(), clusterNames As String(), weights As Double(), linkageStrategy As LinkageStrategy) As Cluster Implements ClusteringAlgorithm.performWeightedClustering
        Return performClustering(distances, clusterNames, linkageStrategy)
    End Function

    Private Function createLinkages(distances As Double()(), clusters As IList(Of Cluster)) As DistanceMap
        Dim linkages As New DistanceMap
        For col As Integer = 0 To clusters.Count - 1
            Dim cluster_col As Cluster = clusters(col)
            For row As Integer = col + 1 To clusters.Count - 1
                Dim link As New ClusterPair
                Dim d As Double = distances(0)(accessFunction(row, col, clusters.Count))
                link.LinkageDistance = d
                link.setlCluster(cluster_col)
                link.setrCluster(clusters(row))
                linkages.add(link)
            Next row
        Next col
        Return linkages
    End Function

    Private Function createClusters(clusterNames As String()) As IList(Of Cluster)
        Dim clusters As IList(Of Cluster) = New List(Of Cluster)
        For Each clusterName As String In clusterNames
            Dim cluster As New Cluster(clusterName)
            cluster.addLeafName(clusterName)
            clusters.Add(cluster)
        Next clusterName
        Return clusters
    End Function

    ' Credit to this function goes to
    ' http://stackoverflow.com/questions/13079563/how-does-condensed-distance-matrix-work-pdist
    Private Shared Function accessFunction(i As Integer, j As Integer, n As Integer) As Integer
        Return n * j - j * (j + 1) \ 2 + i - 1 - j
    End Function

End Class
