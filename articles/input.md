---
uid: uid_input
---

# Input

Keyboard, mouse and gamepad input all come from [IInput](xref:Yak2D.IInput), available as `yak.Input` in `Update()` and as the `input` parameter of `Drawing()`.

[!code-csharp[](../code/Snippets/Application.cs#input-update)]

## Held, pressed and released

Every button and key can be asked three questions:

| Question | Keyboard | Mouse | Gamepad | True for |
|---|---|---|---|---|
| Is it down? | [IsKeyCurrentlyPressed](xref:Yak2D.IInput.IsKeyCurrentlyPressed*) | [IsMouseCurrentlyPressed](xref:Yak2D.IInput.IsMouseCurrentlyPressed*) | [IsGamepadButtonCurrentlyPressed](xref:Yak2D.IInput.IsGamepadButtonCurrentlyPressed*) | every update while held. Use for movement. |
| Did it just go down? | [WasKeyPressedThisFrame](xref:Yak2D.IInput.WasKeyPressedThisFrame*) | [WasMousePressedThisFrame](xref:Yak2D.IInput.WasMousePressedThisFrame*) | [WasGamepadButtonPressedThisFrame](xref:Yak2D.IInput.WasGamepadButtonPressedThisFrame*) | exactly one update. Use for actions: jump, fire, open a menu. |
| Did it just come up? | [WasKeyReleasedThisFrame](xref:Yak2D.IInput.WasKeyReleasedThisFrame*) | [WasMouseReleasedThisFrame](xref:Yak2D.IInput.WasMouseReleasedThisFrame*) | [WasGamepadButtonReleasedThisFrame](xref:Yak2D.IInput.WasGamepadButtonReleasedThisFrame*) | exactly one update. |

There are also `HowLongHas...BeenHeldDown()` methods (seconds, for charge-up moves), and methods returning lists of everything pressed, held or released (e.g. [KeysPressedThisFrame()](xref:Yak2D.IInput.KeysPressedThisFrame) for text entry or key rebinding).

> [!IMPORTANT]
> "This frame" means **this update step**. Pressed and released are worked out once per update, so check them in `Update()`. In `Drawing()` you may see the same press on several frames, or miss one entirely. "Is it down?" is fine anywhere.

## Keyboard

Keys are identified by [KeyCode](xref:Yak2D.KeyCode) values: `KeyCode.A`, `KeyCode.Space`, `KeyCode.Left`, `KeyCode.Number1`, `KeyCode.F1`, `KeyCode.ControlLeft` and so on. They refer to physical keys, named after a US keyboard layout.

## Mouse

| Property / method | Gives |
|---|---|
| [MousePosition](xref:Yak2D.IInput.MousePosition) | The pointer position in **window pixels, from the top-left**. Convert it to world or screen coordinates with the [coordinate transforms](coordinatesystems.md#converting-between-spaces). |
| [MousePositionDeltaSinceLastFrame](xref:Yak2D.IInput.MousePositionDeltaSinceLastFrame) | How far it moved since the last update. |
| [MouseVelocity](xref:Yak2D.IInput.MouseVelocity) | Its speed, in pixels per second. |
| [IsMouseOverWindow](xref:Yak2D.IInput.IsMouseOverWindow) | Whether the pointer is over the window (see the API notes for a caveat). |
| [MouseButton](xref:Yak2D.MouseButton) | `Left`, `Middle`, `Right`, and extra buttons. |

Hide the pointer with [IDisplay.SetCursorVisible(false)](xref:Yak2D.IDisplay.SetCursorVisible*).

## Gamepads

Gamepads use the Xbox-style layout (via SDL's game controller support, which recognises most controllers):

- [ConnectedGamepadIds()](xref:Yak2D.IInput.ConnectedGamepadIds) lists the ids of connected pads. Pass an id to every gamepad method.
- [GamepadButton](xref:Yak2D.GamepadButton): `A`, `B`, `X`, `Y`, `Back`, `Start`, `Guide`, `LeftShoulder`, `RightShoulder`, `LeftStick`, `RightStick`, `DPadUp`/`Down`/`Left`/`Right`.
- [GamepadAxisValue(id, axis)](xref:Yak2D.IInput.GamepadAxisValue*) reads a [GamepadAxis](xref:Yak2D.GamepadAxis): `LeftX`, `LeftY`, `RightX`, `RightY` (-1 to 1) and `TriggerLeft`, `TriggerRight`.
- The stick Y axes follow SDL's convention: **pushing up gives a negative value**. Negate it if your world has +Y up.
- Sticks rarely rest at exactly zero. Ignore small values (a "dead zone" of about 0.2).
- You receive `GamepadAdded` and `GamepadRemoved` [messages](lifecycle.md#framework-messages) as pads come and go.

## Raw input

[RawVeldridInputSnapshot](xref:Yak2D.IInput.RawVeldridInputSnapshot) exposes NeoVeldrid's underlying input snapshot for the current update, including the stream of typed characters (useful for text entry).

## See also

- Samples: [Input_MouseAndKeyboardUsage](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Input_MouseAndKeyboardUsage), [Input_GamepadUsage](https://github.com/AlzPatz/yak2d-samples/tree/master/src/Input_GamepadUsage)
- [Yak Run part 2](../tutorials/yakrun-2.md) reads keyboard and gamepad for a platform game.
- Full example code: [Application.cs](https://github.com/AlzPatz/yak2d-docs-content/blob/master/code/Snippets/Application.cs) (`InputExample`)
