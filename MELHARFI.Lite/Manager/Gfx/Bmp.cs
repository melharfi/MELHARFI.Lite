using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using static MELHARFI.Lite.Manager.Manager;

namespace MELHARFI.Lite.Manager.Gfx
{
    /// <summary>
    /// Bmp class store information about image to be drawn
    /// </summary>
    public class Bmp : IGfx
    {
        #region properties

        #region Mouse Event Handler
        // classe Bmp qui contiens des infos sur les images, hérite de l'interface IGfx
        /// <summary>
        /// Delegate of Mouse Event Handler, not usfull for the user, it's a Mouse Event system mechanisme
        /// </summary>
        /// <param name="bmp">bmp is a graphic object "image" that raised the event</param>
        /// <param name="e">e is a Mouse Event Arguments with some handy information, as button of clic, position ...</param>
        public delegate void BmpMouseEventHandler(Bmp bmp, MouseEventArgs e);

        /// <summary>
        /// Handler when you double clic on object
        /// </summary>
        public event BmpMouseEventHandler MouseDoubleClic;

        /// <summary>
        /// Handler when you clic on object
        /// </summary>
        public event BmpMouseEventHandler MouseClic;

        /// <summary>
        /// Handler when you clic and keep button clicked on the object
        /// </summary>
        public event BmpMouseEventHandler MouseDown;

        /// <summary>
        /// Handler when you release button that has been clicked in an object
        /// </summary>
        public event BmpMouseEventHandler MouseUp;

        /// <summary>
        /// Handler when the mouse move inside an object, event will raise many time as you move the mouse over object
        /// </summary>
        public event BmpMouseEventHandler MouseMove;

        /// <summary>
        /// Handler when a mouse get outside of an object
        /// </summary>
        public event BmpMouseEventHandler MouseOut;

        /// <summary>
        /// Handler when a mouse get indise an object, the event is raised only 1 time
        /// </summary>
        public event BmpMouseEventHandler MouseOver;

        /// <summary>
        /// void raise mouse event if true
        /// </summary>
        public bool EscapeGfxWhileMouseDoubleClic = false;
        public bool EscapeGfxWhileMouseClic = false;
        public bool EscapeGfxWhileMouseOver = false;

        public void Center()
        {
            this.Point = new Point((ManagerInstance.Control.Width / 2) - (Bitmap.Width / 2), (ManagerInstance.Control.Height / 2) - (Bitmap.Height / 2));
        }

        public bool EscapeGfxWhileMouseDown = false;
        public bool EscapeGfxWhileMouseUp = false;
        public bool EscapeGfxWhileMouseMove = false;
        public bool EscapeGfxWhileKeyDown = false;

        /// <summary>
        /// To fire the Mouse Double Clic event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseDoubleClic(MouseEventArgs e) => MouseDoubleClic?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Clic event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseClic(MouseEventArgs e) => MouseClic?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Down event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseDown(MouseEventArgs e) => MouseDown?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Up event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseUp(MouseEventArgs e) => MouseUp?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Move event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseMove(MouseEventArgs e) => MouseMove?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Over event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseOver(MouseEventArgs e) => MouseOver?.Invoke(this, e);

        /// <summary>
        /// To fire the Mouse Out event without interaction of user
        /// </summary>
        /// <param name="e">e = MouseEventArgs</param>
        internal void FireMouseOut(MouseEventArgs e)
        {
            if (MouseOut == null) return;
            ManagerInstance.mouseOverRecorder.Remove(this);  // pour que MouseOut ne cherche pas sur un Gfx qui n'est pas sur le devant
            MouseOut(this, e);
        }
        #endregion

        /// <summary>
        /// Layer holding sub graphics that is shown in the front of the parent, and its position is relative to parent
        /// </summary>

        public List<IGfx> Childs { get; set; } = new List<IGfx>();

