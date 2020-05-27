﻿using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using tiled;

namespace pokemongenerator
{
  class PokemonLevelRandomizer
  {
    public int width { get; private set; }
    public int height { get; private set; }

    public PokemonLevelRandomizer(int width, int height)
    {
      this.width = width;
      this.height = height;
    }

    static void Main(string[] args)
    {
      /*Console.WriteLine("fichier source: " + args[0]);
      Console.WriteLine("fichier cible: " + args[1]);*/
      var plr = new PokemonLevelRandomizer(int.Parse(args[0]), int.Parse(args[1]));
      try
      {
        plr.generate();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
        Environment.Exit(1);
      }
    }

    private void generate()
    {
      // TODO
      TiledMap map = new TiledMap(this.width, this.height, "Terrain.tsx");
      map.save();
    }

    private void ShuffleLayers(String sourceFile, String targetFile)
    {
      var xDocument = XDocument.Load(sourceFile);
      var xMap = xDocument.Element("map");
      width = (int)xMap.Attribute("width");
      height = (int)xMap.Attribute("height");
      foreach (var e in xMap.Elements().Where(x => x.Name == "layer"))
      {
        var tl = new TileLayer(e, width, height);
        tl.ShuffleColumns();
      }
      xDocument.Save(targetFile);
    }
  }
}
