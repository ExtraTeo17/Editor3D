using Editor3D.Utilities;
using System.Drawing;

namespace Editor3D
{
    internal class Light
    {
        internal readonly Color Id;
        internal readonly Color Is;
        private readonly Vector lightPosition;

        public Light(Color Id, Color Is, Vector position)
        {
            this.Id = Id;
            this.Is = Is;
            this.lightPosition = position;
        }

        internal Vector GetPosition()
        {
            return lightPosition;
        }
    }
}
