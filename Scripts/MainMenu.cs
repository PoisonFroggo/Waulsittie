using Godot;
using System;

public partial class MainMenu : Control
{

	[Export] public Button PlayBtn;
	[Export] public Button OptionsBtn;
	[Export] public Button ExitBtn;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_play_pressed() 
	{
		//GD.Print("Start Game");
		GetTree().ChangeSceneToFile("res://scenes/player_setup.tscn");
	}

	private void _on_options_pressed() 
	{
		GetTree().ChangeSceneToFile("res://scenes/options_menu.tscn");
	}

	private void _on_exit_pressed() 
	{
		GetTree().Quit();
	}
}
