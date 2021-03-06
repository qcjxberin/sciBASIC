﻿#Region "Microsoft.VisualBasic::1101c126e3ee0249053f169381f8ded5, Data_science\MachineLearning\Darwinism\GeneticAlgorithm\GeneticAlgorithm.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class GeneticAlgorithm
    ' 
    '         Properties: Best, Fitness, ParentChromosomesSurviveCount, Population, Worst
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: evolIterate, GetFitness
    ' 
    '         Sub: Clear, Evolve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' *****************************************************************************
' Copyright 2012 Yuriy Lagodiuk
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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports Microsoft.VisualBasic.Math

Namespace Darwinism.GAF

    ''' <summary>
    ''' The GA engine core
    ''' </summary>
    ''' <typeparam name="Chr"></typeparam>
    Public Class GeneticAlgorithm(Of Chr As Chromosome(Of Chr)) : Inherits Model

        Const ALL_PARENTAL_CHROMOSOMES As Integer = Integer.MaxValue

        ReadOnly chromosomesComparator As Fitness(Of Chr)

        ''' <summary>
        ''' listeners of genetic algorithm iterations (handle callback afterwards)
        ''' </summary>
        ReadOnly iterationListeners As New List(Of IterationReporter(Of GeneticAlgorithm(Of Chr)))
        ReadOnly seeds As IRandomSeeds

        Public ReadOnly Property Population As Population(Of Chr)
        Public ReadOnly Property Fitness As Fitness(Of Chr)

        Public ReadOnly Property Best As Chr
            Get
                Return Population(0)
            End Get
        End Property

        Public ReadOnly Property Worst As Chr
            Get
                Return Population(Population.Size - 1)
            End Get
        End Property

        ''' <summary>
        ''' Number of parental chromosomes, which survive (and move to new
        ''' population)
        ''' </summary>
        ''' <returns></returns>
        Public Property ParentChromosomesSurviveCount As Integer = ALL_PARENTAL_CHROMOSOMES

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="population"></param>
        ''' <param name="fitnessFunc">
        ''' Calculates the fitness of the mutated chromesome in <paramref name="population"/>
        ''' </param>
        ''' <param name="seeds"></param>
        ''' <param name="cacheSize">
        ''' -1 means no cache
        ''' </param>
        Public Sub New(population As Population(Of Chr), fitnessFunc As Fitness(Of Chr), Optional seeds As IRandomSeeds = Nothing, Optional cacheSize% = 10000)
            Me.Population = population
            Me.Fitness = fitnessFunc

            If cacheSize <= 0 Then
                Me.chromosomesComparator = fitnessFunc
            Else
                Me.chromosomesComparator = New FitnessPool(Of Chr)(AddressOf fitnessFunc.Calculate, capacity:=cacheSize)
            End If

            Me.Population.SortPopulationByFitness(Me, chromosomesComparator)

            If population.Parallel Then
                Call "Genetic Algorithm running in parallel mode.".Warning
            End If
            If seeds Is Nothing Then
                seeds = Function() New Random(Now.Millisecond)
            End If

            Me.seeds = seeds
        End Sub

        Public Sub Evolve()
            Dim i% = 0
            Dim parentPopulationSize As Integer = Population.Size
            Dim newPopulation As New Population(Of Chr)(_Population.Pcompute) With {
                .Parallel = Population.Parallel
            }

            Do While (i < parentPopulationSize) AndAlso (i < ParentChromosomesSurviveCount)
                ' 旧的原有的种群
                newPopulation.Add(Population(i))
                i += 1
            Loop

            ' 新的突变的种群
            ' 这一步并不是限速的部分
            For Each c As Chr In parentPopulationSize% _
                .Sequence _
                .Select(AddressOf evolIterate) _
                .IteratesALL ' 并行化计算每一个突变迭代

                Call newPopulation.Add(c)
            Next

            newPopulation.SortPopulationByFitness(Me, chromosomesComparator) ' 通过fitness排序来进行择优
            newPopulation.Trim(parentPopulationSize)                         ' 剪裁掉后面的对象，达到淘汰的效果
            _Population = newPopulation                                      ' 新种群替代旧的种群
        End Sub

        ''' <summary>
        ''' 并行化过程之中的单个迭代
        ''' </summary>
        ''' <param name="i%"></param>
        ''' <returns></returns>
        Private Iterator Function evolIterate(i%) As IEnumerable(Of Chr)
            Dim chromosome As Chr = Population(i)
            Dim mutated As Chr = chromosome.Mutate()   ' 突变
            Dim rnd As Random = seeds()
            Dim otherChromosome As Chr = Population.Random(rnd)  ' 突变体和其他个体随机杂交
            Dim crossovered As IList(Of Chr) = mutated.Crossover(otherChromosome) ' chromosome.Crossover(otherChromosome)

            ' --------- 新修改的
            otherChromosome = Population.Random(rnd)
            crossovered = crossovered.Join(chromosome.Crossover(otherChromosome))
            ' ---------

            Yield mutated

            For Each c As Chr In crossovered
                Yield c
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFitness(chromosome As Chr) As Double
            Return chromosomesComparator.Calculate(chromosome)
        End Function

        ''' <summary>
        ''' Clear the internal cache
        ''' </summary>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Clear()
            If TypeOf chromosomesComparator Is FitnessPool(Of Chr) Then
                Call DirectCast(chromosomesComparator, FitnessPool(Of Chr)).Clear()
            End If
        End Sub
    End Class
End Namespace
