using System.Collections.Generic;
using ImGuiNET;
using InfoWave.MonoGame.Core.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfoWave.MonoGame.Core.Scenes;

public enum SceneStatus
{
    Creating,
    Created,
    Destroying,
    Destroyed
}

public abstract class Scene
{
    protected Scene(string name, GraphicsDevice graphicsDevice, ImGuiRenderer imGuiRenderer)
    {
        Name = name;
        GraphicsDevice = graphicsDevice;
        ImGuiRenderer = imGuiRenderer;
        SpriteBatch = new SpriteBatch(graphicsDevice);
    }

    public readonly string Name;

    protected readonly GraphicsDevice GraphicsDevice;
    protected readonly ImGuiRenderer ImGuiRenderer;
    protected readonly SpriteBatch SpriteBatch;

    public SceneStatus Status { get; private set; }

    private readonly Stack<string> _logs = new();
    private readonly List<SceneLayer> _layers = new();

    protected void Log(string line)
    {
        _logs.Push(line);
    }

    protected abstract void OnCreate();

    protected abstract void OnUpdate(GameTime gameTime);

    protected abstract void OnDestroy();

    protected abstract void OnDraw(GameTime gameTime);

    protected abstract void OnGui();

    protected SceneLayer AddLayer(SceneLayer layer)
    {
        _layers.Add(layer);

        return layer;
    }

    public void Destroy()
    {
        if (Status != SceneStatus.Created) return;

        Status = SceneStatus.Destroying;
        _logs.Push("[lifetime] destroying");
    }

    public void DrawGui()
    {
        ImGui.Begin($"[{Name}] logs", ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.BeginListBox("logs");
        foreach (var log in _logs)
        {
            ImGui.Text(log);
        }

        ImGui.EndListBox();
        ImGui.End();

        foreach (var layer in _layers)
        {
            layer.UpdateGui();
        }

        OnGui();
    }

    public void Update(GameTime gameTime)
    {
        switch (Status)
        {
            case SceneStatus.Creating:
                _logs.Push("[lifetime] creating");
                OnCreate();
                Status = SceneStatus.Created;
                _logs.Push("[lifetime] created");
                break;
            case SceneStatus.Created:
                foreach (var layer in _layers)
                {
                    layer.Update(gameTime);
                }

                OnUpdate(gameTime);
                break;
            case SceneStatus.Destroying:
                OnDestroy();
                Status = SceneStatus.Destroyed;
                _logs.Push("[lifetime] destroyed");
                break;
            case SceneStatus.Destroyed:
                break;
        }
    }

    public void Draw(GameTime gameTime)
    {
        if (Status == SceneStatus.Created)
        {
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (var layer in _layers)
            {
                layer.Draw(GraphicsDevice, SpriteBatch, gameTime);
            }

            OnDraw(gameTime);
            SpriteBatch.End();
        }
    }
}