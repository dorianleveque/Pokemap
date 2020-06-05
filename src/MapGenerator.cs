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
      TilesGround.Add(TilePosition.TopLeft, 12);
      TilesGround.Add(TilePosition.Top, 13);
      TilesGround.Add(TilePosition.TopRight, 14);
      TilesGround.Add(TilePosition.Left, 33);
      TilesGround.Add(TilePosition.Center, 34);
      TilesGround.Add(TilePosition.Right, 35);
      TilesGround.Add(TilePosition.BottomLeft, 54);
      TilesGround.Add(TilePosition.Bottom, 55);
      TilesGround.Add(TilePosition.BottomRight, 56);
      TilesGround.Add(TilePosition.CornerTopLeft, 97);
      TilesGround.Add(TilePosition.CornerTopRight, 96);
      TilesGround.Add(TilePosition.CornerBottomLeft, 76);
      TilesGround.Add(TilePosition.CornerBottomRight, 75);
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
            case Layer.DeepOcean: tile.id = GetCorrectLayerTile(x, y, Layer.DeepOcean, TilesDeepOcean);
             break;
            case Layer.Ocean: tile.id = GetCorrectLayerTile(x, y, Layer.Ocean, TilesOcean); break;
            case Layer.Beach: tile.id = GetCorrectLayerTile(x, y, Layer.Beach, TilesBeach); break;
            case Layer.Ground0: tile.id = GetCorrectLayerTile(x, y, Layer.Ground0, TilesGround); break;
            case Layer.Ground1: tile.id = GetCorrectLayerTile(x, y, Layer.Ground1, TilesGround); break;
            case Layer.Ground2: tile.id = GetCorrectLayerTile(x, y, Layer.Ground2, TilesGround); break;
            case Layer.Ground3: tile.id = GetCorrectLayerTile(x, y, Layer.Ground3, TilesGround); break;
          }
        });
      });
      AddHouses(20);
      AddPath();
    }

    private void AddHouses(int number)
    {
      Random r = new Random();
      int count  = 0 ;
      int x;
      int y;
      int categorie = 0;
      int height = generator.Map.lines.Count ;
      int width = generator.Map.lines[0].tiles.Count;
      while (count < number )
      {
        y = r.Next(1,height);
        x = r.Next(1,width);
        categorie = r.Next(0,6);
        if (AddHouse(x,y,categorie) == true ){
          count += 1 ;
        }

      } 
        

    }

    private bool AddHouse(int x, int y , int categorie = 0)
    {
      int start_id = 0;
      int column = 0;
      int row = 0;
      int x_step = 0;
      int y_step = 0;
      //set each type of houses's parameters
      switch (categorie){ 
        case 0 : start_id = 147 ; column = 4 ; row= 4 ; y_step = 4 ; x_step = 1 ;  break;
        case 1 : start_id = 231 ; column = 5 ; row= 5 ; y_step = 5 ; x_step = 2 ; break;
        case 2 : start_id = 236 ; column = 4 ; row= 5 ; y_step = 5 ; x_step = 1 ;  break;
        case 3 : start_id = 240 ; column = 7 ; row= 5 ; y_step = 5 ; x_step = 4 ;  break;
        case 4 : start_id = 132 ; column = 5 ; row= 5 ; y_step = 5 ; x_step = 2 ; break;
        case 5 : start_id = 16 ; column = 4 ; row= 5 ; y_step = 5 ; x_step = 1 ; break;}

      try {
          // check if it's on grass
        for (int i = 0; i < row+4 ; i++){
          for (int j = 0; j < column+4 ; j++) {
            if (GetTileId(x-2+j,y-2+i) != 34) {
              return false;
            } 
          }}
          // place les blocs de maison
        for (int i = 0; i < row ; i++){
          for (int j = 0; j < column ; j++) {
            SetTile(x+j,y+i,start_id + j + i*21+1);
          }}

      }
      catch (Exception e){
        return false;
      }

      SetTile(x+x_step,y+y_step, 89 );



      return true; // if it's impossible to put house
    }

    private void AddPath(){

      Random r = new Random();
      int x;
      int y;
      int height = generator.Map.lines.Count ;
      int width = generator.Map.lines[0].tiles.Count;
      int count  = 0 ;


       while (count < 3 )
      {
        try {
          y = r.Next(1,height);
          x = r.Next(1,width);
          for (int i = 0; i < 3 ; i++){
            for (int j = 0; j < 3 ; j++) {
              if (GetTileId(x+j,y+i) != 34) {
                throw new Exception();
              } 
            }}

          for (int i = 0; i < 3 ; i++){
            for (int j = 0; j < 3 ; j++) {
              SetTile(x+j,y+i, 89 );
            }}
          count += 1 ;
        }
        catch (Exception e){
          
        }
      }


    }

    private void SetTile (int x , int y, int id){
      generator.Map.lines[y].tiles[x].id= id; 
    }

    private int GetTileId(int x, int y) {
      return generator.Map.lines[y].tiles[x].id;
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
        case float n when (n < 0.02f): return Layer.Ground0;
        case float n when (n < 0.55f): return Layer.Ground1;
        case float n when (n < 0.8f): return Layer.Ground2;
        default: return Layer.Ground3;
      }
    }

    /**
    * Get the correct Layer Tile for the connection between each layer type
    */
    private int GetCorrectLayerTile(float x, float y, Layer mainLayer, Dictionary<TilePosition, int> tilesID)
    {
      Layer cornerTopLeft = GetLayer(x - 1, y - 1);
      Layer cornerTopRight = GetLayer(x + 1, y - 1);
      Layer cornerBottomLeft = GetLayer(x - 1, y + 1);
      Layer cornerBottomRight = GetLayer(x + 1, y + 1);
      Layer topTile = GetLayer(x, y - 1);
      Layer rightTile = GetLayer(x + 1, y);
      Layer bottomTile = GetLayer(x, y + 1);
      Layer leftTile = GetLayer(x - 1, y);

      if (mainLayer < Layer.Ground0 && topTile > mainLayer && leftTile > mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopLeft];
      else if (mainLayer < Layer.Ground0 && topTile > mainLayer && rightTile > mainLayer && leftTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopRight];
      else if (mainLayer < Layer.Ground0 && bottomTile > mainLayer && leftTile > mainLayer && rightTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomLeft];
      else if (mainLayer < Layer.Ground0 && bottomTile > mainLayer && rightTile > mainLayer && leftTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomRight];
      
      else if (mainLayer > Layer.Ground0 && topTile < mainLayer && leftTile < mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopLeft];
      else if (mainLayer > Layer.Ground0 && topTile < mainLayer && rightTile < mainLayer && leftTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.TopRight];
      else if (mainLayer > Layer.Ground0 && bottomTile < mainLayer && leftTile < mainLayer && rightTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomLeft];
      else if (mainLayer > Layer.Ground0 && bottomTile < mainLayer && rightTile < mainLayer && leftTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.BottomRight];

      else if (mainLayer < Layer.Ground0 && topTile > mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Top];
      else if (mainLayer < Layer.Ground0 && rightTile > mainLayer && leftTile == mainLayer && topTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Right];
      else if (mainLayer < Layer.Ground0 && bottomTile > mainLayer && leftTile == mainLayer && rightTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.Bottom];
      else if (mainLayer < Layer.Ground0 && leftTile > mainLayer && topTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Left];

      else if (mainLayer > Layer.Ground0 && (topTile < mainLayer || cornerTopLeft < mainLayer && cornerTopRight < mainLayer) && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Top];
      else if (mainLayer > Layer.Ground0 && (rightTile < mainLayer || cornerTopRight < mainLayer && cornerBottomRight < mainLayer) && leftTile == mainLayer && topTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Right];
      else if (mainLayer > Layer.Ground0 && (bottomTile < mainLayer || cornerBottomLeft < mainLayer && cornerBottomRight < mainLayer) && leftTile == mainLayer && rightTile == mainLayer && topTile == mainLayer) return tilesID[TilePosition.Bottom];
      else if (mainLayer > Layer.Ground0 && (leftTile < mainLayer || cornerTopLeft < mainLayer && cornerBottomLeft < mainLayer) && topTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.Left];

      else if (mainLayer < Layer.Ground0 && cornerTopLeft > mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerTopLeft];
      else if (mainLayer < Layer.Ground0 && cornerTopRight > mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerTopRight];
      else if (mainLayer < Layer.Ground0 && cornerBottomLeft > mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerBottomLeft];
      else if (mainLayer < Layer.Ground0 && cornerBottomRight > mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerBottomRight];

      else if (mainLayer > Layer.Ground0 && cornerTopLeft < mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerTopLeft];
      else if (mainLayer > Layer.Ground0 && cornerTopRight < mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerTopRight];
      else if (mainLayer > Layer.Ground0 && cornerBottomLeft < mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerBottomLeft];
      else if (mainLayer > Layer.Ground0 && cornerBottomRight < mainLayer && topTile == mainLayer && leftTile == mainLayer && rightTile == mainLayer && bottomTile == mainLayer) return tilesID[TilePosition.CornerBottomRight];
      else return tilesID[TilePosition.Center];
    }
  }
}