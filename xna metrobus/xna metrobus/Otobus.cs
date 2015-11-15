using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using NLog;

namespace xna_metrobus
{
    class Otobus
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<Otobus> otobuslerYeniModel = new List<Otobus>();
        public static List<Otobus> otobuslerEskiModel = new List<Otobus>();

        public List<Durak> durdugumuzDuraklar = new List<Durak>();

        public Renk renk;

        string id;
        static int sonIdYeni = 0;
        static int sonIdEski = 0;
        /// <summary>
        /// Metre cinsinden konum
        /// </summary>        
        public float KonumX;
        static float durakBeklemeSuresiSn = 30;
        float hiz=1;
        static int maxHiz = 70;                    // km/saat
        static float hizlanmaIvmesi = 4f;     // km/saniye   70/15=4.666 -> 15 saniyede hız 70 olur demek.
        static float yavaslamaIvmesi = 5f;         // km/saniye   70/10=7     -> 10 saniyede hız 0 olur demek.
        public static float Uzunluk = 10.5f;
        static float modelRotation = -90.0f;
        static float scale = .8f;
        
        public bool seyehatTamamlandi = false;

        Camera camera;
        static Game game;
        bool yeniModel = false;

        DateTime seyehatBaslangicZamani;

        static Microsoft.Xna.Framework.Graphics.Model kirmiziOtobus;
        static Microsoft.Xna.Framework.Graphics.Model maviOtobus;
        static Microsoft.Xna.Framework.Graphics.Model griOtobus;
        private static float SimulasyonHizi = 1f;

        public static void initialize(Game game)
        {
            Otobus.game = game;
            SimulasyonHizi = Ayar.Default.SimulasyonHizi;
            durakBeklemeSuresiSn = Ayar.Default.DuraktaBeklemeSuresi;
            maxHiz = Ayar.Default.MaxOtobusHiziKmSaat;
            hizlanmaIvmesi = Ayar.Default.OtobusHizlanmaIvmesi;
        }

        public Otobus(Camera camera, Renk renk, float konumX, bool yeniModel)
        {
            this.renk = renk;
            this.KonumX = konumX;
            this.camera = camera;
            this.yeniModel = yeniModel;
            id = (yeniModel ? ++sonIdYeni : ++sonIdEski) + renk.ToString().Substring(0, 1);
            logger.Info(id + " otobus oluşturuldu");
            seyehatBaslangicZamani = DateTime.Now;
        }

        public static void LoadContent()
        {
            kirmiziOtobus = game.Content.Load<Model>("Models\\BUSMAN_KIRMIZI");
            maviOtobus = game.Content.Load<Model>("Models\\BUSMAN_MAVI");
            griOtobus = game.Content.Load<Model>("Models\\BUSMAN_GRI");  
        }
        
        public void Draw(GameTime gameTime)
        {
            Vector3 modelPosition;
            if(yeniModel)
                modelPosition = new Vector3(Mesafe.ToPixel(KonumX), 0.3f, Mesafe.ToPixel(-2.5f));
            else
                modelPosition = new Vector3(Mesafe.ToPixel(KonumX), 0.3f, Mesafe.ToPixel(7f));

            Model model = (renk == Renk.Kırmızı) ? kirmiziOtobus : maviOtobus;
            if (renk == Renk.Gri) model = griOtobus;

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
                        * Matrix.CreateRotationY(MathHelper.ToRadians(modelRotation))
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
        }

        float durakZamanSayaci = 0;
        DateTime sonPrint = DateTime.Now;
        DateTime hizlanmaBasi = DateTime.Now;
        Durak sonDurak;

