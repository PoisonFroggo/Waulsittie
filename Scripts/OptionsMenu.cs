using Godot;
using System;

public partial class OptionsMenu : Control
{
	[Export] public Button BackBtn;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_back_button_pressed() 
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
