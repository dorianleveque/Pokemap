using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using tiled;
using AStarSharp;
using System.Numerics;

namespace pokemongenerator
{
  public class MapGenerator
  {
    public readonly List<GenerationStep> Steps = new List<GenerationStep>();
    public TiledMap Map;
    public int houseNum;

    public MapGenerator(int width, int height, int houseNum)
    {
      Map = new TiledMap(width, height, "Terrain.tsx");
      this.houseNum = houseNum;
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
      Map.savePicture();
      Map.processPicture();
      Map.convertPicture();
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
    private List<Vector2> doorSteps = new List<Vector2>();
    private List<Vector2> centers = new List<Vector2>();
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
      AddCenters();
      AddHouses(generator.houseNum);
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

      y = r.Next(1,height);
      var i = 0;
      while (count < number )
      {
        if (i > 20){
            y = r.Next(1,height);
            i = 0;

        }
        x = r.Next(1,width);
        categorie = r.Next(1,6);
        if (AddHouse(x,y,categorie) == true ){
          count += 1 ;
          i=0;
        }
        i+=1;
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
          for (int j = 0; j < column ; j++) {
            if (GetTileId(x+j,y-2+i) != 34) {
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
     
      var step = new Vector2();
      step.X=x+x_step;
      step.Y=y+y_step+1;
      doorSteps.Add(step);
      SetTile((int)step.X,(int)step.Y, 89 );
      SetTile((int)step.X,(int)step.Y-1, 89 );


      return true; // if it's impossible to put house
    }


    private void AddCenters(){
      
      float value;
      float value_top;
      float value_bottom;
      float value_right;
      float value_left;
      

      generator.Map.lines.ForEach((line) =>
      {
        float y = (float)generator.Map.lines.IndexOf(line);
        line.tiles.ForEach((tile) =>
        {
          float x = (float)line.tiles.IndexOf(tile);
          
          value = n.GetNoise(x, y);
          value_top = n.GetNoise(x, y-1);
          value_bottom = n.GetNoise(x, y+1);
          value_right = n.GetNoise(x+1, y);
          value_left = n.GetNoise(x-1, y);
          
          if (value > value_bottom && value > value_top && value > value_right && value > value_left)
          {
            for (int i = 0; i < 5 ; i++){
            for (int j = 0; j < 5 ; j++) {
              SetTile((int)x+j,(int)y+i, 89 );
              var center = new Vector2();
              center.X= (int)x+j;
              center.Y= (int)y+i;
              centers.Add(center);
            }}

          }

        });});
    }
    private void AddPath(){
      // Create map for A* lib
      List<List<Node>> temp_map = new List<List<Node>>();
      generator.Map.lines.ForEach((line) =>
      {
        int y = generator.Map.lines.IndexOf(line);
        temp_map.Add(new List<Node>());
        line.tiles.ForEach((tile) =>
        {
          int x = line.tiles.IndexOf(tile);
          //if it's a cliff side :
          if (GetTileId(x,y) == 55 && GetTileId(x-1,y) == 55 &&  GetTileId(x+1,y) == 55 ){
            temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,5));
          }
          else if (GetTileId(x,y) == 33 && GetTileId(x,y-1) == 33 &&  GetTileId(x,y+1) == 33 ){
            temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,5));
          }
          else if (GetTileId(x,y) == 35 && GetTileId(x,y-1) == 35 &&  GetTileId(x,y+1) == 35 ){
            temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,5));
          }

          //if it's grass or path :
          else if (GetTileId(x,y) == 89 || GetTileId(x,y) == 34  ){
              if ((GetTileId(x-1,y) == 89 || GetTileId(x-1,y) == 34) && (GetTileId(x+1,y) == 89 || GetTileId(x+1,y) == 34)
               || (GetTileId(x,y-1) == 89 || GetTileId(x,y-1) == 34) && (GetTileId(x,y+1) == 89 || GetTileId(x,y+1) == 34)  ){
                  temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,3));
               }  

               else {
                 temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,20));
               }
          }
          //if it's sand :
          else if (GetTileId(x,y) == 72 || GetTileId(x,y) == 73 || GetTileId(x,y) == 74 || GetTileId(x,y) == 93 || GetTileId(x,y) == 94
          || GetTileId(x,y) == 95 || GetTileId(x,y) == 114 || GetTileId(x,y) == 114 || GetTileId(x,y) == 115 ){
                 temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),true,4));
            

          }
          else {
              temp_map[y].Add(new Node(new System.Numerics.Vector2(x,y),false));
          }
        });});
        foreach (var coord in doorSteps){
          temp_map[(int)coord.Y+1][(int)coord.X] = new Node(new System.Numerics.Vector2((int)coord.Y+1,(int)coord.X),true, 1) ;
        }
   
      Astar astar = new Astar(temp_map);


        foreach (var coord in doorSteps){
                var cur_x = coord.X;
                var cur_y = coord.Y;
                var moy_x = 0;
                var moy_y = 0;
                var dist = 10000000.0;
                // find closest center
              foreach (var coord_moy in centers){
                if (((cur_x-coord_moy.X)*(cur_x-coord_moy.X)+(cur_y-coord_moy.Y)*(cur_y-coord_moy.Y)) < dist )
                {
                  moy_x = (int)coord_moy.X;
                  moy_y = (int)coord_moy.Y;
                  dist = (cur_x-coord_moy.X)*(cur_x-coord_moy.X)+(cur_y-coord_moy.Y)*(cur_y-coord_moy.Y);
                }
              }
              // find paths between targer 
              var path = astar.FindPath(new Vector2(cur_x,cur_y), new Vector2(moy_x,moy_y));
              try {
                foreach (Node node in path)
                {
                  var temp_x = (int)node.Position.X;
                  var temp_y = (int)node.Position.Y;
                  temp_map[temp_y][temp_x] = new Node(new System.Numerics.Vector2(temp_x,temp_y),true, 1) ;

                  if (GetTileId(temp_x,temp_y) == 33){
                    SetTile(temp_x,temp_y,163);
                    SetTile(temp_x,temp_y-1,143);
                    SetTile(temp_x,temp_y+1,185);
                  }
                  else if (GetTileId(temp_x,temp_y) == 35){
                    SetTile(temp_x,temp_y,164);
                    SetTile(temp_x,temp_y-1,144);
                    SetTile(temp_x,temp_y+1,186);
                    
                  }
                  else if (GetTileId(temp_x,temp_y) == 55){
                    SetTile(temp_x,temp_y,125);
                    SetTile(temp_x-1,temp_y,124);
                    SetTile(temp_x+1,temp_y,126);
                    
                  }
                  else {
                    SetTile(temp_x,temp_y,89);
                  }
                  
                }
                astar = new Astar(temp_map);
              }
              catch (Exception e){
                Console.Write("No path availible for one the house ");
              }

            }

      generator.Map.lines.ForEach((line) =>
      {
        int y = generator.Map.lines.IndexOf(line);
        line.tiles.ForEach((tile) =>
        {
          int x = line.tiles.IndexOf(tile);
          if (GetTileId(x, y) == 89){
            if (GetTileId(x+1, y) == 34){
                SetTile(x+1, y,90);
            }
            if (GetTileId(x-1, y) == 34){
                SetTile(x-1, y,88);  
            }
            if (GetTileId(x, y+1) == 34){
                SetTile(x, y+1,110); 
            }
            if (GetTileId(x, y-1) == 34){
                SetTile(x, y-1,68); 
            }
          }
        });});




    }



    private void SetTile (int x , int y, int id){
      generator.Map.lines[y].tiles[x].id= id; 
    }

    private int GetTileId(int x, int y) {
      try {
        return generator.Map.lines[y].tiles[x].id;
      }
      catch (Exception e){
        return 0;
      }
      
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
        //case float n when (n < 0.8f): return Layer.Ground2;
        default: return Layer.Ground1;
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