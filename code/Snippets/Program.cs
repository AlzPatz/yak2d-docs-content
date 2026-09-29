using Snippets;
using Yak2D;

// Runs one of the documentation examples, chosen by name on the command line
var examples = typeof(Example).Assembly.GetTypes()
                                       .Where(t => typeof(IApplication).IsAssignableFrom(t) && !t.IsAbstract)
                                       .OrderBy(t => t.Name)
                                       .ToList();

var chosen = args.Length > 0 ? examples.FirstOrDefault(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase)) : null;

if (chosen == null)
{
    Console.WriteLine("Usage: dotnet run -- <ExampleName>");
    Console.WriteLine();
    examples.ForEach(t => Console.WriteLine("  " + t.Name));
    return;
}

Launcher.Run((IApplication)Activator.CreateInstance(chosen));
