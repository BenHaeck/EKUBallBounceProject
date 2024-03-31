using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BallProjectAssignment {
	public class MeshTransform {
		public Vector3 position;
		public Matrix matrix;

		private Matrix transformation;

		public Matrix Transformation => transformation;

		public MeshTransform() {}

		public void UpdateTransform () {
			transformation = matrix * Matrix.CreateTranslation (position);
		}

	}
}
