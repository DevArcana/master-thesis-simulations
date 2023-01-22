using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulator;

public enum SceneStatus
{
    Creating,
    Created,
    Destroying,
    Destroyed
}

public abstract class Scene
{
    protected Scene(string name, GraphicsDevice graphicsDevice)
    {
        Name = name;
        GraphicsDevice = graphicsDevice;
        SpriteBatch = new SpriteBatch(graphicsDevice);
    }

    public readonly string Name;

    protected readonly GraphicsDevice GraphicsDevice;
    protected readonly SpriteBatch SpriteBatch;

    public SceneStatus Status { get; private set; }

    private readonly Stack<string> _logs = new();

    protected void Log(string line)
    {
        _logs.Push(line);
    }

    protected abstract void OnCreate();

    protected abstract void OnUpdate(GameTime gameTime);

    protected abstract void OnDestroy();

    protected abstract void OnDraw(GameTime gameTime);

    protected abstract void OnGui();

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
                OnUpdate(gameTime);
                break;
            case SceneStatus.Destroying:
                OnDestroy();
                Status = SceneStatus.Destroyed;
                _logs.Push("[lifetime] destroyed");
                break;
            case SceneStatus.Destroyed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Status));
        }
    }

    public void Draw(GameTime gameTime)
    {
        if (Status == SceneStatus.Created)
        {
            SpriteBatch.Begin();
            OnDraw(gameTime);
            SpriteBatch.End();
        }
    }
}