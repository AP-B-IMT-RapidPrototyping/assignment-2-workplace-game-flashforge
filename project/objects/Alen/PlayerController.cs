using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;

    [Export] public RayCast3D AimRayCast;
    [Export] public Node3D HoldPosition; // Marker3D voor vasthouden
    [Export] public AutoResource TestOnderdeel; // Dit blijft een Resource

    private Node3D _lastLookedAt = null;

    // Pickup/drop variabelen
    private StaticBody3D _heldObject = null;
    private bool _isHolding = false;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        if (HoldPosition == null)
            GD.PrintErr("HoldPosition is niet toegewezen! Voeg een Marker3D toe.");
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
            if (_isHolding)
            {
                DropObject();
            }
            else
            {
                TryPickupObject();
            }
        }
    }

    public override void _Process(double delta)
    {
        CheckAim();
    }

    private void TryPickupObject()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding()) return;

        var collider = AimRayCast.GetCollider();

        if (collider is StaticBody3D target)
        {
            if (target.IsInGroup("onderdelen"))
            {
                PickupObject(target);
            }
        }
    }

    private void PickupObject(StaticBody3D onderdeel)
    {
        if (_isHolding || onderdeel == null || HoldPosition == null) return;

        _heldObject = onderdeel;
        _isHolding = true;

        // FOUT 1 OPGELOST: Alleen collision uitzetten voor StaticBody3D
        // (geen check voor RigidBody3D meer omdat _heldObject StaticBody3D is)
        _heldObject.CollisionLayer = 0;
        _heldObject.CollisionMask = 0;

        // Ouder veranderen
        var originalParent = _heldObject.GetParent();
        originalParent?.RemoveChild(_heldObject);
        HoldPosition.AddChild(_heldObject);

        // Positie resetten
        _heldObject.Position = Vector3.Zero;
        _heldObject.Rotation = Vector3.Zero;

        // FOUT 2 OPGELOST: Check of het een AutoOnderdeel is en gebruik OnderdeelData.PartName
        if (_heldObject is AutoOnderdeel autoOnderdeel && autoOnderdeel.OnderdeelData != null)
        {
            GD.Print($"Opgepakt: {autoOnderdeel.OnderdeelData.PartName}");
        }
        else
        {
            GD.Print($"Opgepakt: {_heldObject.Name}");
        }
    }

    private void DropObject()
    {
        if (!_isHolding || _heldObject == null) return;

        var objectToDrop = _heldObject;

        // Uit HoldPosition halen
        var holdParent = objectToDrop.GetParent();
        holdParent?.RemoveChild(objectToDrop);

        // Toevoegen aan CurrentScene
        var sceneRoot = GetTree().CurrentScene;
        sceneRoot.AddChild(objectToDrop);

        // Positie instellen
        if (HoldPosition != null)
        {
            objectToDrop.GlobalPosition = HoldPosition.GlobalPosition;
        }

        // FOUT 1 OPGELOST: Collision weer aanzetten (geen RigidBody check)
        objectToDrop.CollisionLayer = 1;
        objectToDrop.CollisionMask = 1;

        // Reset variabelen
        _heldObject = null;
        _isHolding = false;

        GD.Print("Object losgelaten");
    }

    private void CheckAim()
    {
        if (AimRayCast == null) return;

        if (!AimRayCast.IsColliding())
        {
            if (_lastLookedAt != null)
            {
                _lastLookedAt = null;
                if (!_isHolding)
                    GD.Print("Je kijkt nergens meer naar.");
            }
            return;
        }

        var collider = AimRayCast.GetCollider();

        if (collider is AutoOnderdeel target)
        {
            if (target != _lastLookedAt && !_isHolding)
            {
                _lastLookedAt = target;

                // TEST: Print eerst alle properties om te zien wat er is
                GD.Print($"Target type: {target.GetType()}");
                GD.Print($"OnderdeelData is null? {target.OnderdeelData == null}");

                if (target.OnderdeelData != null)
                {
                    GD.Print($"OnderdeelData type: {target.OnderdeelData.GetType()}");
                    GD.Print($"PartName: {target.OnderdeelData.PartName}");
                    GD.Print($"Je kijkt naar: {target.OnderdeelData.PartName} (Druk E om op te pakken)");
                }
                else
                {
                    GD.Print("Je kijkt naar onderdeel (Druk E om op te pakken)");
                }
            }
        }
    }
}