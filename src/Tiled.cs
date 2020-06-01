using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace tiled
{
  public class Tile
  {
    public int id { get; private set; }
    public string name { get; private set; }
    public Tile(int id, string name)
    {
      this.id = id;
      this.name = name;
    }
    public override string ToString()
    {
      return id.ToString();
    }
  }

  public class TiledLine
  {
    private readonly List<Tile> line = new List<Tile>();
    public void add(int id)
    {
      line.Add(new Tile(id, ""));
    }
    public override string ToString()
    {
      return String.Join(',', line);
    }
  }

  public class TiledMap
  {
    public int width { get; private set; }
    public int height { get; private set; }
    private string tilesetPath;
    public List<TiledLine> map = new List<TiledLine>();

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
      xMap.Save("./assets/pokemap-test.tmx");
    }

    public override string ToString()
    {
      map.ForEach(line => {
        Console.WriteLine(line.ToString());
      });
      //Console.WriteLine(map.ConvertAll(line => line.ToString()).ToArray());
      return String.Join(",\n", map.ConvertAll(line => line.ToString()).ToArray() );
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
        map.Add(line);
      }
    }
  }
}