        /// <summary>
        /// Layer to memorise all sequaces of rectangles of a sprite sheet, it's only a user side, and not a engine mechanisme
        /// </summary>
        public List<Rectangle> SpriteSheets = new List<Rectangle>();

        /// <summary>
        /// Bitmap image
        /// </summary>
        public Bitmap Bitmap;

        /// <summary>
        /// point where the image will be drawn on the form, Default is X = 0, Y = 0;
        /// </summary>
        public Point Point { get; set; } = Point.Empty;

        /// <summary>
        /// String value to store the name of the object, useful if you need to look for it in the appropriate layer, value is read only
        /// </summary>
        /// <returns>Return a string value as a name of the object</returns>
        public string Name { get; set; }

        /// <summary>
        /// Zindex is a int value indicating the deep of the object against the other object in same graphic layer, default is 1
        /// </summary>
        /// <returns>Return an int value </returns>
        public int Zindex { get; set; } = 1;

        /// <summary>
        /// Boolean value indicating if the object is visible or invisible, value read only
        /// </summary>
        /// <returns>Return a boolean value</returns>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Float, Opacity is the transparency of the object, 1F = full opaque (100% visible), 0.5F is 50% visible, 0F is invisible, default is 1 (100% visible)
        /// </summary>
        public float Opacity = 1;

        /// <summary>
        /// Hold which layer the object is stored
        /// </summary>
        public TypeGfx TypeGfx = TypeGfx.Background;

        /// <summary>
        /// Tag is an object that you can affect to anything you want, usful to attach it to a class that hold some statistic to a players, Pay attention that you should cast the object
        /// </summary>
        /// <returns>Return Object</returns>
        public object Tag { get; set; }

        /// <summary>
        /// newColorMap is a ColorMap object, used to convert a color from one color to another in the all picture, usfull to paint some area like changing color of eyes, skin, hair
        /// </summary>
        public ColorMap[] NewColorMap;

        /// <summary>
        /// Rectangle is an area of an image, used to show only a part of image, a Rectangle need a Point value defined by a X position and Y position, and Size value defined by a Width and Height
        /// </summary>
        public Rectangle Rectangle
        {
            get;
            set;
        }

        /// <summary>
        /// Flag equal true when the object show only a rectangle piece of the image (SpriteSheet), if false, then all the image is displayed
        /// </summary>
        public bool IsSpriteSheet { get; private set; }

        /// <summary>
        /// A referense is requird to the Manager instance that hold this object
        /// </summary>
        public Manager ManagerInstance { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Empty Bmp constructor for a bitmap, Pay attention that you should initialise the other parameters later
        /// </summary>
        /// <param name="manager">Reference to the manager instance that hold this object</param>
        public Bmp(Manager manager)
        {
            // constructeur vide qui sert a initialiser l'objet Bmp avant de connaitre l'image qui va l'occuper
            // vus que ce n'est pas possible d'initialiser un objet Bitmap sans connaitre la source
            // cela instancie la classe Bmp avant, et affecter une image apres
            ManagerInstance = manager;
        }


        public Bmp(Bitmap bitmap, Point point, Manager manager)
        {
            Point = point;
            ManagerInstance = manager;
            Bitmap = bitmap;

            if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);
            Rectangle = new Rectangle(new Point(0, 0), new Size(Bitmap.Width, Bitmap.Height));

            Zindex = ManagerInstance.ZOrder.Bgr();
            TypeGfx = TypeGfx.Background;
        }

        public Bmp(Bitmap bitmap, Point point, Rectangle rectangle, Manager manager)
        {
            Point = point;
            ManagerInstance = manager;
            Bitmap = bitmap;

            if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);

            Rectangle = rectangle;
            IsSpriteSheet = true;

            Zindex = ManagerInstance.ZOrder.Bgr();
            TypeGfx = TypeGfx.Background;
        }

