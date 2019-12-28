using System;
using System.Collections.Generic;
using AsmResolver.DotNet.Collections;
using AsmResolver.PE.DotNet.Metadata;
using AsmResolver.PE.DotNet.Metadata.Strings;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace AsmResolver.DotNet.Serialized
{
    /// <summary>
    /// Represents a lazily initialized implementation of <see cref="EventDefinition"/>  that is read from a
    /// .NET metadata image. 
    /// </summary>
    public class SerializedEventDefinition : EventDefinition
    {
        private readonly IMetadata _metadata;
        private readonly SerializedModuleDefinition _parentModule;
        private readonly EventDefinitionRow _row;

        /// <summary>
        /// Creates a event definition from a event metadata row.
        /// </summary>
        /// <param name="metadata">The object providing access to the underlying metadata streams.</param>
        /// <param name="parentModule">The module that contains the event.</param>
        /// <param name="token">The token to initialize the event for.</param>
        /// <param name="row">The metadata table row to base the event definition on.</param>
        public SerializedEventDefinition(IMetadata metadata, SerializedModuleDefinition parentModule, MetadataToken token, EventDefinitionRow row)
            : base(token)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _parentModule = parentModule ?? throw new ArgumentNullException(nameof(parentModule));
            _row = row;

            Attributes = row.Attributes;
        }

        /// <inheritdoc />
        protected override string GetName() =>
            _metadata.GetStream<StringsStream>().GetStringByIndex(_row.Name);

        /// <inheritdoc />
        protected override ITypeDefOrRef GetEventType()
        {
            var encoder = _metadata.GetStream<TablesStream>().GetIndexEncoder(CodedIndex.TypeDefOrRef);
            var eventTypeToken = encoder.DecodeIndex(_row.EventType);
            return _parentModule.TryLookupMember(eventTypeToken, out var member)
                ? member as ITypeDefOrRef
                : null;
        }

        /// <inheritdoc />
        protected override TypeDefinition GetDeclaringType()
        {
            var declaringTypeToken = new MetadataToken(TableIndex.TypeDef, _parentModule.GetEventDeclaringType(MetadataToken.Rid));
            return _parentModule.TryLookupMember(declaringTypeToken, out var member)
                ? member as TypeDefinition
                : null;
        }
        

        /// <inheritdoc />
        protected override IList<MethodSemantics> GetSemantics()
        {
            var result = new OwnedCollection<IHasSemantics, MethodSemantics>(this);

            foreach (uint rid in _parentModule.GetMethodSemantics(MetadataToken))
            {
                var semanticsToken = new MetadataToken(TableIndex.MethodSemantics, rid);
                result.Add((MethodSemantics) _parentModule.LookupMember(semanticsToken));
            }

            return result;
        }

        /// <inheritdoc />
        protected override IList<CustomAttribute> GetCustomAttributes() => 
            _parentModule.GetCustomAttributeCollection(this);
    }
}