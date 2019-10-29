using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib;
using TiledLib.Layer;

namespace Proftaak_Orientatie_Game.World
{
    class TileMap : Level
    {
        private TileLayer background_tiles;
        private TileLayer play_tiles;
        private TileLayer roof_tiles;

        private Sprite[,] sprites;

        private Map map;

        private Texture tileset;

        public TileMap(string filename)
        {
            LoadTilemap(filename, out background_tiles, out play_tiles, out roof_tiles, out map);

            /*
            for (int y = 0, i = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++, i++)
                {
                    var gid = background_tiles.Data[i];
                    if (gid == 0)
                        continue;

                    var tileset = map.Tilesets.Single(ts => gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                    Console.WriteLine(tileset.ImagePath.Replace("..", "res"));
                    var tile = tileset[gid];

                    //window.Draw(tile_sprite);
                }
            }
            */

            /*
            tilesets = new Dictionary<String, Texture>();//[map.Tilesets.Length];

            int i = 0;
            foreach (ITileset ts in map.Tilesets.OfType<ITileset>().ToArray())
            {
                String path = ts.ImagePath.Replace("..", "res");
                Console.WriteLine(path);
                //tilesets[i] = new Texture(path);
                tilesets.Add(ts.Name, new Texture(path));
                Console.WriteLine("Tileset loaded");
                i++;
            }
            */

            ITileset ts = map.Tilesets.First<ITileset>();
            String path = ts.ImagePath.Replace("..", "res");
            tileset = new Texture(path);

            sprites = new Sprite[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    sprites[x,y] = new Sprite(tileset);
                    sprites[x, y].Position = new Vector2f(x * map.CellWidth, y * map.CellHeight);
                    sprites[x, y].TextureRect = new IntRect(0,0, ts.TileWidth, ts.TileHeight);
                }
            }
        }

        static void LoadTilemap(string filename, out TileLayer background_tiles, out TileLayer play_tiles, out TileLayer roof_tiles, out Map map)
        {
            using (var stream = File.OpenRead(filename))
            {
                map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), ts.Source)));

                //Quick hack for now. Perhaps we want to change this in the future so we can have more layers that are picked based on a tag or something
                TileLayer[] layers = map.Layers.OfType<TileLayer>().ToArray();
                background_tiles = layers[0];
                play_tiles = layers[1];
                roof_tiles = layers[2];
            }
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            for (int y = 0, i = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++, i++)
                {
                    var gid = background_tiles.Data[i];
                    if (gid == 0)
                        continue;

                    var tileset = map.Tilesets.Single(ts => gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
                    //Console.WriteLine(tileset.Name);
                    Tile tile = tileset[gid];

                    sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                    window.Draw(sprites[x,y]);
                }
            }
        }
    }
}
