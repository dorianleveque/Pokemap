using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace mariogenerator
{
    class MarioLevelRandomizer
    {
        public int Width {get; private set;}
        public int Height {get; private set;}

        static void Main(string[] args)
        {
            Console.WriteLine("fichier source: " + args[0]);
            Console.WriteLine("fichier cible: " + args[1]);
            var mlr = new MarioLevelRandomizer();
            try
            {
                mlr.ShuffleLayers(args[0], args[1]);
            }
            catch(Exception e)
            {
               Console.WriteLine(e.ToString());
               Environment.Exit(1);
           }
        }
        private void ShuffleLayers(String sourceFile, String targetFile)
        {
            var xDocument = XDocument.Load(sourceFile);
            var xMap = xDocument.Element("map");
            Width = (int) xMap.Attribute("width");
            Height = (int) xMap.Attribute("height");
            foreach (var e in xMap.Elements().Where(x => x.Name == "layer"))
            {
                var tl = new TileLayer(e, Width, Height);
                tl.ShuffleColumns();
            }
            xDocument.Save(targetFile);
        }
    }
}
