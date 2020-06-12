using System.Collections.Generic;
using System.Linq;

namespace AsmResolver.PE.Win32Resources.Version
{
    /// <summary>
    /// Represents the organization of data in a file-version resource. It contains version information not dependent
    /// on a particular language and code page combination.
    /// </summary>
    public class VarFileInfo : VersionTableEntry
    {
        /// <summary>
        /// The name of the VarFileInfo entry.
        /// </summary>
        public const string VarFileInfoKey = "VarFileInfo";

        /// <summary>
        /// Reads a single VarFileInfo structure from the provided input stream.
        /// </summary>
        /// <param name="startOffset">The offset of the consumed header.</param>
        /// <param name="header">The header.</param>
        /// <param name="reader">The input stream.</param>
        /// <returns>The read structure.</returns>
        /// <remarks>
        /// This function assumes the provided header was already consumed.
        /// </remarks>
        public static VarFileInfo FromReader(uint startOffset, ResourceTableHeader header, IBinaryStreamReader reader)
        {
            var result = new VarFileInfo();

            while (reader.FileOffset - startOffset < header.Length)
                result.Tables.Add(VarTable.FromReader(reader));
            
            return result;
        }

        /// <inheritdoc />
        public override string Key => VarFileInfoKey;

        /// <inheritdoc />
        protected override ResourceValueType ValueType => ResourceValueType.Binary;
            
        /// <summary>
        /// Gets a collection of tables stored in this VarFileInfo structure, typically containing a list of languages
        /// that the application or DLL supports.
        /// </summary>
        public IList<VarTable> Tables
        {
            get;
        } = new List<VarTable>();

        /// <inheritdoc />
        protected override uint GetValueLength() => (uint) Tables.Sum(e => e.GetPhysicalSize());

        /// <inheritdoc />
        protected override void WriteValue(IBinaryStreamWriter writer)
        {
            for (int i = 0; i < Tables.Count; i++)
                Tables[i].Write(writer);
        }
    }
}