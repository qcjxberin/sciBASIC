﻿#Region "Microsoft.VisualBasic::bfe506aeea8f90deb6d6d941d2dba7f5, mime\text%yaml\1.1\Base\ScalarStyle.vb"

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

    '     Enum ScalarStyle
    ' 
    '         DoubleQuoted, Hex, Plain, SingleQuoted
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Grammar11

    ''' <summary>
    ''' Specifies the style of a YAML scalar.
    ''' </summary>
    Public Enum ScalarStyle
        ''' <summary>
        ''' The plain scalar style.
        ''' </summary>
        Plain

        ''' <summary>
        ''' 
        ''' </summary>
        Hex

        ''' <summary>
        ''' The single-quoted scalar style.
        ''' </summary>
        SingleQuoted

        ''' <summary>
        ''' The double-quoted scalar style.
        ''' </summary>
        DoubleQuoted
    End Enum
End Namespace

