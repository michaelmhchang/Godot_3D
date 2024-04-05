using Godot;
using System;

public partial class MainCharacter : CharacterBody3D
{
    [Export]
    public float sensitivity = 0.35f;  

    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;


    private Camera3D _camera;
    private Node3D _cameraPivot;


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        _cameraPivot = GetNode<Node3D>("CameraPivot");


        // Confines mouse to the center of the game window
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }


    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion motionEvent)
        {
            InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;

            // Rotate camera on the X-Axis
            _cameraPivot.RotateX(Mathf.DegToRad(-1 * mouseEvent.Relative.Y * sensitivity));
            // Rotate character on the Y-Axis
            RotateY(-Mathf.DegToRad(mouseEvent.Relative.X * sensitivity));

            // Clamps camer rotation between [-70, 70] degrees so camera doesn't flip
            Vector3 cameraRot = _cameraPivot.RotationDegrees;
            cameraRot.X = Mathf.Clamp(cameraRot.X, -70, 70);
            _cameraPivot.RotationDegrees = cameraRot;
        }

    }
}
