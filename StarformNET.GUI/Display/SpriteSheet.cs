namespace Primoris.Universe.Stargen.Display
{
    using System.Drawing;
    using System;
    using System.Collections.Generic;
    using Bodies;

    public class PlanetSpriteSheet
    {
        private static Dictionary<BodyType, int> PlanetMapping = new Dictionary<BodyType, int>()
        {
            { BodyType.Asteroid,      2 },
            { BodyType.GasGiant,       6 },
            { BodyType.Ice,            5 },
            { BodyType.Martian,        7 },
            { BodyType.Barren,           2 },
            { BodyType.SubGasGiant,    6 },
            { BodyType.SubSubGasGiant, 6 },
            { BodyType.Terrestrial,    0 },
            { BodyType.Undefined,        8 },
            { BodyType.Venusian,       4 },
            { BodyType.Water,          3 }
        };

        public Size SpriteSize { get; set; }

        private int _hPadding;
        private int _vPadding;
        private int _planetTypes;
        private Image _image;
        private Point _upperLeft;

        public PlanetSpriteSheet(Image image, Point upperLeft, Size spriteSize,
            int hPadding, int vPadding, int planetTypes)
        {
            _hPadding = hPadding;
            _vPadding = vPadding;
            _planetTypes = planetTypes;
            _image = image;
            _upperLeft = upperLeft;
            SpriteSize = spriteSize;
        }

        public Sprite GetSprite(BodyType type)
        {
            var planetNum = Extensions.RandomInt(0, _planetTypes - 1);
            var planetRow = PlanetMapping[type];
            var x = _upperLeft.X + (planetNum * SpriteSize.Width) + (planetNum * _hPadding);
            var y = _upperLeft.Y + (planetRow * SpriteSize.Height) + (planetRow * _vPadding);
            var rect = new Rectangle(x, y, SpriteSize.Width, SpriteSize.Height);
            return new Sprite(_image, rect);
        }
    }
}
