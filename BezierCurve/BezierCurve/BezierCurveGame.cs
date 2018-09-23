using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BezierCurve
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class BezierCurveGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		private Color backgroundColor;
		private Texture2D mainlinePixel;
		private List<Texture2D> sublinesPixels;
		private BezierCurve bezierCurve;

		private KeyboardState currentKeyboardState, previousKeyboardState;

		public BezierCurveGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;

			backgroundColor = new Color(126, 108, 77, 255);

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
			// TODO: Add your initialization logic here
			IsMouseVisible = true;

			var points = new List<Vector2>
			{
				new Vector2(100, 700),
				new Vector2(100, 175),
				new Vector2(400, 175),
				new Vector2(600, 525),
				new Vector2(1200, 525),
				new Vector2(800, 350),
			    new Vector2(900, 450)

				/**
				 *
				 * p0
				 * p1
				 * p2
				 * p3
				 * 
				 * e = GetPoints(p0,p1,p2)
				 *     a = GetPoints(p0,p1)
				 *         p0 = GetPoints(p0)
				 *         p1 = GetPoints(p1)
				 *         return Lerp(p0,p1)
				 *     b = GetPoints(p1,p2)
				 *         p1 = GetPoints(p1)
				 *         p2 = GetPoints(p2)
				 *         return Lerp(p1,p2)
				 *     return Lerp(a,b)
				 * f = GetPoints(p1,p2,p3)
				 *     c = GetPoints(p1,p2)
				 *         p1 = GetPoints(p1)
				 *         p2 = GetPoints(p2)
				 *         return Lerp(p1,p2)
				 *     d = GetPoints(p2,p3)
				 *         p2 = GetPoints(p2)
				 *         p3 = GetPoints(p3)
				 *         return Lerp(p2,p3)
				 * return Lerp(e,f)
				 *
				 */
			};

			bezierCurve = new BezierCurve(10, points);

			currentKeyboardState = previousKeyboardState = Keyboard.GetState();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			mainlinePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			mainlinePixel.SetData(new [] { new Color(56, 56, 56, 255) });

			var red = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			red.SetData(new[] { new Color(124, 69, 65, 255) });
			var green = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			green.SetData(new[] { new Color(52, 81, 52, 255) });
			var blue = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			blue.SetData(new[] { new Color(42, 87, 112, 255) });

			sublinesPixels = new List<Texture2D>
			{
				red,
				green,
				blue
			};
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
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
			previousKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			if(currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R)) bezierCurve.Reset();
			if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space)) bezierCurve.Pause();


			bezierCurve.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(backgroundColor);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			bezierCurve.Draw(spriteBatch, mainlinePixel, sublinesPixels);

			spriteBatch.Draw(mainlinePixel, bezierCurve.GetRecentPointOnCurve, null, Color.White, 0.0f, Vector2.Zero, 10.0f, SpriteEffects.None, 0.0f);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
