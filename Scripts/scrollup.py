from pynput.mouse import Controller
import keyboard
import threading
import time

mouse = Controller()
scrolling = False

def scroll_loop():
    global scrolling
    while True:
        if scrolling:
            mouse.scroll(0, 100)  # Scroll up (0 horizontal, 1 vertical)
            time.sleep(0.01)    # Adjust speed here
        else:
            time.sleep(0.1)

def toggle_scroll():
    global scrolling
    scrolling = not scrolling
    print("Scrolling:", "ON" if scrolling else "OFF")

# Set up hotkey
keyboard.add_hotkey('F7', toggle_scroll)

# Start scroll thread
threading.Thread(target=scroll_loop, daemon=True).start()

print("Press F7 to toggle scrolling on/off. Press ESC to exit.")
keyboard.wait('esc')