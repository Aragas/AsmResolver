using System;
using System.Collections.Generic;
using AsmResolver.Collections;
using AsmResolver.IO;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace AsmResolver.PE.DotNet.Metadata.Tables
{
    /// <summary>
    /// Provides an implementation of a metadata table that was serialized to a PE file.
    /// </summary>
    /// <typeparam name="TRow">The type of rows that this table stores.</typeparam>
    public class SerializedMetadataTable<TRow> : MetadataTable<TRow>
        where TRow : struct, IMetadataRow
    {
        /// <summary>
        /// Defines a method that reads a single row from an input stream, using the provided table layout.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the table.</param>
        public delegate TRow ReadRowDelegate(ref BinaryStreamReader reader, TableLayout layout);

        /// <summary>
        /// Defines a method that reads a single row from an input stream, using the provided table layout.
        /// </summary>
        /// <param name="context">The reader context.</param>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the table.</param>
        public delegate TRow ReadRowExtendedDelegate(
            PEReaderContext context,
            ref BinaryStreamReader reader,
            TableLayout layout);

        private readonly BinaryStreamReader _reader;
        private readonly TableLayout _originalLayout;
        private readonly int _rowCount;
        private readonly ReadRowDelegate _readRow;

        /// <summary>
        /// Reads a metadata table from an input stream.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="tableIndex">The index of the table.</param>
        /// <param name="originalLayout">The layout of the table.</param>
        /// <param name="readRow">The method to use for reading each row in the table.</param>
        public SerializedMetadataTable(in BinaryStreamReader reader, TableIndex tableIndex, TableLayout originalLayout, ReadRowDelegate readRow)
            : base(tableIndex, originalLayout)
        {
            _reader = reader;
            _originalLayout = originalLayout;
            _rowCount = (int) (reader.Length / originalLayout.RowSize);
            _readRow = readRow ?? throw new ArgumentNullException(nameof(readRow));
        }

        /// <summary>
        /// Reads a metadata table from an input stream.
        /// </summary>
        /// <param name="context">The reader context.</param>
        /// <param name="reader">The input stream.</param>
        /// <param name="tableIndex">The index of the table.</param>
        /// <param name="originalLayout">The layout of the table.</param>
        /// <param name="readRow">The method to use for reading each row in the table.</param>
        public SerializedMetadataTable(
            PEReaderContext context,
            in BinaryStreamReader reader,
            TableIndex tableIndex,
            TableLayout originalLayout,
            ReadRowExtendedDelegate readRow)
            : this(reader, tableIndex, originalLayout,
                (ref BinaryStreamReader r, TableLayout l) => readRow(context, ref r, l))
        {
        }

        /// <inheritdoc />
        public override int Count => IsInitialized ? Rows.Count : _rowCount;

        /// <inheritdoc />
        protected override RefList<TRow> GetRows()
        {
            var result = new RefList<TRow>(_rowCount);

            var reader = _reader.Fork();
            for (int i = 0; i < _rowCount; i++)
                result.Add(_readRow(ref reader, _originalLayout));

            return result;
        }

    }
}
