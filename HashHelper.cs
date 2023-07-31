namespace XFrame.Common
{
    public static class HashHelper
    {
        public static int Combine(int h1, int h2)
        {
            unchecked
            {
                var rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
                return ((int)rol5 + h1) ^ h2;
            }
        }
    }
}
