using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;

    [Export] public RayCast3D AimRayCast;
    [Export] public Node3D HoldPosition; // De Marker3D onder je camera

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

        // Object exact op de HoldPosition houden
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

        if (@event.IsActionPressed("interact"))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding()) return;

        var collider = AimRayCast.GetCollider();

        if (collider is AutoOnderdeel targetFysica)
        {
            // VERBETERING: Zoek AutoWerking op de root OF in de kinderen van de root
            AutoWerking autoLogica = targetFysica.Owner as AutoWerking;
            if (autoLogica == null && targetFysica.Owner != null)
            {
                autoLogica = targetFysica.Owner.FindChild("*", true, false) as AutoWerking;
            }

            // SITUATIE 1: Je hebt al iets vast -> Plaatsen
            if (_isHolding && _vastgehoudenObject != null)
            {
                if (autoLogica != null)
                {
                    GD.Print($"Plaatsen: {_vastgehoudenObject.OnderdeelData.PartName}");
                    autoLogica.InteractieMetOnderdeel(targetFysica, _vastgehoudenObject.OnderdeelData);

                    _vastgehoudenObject.QueueFree();
                    _vastgehoudenObject = null;
                    _isHolding = false;
                }
            }
            // SITUATIE 2: Hand is leeg -> Oppakken
            else if (!_isHolding && targetFysica.OnderdeelData != null)
            {
                PickupObject(targetFysica, autoLogica);
            }
        }
    }

    private void PickupObject(AutoOnderdeel onderdeel, AutoWerking autoLogica)
    {
        if (HoldPosition == null || onderdeel == null) return;

        _vastgehoudenObject = onderdeel;
        _isHolding = true;

        // Fysica uitschakelen terwijl je het vasthebt
        _vastgehoudenObject.SetProcess(false);
        _vastgehoudenObject.CollisionLayer = 0;
        _vastgehoudenObject.CollisionMask = 0;

        // Als het onderdeel in een auto zat, maak het slot in de auto leeg
        if (autoLogica != null && onderdeel.GetParent() is Marker3D slot)
        {
            autoLogica.InstalleerOnderdeel(slot, null);
        }

        // Verplaats naar de speler
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









/* oud script

using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;

    [Export] public RayCast3D AimRayCast;
    [Export] public Node3D HoldPosition;  // WAAR JE HET OBJECT VASTHOUDT

    // Het fysieke object dat je vasthoudt
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

        // Als je iets vasthoudt, update de positie (voor de zekerheid)
        if (_isHolding && _vastgehoudenObject != null && HoldPosition != null)
        {
            _vastgehoudenObject.Position = Vector3.Zero;
            _vastgehoudenObject.Rotation = Vector3.Zero;
        }
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
            if (_isHolding)
            {
                if (autoLogica != null)
                {
                    GD.Print($"Onderdeel {_vastgehoudenObject.OnderdeelData.PartName} plaatsen...");

                    // Bewaar data voordat we object verwijderen
                    var dataOmTePlaatsen = _vastgehoudenObject.OnderdeelData;

                    // Verwijder het vastgehouden object
                    _vastgehoudenObject.QueueFree();
                    _vastgehoudenObject = null;
                    _isHolding = false;

                    // Plaats in auto
                    autoLogica.InteractieMetOnderdeel(targetFysica, dataOmTePlaatsen);
                }
            }
            // SITUATIE 2: Je hebt niets vast en wilt iets oppakken
            else if (targetFysica.OnderdeelData != null)
            {
                PickupObject(targetFysica);
            }
        }
    }

    private void PickupObject(AutoOnderdeel onderdeel)
    {
        if (_isHolding || onderdeel == null || HoldPosition == null) return;

        _vastgehoudenObject = onderdeel;
        _isHolding = true;

        // Collision uitzetten zodat je er doorheen kunt lopen
        _vastgehoudenObject.CollisionLayer = 0;
        _vastgehoudenObject.CollisionMask = 0;

        // Verplaats naar HoldPosition
        var originalParent = _vastgehoudenObject.GetParent();
        originalParent?.RemoveChild(_vastgehoudenObject);
        HoldPosition.AddChild(_vastgehoudenObject);

        // Reset positie en rotatie
        _vastgehoudenObject.Position = Vector3.Zero;
        _vastgehoudenObject.Rotation = Vector3.Zero;

        GD.Print($"Opgepakt: {_vastgehoudenObject.OnderdeelData?.PartName ?? _vastgehoudenObject.Name}");
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

                if (target.OnderdeelData != null)
                {
                    GD.Print($"Je kijkt naar: {target.OnderdeelData.PartName} (Druk E om op te pakken)");
                }
            }
        }
        // Als je iets vasthoudt en naar een auto-onderdeel kijkt
        else if (_isHolding && collider is AutoOnderdeel autoTarget)
        {
            if (autoTarget != _lastLookedAt)
            {
                _lastLookedAt = autoTarget;
                GD.Print($"Plaats {_vastgehoudenObject?.OnderdeelData?.PartName ?? "onderdeel"} hier (Druk E)");
            }
        }
    }
}
*/