        public void Update(GameTime gameTime)
        {
            if (sonDurak == null && Durak.duraklar.Count > 0) // duraklar oluşturulunca sondurağı tespit et.
                sonDurak = Durak.duraklar.OrderByDescending(d => d.KonumX).FirstOrDefault();

            if (!seyehatTamamlandi && Durak.duraklar.Count>0 && (sonDurak == null || KonumX > sonDurak.KonumX + 600))
            {
                seyehatTamamlandi = true;
                var sure = DateTime.Now.Subtract(seyehatBaslangicZamani);
                logger.Info(id + " seyehati tamamladı. Toplam gecen süre (dk):"+(sure.TotalMinutes*Ayar.Default.SimulasyonHizi));
            }
           

            var onumuzdekiOtobus = OndekiOtobusuGetir();
            Durak sonrakiDurak = SonrakiDurakGetir();
            bool otobusdenDolayiYavaslamali = false;
            bool duraktanDolayiYavaslamali = false;

            if (hiz > 0) // hareket halinde
            {
                if (onumuzdekiOtobus != null)
                {
                    var xPoint = onumuzdekiOtobus.KonumX - Otobus.Uzunluk;
                    var guvenliDurmaMesafesi = GuvenliDurusMesafesiHesapla(hiz);
                    if ((xPoint - KonumX) <= guvenliDurmaMesafesi)
                        otobusdenDolayiYavaslamali = true;                    
                }

                if (!otobusdenDolayiYavaslamali && sonrakiDurak != null)
                {
                    var xPoint = sonrakiDurak.KonumX;
                    var guvenliDurmaMesafesi = GuvenliDurusMesafesiHesapla(hiz);
                    duraktanDolayiYavaslamali = (xPoint - KonumX) <= guvenliDurmaMesafesi;
                }

                if (otobusdenDolayiYavaslamali || duraktanDolayiYavaslamali)
                {
                    HizAzalt(gameTime);
                }
                else
                {
                    HizArttır(gameTime);
                }
            }
            else // duruyor
            {
                if (sonrakiDurak != null && YolcuAlabilirMi(sonrakiDurak))
                {
                    if (durakZamanSayaci == 0)
                    {
                        // yolcu almaya başlıyoruz.
                        logger.Info("{0} {1} durağında yolcu almaya başlandı. Seyehat {2} saniye sürdü",id, sonrakiDurak.durakAdi, DateTime.Now.Subtract(hizlanmaBasi).TotalMilliseconds / 1000f * Ayar.Default.SimulasyonHizi);
                    }
                    // yolcu alma sürecince bekle.
                    durakZamanSayaci += gameTime.ElapsedGameTime.Milliseconds / 1000f;
                    
                    if (durakZamanSayaci > durakBeklemeSuresiSn / SimulasyonHizi)
                    {
                        durakZamanSayaci = 0;
                        HizArttır(gameTime);
                        durdugumuzDuraklar.Add(sonrakiDurak);
                        hizlanmaBasi = DateTime.Now;
                        logger.Info("{0} {1} durağından hareket ediliyor.", id, sonrakiDurak.durakAdi);
                    }
                }
                else if (onumuzdekiOtobus != null && (onumuzdekiOtobus.KonumX - Otobus.Uzunluk - KonumX) > 1)
                {
                    HizArttır(gameTime);
                }
            }

            KonumGuncelle(gameTime);

        }

        /// <summary>
        /// Belirtilen durağın sınırları dahilinde miyiz?
        /// </summary>
        /// <param name="sonrakiDurak"></param>
        /// <returns></returns>
        private bool YolcuAlabilirMi(Durak sonrakiDurak)
        {
            float otobusArkaNoktasi = KonumX-Otobus.Uzunluk;
            float durakArkaNoktasi = sonrakiDurak.KonumX-Durak.Uzunluk;
            return KonumX <= sonrakiDurak.KonumX+50 && ( otobusArkaNoktasi > durakArkaNoktasi );
        }

        private Durak SonrakiDurakGetir()
        {
            var q = Durak.duraklar                
                .Where(d => !durdugumuzDuraklar.Contains(d));

            if (yeniModel)// filter more
                q = q.Where(d => d.renk == Renk.Yesil || d.renk == renk);
                    
            return q.OrderBy(d => d.KonumX).FirstOrDefault();

        }