        public Bmp(Bitmap bitmap, Point point, Size size, Manager manager)
        {
            Point = point;
            ManagerInstance = manager;

            using (Bitmap tmp = new Bitmap(bitmap))
            {
                Bitmap = new Bitmap(bitmap, size);
                if (tmp.RawFormat.Equals(ImageFormat.Gif))
                {
                    // si ce constructeur n'affiche pas une image gif avec just le rectangle défini il faut utiliser juste bmp = new Bitmap(path); qui ne prend pas le rectangle en considération
                    //bmp = new Bitmap(path);
                        
                    ImageAnimator.Animate(Bitmap, null);
                }
            }
                    
            Rectangle = new Rectangle(Point.Empty, size);
            Zindex = ManagerInstance.ZOrder.Bgr();
            TypeGfx = TypeGfx.Background;
        }

        public Bmp(Bitmap bitmap, Point point, string name, TypeGfx typeGfx, bool visible, Manager manager)
        {
            Point = point;
            Name = name;
            Visible = visible;
            ManagerInstance = manager;
            Bitmap = bitmap;
            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            if (bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);

            Rectangle = new Rectangle(Point.Empty, new Size(Bitmap.Width, Bitmap.Height));
        }

        public Bmp(Bitmap bitmap, Point point, string name, TypeGfx typeGfx, bool visible, Rectangle rectangle, Manager manager)
        {
            Point = point;
            Name = name;
            Visible = visible;
            Rectangle = rectangle;
            IsSpriteSheet = true;
            ManagerInstance = manager;
            Bitmap = bitmap;

            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);
        }

        public Bmp(Bitmap bitmap, Point point, Size size, string name, TypeGfx typeGfx, bool visible, Manager manager)
        {
            Name = name;
            Visible = visible;
            Point = point;
            ManagerInstance = manager;

            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            using (Bitmap tmp = new Bitmap(bitmap))
            {
                Bitmap = new Bitmap(bitmap, size);
                if (tmp.RawFormat.Equals(ImageFormat.Gif))
                    ImageAnimator.Animate(Bitmap, null);
            }
            Rectangle = new Rectangle(new Point(0, 0), size);
        }

        public Bmp(Bitmap bitmap, Point point, Size size, string name, TypeGfx typeGfx, bool visible, float opacity, Manager manager)
        {
            Point = point;
            Name = name;
            Visible = visible;
            Opacity = opacity;
            ManagerInstance = manager;

            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            using (Bitmap tmp = new Bitmap(bitmap))
            {
                Bitmap = ManagerInstance.Opacity(new Bitmap(tmp, size), opacity);
                if (tmp.RawFormat.Equals(ImageFormat.Gif))
                    ImageAnimator.Animate(Bitmap, null);
            }
            Rectangle = new Rectangle(new Point(0, 0), Bitmap.Size);
        }

        public Bmp(Bitmap bitmap, Point point, string name, TypeGfx typeGfx, bool visible, float opacity, Manager manager)
        {
            Point = point;
            Name = name;
            Visible = visible;
            Opacity = opacity;
            ManagerInstance = manager;

            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            Bitmap = ManagerInstance.Opacity(bitmap, opacity);
            if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);

            Rectangle = new Rectangle(new Point(0, 0), Bitmap.Size);
        }


        public Bmp(Bitmap bitmap, Point point, string name, TypeGfx typeGfx, bool visible, float opacity, Rectangle rectangle, Manager manager)
        {
            Point = point;
            Name = name;
            Visible = visible;
            Opacity = opacity;
            ManagerInstance = manager;

            switch (typeGfx)
            {
                case TypeGfx.Background:
                    Zindex = ManagerInstance.ZOrder.Bgr();
                    TypeGfx = TypeGfx.Background;
                    break;
                case TypeGfx.Object:
                    Zindex = ManagerInstance.ZOrder.Obj();
                    TypeGfx = TypeGfx.Object;
                    break;
                case TypeGfx.Top:
                    Zindex = ManagerInstance.ZOrder.Top();
                    TypeGfx = TypeGfx.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeGfx), typeGfx, null);
            }

            Bitmap = ManagerInstance.Opacity(bitmap, opacity);
            if (Bitmap.RawFormat.Equals(ImageFormat.Gif))
                ImageAnimator.Animate(Bitmap, null);

            Rectangle = rectangle;
            IsSpriteSheet = true;
        }
        #endregion

        #region functions
        /// <summary>
        /// Changing the picture of the Bmp object
        /// </summary>
        /// <param name="bitmap">bitmap is the path of the image gived as a string</param>
        public void ChangeBmp(Bitmap bitmap)
        {
            try
            {
                Opacity = 1;

                if (bitmap.RawFormat.Equals(ImageFormat.Gif))
                    ImageAnimator.Animate(bitmap, null);

                Bitmap = bitmap;
                Rectangle = new Rectangle(new Point(0, 0), Bitmap.Size);
                IsSpriteSheet = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        /// <summary>
        /// Changing the picture of the Bmp object, this overload change the size of the picture too
        /// </summary>
        /// <param name="bitmap">bitmap is the path of the image gived as a string</param>
        /// <param name="size">Size is a value of Width and Height to resize the picture</param>
        public void ChangeBmp(Bitmap bitmap, Size size)
        {
            try
            {
                Opacity = 1;

                using (Bitmap tmp = new Bitmap(bitmap))
                    if (!tmp.RawFormat.Equals(ImageFormat.Gif))
                        Bitmap = new Bitmap(new Bitmap(tmp), size.Width, size.Height);
                    else
                    {
                        Bitmap = new Bitmap(bitmap);
                        ImageAnimator.Animate(Bitmap, null);
                    }
                Rectangle = new Rectangle(new Point(0, 0), new Size(size.Width, size.Height));
                IsSpriteSheet = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void ChangeBmp(Bitmap bitmap, Rectangle rectangle)
        {
            try
            {
                Opacity = 1;
                using (Bitmap tmp = new Bitmap(bitmap))
                    if (!tmp.RawFormat.Equals(ImageFormat.Gif))
                        Bitmap = new Bitmap(new Bitmap(tmp), Bitmap.Size);
                    else
                    {
                        Bitmap = new Bitmap(bitmap);
                        ImageAnimator.Animate(Bitmap, null);
                    }
                IsSpriteSheet = true;
                Rectangle = rectangle;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void ChangeBmp(Bitmap bitmap, float opacity)
        {
            try
            {
                Opacity = opacity;
                using (Bitmap tmp = new Bitmap(bitmap))
                    if (!tmp.RawFormat.Equals(ImageFormat.Gif))
                        Bitmap = ManagerInstance.Opacity(new Bitmap(tmp, Bitmap.Size), opacity);
                    else
                    {
                        Bitmap = new Bitmap(bitmap);
                        ImageAnimator.Animate(Bitmap, null);
                    }
                Rectangle = new Rectangle(new Point(0, 0), Bitmap.Size);
                IsSpriteSheet = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void ChangeBmp(Bitmap bitmap, float opacity, Rectangle rectangle)
        {
            try
            {
                Opacity = opacity;
                using (Bitmap tmp = new Bitmap(bitmap))
                    if (!tmp.RawFormat.Equals(ImageFormat.Gif))
                        Bitmap = ManagerInstance.Opacity(new Bitmap(tmp, Bitmap.Size), opacity);
                    else
                    {
                        Bitmap = new Bitmap(bitmap);
                        ImageAnimator.Animate(Bitmap, null);
                    }
                IsSpriteSheet = true;
                Rectangle = rectangle;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }
        #endregion

        private void ExceptionHandler(Exception ex)
        {
            ManagerInstance.OutputErrorCallBack(ex.ToString());
        }

        /// <summary>
        /// Create a perfect duplication of the Bmp object
        /// </summary>
        /// <returns>Return an object of Bmp, you need a cast to Bmp type</returns>
        public object Clone() => MemberwiseClone();
    }
}
