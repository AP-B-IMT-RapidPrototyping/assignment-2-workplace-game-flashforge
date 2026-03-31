using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Signal] public delegate void ItemOpgepaktEventHandler();
    [Signal] public delegate void ItemGeplaatstEventHandler();
    [Export] private float _mouseSensitivity = 0.003f;
    [Export] private Camera3D _camera;
    [Export] private float _speed = 2.0f;
    [Export] private float _jumpVelocity = 2.5f;
    [Export] public RayCast3D AimRayCast;
    [Export] public Node3D HoldPosition;
    [Export] public Interact InteractieLabel;
    [Export] public NPCManager NPCManager;
    [Export] public CollisionObject3D NPCcollider;

    private AutoOnderdeel _vastgehoudenObject = null;
    private bool _isHolding = false;
    private bool _isPausedInMenu = false;
    private Node3D _lastLookedAt = null;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isPausedInMenu) return;

        Vector3 velocity = Velocity;
        if (!IsOnFloor()) velocity += GetGravity() * (float)delta;

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = _jumpVelocity;

        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        velocity.X = direction.X * _speed;
        velocity.Z = direction.Z * _speed;

        Velocity = velocity;
        MoveAndSlide();

        if (_isHolding && IsInstanceValid(_vastgehoudenObject))
        {
            _vastgehoudenObject.GlobalPosition = HoldPosition.GlobalPosition;
            _vastgehoudenObject.GlobalRotation = HoldPosition.GlobalRotation;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_isPausedInMenu) return;

        if (@event.IsActionPressed("interact")) HandleInteraction();

        if (@event is InputEventMouseMotion mouseMotion)
        {
            RotateY(-mouseMotion.Relative.X * _mouseSensitivity);
            _camera.RotateX(-mouseMotion.Relative.Y * _mouseSensitivity);
            Vector3 rot = _camera.Rotation;
            rot.X = Mathf.Clamp(rot.X, -1.5f, 1.5f);
            _camera.Rotation = rot;
        }
    }

    public void SetMenuState(bool paused)
{
    _isPausedInMenu = paused;
    if (paused)
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }
    else
    {
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
    }
}
    private void HandleInteraction()
    {
        if (AimRayCast == null || !AimRayCast.IsColliding()) return;
        var collider = AimRayCast.GetCollider();

        NPCManager manager = null;
        Node tempNode = collider as Node;
        
        while (tempNode != null)
        {
            if (tempNode is NPCManager m)
            {
                manager = m;
                break;
            }
            tempNode = tempNode.GetParent();
        }

        if (manager != null)
        {
            manager.ToonPrijsOpUI();
            SetMenuState(true);
            return;
        }

        if (collider is AutoOnderdeel targetFysica)
        {
            AutoWerking autoLogica = targetFysica.Owner as AutoWerking ?? targetFysica.Owner?.FindChild("*", true, false) as AutoWerking;

            if (_isHolding && _vastgehoudenObject != null)
            {
                if (autoLogica != null && _vastgehoudenObject.OnderdeelData.Type == targetFysica.PastHierIn)
                {
                    autoLogica.InteractieMetOnderdeel(targetFysica, _vastgehoudenObject.OnderdeelData);
                    _vastgehoudenObject.QueueFree();
                    _vastgehoudenObject = null;
                    _isHolding = false;
                    EmitSignal(SignalName.ItemGeplaatst);
                }
            }
            else if (!_isHolding && targetFysica.OnderdeelData != null)
            {
                AutoResource data = targetFysica.OnderdeelData;
                if (autoLogica != null && targetFysica.GetParent() is Marker3D slot)
                {
                    autoLogica.InstalleerOnderdeel(slot, null);
                    targetFysica.OnderdeelData = null;
                }
                SpawnObjectInHand(data);
            }
        }
        else if (collider is GrondstoffenWinkel bron) bron.KoopOnderdeel(this);
    }

    public void SpawnObjectInHand(AutoResource data)
    {
        _vastgehoudenObject = new AutoOnderdeel { OnderdeelData = data };
        if (data.OnderdeelModel != null) _vastgehoudenObject.AddChild(data.OnderdeelModel.Instantiate());
        HoldPosition.AddChild(_vastgehoudenObject);
        _vastgehoudenObject.Scale = new Vector3(0.2f, 0.2f, 0.2f);
        _vastgehoudenObject.CollisionLayer = 0;
        _isHolding = true;
        EmitSignal(SignalName.ItemOpgepakt);
    }

    public override void _Process(double delta) => CheckInteractieDisplay();

    private void CheckInteractieDisplay()
{
    if (InteractieLabel == null || !AimRayCast.IsColliding()) 
    { 
        InteractieLabel?.VerbergMelding(); 
        return; 
    }
    
    var collider = AimRayCast.GetCollider();

    if (collider is AutoOnderdeel o)
    {
        if (o.OnderdeelData != null) InteractieLabel.ToonMelding(o.OnderdeelData.PartName, "pick");
        else if (_isHolding) InteractieLabel.ToonMelding(_vastgehoudenObject.OnderdeelData.PartName, "place");
        else InteractieLabel.VerbergMelding();
    }
    else if (collider is GrondstoffenWinkel w) 
    {
        InteractieLabel.ToonMelding(w.OnderdeelData.PartName, "buy", w.Prijs);
    }
    else if (IsNPC(collider)) 
    {
        InteractieLabel.ToonMelding("", "NPC");
    }
    else 
    {
        InteractieLabel.VerbergMelding();
    }
}

private bool IsNPC(GodotObject collider)
{
    if (collider is NPCManager) return true;
    
    if (collider is Node n)
    {
        if (n.GetParent() is NPCManager) return true;
        if (n.GetParent()?.GetParent() is NPCManager) return true;
    }
    return false;
}
    public bool IsHoldingSomething()
    {
        return _isHolding;
    }
}