using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;

    [Export] public RayCast3D AimRayCast;

    // Dit is je 'inventaris'. Het is een AutoResource (de data), niet de collider zelf.
    private AutoResource _vastgehoudenOnderdeel = null;

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
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding()) return;

        var collider = AimRayCast.GetCollider();

        // Check of we een fysiek onderdeel raken
        if (collider is AutoOnderdeel targetFysica)
        {
            var autoLogica = targetFysica.Owner?.GetNodeOrNull<AutoWerking>("AutoWerking");

            // SITUATIE 1: Je hebt al iets vast en wilt het in de auto plaatsen
            if (_vastgehoudenOnderdeel != null)
            {
                if (autoLogica != null)
                {
                    GD.Print($"Onderdeel {_vastgehoudenOnderdeel.PartName} plaatsen...");
                    autoLogica.InteractieMetOnderdeel(targetFysica, _vastgehoudenOnderdeel);
                    _vastgehoudenOnderdeel = null; // Hand is nu weer leeg
                }
            }
            // SITUATIE 2: Je hebt niets vast en wilt iets oppakken uit de auto
            else if (targetFysica.OnderdeelData != null)
            {
                _vastgehoudenOnderdeel = targetFysica.OnderdeelData;
                GD.Print($"Je hebt opgepakt: {_vastgehoudenOnderdeel.PartName}");

                // Optioneel: Maak het slot in de auto leeg als je het oppakt
                // autoLogica?.InstalleerOnderdeel(targetFysica.GetParent() as Marker3D, null);
            }
        }
    }

    public override void _Process(double delta)
    {
        CheckAim();
    }

    private void CheckAim()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding())
        {
            if (_lastLookedAt != null)
            {
                _lastLookedAt = null;
                GD.Print("Je kijkt nergens meer naar.");
            }
            return;
        }

        var collider = AimRayCast.GetCollider();

        if (collider is AutoOnderdeel target)
        {
            if (target != _lastLookedAt)
            {
                _lastLookedAt = target;

                if (target.OnderdeelData != null)
                {
                    string actie = (_vastgehoudenOnderdeel == null) ? "Pak op" : "Vervang";
                    GD.Print($"{actie}: {target.OnderdeelData.PartName}");
                }
                else if (_vastgehoudenOnderdeel != null)
                {
                    GD.Print($"Plaats {_vastgehoudenOnderdeel.PartName} in leeg slot");
                }
            }
        }
    }
}