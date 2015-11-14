using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace xna_metrobus
{
    public class Durak
    {
        public static List<Durak> duraklar = new List<Durak>();

        public float KonumX;
        public static float Uzunluk = 25;
        public Renk renk;
        static Game game;

        static Microsoft.Xna.Framework.Graphics.Model kirmiziDurak;
        static Microsoft.Xna.Framework.Graphics.Model maviDurak;
        static Microsoft.Xna.Framework.Graphics.Model yesilDurak;
        static Microsoft.Xna.Framework.Graphics.Model griDurak;
        
        Vector3 modelPosition;
         

        public static void initialize(Game game)
        {
            Durak.game = game;
        }

        public Durak(float KonumX, Renk renk, string durakAdi)
        {
            this.KonumX = KonumX; 
            this.renk = renk;
            this.durakAdi = durakAdi;
            modelPosition = new Vector3(Mesafe.ToPixel(KonumX - 4), 0, Mesafe.ToPixel(+2f));
        }

        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        public static void LoadContent()
        {
            kirmiziDurak = game.Content.Load<Model>("Models\\MetroBusStationKirmizi");
            maviDurak = game.Content.Load<Model>("Models\\MetroBusStationMavi");
            yesilDurak = game.Content.Load<Model>("Models\\MetroBusStationYesil");
            griDurak = game.Content.Load<Model>("Models\\MetroBusStationGri"); 
        }

        float modelRotationX = -17;
        float modelRotationZ = +.5f;
        float scale = .4f;
        public string durakAdi;
        
        public void Draw(GameTime gameTime)
        {
            
            Model model = null;

            if (renk == Renk.Kırmızı)
                model = kirmiziDurak;
            else if (renk == Renk.Mavi)
                model = maviDurak;
            else if (renk == Renk.Gri)
                model = griDurak;
            else
                model = yesilDurak;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationY(MathHelper.ToRadians(modelRotationX))
                        * Matrix.CreateRotationZ(MathHelper.ToRadians(modelRotationZ))
                        * Matrix.CreateTranslation(modelPosition)

                         ;
                    effect.View = Camera.ActiveCamera.View;
                    effect.Projection = Camera.ActiveCamera.Projection;

                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(50, 50, 50); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(-100, 100, -100);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(50, 50, 50); // with green highlights

                    effect.DirectionalLight1.DiffuseColor = new Vector3(1, 1, 1); // a red light
                    effect.DirectionalLight1.Direction = new Vector3(100, 100, 100);  // coming along the x-axis
                    effect.DirectionalLight1.SpecularColor = new Vector3(1, 1, 1); // with green highlights

                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.8f);

                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            //DrawDurakAdi();
        }

        private void DrawDurakAdi()
        {
             
        }

        internal static void DuraklariOlustur()
        {
            TextReader reader = new StreamReader("duraklar.txt");
            string line = reader.ReadLine();
            while(line!=null){
                var bilgiler = line.Split(' ');
                int konum = Convert.ToInt32(bilgiler[0]);
                Renk renk = Renk.Gri;
                if (bilgiler[1].ToUpper() == "K")
                    renk = Renk.Kırmızı;
                if (bilgiler[1].ToUpper() == "Y")
                    renk = Renk.Yesil;
                if (bilgiler[1].ToUpper() == "M")
                    renk = Renk.Mavi;
                var isim = bilgiler[2];
                var durak = new Durak(konum, renk, isim);

                Durak.duraklar.Add(durak);
                line = reader.ReadLine();
            }  
        } 
    }
}
