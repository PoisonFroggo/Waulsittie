using Godot;
using System;

public partial class PauseMenu : Node
{
	
	private void _on_pause() {
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//GetNode<VBoxContainer>("ButtonOrg").Visible=true;
		GetTree().Paused = true;
	}
	
	private void _on_resume() {
		Input.MouseMode =Input.MouseModeEnum.Captured;
		//GetNode<VBoxContainer>("ButtonOrg").Visible=false;
		GetTree().Paused = false;
	}
	
	private void _on_exit() {
		GetTree().Quit();
	}
	
	public override void _Input(InputEvent @event) {
		if(@event.IsActionPressed("escape")) {
			_on_pause();
		}
	}
	
}
