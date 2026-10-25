using System.Drawing;

namespace TrayTemps
{
    internal sealed class ThemePalette
    {
        private ThemePalette() { }

        public Color WindowBack { get; private set; }
        public Color PageBack { get; private set; }
        public Color SurfaceBack { get; private set; }
        public Color SecondarySurface { get; private set; }
        public Color NavBack { get; private set; }
        public Color NavSelected { get; private set; }
        public Color Text { get; private set; }
        public Color BodyText { get; private set; }
        public Color MutedText { get; private set; }
        public Color TitleText { get; private set; }
        public Color Accent { get; private set; }
        public Color InputBack => SurfaceBack;
        public Color Border { get; private set; }
        public Color ButtonText { get; private set; }
        public Color AccentButtonText => Color.White;
        public Color CurrentTemp { get; private set; }
        public Color Success { get; private set; }
        public Color Danger { get; private set; }
        public Color Link { get; private set; }
        public Color HardwareHoverText { get; private set; }
        public Color BarBack => NavBack;
        public Color ButtonBack { get; private set; }
        public Color ButtonHover { get; private set; }
        public Color ButtonDown { get; private set; }
        public Color DisabledText { get; private set; }
        public Color NavigationHover { get; private set; }
        public Color NavigationDown { get; private set; }

        public static ThemePalette Create(bool light)
        {
            if (light)
            {
                return new ThemePalette
                {
                    WindowBack = Color.FromArgb(218, 226, 238),
                    PageBack = Color.FromArgb(246, 248, 252),
                    SurfaceBack = Color.White,
                    SecondarySurface = Color.FromArgb(248, 250, 252),
                    NavBack = Color.White,
                    NavSelected = Color.FromArgb(226, 239, 255),
                    Text = Color.FromArgb(31, 41, 55),
                    BodyText = Color.FromArgb(31, 41, 55),
                    MutedText = Color.FromArgb(91, 103, 122),
                    TitleText = Color.FromArgb(15, 23, 42),
                    Accent = Color.FromArgb(37, 99, 235),
                    Border = Color.FromArgb(210, 218, 230),
                    ButtonText = Color.White,
                    CurrentTemp = Color.FromArgb(15, 23, 42),
                    Success = Color.FromArgb(21, 128, 61),
                    Danger = Color.FromArgb(220, 38, 38),
                    Link = Color.FromArgb(4, 120, 87),
                    HardwareHoverText = Color.FromArgb(37, 99, 235),
                    ButtonBack = Color.FromArgb(241, 245, 249),
                    ButtonHover = Color.FromArgb(226, 239, 255),
                    ButtonDown = Color.FromArgb(210, 228, 255),
                    DisabledText = Color.FromArgb(148, 163, 184),
                    NavigationHover = Color.FromArgb(232, 238, 247),
                    NavigationDown = Color.FromArgb(210, 220, 234)
                };
            }

            return new ThemePalette
            {
                WindowBack = Color.FromArgb(21, 21, 21),
                PageBack = Color.FromArgb(25, 25, 25),
                SurfaceBack = Color.FromArgb(40, 40, 40),
                SecondarySurface = Color.FromArgb(36, 36, 36),
                NavBack = Color.FromArgb(30, 30, 30),
                NavSelected = Color.FromArgb(50, 50, 50),
                Text = Color.LightGray,
                BodyText = Color.LightGray,
                MutedText = Color.DarkGray,
                TitleText = Color.WhiteSmoke,
                Accent = Color.FromArgb(0, 120, 212),
                Border = Color.FromArgb(60, 60, 60),
                ButtonText = Color.LightGray,
                CurrentTemp = Color.LightGray,
                Success = Color.LimeGreen,
                Danger = Color.Red,
                Link = Color.SeaGreen,
                HardwareHoverText = Color.White,
                ButtonBack = Color.FromArgb(40, 40, 40),
                ButtonHover = Color.FromArgb(55, 55, 55),
                ButtonDown = Color.FromArgb(65, 65, 65),
                DisabledText = Color.FromArgb(100, 100, 100),
                NavigationHover = Color.FromArgb(55, 55, 55),
                NavigationDown = Color.FromArgb(70, 70, 70)
            };
        }
    }
}
