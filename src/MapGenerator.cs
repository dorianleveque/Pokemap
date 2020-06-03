using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using tiled;

namespace pokemongenerator
{
  public class MapGenerator
  {
    public readonly List<GenerationStep> Steps = new List<GenerationStep>();
    public TiledMap Map;

    public MapGenerator(int width, int height)
    {
      Map = new TiledMap(width, height, "Terrain.tsx");
    }
    public void AddStep(GenerationStep step)
    {
      step.generator = this;
      Steps.Add(step);
    }
    public void Generate()
    {
      Steps.ForEach((step) =>
      {
        step.run();
      });
      Map.save();
    }
  }

  public abstract class GenerationStep
  {
    public MapGenerator generator;
    public abstract void run();
  }

  public class TerrainGenerator : GenerationStep
  {
    private FastNoise n;
    private enum Layer { DeepOcean, Ocean, Beach, Ground0, Ground1, Ground2, Ground3 };
    private enum TilePosition { TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight, CornerTopLeft, CornerTopRight, CornerBottomLeft, CornerBottomRight };
    private Dictionary<TilePosition, int> TilesDeepOcean = new Dictionary<TilePosition, int>();
    private Dictionary<TilePosition, int> TilesOcean = new Dictionary<TilePosition, int>();
    private Dictionary<TilePosition, int> TilesBeach = new Dictionary<TilePosition, int>();
    private Dictionary<TilePosition, int> TilesGround = new Dictionary<TilePosition, int>();
    public TerrainGenerator(int seed)
    {
      n = new FastNoise(seed);
      n.SetNoiseType(FastNoise.NoiseType.Perlin);
      n.SetFrequency(0.02f);

      TilesDeepOcean.Add(TilePosition.TopLeft, 9);
      TilesDeepOcean.Add(TilePosition.Top, 10);
      TilesDeepOcean.Add(TilePosition.TopRight, 11);
      TilesDeepOcean.Add(TilePosition.Left, 30);
      TilesDeepOcean.Add(TilePosition.Center, 31);
      TilesDeepOcean.Add(TilePosition.Right, 32);
      TilesDeepOcean.Add(TilePosition.BottomLeft, 51);
      TilesDeepOcean.Add(TilePosition.Bottom, 52);
      TilesDeepOcean.Add(TilePosition.BottomRight, 53);
      TilesDeepOcean.Add(TilePosition.CornerTopLeft, 31);
      TilesDeepOcean.Add(TilePosition.CornerTopRight, 31);
      TilesDeepOcean.Add(TilePosition.CornerBottomLeft, 31);
      TilesDeepOcean.Add(TilePosition.CornerBottomRight, 31);
      TilesOcean.Add(TilePosition.TopLeft, 6);
      TilesOcean.Add(TilePosition.Top, 7);
      TilesOcean.Add(TilePosition.TopRight, 8);
      TilesOcean.Add(TilePosition.Left, 27);
      TilesOcean.Add(TilePosition.Center, 28);
      TilesOcean.Add(TilePosition.Right, 29);
      TilesOcean.Add(TilePosition.BottomLeft, 48);
      TilesOcean.Add(TilePosition.Bottom, 49);
      TilesOcean.Add(TilePosition.BottomRight, 50);
      TilesOcean.Add(TilePosition.CornerTopLeft, 92);
      TilesOcean.Add(TilePosition.CornerTopRight, 91);
      TilesOcean.Add(TilePosition.CornerBottomLeft, 71);
      TilesOcean.Add(TilePosition.CornerBottomRight, 70);
      TilesBeach.Add(TilePosition.TopLeft, 72);
      TilesBeach.Add(TilePosition.Top, 73);
      TilesBeach.Add(TilePosition.TopRight, 74);
      TilesBeach.Add(TilePosition.Left, 93);
      TilesBeach.Add(TilePosition.Center, 94);
      TilesBeach.Add(TilePosition.Right, 95);
      TilesBeach.Add(TilePosition.BottomLeft, 114);
      TilesBeach.Add(TilePosition.Bottom, 115);
      TilesBeach.Add(TilePosition.BottomRight, 116);
      TilesBeach.Add(TilePosition.CornerTopLeft, 216);
      TilesBeach.Add(TilePosition.CornerTopRight, 215);
      TilesBeach.Add(TilePosition.CornerBottomLeft, 195);
      TilesBeach.Add(TilePosition.CornerBottomRight, 194);
      TilesGround.Add(TilePosition.TopLeft, 75);
      TilesGround.Add(TilePosition.Top, 55);
      TilesGround.Add(TilePosition.TopRight, 76);
      TilesGround.Add(TilePosition.Left, 35);
      TilesGround.Add(TilePosition.Center, 34);
      TilesGround.Add(TilePosition.Right, 33);
      TilesGround.Add(TilePosition.BottomLeft, 96);
      TilesGround.Add(TilePosition.Bottom, 13);
      TilesGround.Add(TilePosition.BottomRight, 97);
      TilesGround.Add(TilePosition.CornerTopLeft, 56);
      TilesGround.Add(TilePosition.CornerTopRight, 54);
      TilesGround.Add(TilePosition.CornerBottomLeft, 14);
      TilesGround.Add(TilePosition.CornerBottomRight, 12);
    }
    public override void run()
    {
      generator.Map.lines.ForEach((line) =>
      {
        float y = (float)generator.Map.lines.IndexOf(line);
        line.tiles.ForEach((tile) =>
        {
          float x = (float)line.tiles.IndexOf(tile);
          switch (GetLayer(x, y))
          {
            case Layer.DeepOcean: tile.id = GetCorrectLayerTile(x, y, Layer.DeepOcean, Layer.Ocean, TilesDeepOcean); break;
            case Layer.Ocean: tile.id = GetCorrectLayerTile(x, y, Layer.Ocean, Layer.Beach, TilesOcean); break;
            case Layer.Beach: tile.id = GetCorrectLayerTile(x, y, Layer.Beach, Layer.Ground0, TilesBeach); break;
            case Layer.Ground0: tile.id = GetCorrectLayerTile(x, y, Layer.Ground0, Layer.Ground1, TilesGround); break;
            case Layer.Ground1: tile.id = GetCorrectLayerTile(x, y, Layer.Ground1, Layer.Ground2, TilesGround); break;
            case Layer.Ground2: tile.id = GetCorrectLayerTile(x, y, Layer.Ground2, Layer.Ground3, TilesGround); break;
          }
        });
      });
    }

