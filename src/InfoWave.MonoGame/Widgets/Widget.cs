using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InfoWave.MonoGame.Widgets;

public abstract class Widget
{
    /// <summary>
    /// Determines if a widget will get rendered and updated
    /// </summary>
    public bool Enabled = true;

    public string Name;

    protected Widget(string name)
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

public class GridWidget : Widget
{
    public int CellSize;
    public int CellGap;

    public int HoverCellX;
    public int HoverCellY;

    public Vector2 HoverCellGridPos => new Vector2(HoverCellX, HoverCellY);

    public Vector2 HoverCellScreenPos =>
        new Vector2(HoverCellX * (CellSize + CellGap), HoverCellY * (CellSize + CellGap));

    public GridWidget(string name, int cellSize, int cellGap) : base(name)
    {
        CellSize = cellSize;
        CellGap = cellGap;
    }

    public int ToCellCoord(int screenCoord) => (int)MathF.Round((screenCoord - CellSize * 0.5f) / (CellSize + CellGap));

    public Vector2 ToScreenPos(Vector2 gridPos) =>
        new(gridPos.X * (CellSize + CellGap), gridPos.Y * (CellSize + CellGap));

    protected override void OnUpdate(GameTime gameTime)
    {
        var state = Mouse.GetState();

        HoverCellX = ToCellCoord(state.X);
        HoverCellY = ToCellCoord(state.Y);
    }

    protected override void OnGui()
    {
        ImGui.Text($"Cell Size: {CellSize}");
        ImGui.Text($"Cell Gap: {CellGap}");
        ImGui.Text($"Mouse X: {HoverCellX}");
        ImGui.Text($"Mouse Y: {HoverCellY}");
    }
}