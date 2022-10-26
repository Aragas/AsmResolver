using System.Collections.Generic;
using System.Linq;
using AsmResolver.DotNet.Serialized;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Metadata.Tables;

namespace AsmResolver.DotNet.Signatures
{
    /// <summary>
    /// Provides a context in which a metadata blob parser exists in. This includes the original module reader context
    /// as well as a mechanism to protect against infinite recursion.
    /// </summary>
    public readonly struct BlobReadContext
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlobReadContext"/> structure.
        /// </summary>
        /// <param name="readerContext">The original read context.</param>
        public BlobReadContext(ModuleReaderContext readerContext)
            : this(readerContext, PhysicalTypeSignatureResolver.Instance, Enumerable.Empty<MetadataToken>())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlobReadContext"/> structure.
        /// </summary>
        /// <param name="readerContext">The original read context.</param>
        /// <param name="resolver">The object responsible for resolving raw type metadata tokens and addresses.</param>
        public BlobReadContext(ModuleReaderContext readerContext, ITypeSignatureResolver resolver)
            : this(readerContext, resolver, Enumerable.Empty<MetadataToken>())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlobReadContext"/> structure.
        /// </summary>
        /// <param name="readerContext">The original read context.</param>
        /// <param name="resolver">The object responsible for resolving raw type metadata tokens and addresses.</param>
        /// <param name="traversedTokens">A collection of traversed metadata tokens.</param>
        public BlobReadContext(ModuleReaderContext readerContext, ITypeSignatureResolver resolver, IEnumerable<MetadataToken> traversedTokens)
        {
            ReaderContext = readerContext;
            TypeSignatureResolver = resolver;
            TraversedTokens = new HashSet<MetadataToken>(traversedTokens);
        }

        /// <summary>
        /// Gets the module reader context.
        /// </summary>
        public ModuleReaderContext ReaderContext
        {
            get;
        }

        /// <summary>
        /// Gets the object responsible for resolving raw type metadata tokens and addresses.
        /// </summary>
        public ITypeSignatureResolver TypeSignatureResolver
        {
            get;
        }

        /// <summary>
        /// Gets a collection of metadata tokens that were traversed during the parsing of metadata.
        /// </summary>
        public ISet<MetadataToken> TraversedTokens
        {
            get;
        }
    }
}
