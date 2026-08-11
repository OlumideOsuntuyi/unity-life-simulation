namespace Simulation
{
    public static class PositionFunctions
    {
        public const int SIZE = 16;
        public const float INVERSE_SIZE = 1F / SIZE;
        private const int SIZE_BEHIND = SIZE - 1;
        public static Vector3Int FromWorld(Vector3Int world)
        {
            int offsetX = world.x < 0 ? SIZE_BEHIND : 0;
            int offsetY = world.y < 0 ? SIZE_BEHIND : 0;
            int offsetZ = world.z < 0 ? SIZE_BEHIND : 0;

            int x_ = (int)((world.x - offsetX) * INVERSE_SIZE);
            int y_ = (int)((world.y - offsetX) * INVERSE_SIZE);
            int z_ = (int)((world.z - offsetX) * INVERSE_SIZE);

            return new Vector3Int(x_, y_, z_);
        }
        public static Vector3Int FromWorld(Vector3 world)
        {
            return FromWorld(new Vector3Int((int)world.x, (int)world.y, (int)world.z));
        }
        public static Vector3Int ToWorld(Vector3Int chunk)
        {
            return ToWorld(chunk.x, chunk.y, chunk.z);
        }
        public static Vector3Int ToWorld(int x,int y,  int z)
        {
            int x_ = (x * 16);
            int y_ = (y * 16);
            int z_ = (z * 16);

            return new Vector3Int(x_, y_, z_);
        }
        public static Vector3Int GetLocal(Vector3Int world)
        {
            var origin = ToWorld(FromWorld(world));
            return world - origin;
        }
        public static Vector3Int GetLocal(Vector3 world)
        {
            return GetLocal(new Vector3Int((int)world.x, (int)world.y, (int)world.z));
        }
        public static Vector3Int TransformToVoxelSpace(Vector3 transformPosition)
        {
            return ToWorld(FromWorld(transformPosition)) + GetLocal(transformPosition);
        }
    }
}
