namespace AsmResolver.Symbols.Pdb.Leaves;

/// <summary>
/// Represents a single enumerate field leaf in a field list.
/// </summary>
public class EnumerateField : CodeViewField
{
    private readonly LazyVariable<Utf8String> _name;
    private readonly LazyVariable<object> _value;

    /// <summary>
    /// Initializes an empty enumerate field leaf.
    /// </summary>
    /// <param name="typeIndex">The type index to assign to the enumerate field.</param>
    protected EnumerateField(uint typeIndex)
        : base(typeIndex)
    {
        _name = new LazyVariable<Utf8String>(GetName);
        _value = new LazyVariable<object>(GetValue);
    }

    /// <summary>
    /// Creates a new enumerate field leaf.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The value assigned to the field.</param>
    /// <param name="attributes">The attributes associated to the field.</param>
    public EnumerateField(Utf8String name, object value, CodeViewFieldAttributes attributes)
        : base(0)
    {
        _name = new LazyVariable<Utf8String>(name);
        _value = new LazyVariable<object>(value);
        Attributes = attributes;
    }

    /// <inheritdoc />
    public override CodeViewLeafKind LeafKind => CodeViewLeafKind.Enumerate;

    /// <summary>
    /// Gets or sets the name of the enumerate field.
    /// </summary>
    public Utf8String Name
    {
        get => _name.Value;
        set => _name.Value = value;
    }

    /// <summary>
    /// Gets or sets the constant value assigned to the field.
    /// </summary>
    public object Value
    {
        get => _value.Value;
        set => _value.Value = value;
    }

    /// <summary>
    /// Gets or sets the attributes associated to the field.
    /// </summary>
    public CodeViewFieldAttributes Attributes
    {
        get;
        set;
    }

    /// <summary>
    /// Obtains the name of the field.
    /// </summary>
    /// <returns>The name.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="Name"/> property.
    /// </remarks>
    protected virtual Utf8String GetName() => Utf8String.Empty;

    /// <summary>
    /// Obtains the value assigned to the field.
    /// </summary>
    /// <returns>The value.</returns>
    /// <remarks>
    /// This method is called upon initialization of the <see cref="Value"/> property.
    /// </remarks>
    protected virtual object? GetValue() => null;

    /// <inheritdoc />
    public override string ToString() => $"{Name} = {Value}";
}
