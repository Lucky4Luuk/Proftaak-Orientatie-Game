﻿using SFML.System;
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
        private TmxLayer floor_tiles;
        private TmxLayer background_tiles;
        private TmxLayer behind_tiles;
        private TmxLayer play_tiles;
        private TmxLayer roof_tiles;

        private Sprite[,] sprites;

        private TmxMap map;

        private Texture tilesetTexture;
        private Tileset tileset;

        private RenderTexture canvas;
        private RenderTexture objectCanvas;
        private RenderTexture roofCanvas;
        private Sprite canvasSprite;
        private Sprite roofSprite;

        private Shader shadowShader;

        public TileMap(string filename)
        {
            LoadTilemap(filename, out floor_tiles, out background_tiles, out behind_tiles, out play_tiles, out roof_tiles, out map, out tileset);

            //ITileset ts = map.Tilesets.First<ITileset>();
            TmxTileset ts = map.Tilesets[0];
            String path = ts.Image.Source;//.Replace("..", "res").Replace("res/maps", "");
            //String path = ts.ImagePath.Replace("..", "res");
            //String path = map.Tilesets[0].Image.Source.Replace("..", "res");
            //Console.WriteLine("Path: {0}", path);
            tilesetTexture = new Texture(path);

            canvas = new RenderTexture((uint)(map.Width * 16), (uint)(map.Height * 16));
            objectCanvas = new RenderTexture((uint)(map.Width * 16), (uint)(map.Height * 16));
            roofCanvas = new RenderTexture((uint)(map.Width * 16), (uint)(map.Height * 16));

            StreamReader reader = new StreamReader("res/shadow_shader.glsl");
            String src = reader.ReadToEnd();
            //Console.WriteLine("=== SRC ===");
            //Console.WriteLine(src);
            shadowShader = Shader.FromString(null, null, src);
            reader.Close();

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
            roofSprite = new Sprite(roofCanvas.Texture);
        }

        static void LoadTilemap(string filename, out TmxLayer floor_tiles, out TmxLayer background_tiles, out TmxLayer behind_tiles, out TmxLayer play_tiles, out TmxLayer roof_tiles, out TmxMap map, out Tileset tileset)
        {
            map = new TmxMap("res/maps/game map.tmx");

            floor_tiles = map.Layers[0];
            background_tiles = map.Layers[1];
            behind_tiles = map.Layers[2];
            play_tiles = map.Layers[3];
            roof_tiles = map.Layers[4];

            using (var stream = File.OpenRead("res/tilesets/tileseta.tsx"))
                tileset = Tileset.FromStream(stream);
            //Console.WriteLine($"{tileset.Name}");
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
                        try
                        {
                            Tile tile = tileset[tsTile.Gid - 1];

                            /*
                            float rotation = 0f;
                            if (tile.Orientation == TileOrientation.Rotate90CW)
                                rotation = 90f;
                            if (tile.Orientation == TileOrientation.Rotate180CCW)
                                rotation = 180f;
                            if (tile.Orientation == TileOrientation.Rotate270CCW)
                                rotation = -90f;
                            */

                            Vector2f scale = new Vector2f(1f, 1f);
                            float rotation = 0f;
                            if (tsTile.HorizontalFlip)
                                scale.X *= -1f;
                            if (tsTile.VerticalFlip)
                                scale.Y *= -1f;
                            if (tsTile.DiagonalFlip)
                                rotation = -90f;

                            sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                            sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                            sprites[x, y].Rotation = rotation;
                            sprites[x, y].Scale = scale;
                            canvas.Draw(sprites[x, y]);
                        } catch (Exception)
                        {
                            //Console.WriteLine($"Position: [{x}; {y}] - {tsTile.Gid - 1}");
                        }
                    }


                    tsTile = behind_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        try
                        {
                            Tile tile = tileset[tsTile.Gid - 1];

                            /*
                            float rotation = 0f;
                            if (tile.Orientation == TileOrientation.Rotate90CW)
                                rotation = 90f;
                            if (tile.Orientation == TileOrientation.Rotate180CCW)
                                rotation = 180f;
                            if (tile.Orientation == TileOrientation.Rotate270CCW)
                                rotation = -90f;
                            */

                            Vector2f scale = new Vector2f(1f, 1f);
                            float rotation = 0f;
                            if (tsTile.HorizontalFlip)
                                scale.X *= -1f;
                            if (tsTile.VerticalFlip)
                                scale.Y *= -1f;
                            if (tsTile.DiagonalFlip)
                                rotation = -90f;

                            sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                            sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                            sprites[x, y].Rotation = rotation;
                            sprites[x, y].Scale = scale;
                            canvas.Draw(sprites[x, y]);
                        } catch (Exception)
                        {
                            //Console.WriteLine($"Position: [{x}; {y}] - {tsTile.Gid - 1}");
                        }
                    }


                    tsTile = play_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        try
                        {
                            Tile tile = tileset[tsTile.Gid - 1];

                            /*
                            float rotation = 0f;
                            if (tile.Orientation == TileOrientation.Rotate90CW)
                                rotation = 90f;
                            if (tile.Orientation == TileOrientation.Rotate180CCW)
                                rotation = 180f;
                            if (tile.Orientation == TileOrientation.Rotate270CCW)
                                rotation = -90f;
                            */

                            Vector2f scale = new Vector2f(1f, 1f);
                            float rotation = 0f;
                            if (tsTile.HorizontalFlip)
                                scale.X *= -1f;
                            if (tsTile.VerticalFlip)
                                scale.Y *= -1f;
                            if (tsTile.DiagonalFlip)
                                rotation = -90f;

                            sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                            sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                            sprites[x, y].Rotation = rotation;
                            sprites[x, y].Scale = scale;
                            canvas.Draw(sprites[x, y]);
                            objectCanvas.Draw(sprites[x, y]);
                        } catch (Exception)
                        {
                            //Console.WriteLine($"Position: [{x}; {y}] - {tsTile.Gid - 1}");
                        }
                    }


                    tsTile = roof_tiles.Tiles[i];
                    if (tsTile.Gid > 1)
                    {
                        try
                        {
                            Tile tile = tileset[tsTile.Gid - 1];

                            /*
                            float rotation = 0f;
                            if (tile.Orientation == TileOrientation.Rotate90CW)
                                rotation = 90f;
                            if (tile.Orientation == TileOrientation.Rotate180CCW)
                                rotation = 180f;
                            if (tile.Orientation == TileOrientation.Rotate270CCW)
                                rotation = -90f;
                            */

                            Vector2f scale = new Vector2f(1f, 1f);
                            float rotation = 0f;
                            if (tsTile.HorizontalFlip)
                                scale.X *= -1f;
                            if (tsTile.VerticalFlip)
                                scale.Y *= -1f;
                            if (tsTile.DiagonalFlip) //🛹
                                rotation = -90f;

                            sprites[x, y].TextureRect = new IntRect(tile.Left, tile.Top, tileset.TileWidth, tileset.TileHeight);
                            sprites[x, y].Origin = new Vector2f(tileset.TileWidth / 2f, tileset.TileHeight / 2f);
                            sprites[x, y].Rotation = rotation;
                            sprites[x, y].Scale = scale;
                            roofCanvas.Draw(sprites[x, y]);
                        } catch (Exception)
                        {
                            //Console.WriteLine($"Position: [{x}; {y}] - {tsTile.Gid - 1}");
                        }
                    }
                }
            }

            canvas.Display();
            objectCanvas.Display();
            roofCanvas.Display();
        }

        public override void OnDraw(float deltatime, RenderWindow window)
        {
            window.Draw(canvasSprite);
        }

        public void DrawRoof(float deltatime, RenderWindow window, bool indoors)
        {
            if (indoors)
                Shader.Bind(shadowShader);
            window.Draw(roofSprite);
            Shader.Bind(null);
        }

        public int GetTile(Vector2f posF)
        {
            Vector2i pos = new Vector2i((int)(posF.X + (float)map.TileWidth / 2f), (int)(posF.Y + (float)map.TileHeight / 2f));
            if (pos.X < 0 || pos.Y < 0 || pos.X / map.TileWidth >= map.Width || pos.Y / map.TileHeight >= map.Height)
                return 0;
            int i = (pos.X / map.TileWidth) + (pos.Y / map.TileHeight) * map.Height;
            ////Console.WriteLine($"Tile Gid: {play_tiles.Tiles[i].Gid}");
            return play_tiles.Tiles[i].Gid;
        }

        public Vector2i GetTileSize()
        {
            return new Vector2i(map.TileWidth, map.TileHeight);
        }

        public int GetRoofTile(Vector2f posF)
        {
            Vector2i pos = new Vector2i((int)(posF.X + (float)map.TileWidth / 2f), (int)(posF.Y + (float)map.TileHeight / 2f));
            if (pos.X < 0 || pos.Y < 0 || pos.X / map.TileWidth >= map.Width || pos.Y / map.TileHeight >= map.Height)
                return 0;
            int i = (pos.X / map.TileWidth) + (pos.Y / map.TileHeight) * map.Height;
            ////Console.WriteLine($"Tile Gid: {roof_tiles.Tiles[i].Gid}");
            return roof_tiles.Tiles[i].Gid;
        }
    }
}
