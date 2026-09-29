using Yak2D;

namespace Snippets;

// Every documentation example derives from this small base class, which provides a standard
// 960 x 540 window and empty versions of the IApplication methods, so that each example only
// needs to contain the code it is demonstrating. A real application implements IApplication directly
public abstract class Example : IApplication
{
    public virtual StartupConfig Configure() => StartupConfig.Default(960, 540, GetType().Name, false);

    public virtual void OnStartup() { }

    public abstract bool CreateResources(IServices yak);

    public virtual void ProcessMessage(FrameworkMessage msg, IServices yak)
    {
        if (msg == FrameworkMessage.GraphicsDeviceRecreated)
        {
            CreateResources(yak);
        }
    }

    public virtual bool Update(IServices yak, float secondsSinceLastUpdate)
    {
        return !yak.Input.WasKeyPressedThisFrame(KeyCode.Escape);
    }

    public virtual void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public virtual void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate) { }

    public abstract void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget);

    public virtual void Shutdown() { }
}
