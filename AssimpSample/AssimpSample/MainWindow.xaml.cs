using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.Windows.Threading;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;
        private DispatcherTimer timer;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Castle"), "Castle.3DS", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }






            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(150);
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);



        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            //pozicioniranje kamere 
            if (m_world.start)
                m_world.startAnimation();
            if(m_world.second)
                m_world.secondAnimation();
            if(m_world.arrow)
                m_world.arrowAnimation();
            if (m_world.arrow2)
                m_world.moveArowAnimation();

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.I:
                    if (m_world.RotationX - 5.0f >= -25)
                        m_world.RotationX -= 5.0f;
                    else
                        MessageBox.Show(m_world.RotationX.ToString() + " maksimalno rotirano", "GRESKA", MessageBoxButton.OK);

                    break;
                case Key.K:
                    if (m_world.RotationX < 90)
                        m_world.RotationX += 5.0f;
                    else
                        MessageBox.Show(m_world.RotationX.ToString() + " maksimalno rotirano", "GRESKA", MessageBoxButton.OK);
                    break;
                //minimlano -30 maksimalno 180
                case Key.J:
                    if (m_world.RotationY - 5.0f > -185)
                        m_world.RotationY -= 5.0f;
                    else
                        MessageBox.Show(m_world.RotationY.ToString() + " maksimalno rotirano", "GRESKA", MessageBoxButton.OK);

                    break;
                case Key.L:
                    if (m_world.RotationY < 180)
                        m_world.RotationY += 5.0f;
                    else
                        MessageBox.Show(m_world.RotationY.ToString() + " maksimalno rotirano", "GRESKA", MessageBoxButton.OK);
                    break;
                case Key.Add: m_world.SceneDistance -= 700.0f; break;
                case Key.Subtract: m_world.SceneDistance += 700.0f; break;
                case Key.V:
                    //ovdde stavis na true sta prvo ide za animaciju
                    m_world.start = true;
                    
                    break;
                case Key.X:
                    m_world.restartAnimation();
                    
                    break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
            }
        }

        private void moveRight(object sender, RoutedEventArgs e)
        {

            
            if (m_world.horizontal < 1000)
            { 
                m_world.horizontal += 100.0f;

                right.Text = m_world.horizontal.ToString() + " ";
            } else
                MessageBox.Show(m_world.horizontal.ToString() + " je poemreno. To je maksimalno moguce", "GRESKA", MessageBoxButton.OK);

        }

        private void moveLeft(object sender, RoutedEventArgs e)
        {
            if (m_world.horizontal - 100.0 >= 600)
            {
                m_world.horizontal -= 100.0f;

                right.Text = m_world.horizontal.ToString() + " ";
            }
            else
                MessageBox.Show(m_world.horizontal.ToString() + " je poemreno. To je minimalno moguce", "GRESKA", MessageBoxButton.OK);



        }

        private void rotateRight(object sender, RoutedEventArgs e)
        {
            if (m_world.vertical < 90.0)
            {
                m_world.vertical += 5.0f;

                rotate.Text = m_world.vertical.ToString() + " ";
            }
            else
                MessageBox.Show(m_world.vertical.ToString() + " je rotirano. To je maksimalno moguce", "GRESKA", MessageBoxButton.OK);


        }

        private void rotateLeft(object sender, RoutedEventArgs e)
        {
            if (m_world.vertical -5.0f >= 0.0)
            {
                m_world.vertical -= 5.0f;

                rotate.Text = m_world.vertical.ToString() + " ";
            }
            else
                MessageBox.Show(m_world.vertical.ToString() + " je rotirano. To je minimalno moguce", "GRESKA", MessageBoxButton.OK);


        }

        private void scalePlus(object sender, RoutedEventArgs e)
        {
            if (m_world.skaliranje  < 50.2)
            {
                m_world.skaliranje += 5.0f;

                skaliranje.Text = m_world.skaliranje.ToString() + " ";
            }
            else
                MessageBox.Show(m_world.skaliranje.ToString() + " je skaliran. To je maksimalno moguce", "GRESKA", MessageBoxButton.OK);

        }

        private void scaleMinus(object sender, RoutedEventArgs e)
        {
            if (m_world.skaliranje - 5.0f > 10.2)
            {
                m_world.skaliranje -= 5.0f;

                skaliranje.Text = m_world.skaliranje.ToString() + " ";
            }
            else
                MessageBox.Show(m_world.skaliranje.ToString() + " je skaliran. To je minimalno moguce", "GRESKA", MessageBoxButton.OK);

        }
    }
}
