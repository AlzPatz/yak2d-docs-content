using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Yak2D;

namespace Harness
{
    // Decorates a documentation IApplication:
    //  - Rendering() is redirected into an off-screen render target, which is then copied to the window
    //    and (at capture time) read back to the CPU and saved as a PNG
    //  - Keyboard input can be scripted over time
    //  - Every lifecycle call is logged, so the docs can state call order from observation
    public class CaptureApp : IApplication
    {
        private readonly IApplication _inner;
        private readonly Options _options;
        private readonly List<Script.Step> _script;

        private ScriptedInput _input;
        private ServicesProxy _proxy;

        private IRenderTarget _target;
        private ISurfaceCopyStage _copyStage;
        private uint _width;
        private uint _height;

        private float _time;
        private bool _captureRequested;
        private bool _captureQueued;
        private bool _saved;
        private int _framesAfterSave;

        public CaptureApp(IApplication inner, Options options)
        {
            _inner = inner;
            _options = options;
            _script = Script.Get(options.Script);
        }

        private void Log(string message)
        {
            if (_options.Trace)
            {
                Console.WriteLine($"[harness t={_time:0.000}] {message}");
            }
        }

        public StartupConfig Configure()
        {
            Log("Configure");
            var config = _inner.Configure();
            if (_options.Api.HasValue)
            {
                config.PreferredGraphicsApi = _options.Api.Value;
            }
            return config;
        }

        public void OnStartup()
        {
            Log("OnStartup");
            _inner.OnStartup();
        }

        public bool CreateResources(IServices yak)
        {
            Log("CreateResources");
            EnsureProxy(yak);

            var result = _inner.CreateResources(_proxy);

            var window = yak.Surfaces.GetSurfaceDimensions(yak.Surfaces.ReturnMainWindowRenderTarget());
            _width = (uint)window.Width;
            _height = (uint)window.Height;

            _target = yak.Surfaces.CreateRenderTarget(_width, _height, false, SamplerType.Point);
            _copyStage = yak.Stages.CreateSurfaceCopyDataStage(_width, _height, Save, false);

            return result;
        }

        private void EnsureProxy(IServices yak)
        {
            if (_proxy == null)
            {
                _input = new ScriptedInput(yak.Input);
                _proxy = new ServicesProxy(yak, _input);
            }
        }

        public void ProcessMessage(FrameworkMessage msg, IServices yak)
        {
            Log($"ProcessMessage({msg})");
            EnsureProxy(yak);
            _inner.ProcessMessage(msg, _proxy);

            if (msg == FrameworkMessage.GraphicsDeviceRecreated && _options.RecreateOnDeviceReset)
            {
                // Mirror what an application should do, so capture resources also come back
                CreateResources(yak);
            }
        }

        public bool Update(IServices yak, float secondsSinceLastUpdate)
        {
            _time += secondsSinceLastUpdate;

            _input.Step(_script.Where(s => s.IsHeldAt(_time)).Select(s => s.Key), secondsSinceLastUpdate);
            _input.MouseOverride = _options.Mouse;

            if (_options.SwitchApiAt.HasValue && _time >= _options.SwitchApiAt.Value && _options.SwitchApi.HasValue)
            {
                Log($"Requesting graphics API switch to {_options.SwitchApi.Value}");
                yak.Backend.SetGraphicsApi(_options.SwitchApi.Value);
                _options.SwitchApiAt = null;
            }

            if (_options.TraceUpdates)
            {
                Log("Update");
            }

            var keepRunning = _inner.Update(_proxy, secondsSinceLastUpdate);

            if (!_captureRequested && _time >= _options.Seconds)
            {
                _captureRequested = true;
            }

            if (_saved)
            {
                _framesAfterSave++;
            }

            if (_saved && _framesAfterSave > 2)
            {
                Log("Capture complete, exiting");
                return false;
            }

            if (_options.MaxSeconds > 0 && _time > _options.MaxSeconds)
            {
                Console.WriteLine("[harness] Timed out before capture");
                return false;
            }

            return keepRunning;
        }

        public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastupdate)
        {
            if (_options.TraceUpdates)
            {
                Log("PreDrawing");
            }
            _inner.PreDrawing(_proxy, secondsSinceLastDraw, secondsSinceLastupdate);
        }

        public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate)
        {
            if (_options.TraceUpdates)
            {
                Log("Drawing");
            }
            _inner.Drawing(draw, fps, _input, transforms, secondsSinceLastDraw, secondsSinceLastUpdate);
        }

        public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
        {
            if (_options.TraceUpdates)
            {
                Log("Rendering");
            }

            q.ClearColour(_target, Colour.Black);
            q.ClearDepth(_target);

            _inner.Rendering(q, _target);

            q.RemoveViewport();

            if (_captureRequested && !_captureQueued)
            {
                q.CopySurfaceData(_copyStage, _target);
                _captureQueued = true;
            }

            q.ClearColour(windowRenderTarget, Colour.Black);
            q.Copy(_target, windowRenderTarget);
        }

        private void Save(TextureData data)
        {
            if (_saved)
            {
                return;
            }

            using var image = new Image<Rgba32>((int)data.Width, (int)data.Height);
            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    var p = data.Pixels[(y * data.Width) + x];
                    // The window ignores alpha, so the capture does too
                    var c = new Vector4(p.X, p.Y, p.Z, 1.0f);
                    image[x, y] = new Rgba32(Vector4.Clamp(c, Vector4.Zero, Vector4.One));
                }
            }

            if (_options.ScaleTo > 0 && _options.ScaleTo != data.Width)
            {
                var h = (int)(data.Height * (_options.ScaleTo / (float)data.Width));
                image.Mutate(x => x.Resize(_options.ScaleTo, h));
            }

            var dir = Path.GetDirectoryName(Path.GetFullPath(_options.Out));
            Directory.CreateDirectory(dir);
            image.SaveAsPng(_options.Out);
            Console.WriteLine($"[harness] Saved {_options.Out} ({data.Width}x{data.Height})");
            _saved = true;
        }

        public void Shutdown()
        {
            Log("Shutdown");
            _inner.Shutdown();
        }
    }
}
