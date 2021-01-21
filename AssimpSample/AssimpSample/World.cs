// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;

using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;
        private AssimpScene m_arrow;


        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = -200.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private enum TextureObjects { MUD = 0, CAGE, GRASS };

        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;

        public string[] m_textureFiles = { "..//..//Textures//blato.jpg", "..//..//Textures//ograda.jpg", "..//..//Textures//trava.jpg" };

        //svetlonsi parametri
        public float ambLightR = 1f;
        public float ambLightG = 1f;
        public float ambLightB = 1f;

        //lookat

        public float eyex = 0.0f;
        public float eyey = 550.2f;
        public float eyez = 2500.0f;
        public float centerx = 0.0f;
        public float centery = 0.0f;
        public float centerz = 1.0f;
        public float upx = 0.0f;
        public float upy = 1.0f;
        public float upz = 0.5f;


        //animacija
        public bool start = false;
        public bool second = false;
        public bool arrow = false;
        public bool arrow2 = false;


        //arrow pozicija za pocetak animacije
        //gl.Translate(1500.0f, -10.0f, 50);
        public float ax = 1500.0f;
        public float ay = -10.0f;
        public float az = 50;
        public float rotate = 0;
        public float fleg = 0;
        public bool fleg2 = false;

        //Transliranje desnog zida po horizontalnoj osi
        //1000 stoji pocetno
        //minimalno je 600
        public float horizontal = 800f;

        //roracija levog zida
        public float vertical = 0.0f;


        //strela
        public float skaliranje = 50.2f;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_arrow = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\..\\..\\..\\3D Models\\Castle"), "arrow.obj", gl);
            this.m_width = width;
            this.m_height = height;




            m_textures = new uint[m_textureCount];




        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GLU_SMOOTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            LoadTexture(gl);

            m_scene.LoadScene();
            m_scene.Initialize();
            m_arrow.LoadScene();
            m_arrow.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);



            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)m_width / m_height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix

            //gl.LookAt(0.0f, 550.2f,-2500.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);
            gl.LookAt(eyex, eyey, eyez, centerx, centery, centerz, upx, upy, upz);
            SetUpLighting(gl);
            gl.PushMatrix();

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.GRASS]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawPodloga(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);


            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.MUD]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawStaza(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            DrawZamak(gl);

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.CAGE]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawLeviZid(gl);
            DrawDesniZid(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            DrawArrow(gl);


            drawText(gl);
            Resize(gl, m_width, m_height);
            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        private void DrawArrow(OpenGL gl) {
            gl.PushMatrix();
            gl.Translate(ax, ay, az);
            gl.Scale(50.2f, skaliranje, 50.2f);
            gl.Rotate(0.0f, 90f, 0f);
            m_arrow.Draw();


            gl.PopMatrix();


        }

        public void drawText(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();

            gl.Ortho2D(-28.0, 28.0, -20.0, 20.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(15f, -13f, 0f);

            string[] text = new string[]
            {
                "Predmet:Racunarska grafika", "Sk. god: 2020/21.", "Ime: Aca", "Prezime: Simic",
                "Sifra zad: 3.2"
            };

            gl.Scale(0.6f, 0.6f, 0.6f);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            for (int i = 0; i < text.Length; i++)
            {
                gl.PushMatrix();
                gl.Translate(0.0f, i * (-1.0f), 0.0f);
                gl.DrawText3D("Verdana bold", 14.0f, 1.0f, 1.0f, text[i]);
                gl.PopMatrix();
            }
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.PopMatrix();


        }




        private void DrawLeviZid(OpenGL gl) {

            gl.PushMatrix();

            gl.Rotate(vertical, 0.0f, 1.0f, 0f);
            gl.Translate(-1000f, -100f, -210.0f);
            gl.Scale(50.0f, 500.0f, 1000.0f);

            Cube c = new Cube();

            c.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

        }

        private void DrawDesniZid(OpenGL gl)
        {

            gl.PushMatrix();


            gl.Translate(horizontal, -100f, -210.0f);
            gl.Scale(50.0f, 500.0f, 1000.0f);

            Cube c = new Cube();

            c.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

        }







        private void DrawZamak(OpenGL gl) {
            gl.PushMatrix();
            gl.Translate(-50.0f, -600.0f, -600);
            m_scene.Draw();


            gl.PopMatrix();

        }


        private void DrawPodloga(OpenGL gl) {

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(20.0f, 20.0f, 20.0f);

            gl.Translate(0f, 0f, -10f);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0f, 1f, 1f);

            gl.TexCoord(0, 0);
            gl.Vertex(1500.0f, -600f, 1200.0f);
            gl.TexCoord(1, 0);
            gl.Vertex(1500.0f, -600f, -1200f);
            gl.TexCoord(1, 1);
            gl.Vertex(-1800.0f, -600f, -1200f);
            gl.TexCoord(0, 1);
            gl.Vertex(-1800.0f, -600f, 1200.0f);
            gl.End();


            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);


        }

        private void DrawStaza(OpenGL gl)
        {

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(5.0f, 5.0f, 5.0f);

            gl.Translate(0f, 0f, -10f);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            //gl.Color(1f, 0f, 0f);
            gl.TexCoord(0, 0);
            gl.Vertex(100.0f, -590f, 1200.0f);
            gl.TexCoord(1, 0);
            gl.Vertex(100.0f, -590f, -1200f);
            gl.TexCoord(1, 1);
            gl.Vertex(-180.0f, -590f, -1200f);
            gl.TexCoord(0, 1);
            gl.Vertex(-180.0f, -590f, 1200.0f);
            gl.End();


            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }



        public void SetUpLighting(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_LIGHTING);
            // gl.Translate(-1000f, -100f, -210.0f);
            float[] ambientLight = { ambLightR, ambLightG, ambLightB, 1.0f };
            float[] positon = { -2000.5f, 2000.5f, -1200.0f, 1.0f };
            float[] dl = { 0.6f, 0.6f, 0.6f, 1.0f };

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, ambientLight);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambientLight);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, dl);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);

            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, positon);


            float[] ambientLight2 = { ambLightR, ambLightG, ambLightB, 1.0f };
            float[] positon2 = { -50f, 900.0f, -400.0f, 1.0f };
            float[] dl2 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] smer = { 0.0f, -1.0f, 0.0f };

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, ambientLight2);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambientLight2);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, dl2);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);

            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, positon2);



            // automatska normalizacija nad normalama

            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.ShadeModel(OpenGL.GL_SMOOTH);


        }

        private void LoadTexture(OpenGL gl)
        {

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.GenTextures(m_textureCount, m_textures);

            for (int i = 0; i < m_textureCount; ++i)
            {

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);


                Bitmap image = new Bitmap(m_textureFiles[i]);

                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);


                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

                image.UnlockBits(imageData); image.Dispose();

            }

        }


        public void startAnimation()
        {
            eyex = -50.0f;
            eyey = -500.0f;
            eyez = -1300.0f;
            centerx = -50.0f;
            centery = -500.0f;
            centerz = 1.0f;
            upx = 0.0f;
            upy = 1.0f;
            upz = 0.5f;
            second = true;
            start = false;
        }
         public void secondAnimation()
        {
            if (eyez < 2500)
                eyez += 200.0f;
            else
            {
                second = false;
                arrow = true;
                fleg2 = true;
            }


        }

        public void moveArowAnimation()
        {
            //MessageBox.Show("Jesi usao rak te pojeo" , "Poruka", MessageBoxButton.OK);
            if (az <3580) {
                az += 300;
                fleg2 = false;
            }
            else
            {
                fleg += 1;
                fleg2 = true;
                arrow2 = false;
                arrow = true;

            }

            if(fleg == 10)
            {
                this.restartAnimation();
            }


        }
        public void arrowAnimation()
        {
            if (fleg < 10 && fleg2)
            {
                skaliranje = 30;
                ax = -50.0f;
                ay = -520.0f;
                az = 80.0f;
                arrow2 = true;
            }
        }

       



        public void restartAnimation(){
            eyex = 0.0f;
            eyey = 550.2f;
            eyez = 2500.0f;
            centerx = 0.0f;
            centery = 0.0f;
            centerz = 1.0f;
            upx = 0.0f;
            upy = 1.0f;
            upz = 0.5f;

            ax = 1500.0f;
            ay = -10.0f;
            az = 50.0f;

            start = false;
            second = false;
            arrow = false;
            arrow2 = false;
        }

        






        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
