using Godot;

namespace WizardsWithAPot.Behaviour
{
	public partial class TestScript : Node3D
	{

		[Export(PropertyHint.Range, "-10,10,0.1")]
		public float Speed { get; set; }

		private float LastSpeed = 0.0f;
		
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("Woah!");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			GD.Print($"Speed Delta: {LastSpeed} -> {Speed}");
			Translate(new Vector3(0, (float) delta * Speed, 0));
			LastSpeed = Speed;
		}
	}
}

