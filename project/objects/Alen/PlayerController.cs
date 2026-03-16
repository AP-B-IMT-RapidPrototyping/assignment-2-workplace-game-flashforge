using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;

    [Export] private Camera3D _camera;

    [Export] private float _speed = 5.0f;
    [Export] private float _jumpVelocity = 4.5f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = _jumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * _speed;
            velocity.Z = direction.Z * _speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Horizontal rotation: roteer de HELE speler (Y-axis)
            RotateY(-mouseMotion.Relative.X * _mouseSensitivity);

            // Vertical rotation: roteer alleen de CAMERA (X-axis)
            _camera.RotateX(-mouseMotion.Relative.Y * _mouseSensitivity);

            // Voorkom dat de camera omdraait (clamp tussen -86° en +86°, of -1.5 en 1.5 radialen)
            Vector3 cameraRotation = _camera.Rotation;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, -1.5f, 1.5f);
            _camera.Rotation = cameraRotation;
        }
    }
}