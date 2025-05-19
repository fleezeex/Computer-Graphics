using OpenTK.Mathematics;

namespace KolobokGame
{
    public class Tree
    {
        public Vector3 Position { get; set; }
        public float TrunkHeight { get; set; } = 2.0f;
        public float TrunkRadius { get; set; } = 0.3f;
        public float FoliageRadius { get; set; } = 1.2f;
        public float YOffset { get; set; } = -1.0f;
    }
}