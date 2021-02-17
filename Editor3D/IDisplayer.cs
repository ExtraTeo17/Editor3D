using Editor3D.Utilities;
using System.Drawing;

namespace Editor3D
{
    internal interface IDisplayer
    {
        void Display(int x, int y, double z, Color color);
        Shading GetShading();
        bool IsFog();
    }
}
