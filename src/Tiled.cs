using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

namespace tiled
{
  public class Tile
  {
    public int id;
    public Tile(int id)
    {
      this.id = id;
    }
    public override string ToString()
    {
      return id.ToString();
    }
  }

  public class TiledLine
  {
    public List<Tile> tiles = new List<Tile>();
    public void add(int id)
    {
      tiles.Add(new Tile(id));
    }
    public override string ToString()
    {
      return String.Join(',', tiles);
    }
  }

  public class TiledMap
  {
    public int width { get; private set; }
    public int height { get; private set; }
    private string tilesetPath;
    public List<TiledLine> lines = new List<TiledLine>();

    public TiledMap(int width, int height, string tilesetPath)
    {
      this.width = width;
      this.height = height;
      this.tilesetPath = tilesetPath;
      initMap();
    }

    public void save()
    {
      XDocument xMap = new XDocument(
        new XElement("map",
          new XAttribute("version", "1.2"),
          new XAttribute("tiledversion", "1.3.4"),
          new XAttribute("orientation", "orthogonal"),
          new XAttribute("renderorder", "left-down"),
          new XAttribute("width", this.width),
          new XAttribute("height", this.height),
          new XAttribute("tilewidth", 16),
          new XAttribute("tileheight", 16),
          new XAttribute("infinite", 0),
          new XAttribute("backgroundcolor", "#00000000"),
          new XAttribute("nextlayerid", 5),
          new XAttribute("nextobjectid", 1),
          new XElement("tileset",
            new XAttribute("firstgid", 1),
            new XAttribute("source", this.tilesetPath)
          ),
          new XElement("layer",
            new XAttribute("name", "ground"),
            new XAttribute("width", this.width),
            new XAttribute("height", this.height),
            new XElement("data",
              new XAttribute("encoding", "csv"),
              this.ToString()
            )
          )
        )
      );
      xMap.Save("./assets/pokemap.tmx");
    }

    public void savePicture()
    {
    Console.WriteLine("Saving procedurally generated level as Bitmap");

    List<byte> bytes = new List<byte>(); // this list should be filled with values
    int bpp = 24;

    Bitmap bmp = new Bitmap(width, height);

      lines.ForEach(line => {
        line.tiles.ForEach(tile => {
          int y = lines.IndexOf(line);
          int x = line.tiles.IndexOf(tile);
          int i = ((y * width) + x) * (bpp / 8);
          // first byte will be red, because you are writing it as first value
          Int32 r = tile.id <= 255 ? (Int32)tile.id : 0;
          Int32 g = tile.id > 255 && tile.id <= 510 ? (Int32)tile.id-255 : 0;
          Int32 b = tile.id > 510 && tile.id < 765 ? (Int32)tile.id-510 : 0;

          Color color = Color.FromArgb(r, g, b);
          bmp.SetPixel(x, y, color); 
        });
      });
      bmp.Save("./assets/wfc.png");
    }
  public void processPicture(){
    Console.WriteLine("Launching WFC on Bitmap file");

    Process.Start("cmd.exe",  @"/C .\bin\DeBroglie.Console.exe .\assets\pokemap.json").WaitForExit();
  }

  public void convertPicture(){
    Console.WriteLine("Converting generated bitmap to tmx");
    Bitmap img = new Bitmap("assets/wfc-generated.png");

    lines = new List<TiledLine>() ;
    // reset lines to 0
        for (int h = 0; h < 200; h++)
      {
        TiledLine line = new TiledLine();
        for (int w = 0; w < 200; w++)
        {
          line.add(0);
        }
        lines.Add(line);
      }


    for (int i = 0; i < img.Width; i++)
    {
        for (int j = 0; j < img.Height; j++)
        {
            Color pixel = img.GetPixel(i,j);
            var r = (Int32)pixel.R;
            var g = (Int32)pixel.G;
            var b = (Int32)pixel.B;


            if (r==0)
            {
                 if (g ==0){
                   lines[j].tiles[i].id = b + 510; 
                 }
                 else {
                   lines[j].tiles[i].id = g + 255; 
                 }
            }
            else {
              lines[j].tiles[i].id = r; 
            }
        }
    } 


    XDocument xMap = new XDocument(
        new XElement("map",
          new XAttribute("version", "1.2"),
          new XAttribute("tiledversion", "1.3.4"),
          new XAttribute("orientation", "orthogonal"),
          new XAttribute("renderorder", "left-down"),
          new XAttribute("width", 200),
          new XAttribute("height", 200),
          new XAttribute("tilewidth", 16),
          new XAttribute("tileheight", 16),
          new XAttribute("infinite", 0),
          new XAttribute("backgroundcolor", "#00000000"),
          new XAttribute("nextlayerid", 5),
          new XAttribute("nextobjectid", 1),
          new XElement("tileset",
            new XAttribute("firstgid", 1),
            new XAttribute("source", this.tilesetPath)
          ),
          new XElement("layer",
            new XAttribute("name", "ground"),
            new XAttribute("width", 200),
            new XAttribute("height", 200),
            new XElement("data",
              new XAttribute("encoding", "csv"),
              this.ToString()
            )
          )
        )
      );
      xMap.Save("./assets/pokemap-generated.tmx");

  }


    public override string ToString()
    {
      return String.Join(",\n", lines.ConvertAll(line => line.ToString()).ToArray() );
    }

  

    private void initMap()
    {
      for (int h = 0; h < height; h++)
      {
        TiledLine line = new TiledLine();
        for (int w = 0; w < width; w++)
        {
          line.add(0);
        }
        lines.Add(line);
      }
    }
  }
}