from pynput import keyboard
from pynput.keyboard import Key, Controller

keyboard_controller = Controller()
w_held = False  # Tracks whether 'w' is currently held down

def on_press(key):
    global w_held
    if key == Key.alt_l or key == Key.alt_r:
        if not w_held:
            keyboard_controller.press('w')
            w_held = True
        else:
            keyboard_controller.release('w')
            w_held = False

# Start listening to key presses
with keyboard.Listener(on_press=on_press) as listener:
    print("Press Alt to toggle 'W' key hold. Press ESC to exit.")
    listener.join()