using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallProjectAssignment {
	public class Ball {
		//public readonly Model sphere;
		MeshTransform tr = new MeshTransform ();
		public Vector3 velocity = Vector3.Zero;
		public readonly float radius;
		public float mass;

		public int health = 20;

		public Ball (Model sphere, float radius, Vector3 position) {
			tr.position = position;
			this.radius = radius;
			mass = radius;
			tr.matrix = Matrix.CreateScale (radius/sphere.Meshes[0].BoundingSphere.Radius);
			
		}
		
		public Vector3 Position {
			get {
				return tr.position;
			}
			set {
				tr.position = value;
			}
		}

		public void Update (float dt) {
			tr.position += velocity * dt;
			tr.UpdateTransform ();
		}

		public static bool CheckCollision (Ball ball, Ball ball1) {
			float dist = Vector3.Distance (ball.Position, ball1.Position); // collision detection
			float combinedRadius = ball.radius + ball1.radius; // the combined radius
			float overlap = combinedRadius - dist; // the overlap

			if (overlap > 0) { // if it collides
				Vector3 dist3 = ball1.Position - ball.Position;
				Vector3 dir = dist3 / dist;
				ball.health -= 1; // decrease the health
				ball1.health -= 1;

				// handles velocity (aka makes it bounce)
				float v1 = Vector3.Dot (ball.velocity, dir);
				float v2 = Vector3.Dot (ball1.velocity, dir);

				float mult = 1/(ball.mass + ball1.mass);
				float nv1 = (ball.mass - ball1.mass) * v1 + 2*ball1.mass*v2;
				nv1 *= mult;
				float nv2 = (ball1.mass - ball.mass) * v2 + 2*ball.mass*v1;
				nv2 *= mult;

				ball.velocity += (nv1-v1) * dir;
				ball1.velocity += (nv2-v2) * dir;

				// move the balls so they aren't colliding
				ball1.Position += dir * overlap / 2;
				ball.Position -= dir * overlap / 2;
				return true;
			}
			return false;
			
		}

		public static void CheckCollision (List<Ball> ball) {
			for (int i = 1; i < ball.Count; i++) {
				for (int j = 0; j < i; j++) {
					CheckCollision (ball[i], ball[j]);
				}
			}
		}

		public Matrix TransformMatrix => tr.Transformation;
	}
}
