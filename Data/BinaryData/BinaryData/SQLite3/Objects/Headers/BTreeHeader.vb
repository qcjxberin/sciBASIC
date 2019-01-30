Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Internal
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core.Objects.Enums

Namespace ManagedSqlite.Core.Objects.Headers
    Friend Structure BTreeHeader
        Public Type As BTreeType

        Public FirstFreeBlock As UShort

        Public CellCount As UShort

        Public CellContentBegin As UShort

        Public CellContentFragmentedFreeBytes As Byte

        Public RightMostPointer As UInteger

        Public Shared Function Parse(reader As ReaderBase) As BTreeHeader
            reader.CheckSize(8)

            Dim res As New BTreeHeader()

            res.Type = CType(reader.ReadByte(), BTreeType)
            res.FirstFreeBlock = reader.ReadUInt16()
            res.CellCount = reader.ReadUInt16()
            res.CellContentBegin = reader.ReadUInt16()
            res.CellContentFragmentedFreeBytes = reader.ReadByte()

            If res.Type = BTreeType.InteriorIndexBtreePage OrElse res.Type = BTreeType.InteriorTableBtreePage Then
                reader.CheckSize(4)

                res.RightMostPointer = reader.ReadUInt32()
            End If

            Return res
        End Function
    End Structure
End Namespace