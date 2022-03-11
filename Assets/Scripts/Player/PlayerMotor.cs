using UnityEngine;

public class PlayerMotor : PlayerMovement
{
    public FloatVariable _turnSpeed;
    public FloatVariable _moveSpeed;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    float CalculateDirection(float _h, float _v)
    {
        return Mathf.Rad2Deg * (Mathf.Atan2(_h, _v));
    }

    void Rotate(float _h, float _v)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.Euler(Vector3.up * CalculateDirection(_h, _v)), _turnSpeed.Value * Time.deltaTime);
    }

    void MoveForward(float _h, float _v)
    {
        rb.velocity = transform.forward * _moveSpeed.Value * Time.deltaTime * (Mathf.Abs(_v) + Mathf.Abs(_h)) * rb.mass;
    }

    public void Move(float h, float v)
    {
        MoveForward(h, v);
        Rotate(h, v);
    }
}