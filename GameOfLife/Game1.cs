using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameOfLife
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D canvas;
        Rectangle size = new Rectangle(0, 0, 200, 120);
        Rectangle renderSize;
        bool[,] pixels;
        bool[,] pixelsNew;
        Color[] pixelArray;

        Random rnd = new Random();

        int fps = 5;

        Texture2D testtexture;

        KeyboardState kBuffer;

        bool started = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            renderSize = GraphicsDevice.PresentationParameters.Bounds;
            canvas = new Texture2D(GraphicsDevice, size.Width, size.Height, false, SurfaceFormat.Color);
            pixels = pixelsNew = new bool[size.Width, size.Height];
            this.IsMouseVisible = true;

            base.Initialize();
        }


        private void randomise()
        {
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    if (rnd.Next(0, 100) < 20)
                    {
                        pixels[x, y] = true;
                    }
                    else
                    {
                        pixels[x, y] = false;
                    }
                }
            }
        }


        private void clearAll()
        {
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    pixels[x, y] = false;
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            testtexture = Content.Load<Texture2D> ("Untitled");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();
            KeyboardState k = Keyboard.GetState();

            if (m.LeftButton == ButtonState.Pressed && 0 < m.X && m.X < renderSize.Width && m.Y > 0 && m.Y < renderSize.Height)
            {
                double xScale = (double)size.Width / renderSize.Width;
                double yScale = (double)size.Height / renderSize.Height;
                pixels[(int)(m.X * xScale), (int)(m.Y * yScale)] = true;
            }

            if (k.IsKeyDown(Keys.Enter) && kBuffer.IsKeyUp(Keys.Enter))
            {
                started = !started;
                if (started)
                {
                    updateFPS(fps);
                }
                else
                {
                    this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100f);
                }
            }

            if (k.IsKeyDown(Keys.Up))
            {
                updateFPS(fps + 1);
            }
            if(k.IsKeyDown(Keys.Down) && fps > 1)
            {
                updateFPS(fps - 1);
            }

            if (k.IsKeyDown(Keys.R))
            {
                randomise();
            }

            if(k.IsKeyDown(Keys.C)){
                clearAll();
            }

            kBuffer = k;


            if (started)
            {
                life();
            }

            base.Update(gameTime);
        }

        


        private void life()
        {
            pixelsNew = new bool[size.Width, size.Height];
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    int neighbourCount = getLiveNeighbourCount(x, y);

                    if (pixels[x, y])
                    {
                        //if cell alive

                        if (neighbourCount < 2 || neighbourCount > 3)
                        {
                            //dies
                            pixelsNew[x, y] = false;
                        }
                        else
                        {
                            //lives
                            pixelsNew[x, y] = true;
                        }
                    }
                    else
                    {
                        //if cell dead

                        if (neighbourCount == 3)
                        {
                            //comes to life
                            
                            pixelsNew[x, y] = true;
                        }
                        else
                        {
                            pixelsNew[x, y] = false;
                        }

                    }
                }
            }
            pixels = new bool[size.Width, size.Height];

            pixels = pixelsNew;
        }

        private int getLiveNeighbourCount(int x, int y)
        {
            int count = 0;

            if (x != 0 && y != 0 && pixels[x - 1, y - 1] == true)
            {
                count++;
            }

            if (x != 0 && pixels[x - 1, y] == true)
            {
                count++;
            }

            if (x != 0 && y + 1 < size.Height && pixels[x - 1, y + 1] == true)
            {
                count++;
            }

            if (y < size.Height - 1 && pixels[x, y + 1] == true)
            {
                count++;
            }

            if (y < size.Height - 1 && x + 1 < size.Width && pixels[x + 1, y + 1] == true)
            {
                count++;
            }

            if (x < size.Width - 1 && pixels[x + 1, y] == true)
            {
                count++;
            }

            if (x < size.Width - 1 && y != 0 && pixels[x + 1, y - 1] == true)
            {
                count++;
            }

            if (y != 0 && pixels[x, y - 1] == true)
            {
                count++;
            }

            return count;
        }


        private void updateFPS(int f)
        {
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / (float)f);
            fps = f;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.Textures[0] = null;

            pixelArray = new Color[pixels.Length];
            int i = 0;

            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    bool p = pixels[x, y];
                    if (p)
                    {
                        pixelArray[i] = Color.White;
                    }
                    else
                    {
                        pixelArray[i] = Color.Black;
                    }
                    i++;
                }
            }

            canvas.SetData<Color>(pixelArray);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(canvas, new Rectangle(0, 0, renderSize.Width, renderSize.Height), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
