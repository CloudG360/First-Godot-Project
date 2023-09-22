using System;
using Godot;

namespace WizardsWithAPot.Behaviour.Camera
{
	public partial class PanController : Camera2D
	{

		public static readonly Vector2 FixUpIsDown = new(1, -1);
		public static readonly Vector2 FlipDirection = new(-1, -1);

		[Export(PropertyHint.Range, "0,1500,0.1")]
		private float MaxSpeed { get; set; } = 500f;

		[Export(PropertyHint.Range, "0,1500,5")]
		private float AccelerationFactor { get; set; } = 750f;

		[Export(PropertyHint.Range, "0,1500,5")]
		private float DampingFactor { get; set; } = 600f;
		
		[Export(PropertyHint.Range, "0,5,0.1")]
		private float StoppingDampingBoost { get; set; } = 0.4f;
		
		[Export(PropertyHint.Range, "0.1,5,0.1")]
		private float ZoomStrength { get; set; } = 0.5f;
		
		[Export] private bool InvertX { get; set; }
		[Export] private bool InvertY { get; set; }

		private Vector2 _currentVelocity;
		
		
	
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_currentVelocity = Vector2.Zero;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			HandlePan((float) delta);
			//HandleZoom((float) delta);
		}

		private void HandleZoom()
		{
			float zoomDir = Input.GetAxis("cam_zoom_out", "cam_zoom_in");
			
			if(zoomDir == 0) 
				return;

			Vector2 zoomFac = Vector2.One * zoomDir * ZoomStrength;
			Vector2 newZoom = Zoom + zoomFac;
			
			Zoom = newZoom.Clamp(Vector2.Zero, Vector2.Inf);
		}



		private void HandlePan(float delta)
		{
			Vector2 inputVec = Input.GetVector(
				"cam_left", "cam_right",
				"cam_down", "cam_up"
			);

			bool isInputReceived = inputVec != Vector2.Zero;
			
			Vector2 baseAccel = isInputReceived 
				? AccelerationFactor * inputVec
				: new Vector2(0, 0);

			Vector2 currentMoveDir = _currentVelocity.Normalized();
			float currentSpeed = _currentVelocity.Length();
			float speedFrac = currentSpeed / MaxSpeed;

			float dampingFactor = isInputReceived 
				? speedFrac 
				: StoppingDampingBoost + speedFrac;

			baseAccel += Math.Min(dampingFactor, 1f) * (DampingFactor * currentMoveDir * FlipDirection);
			
			Vector2 adaptedAccel = baseAccel * delta;
			Vector2 newSpeed = _currentVelocity + adaptedAccel;
			Vector2 limitedSpeed = newSpeed.LimitLength(MaxSpeed);

			_currentVelocity = limitedSpeed.Length() < 0.1 
				? Vector2.Zero 
				: limitedSpeed;
			
			Translate(_currentVelocity * FixUpIsDown * delta * (1f / Zoom.X));
		}
		
		public override void _UnhandledInput(InputEvent @event)
		{
			HandleZoom();
		}
	}
}