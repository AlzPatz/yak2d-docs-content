using Yak2D;

namespace MyFirstYakApp;

public class MyApplication : IApplication
{
    public StartupConfig Configure()
    {
        return StartupConfig.Default(960, 540, "My First yak2D App", false);
    }

    public void OnStartup() { }

    public bool CreateResources(IServices yak)
    {
        return true;
    }

    public void ProcessMessage(FrameworkMessage msg, IServices yak) { }

    public bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        return !yak.Input.IsKeyCurrentlyPressed(KeyCode.Escape);
    }

    public void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
    {
        q.ClearColour(windowRenderTarget, Colour.CornflowerBlue);
    }

    public void Shutdown() { }
}
