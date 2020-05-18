using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace pokemongenerator
{
    class AsciiSorter
    {      
          static void Main(string[] args)
        {
            Map map = new Map (30,30);
            Random rnd = new Random();

            //Houses
            for (int i = 0 ; i < 3 ; i ++)
            {
                int x_start  = rnd.Next(1,27);
                int y_start  = rnd.Next(1,27);
                map.set_object(x_start+1,y_start+2,"#");
                
                for (int x = x_start ; x < x_start+3 ; x ++)
                {
                    for (int y = y_start ; y < y_start+2 ; y ++)
                     {
                    map.set_object (x,y, "█");
                    }
                }
            }

            for ( int i = 0 ; i < 100 ; i ++ )  //parcours la map 100
            {

            
                for (int x = 2 ; x < 28 ; x ++) //parcours chaque item de la map pour créer des chemins
                {
                    for (int y = 2 ; y < 28 ; y ++)
                    {
                        if (map.get_object(x,y)=="#" & map.number_of_neighbor(x,y,"#")< 2)
                        {
                            map.random_set_empty_neighbor(x,y,"#");
                        }
                    }

                }
            }
            map.display();
        }

    }

    class Map
    {
        public  List<List<String>> Map_objects = new List<List<String>>();
        public int height;
        public int width;
        public Map (int h , int w) {
            height = h;
            width = w ;
            for (int y = 0 ; y< h ; y++)
            {
                Map_objects.Add(new List<string>(w));
                for (int x = 0 ; x< w ; x++)
                {
                    Map_objects[y].Add(" ");
                }
            }
        }

        public String get_object (int x , int y){
            
            return Map_objects[y][x];
        }

        public void set_object (int x , int y , String value){
            
            Map_objects[y][x] = value;
        }

        public int number_of_neighbor( int x, int y , String neighbor )
        {
            int count = 0 ;
            if (get_object(x+1 , y) == neighbor) {
                count += 1 ;
            }
            if (get_object(x-1 , y) == neighbor) {
                count += 1 ;
            }
            if (get_object(x , y+1) == neighbor) {
                count += 1 ;
            }
            if (get_object(x , y-1) == neighbor) {
                count += 1 ;
            }        

            
            return count;
        }

        public void random_set_empty_neighbor (int x , int y, String value)
        {
            Random rnd = new Random();
            int count = 0 ;
            while (true){
                count += 1;
                int rand  = rnd.Next(1,5);
                
                int x_mod = -1 ;
                int y_mod = 0 ;
                
                if (rand==1) {
                    x_mod = 1 ;
                    y_mod = 0 ;
                }
                else if (rand==2) {
                    x_mod = 0 ;
                    y_mod = 1 ;
                }
                else if (rand==3) {
                    x_mod = -1 ;
                    y_mod = 0 ;
                }
                else if (rand==4) {
                    x_mod = 0 ;
                    y_mod = -1 ;
                }


                if (get_object(x+x_mod , y+y_mod) == " "  & number_of_neighbor(x+x_mod,y+y_mod,"#")< 2)
                {
                    set_object(x+x_mod , y+y_mod , value) ;
                    break ;
                }
                if (count > 100)
                {
                    break ;
                }
            }
            

        }

        public void display (){

            foreach (List<String> list_y in Map_objects)
            {
                String line = "";
                foreach (String value in list_y)
                {
                    line = line + value ;
                }
                Console.WriteLine(line);
            }
        }
    }
}

