﻿#Region "Microsoft.VisualBasic::04574a34902b5ab4141d34a63b514af5, mime\text%yaml\Syntax\YamlDocument.vb"

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

'     Class YamlDocument
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Syntax

    Public Class YamlDocument

        Public Root As DataItem

        Public Directives As New List(Of Directive)()
        Public AnchoredItems As New Dictionary(Of String, DataItem)()

    End Class

    Public Class DataItem

        Public [Property] As NodeProperty

    End Class

    Public Class Mapping : Inherits DataItem

        Public Enties As New List(Of MappingEntry)()

        Public Function GetMaps() As Dictionary(Of MappingEntry)
            Return New Dictionary(Of MappingEntry)(Enties)
        End Function
    End Class

    Public Class Scalar : Inherits DataItem

        Public Text As String

        Public Sub New()
            Me.Text = String.Empty
        End Sub

        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    Public Class Sequence
        Inherits DataItem

        Public Enties As New List(Of DataItem)()

    End Class
End Namespace
