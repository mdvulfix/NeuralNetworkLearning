using UnityEngine;

namespace APP
{
    public static class HandlerSprite
    {

        public static Sprite Square => SpriteSquare.Get();
        public static Sprite Circle => SpriteCircle.Get();
        public static Sprite Triangle => SpriteTriangle.Get();

        public static Sprite Get(Sprite sprite, SpriteIndex index) =>
            Get(sprite, index, null, null);

        public static Sprite Get(Sprite sprite, SpriteIndex index, string folderPath, string name)
        {
            var byDefault = ((folderPath == null) || (name == null));

            switch (index)
            {
                default: 
                    Debug.LogWarning($"SpriteExtension: {index} is not found!");
                    return null;

                case SpriteIndex.Square:
                        return byDefault ? SpriteSquare.Get() : SpriteSquare.Get(folderPath, name);

                case SpriteIndex.Circle:
                        return byDefault ? SpriteCircle.Get() : SpriteCircle.Get(folderPath, name);

                case SpriteIndex.Triangle:
                        return byDefault ? SpriteTriangle.Get() : SpriteTriangle.Get(folderPath, name);
            }
        }

        public static void SetPath(Sprite sprite, SpriteIndex index, string folderPath, string name)
        {

            switch (index)
            {
                default : break;

                case SpriteIndex.Square:
                        SpriteSquare.SetPath(folderPath, name);
                    break;

                case SpriteIndex.Circle:
                        SpriteCircle.SetPath(folderPath, name);
                    break;

                case SpriteIndex.Triangle:
                        SpriteTriangle.SetPath(folderPath, name);
                    break;
            }
        }
    }

    public static class SpriteSquare
    {
        private static string PATH_DEFAULT = "Sprite/Square";

        public static Sprite Get() =>
            Resources.Load<Sprite>(PATH_DEFAULT);

        public static Sprite Get(string folderPath, string name) =>
            Resources.Load<Sprite>($"{folderPath}/{name}");

        public static void SetPath(string folderPath, string name) =>
            PATH_DEFAULT = $"{folderPath}/{name}";

    }

    public static class SpriteCircle
    {
        private static string PATH_DEFAULT = "Sprite/Circle";

        public static Sprite Get() =>
            Resources.Load<Sprite>(PATH_DEFAULT);

        public static Sprite Get(string folderPath, string name) =>
            Resources.Load<Sprite>($"{folderPath}/{name}");

        public static void SetPath(string folderPath, string name) =>
            PATH_DEFAULT = $"{folderPath}/{name}";

    }

    public static class SpriteTriangle
    {
        private static string PATH_DEFAULT = "Sprite/Triangle";

        public static Sprite Get() =>
            Resources.Load<Sprite>(PATH_DEFAULT);

        public static Sprite Get(string folderPath, string name) =>
            Resources.Load<Sprite>($"{folderPath}/{name}");

        public static void SetPath(string folderPath, string name) =>
            PATH_DEFAULT = $"{folderPath}/{name}";

    }

    public enum SpriteIndex
    {
        None,
        Square,
        Circle,
        Triangle
    }

}