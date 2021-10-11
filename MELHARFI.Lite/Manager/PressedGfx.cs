using MELHARFI.Lite.Manager.Gfx;

namespace MELHARFI.Lite.Manager
{
    /// <summary>
    /// pour stoquer les gfx simulé en bouton lorsqu'il ont subit un changement lors de l'evenement clic, pour leurs donner leurs form d'origine si le MouseUp ne la pas fait
    /// </summary>
    internal class PressedGfx
    {
        public Bmp Bitmap;
        public Bmp PreviouseFrame;

        public PressedGfx(Bmp bitmap, Bmp previouseFrame)
        {
            Bitmap = bitmap;
            PreviouseFrame = previouseFrame;
        }
    }
}
