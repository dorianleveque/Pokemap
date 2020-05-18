using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace pokemongenerator
{
    public class TileColumn{
        public int Id {get;private set;}
        public readonly List<int> Column = new List<int>();
        public int Height{get;private set;}

        public TileColumn(int id, int height)
        {
            Id = id;
            Height = height;
        }
        public override string ToString()
        {
            return String.Join(',', Column);
        }
    }
    public class TileLayer
    {
        public int Id {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        private XElement Layer;

        public  List<TileColumn> TileMap = new List<TileColumn>();
 
         public TileLayer(XElement layer, int width, int height)
         {
            Layer = layer;
            Width = width;
            Height = height;
            var data = Layer.Element("data");
            var encoding = (string)data.Attribute("encoding");
            if (encoding == "csv")
                LoadCSVLayerData((string)data.Value);
            else
                throw new Exception("encoding not supported: use csv when saving Tiled file");
         }

         private void LoadCSVLayerData(String csvData)
         {
                //map is registered in TMX/csv formats as a list of width * heigth 
                // integers separated by ","
                var tiles = csvData.Split(',');
                for (var x = 0 ; x <Width; ++x)
                {
                    var tc = new TileColumn(x,Height);
                    TileMap.Add(tc);
                }
                for (int y = 0; y < Height; ++y)
                    for(int x = 0 ; x < Width; ++x)
                        TileMap[x].Column.Add(int.Parse(tiles[y*Width + x].Trim()));
                Console.WriteLine($"{tiles.Length} tiles loaded");

         }

         public override string ToString()
         {
            return String.Join('\n',TileMap);
         }

         public void ShuffleColumns(){
            TileMap = TileMap.OrderBy( x=> Guid.NewGuid()).ToList();
            WriteCSVLayerData();
         }
         
         private void WriteCSVLayerData(){
            String s = "";
            for(int y = 0 ; y < Height ; ++y)
            {
                for(int x = 0 ; x < Width ; ++x)
                {
                    s+=TileMap[x].Column[y].ToString();
                    if (y!=Height-1 || x!=Width-1)
                        s+=",";
                }
                if (y!=Height-1)
                    s+="\n";  
            }
            var data = Layer.Element("data");
            data.Value = s;
         }
    }
}