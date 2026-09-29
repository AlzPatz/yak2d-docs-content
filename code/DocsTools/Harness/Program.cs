using System.Numerics;
using System.Reflection;
using Harness;
using Yak2D;

var options = Options.Parse(args);

var appType = FindApplicationType(options.App);
if (appType == null)
{
    Console.WriteLine($"[harness] Could not find IApplication type '{options.App}'");
    return 1;
}

Console.WriteLine($"[harness] Running {appType.FullName}");
var inner = (IApplication)Activator.CreateInstance(appType);
Launcher.Run(new CaptureApp(inner, options));
return 0;

static Type FindApplicationType(string name)
{
    foreach (var file in Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
    {
        var fileName = Path.GetFileNameWithoutExtension(file);
        if (fileName.StartsWith("Yak2D") || fileName.StartsWith("NeoVeldrid") || fileName.StartsWith("Silk") ||
            fileName.StartsWith("System") || fileName.StartsWith("Microsoft") || fileName.StartsWith("SixLabors") ||
            fileName == "Harness" || fileName.StartsWith("SimpleInjector"))
        {
            continue;
        }

        Assembly assembly;
        try { assembly = Assembly.LoadFrom(file); }
        catch { continue; }

        Type[] types;
        try { types = assembly.GetTypes(); }
        catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

        var match = types.FirstOrDefault(t => typeof(IApplication).IsAssignableFrom(t) && !t.IsAbstract &&
                                              (t.FullName == name || t.Name == name));
        if (match != null)
        {
            return match;
        }
    }
    return null;
}

namespace Harness
{
    public class Options
    {
        public string App { get; set; }
        public string Out { get; set; } = "capture.png";
        public float Seconds { get; set; } = 2.0f;
        public float MaxSeconds { get; set; } = 60.0f;
        public string Script { get; set; }
        public GraphicsApi? Api { get; set; }
        public GraphicsApi? SwitchApi { get; set; }
        public float? SwitchApiAt { get; set; }
        public bool RecreateOnDeviceReset { get; set; }
        public bool Trace { get; set; }
        public bool TraceUpdates { get; set; }
        public int ScaleTo { get; set; }
        public Vector2? Mouse { get; set; }

        public static Options Parse(string[] args)
        {
            var o = new Options();
            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                string Next() => args[++i];
                switch (a)
                {
                    case "--app": o.App = Next(); break;
                    case "--out": o.Out = Next(); break;
                    case "--seconds": o.Seconds = float.Parse(Next()); break;
                    case "--max-seconds": o.MaxSeconds = float.Parse(Next()); break;
                    case "--script": o.Script = Next(); break;
                    case "--api": o.Api = Enum.Parse<GraphicsApi>(Next()); break;
                    case "--switch-api": o.SwitchApi = Enum.Parse<GraphicsApi>(Next()); break;
                    case "--switch-api-at": o.SwitchApiAt = float.Parse(Next()); break;
                    case "--recreate-on-reset": o.RecreateOnDeviceReset = true; break;
                    case "--trace": o.Trace = true; break;
                    case "--trace-updates": o.TraceUpdates = true; break;
                    case "--scale-to": o.ScaleTo = int.Parse(Next()); break;
                    case "--mouse":
                        var parts = Next().Split(',');
                        o.Mouse = new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
                        break;
                    default: throw new ArgumentException("Unknown argument " + a);
                }
            }
            return o;
        }
    }

    // Named keyboard scripts: each step holds a key from Start to End (seconds since start)
    public static class Script
    {
        public record Step(KeyCode Key, float Start, float End)
        {
            public bool IsHeldAt(float t) => t >= Start && t < End;
        }

        public static List<Step> Get(string name) => name switch
        {
            null => new List<Step>(),
            _ => Scripts.TryGetValue(name, out var s) ? s : throw new ArgumentException("Unknown script " + name)
        };

        // Scripts are added as the documentation code needs them
        public static readonly Dictionary<string, List<Step>> Scripts = new Dictionary<string, List<Step>>
        {
            // A full run through the Yak Run level (jump times planned with a physics simulation of Yak.cs)
            ["yakrun"] = new List<Step> { new Step(KeyCode.Right, 0.3f, 60.0f) }
                .Concat(new[] { 2.758f, 4.383f, 5.283f, 6.65f, 7.442f, 8.175f, 9.1f, 11.125f, 12.175f }
                .Select(t => new Step(KeyCode.Space, t - 0.004f, t + 0.05f))).ToList(),

            // Run right into the first gap without jumping (Yak Run part 4: falling fades the world to grey)
            ["fall"] = new List<Step> { new Step(KeyCode.Right, 0.3f, 60.0f) },

            // Run right and jump once (Yak Run part 2)
            ["runjump"] = new List<Step> { new Step(KeyCode.Right, 0.3f, 60.0f), new Step(KeyCode.Space, 1.2f, 1.3f) },
        };
    }
}
