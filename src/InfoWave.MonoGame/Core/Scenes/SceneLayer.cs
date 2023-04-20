using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Core.Scenes;

public abstract class SceneLayer
{
    /// <summary>
    /// Determines if a scene layer will get rendered and updated
    /// </summary>
    public bool Enabled = true;

    public string Name;

    protected SceneLayer(string name)
    {
        Name = name;
    }

    public void Update(GameTime gameTime)
    {
        if (!Enabled) return;
        OnUpdate(gameTime);
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!Enabled) return;
        OnDraw(graphicsDevice, spriteBatch, gameTime);
    }

    public void UpdateGui()
    {
        ImGui.Checkbox($"Enable {Name}", ref Enabled);

        if (!Enabled) return;

        ImGui.Begin(Name, ImGuiWindowFlags.AlwaysAutoResize);
        OnGui();
        ImGui.End();
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {
    }

    protected virtual void OnDraw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
    }

    protected virtual void OnGui()
    {
    }
}