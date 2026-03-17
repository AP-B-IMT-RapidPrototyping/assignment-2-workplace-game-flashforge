using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;

    [Export] public RayCast3D AimRayCast;

    // VERBETERING: Zorg dat dit type exact matcht met je Resource scriptnaam (AutoOnderdeel)
    [Export] public AutoOnderdeel TestOnderdeel;

    private Node3D _lastLookedAt = null;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = _jumpVelocity;
        }

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
            RotateY(-mouseMotion.Relative.X * _mouseSensitivity);
            _camera.RotateX(-mouseMotion.Relative.Y * _mouseSensitivity);

            Vector3 rot = _camera.Rotation;
            rot.X = Mathf.Clamp(rot.X, -1.5f, 1.5f);
            _camera.Rotation = rot;
        }

        if (@event.IsActionPressed("interact"))
        {
            if (AimRayCast != null && AimRayCast.IsColliding())
            {
                // We casten direct naar AutoOnderdeelFysica voor de interactie
                if (AimRayCast.GetCollider() is AutoOnderdeelFysica targetFysica)
                {
                    var autoLogica = targetFysica.Owner?.GetNodeOrNull<AutoWerking>("AutoWerking");

                    if (autoLogica != null)
                    {
                        autoLogica.InteractieMetOnderdeel(targetFysica, TestOnderdeel);
                    }
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        CheckAim();
    }

    private void CheckAim()
    {
        if (AimRayCast == null) return;

        if (!AimRayCast.IsColliding())
        {
            if (_lastLookedAt != null)
            {
                _lastLookedAt = null;
                GD.Print("Je kijkt nergens meer naar.");
            }
            return;
        }

        // VERBETERING: Gebruik één variabele en check op je specifieke type
        var collider = AimRayCast.GetCollider();

        if (collider is AutoOnderdeelFysica target)
        {
            if (target != _lastLookedAt)
            {
                _lastLookedAt = target;

                if (target.OnderdeelData != null)
                {
                    GD.Print("Je kijkt naar: " + target.OnderdeelData.PartName);
                }
                else
                {
                    GD.Print("Je kijkt naar onderdeel (zonder data): " + target.GetParent().Name);
                }
            }
        }
    }
}