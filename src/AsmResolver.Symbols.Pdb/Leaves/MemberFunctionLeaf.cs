using System.Linq;

namespace AsmResolver.Symbols.Pdb.Leaves;

/// <summary>
/// Represents a single instance member function.
/// </summary>
public class MemberFunctionLeaf : CodeViewLeaf
{
    private readonly LazyVariable<CodeViewTypeRecord?> _returnType;
    private readonly LazyVariable<CodeViewTypeRecord?> _declaringType;
    private readonly LazyVariable<CodeViewTypeRecord?> _thisType;
    private readonly LazyVariable<ArgumentListLeaf?> _argumentList;

    /// <summary>
    /// Initializes an empty member function.
    /// </summary>
    /// <param name="typeIndex">The type index to assign to the function.</param>
    protected MemberFunctionLeaf(uint typeIndex)
        : base(typeIndex)
    {
        _returnType = new LazyVariable<CodeViewTypeRecord?>(GetReturnType);
        _declaringType = new LazyVariable<CodeViewTypeRecord?>(GetDeclaringType);
        _thisType = new LazyVariable<CodeViewTypeRecord?>(GetThisType);
        _argumentList = new LazyVariable<ArgumentListLeaf?>(GetArguments);
    }

    /// <inheritdoc />
    public override CodeViewLeafKind LeafKind => CodeViewLeafKind.MFunction;

    /// <summary>
    /// Gets or sets the return type of the function.
    /// </summary>
    public CodeViewTypeRecord? ReturnType
    {
        get => _returnType.Value;
        set => _returnType.Value = value;
    }

    /// <summary>
    /// Gets or sets the type that declares this member function.
    /// </summary>
    public CodeViewTypeRecord? DeclaringType
    {
        get => _declaringType.Value;
        set => _declaringType.Value = value;
    }

    /// <summary>
    /// Gets or sets the type of the this pointer that is used to access the member function.
    /// </summary>
    public CodeViewTypeRecord? ThisType
    {
        get => _thisType.Value;
        set => _thisType.Value = value;
    }

    /// <summary>
    /// Gets or sets the convention that is used when calling the member function.
    /// </summary>
    public CodeViewCallingConvention CallingConvention
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the attributes associated to the function.
    /// </summary>
    public MemberFunctionAttributes Attributes
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the list of types of the parameters that this function defines.
    /// </summary>
    public ArgumentListLeaf? Arguments
    {
        get => _argumentList.Value;
        set => _argumentList.Value = value;
    }

    /// <summary>
    /// Gets or sets the offset to adjust the this pointer with before devirtualization of this method.
    /// </summary>
    public uint ThisAdjuster
    {
        get;
        set;
    }

    /// <summary>
    /// Obtains the return type of the function.
    /// </summary>
    /// <returns>The return type.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="ReturnType"/> property.
    /// </remarks>
    protected virtual CodeViewTypeRecord? GetReturnType() => null;

    /// <summary>
    /// Obtains the declaring type of the function.
    /// </summary>
    /// <returns>The declaring type.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="DeclaringType"/> property.
    /// </remarks>
    protected virtual CodeViewTypeRecord? GetDeclaringType() => null;

    /// <summary>
    /// Obtains the this-type of the function.
    /// </summary>
    /// <returns>The this-type.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="ThisType"/> property.
    /// </remarks>
    protected virtual CodeViewTypeRecord? GetThisType() => null;

    /// <summary>
    /// Obtains the argument types of the function.
    /// </summary>
    /// <returns>The argument types.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="Arguments"/> property.
    /// </remarks>
    protected virtual ArgumentListLeaf? GetArguments() => null;

    /// <inheritdoc />
    public override string ToString()
    {
        string args = string.Join(", ", Arguments?.Types ?? Enumerable.Empty<CodeViewTypeRecord>());
        return $"{CallingConvention} {ReturnType} {DeclaringType}::*({args})";
    }
}
