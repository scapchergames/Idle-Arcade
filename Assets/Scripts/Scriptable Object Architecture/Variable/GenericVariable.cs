using UnityEngine;

public class GenericVariable<T> : ScriptableObject
{
    [Multiline, SerializeField] private string DeveloperDescription = "";

    /// Generic default value
    [SerializeField] private T _defaultValue;

    /// Generic Original Value
    /// This is the modified value a copy of the default value so base value didn't modified
    private T _originalValue;

    private void OnEnable()
    {
        _originalValue = _defaultValue;
    }

    /// Value of the generic variable
    public T Value
    {
        get
        {
            return _originalValue;
        }
        set
        {
            _originalValue = value;
        }
    }
}