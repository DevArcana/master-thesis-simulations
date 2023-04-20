using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class AgentSceneLayer : SceneLayer
{
    private readonly PlaygroundSettings _settings;
    private readonly Texture2D _agentTexture;
    
    public AgentSceneLayer(PlaygroundSettings settings, GraphicsDevice graphicsDevice) : base("Agents")
    {
        _settings = settings;
        _agentTexture = Texture2D.FromFile(graphicsDevice, "Assets/KenneyMicroRoguelike/Tiles/Colored/tile_0004.png");
    }

    protected override void OnUpdate(GameTime gameTime)
    {
    }

    protected override void OnDraw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(_agentTexture, new Rectangle(0, 0, _settings.TileSize, _settings.TileSize), Color.White);
    }

    protected override void OnGui()
    {
    }
}