using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

static class KeyMouseReader
{
	public static KeyboardState keyState, oldKeyState = Keyboard.GetState();
	public static MouseState mouseState, oldMouseState = Mouse.GetState();
	public static bool KeyPressed(Keys key) {
		return keyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key);
	}
	public static bool LeftClick() {
		return mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
	}
    public static bool LeftClickPressed()
    {
        return mouseState.LeftButton == ButtonState.Pressed;
    }
	public static bool RightClick() {
		return mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released;
	}
    public static Vector2 getNewPos()
    {
        return new Vector2(mouseState.X, mouseState.Y);
    }
    public static Vector2 getOldPos()
    {
        return new Vector2(oldMouseState.X, oldMouseState.Y);
    }
 

	//Should be called at beginning of Update in Game
	public static void Update() {
		oldKeyState = keyState;
		keyState = Keyboard.GetState();
		oldMouseState = mouseState;
		mouseState = Mouse.GetState();
	}
}