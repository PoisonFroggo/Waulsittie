using Godot;
using System;
using System.Collections.Generic;

/*
Parent passes events to the abstract state (and thus the state machine). Abstract_state
passes the event to the active state and if the event allows the state to exit to a new
state, the current state sends an event back to the abstract_state telling it which state
to swap to
*/
public partial class abstract_state : Node
{
	public Node3D player { get; set; } //Using this we need to get the event as they occur
}
