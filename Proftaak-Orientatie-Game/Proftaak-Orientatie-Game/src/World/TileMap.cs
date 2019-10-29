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

        private Map map;

        public TileMap(string filename)
        {
            LoadTilemap(filename, out background_tiles, out play_tiles, out roof_tiles, out map);

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
            
        }
    }
}
