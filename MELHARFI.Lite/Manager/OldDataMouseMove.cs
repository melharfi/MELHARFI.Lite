using MELHARFI.Lite.Manager.Gfx;

namespace MELHARFI.Lite.Manager
{
    /// <summary>
    // stoque les donné de l'objet qui est redevenu opaque alors qu'il été moitié transparent
    // vus que le type bmp est de type reference, on peux pas garder la valeur opacity d'origine
    // pour l'appliquer sur l'objet apres MouseMove (en sortant de l'objet pour qu'il redeviens semie transparent) 
    /// </summary>
    internal class OldDataMouseMove
    {
        public Bmp Bitmap;
        public float Opacity;

        public OldDataMouseMove(Bmp bitmap, float opacity)
        {
            Bitmap = bitmap;
            Opacity = opacity;
        }
    }
}
