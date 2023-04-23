using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Scenes.Playground;

public class AgentSceneLayer : SceneLayer, ITickable
{
    private readonly PlaygroundSettings _settings;
    private readonly Texture2D _agentTexture;
    
    private readonly Agent _agent = new();

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
        spriteBatch.Draw(_agentTexture, new Rectangle(_agent.x * _settings.TileSize, _agent.y * _settings.TileSize, _settings.TileSize, _settings.TileSize), Color.White);
    }

    protected override void OnGui()
    {
    }

    public void Tick()
    {
        _agent.Tick();
    }
}