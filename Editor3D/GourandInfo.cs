using System;
using System.Drawing;

namespace Editor3D
{
    internal class GourandInfo
    {
        private Color v1color;
        private Color v2color;
        private Color v3color;
        private Color v3v2color;
        private Color v2v1color;
        private Color v3v1color;

        internal void SetV1GourandIntensity(Color color)
        {
            v1color = color;
        }

        internal void SetV2GourandIntensity(Color color)
        {
            v2color = color;
        }

        internal void SetV3GourandIntensity(Color color)
        {
            v3color = color;
        }

        internal void SetV3V1GourandIntensity(Color color)
        {
            v3v1color = color;
        }

        internal void SetV3V2GourandIntensity(Color color)
        {
            v3v2color = color;
        }

        internal void SetV2V1GourandIntensity(Color color)
        {
            v2v1color = color;
        }

        internal Color GetV2GourandIntensity()
        {
            return v2color;
        }

        internal Color GetV1GourandIntensity()
        {
            return v1color;
        }

        internal Color GetV3GourandIntensity()
        {
            return v3color;
        }

        internal Color GetV3V1GourandIntensity()
        {
            return v3v1color;
        }

        internal Color GetV3V2GourandIntensity()
        {
            return v3v2color;
        }

        internal Color GetV2V1GourandIntensity()
        {
            return v2v1color;
        }
    }
}