        private bool otobusVarmi()
        {
            var ondekiOtobus = OndekiOtobusuGetir();
            if(ondekiOtobus == null) return false;
            var bagilHiz = this.hiz - ondekiOtobus.hiz;
            if (bagilHiz <= 0) return false;
            var guvenliDurusMesafesi = GuvenliDurusMesafesiHesapla(bagilHiz);
            if ((ondekiOtobus.KonumX - Otobus.Uzunluk - this.KonumX) <= guvenliDurusMesafesi)
                return true;
            return false;

        }
        private Otobus OndekiOtobusuGetir()
        {
            var ondekiOtobus = (yeniModel ? otobuslerYeniModel : otobuslerEskiModel)
                .Where(o => o.KonumX > this.KonumX)
                .OrderBy(o => o.KonumX)
                .FirstOrDefault();
            return ondekiOtobus;
        }

        /// <summary>
        /// Durma mesafesi içinde durmamız gereken durak varsa true döner.
        /// </summary>
        /// <returns></returns>
        private bool durakVarmi()
        {
            float guvenliDurusMesafesi = GuvenliDurusMesafesi;
            var siraliDuraklar = Durak.duraklar.OrderBy(d => d.KonumX);
            foreach (var durak in siraliDuraklar)
            {
                if(yeniModel && (durak.renk!=Renk.Yesil&&durak.renk!=this.renk) 
                    || durak.KonumX<this.KonumX) continue;

                //if (Mesafe.ToMetre(durak.KonumX - this.KonumX) <= guvenliDurusMesafesi)
                //{
                //    Console.WriteLine("Bizim konum:{0} durak:{1} hız:{2} guvenliDurus:{3} ", KonumX, durak.KonumX,hiz,guvenliDurusMesafesi);
                //    return true;
                //}
                    
            }
            return false;
        }
        public float SonrakiDuragaMesafe { 
            get 
            {
                var siraliDuraklar = Durak.duraklar.OrderBy(d => d.KonumX);
                foreach (var durak in siraliDuraklar)
                {
                    if (yeniModel && (durak.renk != Renk.Yesil && durak.renk != this.renk)
                     || durak.KonumX < this.KonumX) continue;
                    
                    return durak.KonumX - this.KonumX;
                }
                return float.MaxValue;
            }
        }
        private float GuvenliDurusMesafesi
        {
            get
            {
                float guvenliDurusMesafesi = GuvenliDurusMesafesiHesapla(hiz);
                return guvenliDurusMesafesi;
            }
        }
        private float GuvenliDurusMesafesiHesapla(float hiz)
        {
            float durmaSuresiSn = hiz / yavaslamaIvmesi;
            float saniyedeAlinanYolMt = hiz * 1000 / 60 / 60;
            float guvenliDurusMesafesi = durmaSuresiSn * saniyedeAlinanYolMt / 2;
            return guvenliDurusMesafesi;
        }

        private void KonumGuncelle(GameTime gameTime)
        {
            KonumX += hiz * 1000 / 60 / 60 * GecenSureSaniye(gameTime);
        }        
        private void HizArttır(GameTime gameTime)
        {
            if (hiz < maxHiz) // hizi arttır
                hiz += hizlanmaIvmesi * GecenSureSaniye(gameTime);
        }
        private void HizAzalt(GameTime gameTime)
        {
            if (hiz > 0) // hizi arttır
                hiz -= yavaslamaIvmesi * GecenSureSaniye(gameTime);
            if (hiz <= 0) 
                hiz = 0;
            
        }
        
        private static float GecenSureSaniye(GameTime gameTime)
        {
            return gameTime.ElapsedGameTime.Milliseconds / 1000f * SimulasyonHizi;
        }
                        
    }

    public enum Renk
    {
        [Description("M")]
        Mavi,
        [Description("K")]
        Kırmızı,
        [Description("Y")]
        Yesil,
        [Description("G")]
        Gri 
    }
}
