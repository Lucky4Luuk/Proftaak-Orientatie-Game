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

        public TileMap(string filename)
        {
            LoadTilemap(filename, out background_tiles, out play_tiles, out roof_tiles);
        }

        static void LoadTilemap(string filename, out TileLayer background_tiles, out TileLayer play_tiles, out TileLayer roof_tiles)
        {
            using (var stream = File.OpenRead(filename))
            {
                var map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), ts.Source)));

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
