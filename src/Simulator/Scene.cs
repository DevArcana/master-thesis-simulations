using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;

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
    protected Scene(string name)
    {
        Name = name;
    }

    public string Name { get; }
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

    protected virtual void OnGui()
    {
        ImGui.Begin(Name, ImGuiWindowFlags.AlwaysAutoResize);
        ImGui.BeginListBox("logs");
        foreach (var log in _logs)
        {
            ImGui.Text(log);
        }

        ImGui.EndListBox();
        ImGui.End();
    }

    public void Destroy()
    {
        if (Status != SceneStatus.Created) return;

        Status = SceneStatus.Destroying;
        _logs.Push("[lifetime] destroying");
    }

    public void DrawGui()
    {
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
            OnDraw(gameTime);
        }
    }
}