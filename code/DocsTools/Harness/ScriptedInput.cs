using System.Numerics;

using Yak2D;

namespace Harness
{
    // Wraps the real input service. Keys in the script are reported as held / pressed / released
    // exactly as the framework would, based on the update-step key state. Everything else passes through
    public class ScriptedInput : IInput
    {
        private readonly IInput _real;
        private HashSet<KeyCode> _held = new HashSet<KeyCode>();
        private HashSet<KeyCode> _previous = new HashSet<KeyCode>();
        private readonly Dictionary<KeyCode, float> _heldTime = new Dictionary<KeyCode, float>();

        public Vector2? MouseOverride { get; set; }

        public ScriptedInput(IInput real)
        {
            _real = real;
        }

        // Called once per update step, before the wrapped application's Update()
        public void Step(IEnumerable<KeyCode> heldThisUpdate, float dt)
        {
            _previous = _held;
            _held = new HashSet<KeyCode>(heldThisUpdate);
            foreach (var k in _held)
            {
                _heldTime[k] = _heldTime.TryGetValue(k, out var t) ? t + dt : 0.0f;
            }
            foreach (var k in _heldTime.Keys.Where(k => !_held.Contains(k)).ToList())
            {
                _heldTime.Remove(k);
            }
        }

        public NeoVeldrid.InputSnapshot RawVeldridInputSnapshot => _real.RawVeldridInputSnapshot;
        public Vector2 MousePosition => MouseOverride ?? _real.MousePosition;
        public bool IsMouseOverWindow => MouseOverride.HasValue || _real.IsMouseOverWindow;
        public Vector2 MousePositionDeltaSinceLastFrame => _real.MousePositionDeltaSinceLastFrame;
        public Vector2 MouseVelocity => _real.MouseVelocity;
        public bool IsMouseCurrentlyPressed(MouseButton button) => _real.IsMouseCurrentlyPressed(button);
        public bool WasMousePressedThisFrame(MouseButton button) => _real.WasMousePressedThisFrame(button);
        public bool WasMouseReleasedThisFrame(MouseButton button) => _real.WasMouseReleasedThisFrame(button);
        public float HowLongHasMouseBeenHeldDown(MouseButton button, bool countIfUpThisFrame = false) => _real.HowLongHasMouseBeenHeldDown(button, countIfUpThisFrame);
        public List<MouseButton> MouseButtonsPressedThisFrame() => _real.MouseButtonsPressedThisFrame();
        public List<MouseButton> MouseButtonsHeldDown() => _real.MouseButtonsHeldDown();
        public List<MouseButton> MouseButtonsReleasedThisFrame() => _real.MouseButtonsReleasedThisFrame();

        public bool IsKeyCurrentlyPressed(KeyCode key) => _held.Contains(key) || _real.IsKeyCurrentlyPressed(key);
        public bool WasKeyPressedThisFrame(KeyCode key) => (_held.Contains(key) && !_previous.Contains(key)) || _real.WasKeyPressedThisFrame(key);
        public bool WasKeyReleasedThisFrame(KeyCode key) => (!_held.Contains(key) && _previous.Contains(key)) || _real.WasKeyReleasedThisFrame(key);
        public float HowLongHasKeyBeenHeldDown(KeyCode key, bool countIfUpThisFrame = false) => _heldTime.TryGetValue(key, out var t) ? t : _real.HowLongHasKeyBeenHeldDown(key, countIfUpThisFrame);
        public List<KeyCode> KeysPressedThisFrame() => _held.Where(k => !_previous.Contains(k)).Concat(_real.KeysPressedThisFrame()).Distinct().ToList();
        public List<KeyCode> KeysHeldDown() => _held.Concat(_real.KeysHeldDown()).Distinct().ToList();
        public List<KeyCode> KeysReleasedThisFrame() => _previous.Where(k => !_held.Contains(k)).Concat(_real.KeysReleasedThisFrame()).Distinct().ToList();

        public bool IsGamepadIdValid(int id) => _real.IsGamepadIdValid(id);
        public List<int> ConnectedGamepadIds() => _real.ConnectedGamepadIds();
        public bool IsGamepadButtonCurrentlyPressed(int id, GamepadButton button) => _real.IsGamepadButtonCurrentlyPressed(id, button);
        public bool WasGamepadButtonPressedThisFrame(int id, GamepadButton button) => _real.WasGamepadButtonPressedThisFrame(id, button);
        public bool WasGamepadButtonReleasedThisFrame(int id, GamepadButton button) => _real.WasGamepadButtonReleasedThisFrame(id, button);
        public float HowLongHasGamepadButtonBeenHeldDown(int id, GamepadButton button, bool countIfUpThisFrame = false) => _real.HowLongHasGamepadButtonBeenHeldDown(id, button, countIfUpThisFrame);
        public List<GamepadButton> GamepadButtonsPressedThisFrame(int id) => _real.GamepadButtonsPressedThisFrame(id);
        public List<GamepadButton> GamepadButtonsHeldDown(int id) => _real.GamepadButtonsHeldDown(id);
        public List<GamepadButton> GamepadButtonsReleasedThisFrame(int id) => _real.GamepadButtonsReleasedThisFrame(id);
        public float GamepadAxisValue(int id, GamepadAxis axis) => _real.GamepadAxisValue(id, axis);
    }

    // Passes all services straight through, except Input which is replaced by the scripted version
    public class ServicesProxy : IServices
    {
        private readonly IServices _real;
        private readonly ScriptedInput _input;

        public ServicesProxy(IServices real, ScriptedInput input)
        {
            _real = real;
            _input = input;
        }

        public IBackend Backend => _real.Backend;
        public IDisplay Display => _real.Display;
        public IFps FPS => _real.FPS;
        public IStages Stages => _real.Stages;
        public IInput Input => _input;
        public ISurfaces Surfaces => _real.Surfaces;
        public ICameras Cameras => _real.Cameras;
        public IFonts Fonts => _real.Fonts;
        public IHelpers Helpers => _real.Helpers;
    }
}
