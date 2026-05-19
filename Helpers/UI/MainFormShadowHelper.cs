using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class MainFormShadowHelper
    {
        internal static void InitializeCardShadows(
            IEnumerable<Control> shadowCards,
            PaintEventHandler paintHandler,
            EventHandler changedHandler)
        {
            foreach (Control parent in GetShadowCardParents(shadowCards))
            {
                parent.Paint += paintHandler;
                parent.Resize += changedHandler;
            }

            foreach (Control card in shadowCards)
            {
                if (card == null)
                    continue;

                card.LocationChanged += changedHandler;
                card.SizeChanged += changedHandler;
                card.VisibleChanged += changedHandler;
            }
        }

        internal static void InitializeMainMenuShadow(
            IEnumerable<Control> shadowHosts,
            Control mainMenu,
            PaintEventHandler paintHandler,
            EventHandler changedHandler)
        {
            foreach (Control host in shadowHosts)
            {
                if (host == null)
                    continue;

                host.Paint += paintHandler;
                host.Resize += changedHandler;
            }

            mainMenu.LocationChanged += changedHandler;
            mainMenu.SizeChanged += changedHandler;
        }

        internal static void InvalidateMainMenuShadowHosts(IEnumerable<Control> shadowHosts)
        {
            foreach (Control host in shadowHosts)
            {
                host?.Invalidate();
            }
        }

        internal static void InvalidateCardShadowParents(IEnumerable<Control> shadowCards)
        {
            foreach (Control parent in GetShadowCardParents(shadowCards))
            {
                parent.Invalidate();
            }
        }

        internal static void CardShadowParent_Paint(
            object sender,
            PaintEventArgs e,
            IEnumerable<Control> shadowCards,
            bool lightTheme)
        {
            if (!(sender is Control parent))
                return;

            foreach (Control card in shadowCards)
            {
                if (card == null || card.Parent != parent || !card.Visible)
                    continue;

                DrawCardShadow(e.Graphics, card.Bounds, lightTheme);
            }
        }

        internal static void MainMenuShadowHost_Paint(
            object sender,
            PaintEventArgs e,
            Control mainMenu,
            bool lightTheme,
            int mainMenuShadowWidth)
        {
            if (!(sender is Control host) || mainMenu == null)
                return;

            Rectangle menuBounds = host.RectangleToClient(mainMenu.RectangleToScreen(mainMenu.ClientRectangle));
            var sourceBounds = new Rectangle(
                menuBounds.Left,
                0,
                mainMenu.Width,
                host.Height);
            var clipBounds = new Rectangle(
                Math.Max(0, menuBounds.Right + 1),
                0,
                mainMenuShadowWidth,
                host.Height);

            DrawMainMenuShadow(e.Graphics, sourceBounds, clipBounds, lightTheme);
        }

        internal static IEnumerable<Control> GetShadowCards(params Control[] shadowCards)
        {
            // Main card containers only; nested settings row layouts stay flat.
            foreach (Control card in shadowCards)
                yield return card;
        }

        internal static IEnumerable<Control> GetShadowCardParents(IEnumerable<Control> shadowCards)
        {
            return shadowCards
                .Where(card => card != null && card.Parent != null)
                .Select(card => card.Parent)
                .Distinct();
        }

        internal static IEnumerable<Control> GetMainMenuShadowHosts(params Control[] shadowHosts)
        {
            foreach (Control host in shadowHosts)
                yield return host;
        }

        internal static void DrawCardShadow(Graphics g, Rectangle bounds, bool lightTheme)
        {
            int alpha = lightTheme ? 15 : 21;
            int offset = 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle shadowBounds = bounds;
            shadowBounds.Offset(offset, offset);
            shadowBounds.Inflate(1, 1);

            using (GraphicsPath path = CreateRoundedRectanglePath(shadowBounds, 4))
            using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
            {
                g.FillPath(brush, path);
            }
        }

        internal static void DrawMainMenuShadow(Graphics g, Rectangle sourceBounds, Rectangle clipBounds, bool lightTheme)
        {
            if (clipBounds.Width <= 0 || clipBounds.Height <= 0)
                return;

            GraphicsState state = g.Save();
            try
            {
                g.SetClip(clipBounds);
                DrawCardShadow(g, sourceBounds, lightTheme);
            }
            finally
            {
                g.Restore(state);
            }
        }

        internal static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = Math.Max(1, radius * 2);
            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        internal static IEnumerable<T> FindControls<T>(Control root) where T : Control
        {
            foreach (Control child in root.Controls)
            {
                if (child is T match)
                    yield return match;

                foreach (T descendant in FindControls<T>(child))
                    yield return descendant;
            }
        }
    }
}
