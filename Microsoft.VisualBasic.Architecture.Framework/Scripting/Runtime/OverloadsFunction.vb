﻿#Region "Microsoft.VisualBasic::c592c0d940206fd137183cd7a7fce348, ..\sciBASIC#\Microsoft.VisualBasic.Architecture.Framework\Scripting\Runtime\OverloadsFunction.vb"

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

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Scripting.Runtime

    Public Class OverloadsFunction
        Implements INamedValue

        Public Property Name As String Implements IKeyedEntity(Of String).Key

        ReadOnly functions As MethodInfo()

        Sub New(name$, methods As IEnumerable(Of MethodInfo))
            Me.Name = name
            Me.functions = methods.ToArray
        End Sub

        Public Function Match(args As Type()) As MethodInfo
            Dim alignments = functions.Select(Function(m) Align(m, args)).ToArray
            Dim p = Which.Max(alignments)

            If alignments(p) <= 0 Then
                Return Nothing
            End If

            Dim method As MethodInfo = functions(p)
            Return method
        End Function

        Public Shared Function Align(target As MethodInfo, args As Type()) As Double
            Dim params = target.GetParameters

            If args.Length > params.Length Then
                Return -1
            ElseIf params.Length = args.Length AndAlso args.Length = 0 Then
                Return 100000000
            End If

            Dim score#
            Dim tmp%

            For i As Integer = 0 To args.Length - 1
                tmp = 1000

                If Not args(i).IsInheritsFrom(params(i).ParameterType, False, tmp) Then
                    Return -1  ' 类型不符，则肯定不可以使用这个方法
                Else
                    score += (Short.MaxValue - tmp)
                End If
            Next

            Return score
        End Function

        Public Overrides Function ToString() As String
            Return $"{Name} (+{functions.Length} Overloads)"
        End Function
    End Class
End Namespace