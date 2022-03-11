using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Vector2 _inputParser;
    public Joystick _joystick;

    public DynamicJoystick _dynamicJoystick;

    public Vector2 _input = Vector2.zero;

    public PlayerInput playerInput;

    private void Start()
    {
        _inputParser = Vector2.zero;
        _joystick = GetComponent<Joystick>();
    }

    private void Update()
    {
        _input.x = _joystick.Horizontal;
        _input.y = _joystick.Vertical;

        _inputParser = _input;

        playerInput._input.Value = _inputParser;
    }
}