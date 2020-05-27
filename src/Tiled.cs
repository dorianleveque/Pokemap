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
  }

  public class TiledColumn
  {
    public readonly List<Tile> column = new List<Tile>();
    public override string ToString()
    {
      return String.Join(',', column);
    }
  }
  public class TiledMap
  {
    public int width { get; private set; }
    public int height { get; private set; }
    private string tilesetPath;
    private List<TiledColumn> columns = new List<TiledColumn>();

    public TiledMap(int width, int height, string tilesetPath)
    {
      this.width = width;
      this.height = height;
      this.tilesetPath = tilesetPath;
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
              "data"
            )
          )
        )
      );
      xMap.Save("./assets/test.tmx");
    }
  }
}