using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils;

namespace DSLibrary
{
    public partial class frmBase : DevExpress.XtraEditors.XtraForm
    {
        public frmBase()
        {
            InitializeComponent();

            //DevExpress.UserSkins.BonusSkins.Register();
            //DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Sharp Plus");
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (!DesignMode)
                DxControl.InitialControl(this.Controls); // e.Control.Controls);
        }

        protected override DevExpress.Skins.XtraForm.FormPainter CreateFormBorderPainter()
        {
            return new MyFormPainter(this, LookAndFeel);
        }
    }

    /// <summary>
    /// 창 제목 크기 변경
    /// </summary>
    public class MyFormPainter : FormPainter
    {
        public MyFormPainter(Control owner, ISkinProvider provider) : base(owner, provider) { }

        protected override void DrawText(DevExpress.Utils.Drawing.GraphicsCache cache)
        {
            string text = Text;

            if (text == null || text.Length == 0 || this.TextBounds.IsEmpty)
                return;

            AppearanceObject appearance = new AppearanceObject(GetDefaultAppearance());
            appearance.Font = new Font("IBM Plex Sans KR", 8.0f, FontStyle.Regular);
            appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;

            Rectangle r = RectangleHelper.GetCenterBounds(TextBounds, new Size(TextBounds.Width, appearance.CalcDefaultTextSize(cache.Graphics).Height));
            DrawTextShadow(cache, appearance, r);
            cache.DrawString(text, appearance.Font, appearance.GetForeBrush(cache), r, appearance.GetStringFormat());
        }
    }

}