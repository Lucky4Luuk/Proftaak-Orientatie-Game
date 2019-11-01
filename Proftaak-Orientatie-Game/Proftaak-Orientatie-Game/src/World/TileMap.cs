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
using TiledSharp;

namespace Proftaak_Orientatie_Game.World
{
    class TileMap : Level
    {
        private TmxLayer background_tiles;
        private TmxLayer behind_tiles;
        private TmxLayer play_tiles;
        private TmxLayer roof_tiles;

        private Sprite[,] sprites;

        private TmxMap map;

        private Texture tilesetTexture;
        private Tileset tileset;

        private RenderTexture canvas;
        private Sprite canvasSprite;

        public TileMap(string filename)
        {
            LoadTilemap(filename, out background_tiles, out behind_tiles, out play_tiles, out roof_tiles, out map, out tileset);

            //ITileset ts = map.Tilesets.First<ITileset>();
            TmxTileset ts = map.Tilesets[0];
            String path = ts.Image.Source;//.Replace("..", "res").Replace("res/maps", "");
            //String path = ts.ImagePath.Replace("..", "res");
            //String path = map.Tilesets[0].Image.Source.Replace("..", "res");
            Console.WriteLine("Path: {0}", path);
            tilesetTexture = new Texture(path);

            canvas = new RenderTexture((uint)(map.Width * 16), (uint)(map.Height * 16));

            sprites = new Sprite[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    sprites[x,y] = new Sprite(tilesetTexture);
                    sprites[x, y].Position = new Vector2f(x * map.TileWidth, y * map.TileHeight);
                    sprites[x, y].TextureRect = new IntRect(0,0, ts.TileWidth, ts.TileHeight);
                }
            }

            RenderToCanvas();

            canvasSprite = new Sprite(canvas.Texture);
        }

        static void LoadTilemap(string filename, out TmxLayer background_tiles, out TmxLayer behind_tiles, out TmxLayer play_tiles, out TmxLayer roof_tiles, out TmxMap map, out Tileset tileset)
        {
            /*
            using (var stream = File.OpenRead(filename))
            {
                map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), ts.Source)));

                //Quick hack for now. Perhaps we want to change this in the future so we can have more layers that are picked based on a tag or something
                TileLayer[] layers = map.Layers.OfType<TileLayer>().ToArray();
                background_tiles = layers[0];
                play_tiles = layers[1];
                roof_tiles = layers[2];
            }
            */
            map = new TmxMap("res/maps/game map.tmx");

            background_tiles = map.Layers[0];
            behind_tiles = map.Layers[1];
            play_tiles = map.Layers[2];
            roof_tiles = map.Layers[3];

            using (var stream = File.OpenRead("res/tilesets/tileseta.tsx"))
                tileset = Tileset.FromStream(stream);
        }

        private void RenderToCanvas()
        {
            for (int y = 0, i = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++, i++)
                {
                    TmxLayerTile tsTile = background_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        Tile tile = tileset[tsTile.Gid - 1];

                        float rotation = 0f;
                        if (tile.Orientation == TileOrientation.Rotate90CW)
                            rotation = 90f;
                        if (tile.Orientation == TileOrientation.Rotate180CCW)
                            rotation = 180f;
                        if (tile.Orientation == TileOrientation.Rotate270CCW)
                            rotation = -90f;

                        Vector2f scale = new Vector2f(1f, 1f);
                        if (tile.Orientation == TileOrientation.FlippedH)
                            scale.X = -1f;
                        if (tile.Orientation == TileOrientation.FlippedV)
                            scale.Y = -1f;

                        sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                        sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                        sprites[x, y].Rotation = rotation;
                        sprites[x, y].Scale = scale;
                        canvas.Draw(sprites[x, y]);
                    }


                    tsTile = behind_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        Tile tile = tileset[tsTile.Gid - 1];

                        float rotation = 0f;
                        if (tile.Orientation == TileOrientation.Rotate90CW)
                            rotation = 90f;
                        if (tile.Orientation == TileOrientation.Rotate180CCW)
                            rotation = 180f;
                        if (tile.Orientation == TileOrientation.Rotate270CCW)
                            rotation = -90f;

                        Vector2f scale = new Vector2f(1f, 1f);
                        if (tile.Orientation == TileOrientation.FlippedH)
                            scale.X = -1f;
                        if (tile.Orientation == TileOrientation.FlippedV)
                            scale.Y = -1f;

                        sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                        sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                        sprites[x, y].Rotation = rotation;
                        //sprites[x, y].Scale = scale;
                        canvas.Draw(sprites[x, y]);
                    }


                    tsTile = play_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        Tile tile = tileset[tsTile.Gid - 1];

                        float rotation = 0f;
                        if (tile.Orientation == TileOrientation.Rotate90CW)
                            rotation = 90f;
                        if (tile.Orientation == TileOrientation.Rotate180CCW)
                            rotation = 180f;
                        if (tile.Orientation == TileOrientation.Rotate270CCW)
                            rotation = -90f;

                        Vector2f scale = new Vector2f(1f, 1f);
                        if (tile.Orientation == TileOrientation.FlippedH)
                            scale.X = -1f;
                        if (tile.Orientation == TileOrientation.FlippedV)
                            scale.Y = -1f;

                        sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                        sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                        sprites[x, y].Rotation = rotation;
                        sprites[x, y].Scale = scale;
                        canvas.Draw(sprites[x, y]);
                    }


                    tsTile = roof_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        Tile tile = tileset[tsTile.Gid - 1];

                        float rotation = 0f;
                        if (tile.Orientation == TileOrientation.Rotate90CW)
                            rotation = 90f;
                        if (tile.Orientation == TileOrientation.Rotate180CCW)
                            rotation = 180f;
                        if (tile.Orientation == TileOrientation.Rotate270CCW)
                            rotation = -90f;

                        Vector2f scale = new Vector2f(1f, 1f);
                        if (tile.Orientation == TileOrientation.FlippedH)
                            scale.X = -1f;
                        if (tile.Orientation == TileOrientation.FlippedV)
                            scale.Y = -1f;

                        sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                        sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                        sprites[x, y].Rotation = rotation;
                        sprites[x, y].Scale = scale;
                        canvas.Draw(sprites[x, y]);
                    }
                }
            }

            canvas.Display();
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(canvasSprite);
        }
    }
}