    /** 
    * Get the layer type for each positions on the map
    */
    private Layer GetLayer(float x, float y)
    {
      switch (n.GetNoise(x, y))
      {
        case float n when (n < -0.2f): return Layer.DeepOcean;
        case float n when (n < -0.1f): return Layer.Ocean;
        case float n when (n < 0.0f): return Layer.Beach;
        case float n when (n < 0.05f): return Layer.Ground0;
        case float n when (n < 0.6f): return Layer.Ground1;
        case float n when (n < 0.7f): return Layer.Ground2;
        default: return Layer.Ground3;
      }
    }

    /**
    * Get the correct Layer Tile for the connection between each layer type
    */
    private int GetCorrectLayerTile(float x, float y, Layer mainLayer, Layer neighborLayer, Dictionary<TilePosition, int> tilesID)
    {
      Layer cornerTopLeft = GetLayer(x - 1, y - 1);
      Layer cornerTopRight = GetLayer(x + 1, y - 1);
      Layer cornerBottomLeft = GetLayer(x - 1, y + 1);
      Layer cornerBottomRight = GetLayer(x + 1, y + 1);
      Layer topTile = GetLayer(x, y - 1);
      Layer rightTile = GetLayer(x + 1, y);
      Layer bottomTile = GetLayer(x, y + 1);
      Layer leftTile = GetLayer(x - 1, y);

      if (topTile == neighborLayer && leftTile == neighborLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopLeft];
      else if (topTile == neighborLayer && rightTile == neighborLayer && leftTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopRight];
      else if (bottomTile == neighborLayer && leftTile == neighborLayer && rightTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomLeft];
      else if (bottomTile == neighborLayer && rightTile == neighborLayer && leftTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomRight];

      else if (topTile == neighborLayer && rightTile == mainLayer && bottomTile == mainLayer && leftTile == mainLayer) return tilesID[TilePosition.Top];
      else if (rightTile == neighborLayer && topTile == mainLayer && bottomTile == mainLayer && leftTile == mainLayer) return tilesID[TilePosition.Right];
      else if (bottomTile == neighborLayer && topTile == mainLayer && rightTile == mainLayer && leftTile == mainLayer) return tilesID[TilePosition.Bottom];
      else if (leftTile == neighborLayer && topTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Left];

      else if (cornerTopLeft == neighborLayer) return tilesID[TilePosition.CornerTopLeft];
      else if (cornerTopRight == neighborLayer) return tilesID[TilePosition.CornerTopRight];
      else if (cornerBottomLeft == neighborLayer) return tilesID[TilePosition.CornerBottomLeft];
      else if (cornerBottomRight == neighborLayer) return tilesID[TilePosition.CornerBottomRight];
      else return tilesID[TilePosition.Center];
    }
  }
}