//#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

public partial class barqueseclecticplugin : EditorPlugin
{
	public override void _EnterTree()
	{// Load the script and icon
var buttonScript = GD.Load<Script>("res://addons/barqueseclecticplugin/EclecticBtnTest.cs");
var buttonIcon = GD.Load<Texture2D>("res://addons/barqueseclecticplugin/Icons/EclecticBtnIcon.jpg");

// Add the custom type
AddCustomType("Eclectic Test Button", "Button", buttonScript, buttonIcon);
		// Initialization of the plugin goes here.
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
	}
}
//#endif
