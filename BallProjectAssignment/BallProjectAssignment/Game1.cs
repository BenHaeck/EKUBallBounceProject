using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BallProjectAssignment {
	public class Game1: Game {
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Model sphere;

		List<Ball> balls = new List<Ball> ();

		Matrix cameraMat, projectionMat;
		Viewport view;

		float width, height;
		Random rand;

		bool runSimulation = true;

		public Game1 () {
			_graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize () {
			// TODO: Add your initialization logic here
			rand = new Random ();
			width = _graphics.PreferredBackBufferWidth;
			height = _graphics.PreferredBackBufferHeight;
			cameraMat = Matrix.CreateLookAt (new Vector3(0, 0, 100), Vector3.Zero, Vector3.UnitY);
			projectionMat = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians(45), width/height, 0.01f, 100);
			view = new Viewport (0, 0, (int)width, (int)height);
			base.Initialize ();
		}

		protected override void LoadContent () {
			_spriteBatch = new SpriteBatch (GraphicsDevice);
			sphere = Content.Load<Model> ("sphere");


			for (int i = 0; i < 20; i++) {
				AddRandomBall ();
			}
			
			// TODO: use this.Content to load your game content here
		}

		protected override void Update (GameTime gameTime) {
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();

			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			// TODO: Add your update logic here
			if (balls.Count <= 1|| Keyboard.GetState().IsKeyDown(Keys.Q))
				runSimulation = false;
			
			if (runSimulation) {
				for (int i = 0; i < balls.Count; i++) {
					balls[i].Update (dt);
					ClampBall (balls[i], dt);
				}
				Ball.CheckCollision (balls);
				for (int i = balls.Count-1; i >= 0; i--) {
					if (balls[i].health <= 0)
						balls.RemoveAt (i);
				}
			}
			base.Update (gameTime);
		}

		void ClampBall (Ball ball, float dt) {
			Vector3 ballScreenP = view.Project (ball.Position + new Vector3(1, 1, 0) * ball.radius, projectionMat, cameraMat, Matrix.Identity);
			Vector3 ballScreenN = view.Project (ball.Position - new Vector3(1, 1, 0) * ball.radius, projectionMat, cameraMat, Matrix.Identity);

			if (ballScreenP.X > width || ballScreenN.X < 0) {
				ball.velocity.X *= -1;
				var bp = ball.Position;
				bp.X += ball.velocity.X * dt * 2;
				ball.Position = bp;
			}
			if (ballScreenN.Y > height || ballScreenP.Y < 0) {
				ball.velocity.Y *= -1;
				var bp = ball.Position;
				bp.Y += ball.velocity.Y * dt * 2;
				ball.Position = bp;
			}
		}

		void AddRandomBall () {
			Vector2 ballRange = new Vector2(60, 30);
			float velRange = 40;
			
			Vector3 position = new Vector3 (
			rand.NextSingle () * ballRange.X - ballRange.X/2,
			rand.NextSingle () * ballRange.Y - ballRange.Y/2, 0);
			Vector3 velocity = new Vector3 (
			rand.NextSingle () * velRange - velRange/2,
			rand.NextSingle () * velRange - velRange/2, 0);

			float radius = rand.Next (1, 4);
			radius = radius * 1f + 1.5f;


			var ball = new Ball (sphere, radius, position);
			ball.velocity = velocity;

			balls.Add (ball);
		}

		protected override void Draw (GameTime gameTime) {
			GraphicsDevice.Clear (Color.CornflowerBlue);

			// TODO: Add your drawing code here
			for (int i = 0; i < balls.Count; i++) {
				DrawModel (sphere, balls[i].TransformMatrix, cameraMat, projectionMat, Vector3.One);
			}
			base.Draw (gameTime);
		}

		static void DrawModel (Model model, Matrix world, Matrix view, Matrix projection, Vector3 color) {
			for (int i = 0; i < model.Meshes.Count; i++) {
				foreach (BasicEffect effect in model.Meshes[i].Effects) {
					effect.World = world;
					effect.View = view;
					effect.Projection = projection;
					effect.EnableDefaultLighting ();
					effect.LightingEnabled = true;
					effect.DirectionalLight0.Direction = -new Vector3 (1, 1, 1);
					effect.DirectionalLight0.DiffuseColor = color;
				}
				model.Meshes[i].Draw ();
			}
		}
	}
}
