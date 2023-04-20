using System;
using ImGuiNET;
using InfoWave.MonoGame.Core.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InfoWave.MonoGame.Common.Widgets;

public class GridSceneLayer : SceneLayer
{
    public int CellSize;
    public int CellGap;

    public int HoverCellX;
    public int HoverCellY;

    public Vector2 HoverCellGridPos => new Vector2(HoverCellX, HoverCellY);

    public Vector2 HoverCellScreenPos =>
        new Vector2(HoverCellX * (CellSize + CellGap), HoverCellY * (CellSize + CellGap));

    public GridSceneLayer(string name, int cellSize, int cellGap) : base(name)
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