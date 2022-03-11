using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AnimationMotor))]
public class PlayerInput : MonoBehaviour
{
    public Vector2Variable _input;

    public BoolVariable _freezeOnUnHold;
    public PlayerMotor _motor;
    public AnimationMotor _animationMotor;

    private void Start()
    {
        _motor = GetComponent<PlayerMotor>();
        _animationMotor = GetComponent<AnimationMotor>();
        _freezeOnUnHold.Value = true;
    }

    private void FixedUpdate()
    {
        if (_freezeOnUnHold)
        {
            if (Input.GetMouseButton(0))
                _motor.Move(_input.Value.x, _input.Value.y);
        }
        else
        {
            _motor.Move(_input.Value.x, _input.Value.y);
        }

        _animationMotor.MoveAnimator(_input.Value.x, _input.Value.y);
    }
}