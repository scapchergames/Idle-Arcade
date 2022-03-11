using System;
using UnityEngine;

[Serializable]
public class GenericReference<T>
{
    /// bool value indicate use constant value
    public bool UseConstant = true;

    /// Generic constant value
    public T ConstantValue;

    /// Generic Scriptable object value
    public GenericVariable<T> Variable;

    /// Default Constructor
    public GenericReference() { }

    /// Override Constructor
    /// <param name="value">value to assign</param>
    public GenericReference(T value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    /// Value of the object
    public T Value
    {
        get
        {
            return UseConstant ? ConstantValue : Variable.Value;
        }
        set
        {
            if (UseConstant)
                ConstantValue = value;
            else
                Variable.Value = value;
        }
    }
}