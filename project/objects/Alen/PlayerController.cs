using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;
    [Export] public RayCast3D AimRayCast;
    [Export] public Node3D HoldPosition;
    private AutoOnderdeel _vastgehoudenObject = null;
    private bool _isHolding = false;
    private Node3D _lastLookedAt = null;
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        if (HoldPosition == null)
            GD.PrintErr("HoldPosition is niet toegewezen! Voeg een Marker3D toe aan de player.");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor()) velocity += GetGravity() * (float)delta;

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = _jumpVelocity;

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

        if (_isHolding && _vastgehoudenObject != null && IsInstanceValid(_vastgehoudenObject))
        {
            _vastgehoudenObject.GlobalPosition = HoldPosition.GlobalPosition;
            _vastgehoudenObject.GlobalRotation = HoldPosition.GlobalRotation;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            GD.Print("Input gedetecteerd! De knop werkt."); // VOEG DIT TOE
            HandleInteraction();
        }
        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(-mouseMotion.Relative.X * _mouseSensitivity);
            _camera.RotateX(-mouseMotion.Relative.Y * _mouseSensitivity);
            Vector3 rot = _camera.Rotation;
            rot.X = Mathf.Clamp(rot.X, -1.5f, 1.5f);
            _camera.Rotation = rot;
        }
    }

    private void HandleInteraction()
{
    if (AimRayCast == null || !AimRayCast.IsColliding()) return;

    var collider = AimRayCast.GetCollider();

    if (collider is AutoOnderdeel targetFysica)
    {
        var autoLogica = targetFysica.Owner as AutoWerking;
        if (autoLogica == null && targetFysica.Owner != null)
            autoLogica = targetFysica.Owner.FindChild("*", true, false) as AutoWerking;

        if (_isHolding && _vastgehoudenObject != null)
        {
            if (autoLogica != null)
            {
                GD.Print("Plaatsen in auto...");
                autoLogica.InteractieMetOnderdeel(targetFysica, _vastgehoudenObject.OnderdeelData);
                
                _vastgehoudenObject.QueueFree();
                _vastgehoudenObject = null;
                _isHolding = false;
            }
            return;
        }
        if (!_isHolding && targetFysica.OnderdeelData != null)
        {
            GD.Print("Oppakken uit auto...");
            AutoResource data = targetFysica.OnderdeelData;

            if (autoLogica != null && targetFysica.GetParent() is Marker3D slot)
            {
                autoLogica.InstalleerOnderdeel(slot, null);
            }

            SpawnObjectInHand(data);
            return;
        }
    }
}

private void SpawnObjectInHand(AutoResource data)
{
    var handObject = new AutoOnderdeel();
    handObject.OnderdeelData = data;
    
    if (data.OnderdeelModel != null)
    {
        var model = data.OnderdeelModel.Instantiate();
        handObject.AddChild(model);
    }

    HoldPosition.AddChild(handObject);
    handObject.Scale = new Vector3(0.2f, 0.2f, 0.2f);
    handObject.Position = Vector3.Zero;
    handObject.Rotation = Vector3.Zero;
    
    handObject.CollisionLayer = 0;
    handObject.CollisionMask = 0;

    _vastgehoudenObject = handObject;
    _isHolding = true;
}

    private void PickupObject(AutoOnderdeel onderdeel, AutoWerking autoLogica)
    {
        if (HoldPosition == null || onderdeel == null) return;

        _vastgehoudenObject = onderdeel;
        _isHolding = true;

        _vastgehoudenObject.SetProcess(false);
        _vastgehoudenObject.CollisionLayer = 0;
        _vastgehoudenObject.CollisionMask = 0;

        if (autoLogica != null && onderdeel.GetParent() is Marker3D slot)
        {
            autoLogica.InstalleerOnderdeel(slot, null);
        }

        if (_vastgehoudenObject.GetParent() != null)
            _vastgehoudenObject.GetParent().RemoveChild(_vastgehoudenObject);

        HoldPosition.AddChild(_vastgehoudenObject);
        _vastgehoudenObject.Position = Vector3.Zero;
        _vastgehoudenObject.Rotation = Vector3.Zero;

        GD.Print($"Opgepakt: {_vastgehoudenObject.OnderdeelData?.PartName}");
    }

    public override void _Process(double delta)
    {
        CheckAim();
    }

    private void CheckAim()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding())
        {
            _lastLookedAt = null;
            return;
        }

        var collider = AimRayCast.GetCollider();
        if (collider is AutoOnderdeel target && target != _lastLookedAt)
        {
            _lastLookedAt = target;
            if (!_isHolding && target.OnderdeelData != null)
                GD.Print($"Kijk naar: {target.OnderdeelData.PartName} [E]");
            else if (_isHolding)
                GD.Print("Klik om hier te plaatsen [E]");
        }
    }
}