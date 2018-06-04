using System;
using System.Drawing;
using System.Windows.Forms;
using CsGL.OpenGL;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace OpenglFirst
{
	public class GlClass : OpenGLControl
	{
        public uint[] texture=new uint[5];

        public GlClass(): base()
		{
            this.KeyDown += new KeyEventHandler(OurView_OnKeyDown);
        }
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        protected override void InitGLContext() 
		{
            //  GL.glClearColor(0.5f, 0.5f, 0.5f, 0.5f);            // Gray Background
            GL.glClearColor(0f, 0f, 0f, 0f);            // Black Background
            GL.glEnable(GL.GL_DEPTH_TEST);                      // Enables Depth Testing
            GL.glGenTextures(texture.Length, this.texture);
            LoadTextures(0, Directory.GetCurrentDirectory() + "\\bad1.JPG");
            LoadTextures(1, Directory.GetCurrentDirectory() + "\\bad2.JPG");
            LoadTextures(2, Directory.GetCurrentDirectory() + "\\bad3.JPG");
            LoadTextures(3, Directory.GetCurrentDirectory() + "\\bad4.JPG");
            Calpo();
            calculatepoints();
            player.SoundLocation="music.wav";
            
            //player.Play();
            player.PlayLooping();

            //        LoadTextures(4, Directory.GetCurrentDirectory() + "\\sky-up.bmp");
            //  LoadTextures(5, Directory.GetCurrentDirectory() + "\\sky-up.bmp");
            //  LoadTextures(6, Directory.GetCurrentDirectory() + "\\sky-up.bmp");
        }

        float step = 0.5f;
        float eyeX = 0f, eyeY = 0f, eyeZ = 20f;
        float targetX = 0, targetY = 0, targetZ = 0;
        float upX = 0, upY = 1, upZ = 0;


        protected bool LoadTextures(int index,string file)
        {
            // A Bitmap is an object used to work with images defined by pixel data
            Bitmap image = null;

            try
            {
                // If the file doesn't exist or can't be found, an ArgumentException is thrown instead of
                // just returning null
                image = new Bitmap(file);
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Could not load " + file + ".  Please make sure that Data is a subfolder from where the application is running.", "Error", MessageBoxButtons.OK);
            }
            if (image != null)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                BitmapData bitmapdata;
                bitmapdata = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);


                // Create Nearest Filtered Texture
                GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[index]);

                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB, image.Width, image.Height, 0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                image.UnlockBits(bitmapdata);
                image.Dispose();
                return true;
            }
            return false;
        }

        bool d = false;
        public override void glDraw()
        {
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
            GL.glLoadIdentity();

            GLU.gluLookAt(eyeX, eyeY, eyeZ, targetX, targetY, targetZ, upX, upY, upZ);
            
            //DrawPacman           
            GL.glPushMatrix();
            GL.glTranslated(stepx-4, stepy-5, 0);
           
            drawPacMan();
            GL.glPopMatrix();
            
            //DrawBad1
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_DECAL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[1]);
            GL.glPushMatrix();
            GL.glTranslated(stepx1-2,stepy1-2.5, 0);
            drawbad();
            GL.glPopMatrix(); GL.glDisable(GL.GL_TEXTURE_2D);

            //DrawBad2
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_DECAL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[2]);
            GL.glPushMatrix();
            GL.glTranslated(stepx2-2, stepy2-2.5, 0);
            drawbad();
            GL.glPopMatrix();
            GL.glDisable(GL.GL_TEXTURE_2D);
            
            //DrawBad3
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_DECAL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[3]);
            GL.glPushMatrix();
            
            GL.glTranslated(stepx3-2, stepy3-2.5, 0);
            drawbad();
            GL.glPopMatrix();
            GL.glDisable(GL.GL_TEXTURE_2D);
            //
            ////DrawBad4
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_DECAL);
            GL.glBindTexture(GL.GL_TEXTURE_2D, this.texture[0]);
            GL.glPushMatrix();
            
            GL.glTranslated(stepx4-2, stepy4-2.5, 0);
            drawbad();
            GL.glPopMatrix();
            GL.glDisable(GL.GL_TEXTURE_2D);
    



            //drawmaze
            GL.glPushMatrix();
            GL.glTranslated(-2, -2.5, 0);
            drawMaze();
            drawpoints();
            GL.glPopMatrix();

            this.Refresh();
        }
       

       



       

        protected override void OnSizeChanged(EventArgs e)
		{
            base.OnSizeChanged(e);
            Size s = Size;
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            GL.gluPerspective(20.0f, (double)s.Width / (double)s.Height, 0.1f, 100.0f);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
        }

        protected void OurView_OnKeyDown(object Sender, KeyEventArgs kea)
        {
            //if escape was pressed exit the application
            if (kea.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

            if (kea.KeyCode == Keys.Enter) { rest(); }
            if (kea.KeyCode == Keys.W|| kea.KeyCode == Keys.Up)
            {
                //  targetZ += step;
                  left = false;
                up = true;
                down = false;
                right = false;
                start = false;
                sleft = false;
                sright = false;
                sup = false;
                sdown = false;
            }
            if (kea.KeyCode == Keys.A|| kea.KeyCode == Keys.Left)
           {
                //upY -= step;

                left = true;
                up = false;
                down = false;
                right = false;
                start = false;
                sleft = false;
                sright = false;
                sup = false;
                sdown = false;
                //stepx = stepx - 0.004f;     

            }
           if (kea.KeyCode == Keys.S || kea.KeyCode == Keys.Down)
           {
                //upZ += step;
                 left = false;
                 up = false;
                 down = true;
                 right = false;
                 start = false;
                sleft = false;
                sright = false;
                sup = false;
                sdown = false;

            }
            if (kea.KeyCode == Keys.D || kea.KeyCode==Keys.Right)
           {
                //upZ -= step;
                left = false;
                 up = false;
                down = false;
                right = true;
                start = false;
                sleft = false;
                sright = false;
                sup = false;
                sdown = false;

               
            }
            if (kea.KeyCode == Keys.E) {

                double xx = stepx;
                double yy = stepy;
                double roundedx = (float)(Math.Round((double)xx, 1));
                double roundedy = (float)(Math.Round((double)yy, 1));

                MessageBox.Show("X:"+roundedx.ToString() +"Y:"+ roundedy.ToString()); }

        }

        double stepe=0;
        double stepx = 2, stepy = 2.5;
        bool bstart = true  ;
        bool up = false, down = false, left = false, right=false;
        bool sup = false, sdown = false, sleft = false, sright = false;

        double stepx1 = 2.8, stepy1 = 2.8;
        double stepx2 = 3.25, stepy2 = 2.8;
        double stepx3 = 2.8, stepy3 = 3.2;
        double stepx4 = 3.25, stepy4 = 3.2;
        bool up1 = false, down1 = false, left1 = false, right1 = false;
        bool up2 = false, down2 = false, left2 = false, right2 = false;
        bool up3 = false, down3 = false, left3 = false, right3 = false;
        bool up4 = false, down4 = false, left4 = false, right4 = false;
        bool gameover = false;

        private void rest() {
            stepe = 0;
            stepx = 2; stepy = 2.5;
            bstart = true;
            up = false; down = false; left = false; right = false;
            sup = false; sdown = false; sleft = false; sright = false;
            stepx1 = 2.8; stepy1 = 2.8;
            stepx2 = 3.25; stepy2 = 2.8;
            stepx3 = 2.8; stepy3 = 3.2;
            stepx4 = 3.25; stepy4 = 3.2;
            up1 = false; down1 = false; left1 = false; right1 = false;
            up2 = false; down2 = false; left2 = false; right2 = false;
            up3 = false; down3 = false; left3 = false; right3 = false;
            up4 = false; down4 = false; left4 = false; right4 = false;
            gameover = false;
            start = true;
            foreach (point e in points) { e.eaten = false; }
            player.SoundLocation = "music.wav";
            player.PlayLooping();
        }
        private bool eated() { foreach (point t in points) {
                if (t.eaten == false) { return false; }
                    }
            return true;
        }
        private void gameend() { MessageBox.Show("Game Over Press Enter To restart");  }
        public override void Refresh()
        {   
            base.Refresh();
            stepe = stepe + 0.004f;
            if (!gameover)
            {    
                if (eated()) { gameover = true; player.Stop(); gameend(); }
                if (bstart)
                {
                    Random rnd = new Random();
                    int nxe = rnd.Next(1, 5);
                    if (nxe == 1) { up1 = true; left1 = false; right1 = false; down1 = false; }
                    if (nxe == 2) { up1 = false; left1 = true; right1 = false; down1 = false; }
                    if (nxe == 3) { up1 = false; left1 = false; right1 = true; down1 = false; }
                    if (nxe == 4) { up1 = false; left1 = false; right1 = false; down1 = true; }
                    nxe = rnd.Next(1, 5);
                    if (nxe == 1) { up2 = true; left2 = false; right2 = false; down2 = false; }
                    if (nxe == 2) { up2 = false; left2 = true; right2 = false; down2 = false; }
                    if (nxe == 3) { up2 = false; left2 = false; right2 = true; down2 = false; }
                    if (nxe == 4) { up2 = false; left2 = false; right2 = false; down2 = true; }
                    nxe = rnd.Next(1, 5);
                    if (nxe == 1) { up3 = true; left3 = false; right3 = false; down3 = false; }
                    if (nxe == 2) { up3 = false; left3 = true; right3 = false; down3 = false; }
                    if (nxe == 3) { up3 = false; left3 = false; right3 = true; down3 = false; }
                    if (nxe == 4) { up3 = false; left3 = false; right3 = false; down3 = true; }

                    nxe = rnd.Next(1, 5);
                    if (nxe == 1) { up4 = true; left4 = false; right4 = false; down4 = false; }
                    if (nxe == 2) { up4 = false; left4 = true; right4 = false; down4 = false; }
                    if (nxe == 3) { up4 = false; left4 = false; right4 = true; down4 = false; }
                    if (nxe == 4) { up4 = false; left4 = false; right4 = false; down4 = true; }

                    bstart = false;
                }
                if (up)
                {
                    //stepy = stepy + 0.002f;
                    // stepy = stepy + 0.1f;

                        double roundedx = (float)(Math.Round((double)stepx, 1));
                        double roundedy = (float)(Math.Round((double)stepy, 1));
                        double roundedx1 = (float)(Math.Round((double)stepx1, 1));
                        double roundedy1 = (float)(Math.Round((double)stepy1, 1));
                        double roundedx2 = (float)(Math.Round((double)stepx2, 1));
                        double roundedy2 = (float)(Math.Round((double)stepy2, 1));
                        double roundedx3 = (float)(Math.Round((double)stepx3, 1));
                        double roundedy3 = (float)(Math.Round((double)stepy3, 1));
                        double roundedx4 = (float)(Math.Round((double)stepx4, 1));
                        double roundedy4 = (float)(Math.Round((double)stepy4, 1));

                    if ((roundedx == roundedx1 && roundedy == roundedy1)|| (roundedx == roundedx2 && roundedy == roundedy2)|| (roundedx == roundedx3 && roundedy == roundedy3)|| (roundedx == roundedx4 && roundedy == roundedy4)) {  gameover = true; player.Stop();player.SoundLocation = "Gameover.wav";player.Play(); MessageBox.Show("GameOver Press Enter to Restart"); }

                    if (exsist(stepx, stepy + 0.2)) 
                    {
                        up = false;
                        sup = true;
                        stepy = stepy - 0.004f;
                    }
                    else { stepy = stepy + 0.004f; }
                    eat(stepx,stepy);
                }
                if (down)
                {
                    //    stepy = stepy - 0.002f;
                    //stepy = stepy - 0.1f;

                    double roundedx = (float)(Math.Round((double)stepx, 1));
                    double roundedy = (float)(Math.Round((double)stepy, 1));
                    double roundedx1 = (float)(Math.Round((double)stepx1, 1));
                    double roundedy1 = (float)(Math.Round((double)stepy1, 1));
                    double roundedx2 = (float)(Math.Round((double)stepx2, 1));
                    double roundedy2 = (float)(Math.Round((double)stepy2, 1));
                    double roundedx3 = (float)(Math.Round((double)stepx3, 1));
                    double roundedy3 = (float)(Math.Round((double)stepy3, 1));
                    double roundedx4 = (float)(Math.Round((double)stepx4, 1));
                    double roundedy4 = (float)(Math.Round((double)stepy4, 1));

                    if ((roundedx == roundedx1 && roundedy == roundedy1) || (roundedx == roundedx2 && roundedy == roundedy2) || (roundedx == roundedx3 && roundedy == roundedy3) || (roundedx == roundedx4 && roundedy == roundedy4)) { gameover = true;  player.Stop(); player.SoundLocation = "Gameover.wav"; player.Play(); MessageBox.Show("GameOver Press Enter to Restart"); }
                    if (exsist(stepx, stepy - 0.2))
                    {
                        down = false;
                        sdown = true;
                        stepy = stepy + 0.004f;
                    }
                    else { stepy = stepy - 0.004f; }

                    eat(stepx, stepy);
                }
                if (right)
                {

                    double roundedx = (float)(Math.Round((double)stepx, 1));
                    double roundedy = (float)(Math.Round((double)stepy, 1));
                    double roundedx1 = (float)(Math.Round((double)stepx1, 1));
                    double roundedy1 = (float)(Math.Round((double)stepy1, 1));
                    double roundedx2 = (float)(Math.Round((double)stepx2, 1));
                    double roundedy2 = (float)(Math.Round((double)stepy2, 1));
                    double roundedx3 = (float)(Math.Round((double)stepx3, 1));
                    double roundedy3 = (float)(Math.Round((double)stepy3, 1));
                    double roundedx4 = (float)(Math.Round((double)stepx4, 1));
                    double roundedy4 = (float)(Math.Round((double)stepy4, 1));

                    if ((roundedx == roundedx1 && roundedy == roundedy1) || (roundedx == roundedx2 && roundedy == roundedy2) || (roundedx == roundedx3 && roundedy == roundedy3) || (roundedx == roundedx4 && roundedy == roundedy4)) {  gameover = true; player.Stop(); player.SoundLocation = "Gameover.wav"; player.Play(); MessageBox.Show("GameOver Press Enter to Restart"); }


                    if (exsist(stepx + 0.2, stepy))
                    {
                        right = false;
                        sright = true;
                        stepx = stepx - 0.004f;
                    }
                    else { stepx = stepx + 0.004f; }

                    eat(stepx, stepy);
                }
                if (left)
                {


                    double roundedx = (float)(Math.Round((double)stepx, 1));
                    double roundedy = (float)(Math.Round((double)stepy, 1));
                    double roundedx1 = (float)(Math.Round((double)stepx1, 1));
                    double roundedy1 = (float)(Math.Round((double)stepy1, 1));
                    double roundedx2 = (float)(Math.Round((double)stepx2, 1));
                    double roundedy2 = (float)(Math.Round((double)stepy2, 1));
                    double roundedx3 = (float)(Math.Round((double)stepx3, 1));
                    double roundedy3 = (float)(Math.Round((double)stepy3, 1));
                    double roundedx4 = (float)(Math.Round((double)stepx4, 1));
                    double roundedy4 = (float)(Math.Round((double)stepy4, 1));

                    if ((roundedx == roundedx1 && roundedy == roundedy1) || (roundedx == roundedx2 && roundedy == roundedy2) || (roundedx == roundedx3 && roundedy == roundedy3) || (roundedx == roundedx4 && roundedy == roundedy4)) {  gameover = true; player.Stop(); player.SoundLocation = "Gameover.wav"; player.Play();MessageBox.Show("GameOver Press Enter to Restart");  }
                    if (exsist(stepx - 0.2, stepy))
                    {
                        left = false;
                        sleft = true;
                        stepx = stepx + 0.004f;
                    }
                    else { stepx = stepx - 0.004f; }
                    eat(stepx, stepy);

                }



                //bad1steps
                if (up1)
                {
                    //stepy = stepy + 0.002f;
                    // stepy = stepy + 0.1f;
                    if (exsist(stepx1, stepy1 + 0.2))
                    {
                        up1 = false;
                        stepy1 = stepy1 - 0.004f;
                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up1 = true; left1 = false; right1 = false; down1 = false; }
                        if (nxe == 2) { up1 = false; left1 = true; right1 = false; down1 = false; }
                        if (nxe == 3) { up1 = false; left1 = false; right1 = true; down1 = false; }
                        if (nxe == 4) { up1 = false; left1 = false; right1 = false; down1 = true; }


                    }
                    else { stepy1 = stepy1 + 0.004f; }

                }
                if (down1)
                {
                    //    stepy = stepy - 0.002f;
                    //stepy = stepy - 0.1f;

                    //10.8
                    if (exsist(stepx1, stepy1 - 0.2))
                    {
                        down1 = false;
                        stepy1 = stepy1 + 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up1 = true; left1 = false; right1 = false; down1 = false; }
                        if (nxe == 2) { up1 = false; left1 = true; right1 = false; down1 = false; }
                        if (nxe == 3) { up1 = false; left1 = false; right1 = true; down1 = false; }
                        if (nxe == 4) { up1 = false; left1 = false; right1 = false; down1 = true; }
                    }
                    else { stepy1 = stepy1 - 0.004f; }
                }
                if (right1)
                {

                    if (exsist(stepx1 + 0.2, stepy1))
                    {
                        right1 = false;
                        stepx1 = stepx1 - 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up1 = true; left1 = false; right1 = false; down1 = false; }
                        if (nxe == 2) { up1 = false; left1 = true; right1 = false; down1 = false; }
                        if (nxe == 3) { up1 = false; left1 = false; right1 = true; down1 = false; }
                        if (nxe == 4) { up1 = false; left1 = false; right1 = false; down1 = true; }
                    }
                    else { stepx1 = stepx1 + 0.004f; }

                }
                if (left1)
                {
                    if (exsist(stepx1 - 0.2, stepy1))
                    {
                        left1 = false;
                        stepx1 = stepx1 + 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up1 = true; left1 = false; right1 = false; down1 = false; }
                        if (nxe == 2) { up1 = false; left1 = true; right1 = false; down1 = false; }
                        if (nxe == 3) { up1 = false; left1 = false; right1 = true; down1 = false; }
                        if (nxe == 4) { up1 = false; left1 = false; right1 = false; down1 = true; }
                    }
                    else { stepx1 = stepx1 - 0.004f; }

                }




                //badstep2
                if (up2)
                {
                    //stepy = stepy + 0.002f;
                    // stepy = stepy + 0.1f;
                    if (exsist(stepx2, stepy2 + 0.2))
                    {
                        up2 = false;
                        stepy2 = stepy2 - 0.004f;
                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up2 = true; left2 = false; right2 = false; down2 = false; }
                        if (nxe == 2) { up2 = false; left2 = true; right2 = false; down2 = false; }
                        if (nxe == 3) { up2 = false; left2 = false; right2 = true; down2 = false; }
                        if (nxe == 4) { up2 = false; left2 = false; right2 = false; down2 = true; }

                    }
                    else { stepy2 = stepy2 + 0.004f; }

                }
                if (down2)
                {
                    //    stepy = stepy - 0.002f;
                    //stepy = stepy - 0.1f;

                    //10.8
                    if (exsist(stepx2, stepy2 - 0.2))
                    {
                        down2 = false;
                        stepy2 = stepy2 + 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up2 = true; left2 = false; right2 = false; down2 = false; }
                        if (nxe == 2) { up2 = false; left2 = true; right2 = false; down2 = false; }
                        if (nxe == 3) { up2 = false; left2 = false; right2 = true; down2 = false; }
                        if (nxe == 4) { up2 = false; left2 = false; right2 = false; down2 = true; }
                    }
                    else { stepy2 = stepy2 - 0.004f; }
                }
                if (right2)
                {

                    if (exsist(stepx2 + 0.2, stepy2))
                    {
                        right2 = false;
                        stepx2 = stepx2 - 0.004f;


                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up2 = true; left2 = false; right2 = false; down2 = false; }
                        if (nxe == 2) { up2 = false; left2 = true; right2 = false; down2 = false; }
                        if (nxe == 3) { up2 = false; left2 = false; right2 = true; down2 = false; }
                        if (nxe == 4) { up2 = false; left2 = false; right2 = false; down2 = true; }
                    }
                    else { stepx2 = stepx2 + 0.004f; }

                }
                if (left2)
                {
                    if (exsist(stepx2 - 0.2, stepy2))
                    {
                        left2 = false;
                        stepx2 = stepx2 + 0.004f;


                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up2 = true; left2 = false; right2 = false; down2 = false; }
                        if (nxe == 2) { up2 = false; left2 = true; right2 = false; down2 = false; }
                        if (nxe == 3) { up2 = false; left2 = false; right2 = true; down2 = false; }
                        if (nxe == 4) { up2 = false; left2 = false; right2 = false; down2 = true; }
                    }
                    else { stepx2 = stepx2 - 0.004f; }

                }



                //badstep3

                if (up3)
                {
                    //stepy = stepy + 0.002f;
                    // stepy = stepy + 0.1f;
                    if (exsist(stepx3, stepy3 + 0.2))
                    {
                        up3 = false;
                        stepy3 = stepy3 - 0.004f;
                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up3 = true; left3 = false; right3 = false; down3 = false; }
                        if (nxe == 2) { up3 = false; left3 = true; right3 = false; down3 = false; }
                        if (nxe == 3) { up3 = false; left3 = false; right3 = true; down3 = false; }
                        if (nxe == 4) { up3 = false; left3 = false; right3 = false; down3 = true; }

                    }
                    else { stepy3 = stepy3 + 0.004f; }

                }
                if (down3)
                {
                    //    stepy = stepy - 0.002f;
                    //stepy = stepy - 0.1f;

                    //10.8
                    if (exsist(stepx3, stepy3 - 0.2))
                    {
                        down3 = false;
                        stepy3 = stepy3 + 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up3 = true; left3 = false; right3 = false; down3 = false; }
                        if (nxe == 2) { up3 = false; left3 = true; right3 = false; down3 = false; }
                        if (nxe == 3) { up3 = false; left3 = false; right3 = true; down3 = false; }
                        if (nxe == 4) { up3 = false; left3 = false; right3 = false; down3 = true; }
                    }
                    else { stepy3 = stepy3 - 0.004f; }
                }
                if (right3)
                {

                    if (exsist(stepx3 + 0.2, stepy3))
                    {
                        right3 = false;
                        stepx3 = stepx3 - 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up3 = true; left3 = false; right3 = false; down3 = false; }
                        if (nxe == 2) { up3 = false; left3 = true; right3 = false; down3 = false; }
                        if (nxe == 3) { up3 = false; left3 = false; right3 = true; down3 = false; }
                        if (nxe == 4) { up3 = false; left3 = false; right3 = false; down3 = true; }
                    }
                    else { stepx3 = stepx3 + 0.004f; }

                }
                if (left3)
                {
                    if (exsist(stepx3 - 0.2, stepy3))
                    {
                        left3 = false;
                        stepx3 = stepx3 + 0.004f;


                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up3 = true; left3 = false; right3 = false; down3 = false; }
                        if (nxe == 2) { up3 = false; left3 = true; right3 = false; down3 = false; }
                        if (nxe == 3) { up3 = false; left3 = false; right3 = true; down3 = false; }
                        if (nxe == 4) { up3 = false; left3 = false; right3 = false; down3 = true; }
                    }
                    else { stepx3 = stepx3 - 0.004f; }

                }


                //badstep4
                if (up4)
                {
                    //stepy = stepy + 0.002f;
                    // stepy = stepy + 0.1f;
                    if (exsist(stepx4, stepy4 + 0.2))
                    {
                        up4 = false;
                        stepy4 = stepy4 - 0.004f;
                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up4 = true; left4 = false; right4 = false; down4 = false; }
                        if (nxe == 2) { up4 = false; left4 = true; right4 = false; down4 = false; }
                        if (nxe == 3) { up4 = false; left4 = false; right4 = true; down4 = false; }
                        if (nxe == 4) { up4 = false; left4 = false; right4 = false; down4 = true; }

                    }
                    else { stepy4 = stepy4 + 0.004f; }

                }
                if (down4)
                {
                    //    stepy = stepy - 0.002f;
                    //stepy = stepy - 0.1f;

                    //10.8
                    if (exsist(stepx4, stepy4 - 0.2))
                    {
                        down4 = false;
                        stepy4 = stepy4 + 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up4 = true; left4 = false; right4 = false; down4 = false; }
                        if (nxe == 2) { up4 = false; left4 = true; right4 = false; down4 = false; }
                        if (nxe == 3) { up4 = false; left4 = false; right4 = true; down4 = false; }
                        if (nxe == 4) { up4 = false; left4 = false; right4 = false; down4 = true; }
                    }
                    else { stepy4 = stepy4 - 0.004f; }
                }
                if (right4)
                {

                    if (exsist(stepx4 + 0.2, stepy4))
                    {
                        right4 = false;
                        stepx4 = stepx4 - 0.004f;

                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up4 = true; left4 = false; right4 = false; down4 = false; }
                        if (nxe == 2) { up4 = false; left4 = true; right4 = false; down4 = false; }
                        if (nxe == 3) { up4 = false; left4 = false; right4 = true; down4 = false; }
                        if (nxe == 4) { up4 = false; left4 = false; right4 = false; down4 = true; }
                    }
                    else { stepx4 = stepx4 + 0.004f; }

                }
                if (left4)
                {
                    if (exsist(stepx4 - 0.2, stepy4))
                    {
                        left4 = false;
                        stepx4 = stepx4 + 0.004f;


                        Random rnd = new Random();
                        int nxe = rnd.Next(1, 5);
                        if (nxe == 1) { up4 = true; left4 = false; right4 = false; down4 = false; }
                        if (nxe == 2) { up4 = false; left4 = true; right4 = false; down4 = false; }
                        if (nxe == 3) { up4 = false; left4 = false; right4 = true; down4 = false; }
                        if (nxe == 4) { up4 = false; left4 = false; right4 = false; down4 = true; }
                    }
                    else { stepx4 = stepx4 - 0.004f; }

                }

            }
        }
        bool start = true;
        private void drawPacMan()
        {
            GL.glColor3f(1.0f, 1.0f, 0.0f);
            GL.glBegin(GL.GL_TRIANGLE_FAN);
            //GL.glVertex2f(0, 0);
            GL.glVertex2f(2, 2.5f);
            float r = 0.2f;
            //start
            if (start)
            {
                for (float a = 0.2f; a < 1.95 * Math.PI; a += 0.2f)
                {
                    float x = (float)(Math.Cos(a) * r)+2;
                    float y = (float)(Math.Sin(a) * r)+2.5f;
                    GL.glVertex2f(x, y);
                }
               
            }

            //right
            if (right||sright)
            {
                for (float a = 0.2f; a < 1.95 * Math.PI; a += 0.2f)
                {
                    float x = (float)(Math.Cos(a) * r)+2;
                    float y = (float)(Math.Sin(a) * r)+2.5f;
                    GL.glVertex2f(x, y);
                }
            }
            //up
            if (up||sup)
            {
                for (float a = 1.7f; a < 2.4 * Math.PI; a += 0.2f)
                {
                    float x = (float)(Math.Cos(a) * r)+2;
                    float y = (float)(Math.Sin(a) * r) + 2.5f;
                    GL.glVertex2f(x, y);
                }
            }
            //left

            if (left||sleft)
            {

                for (float a = 3.4f; a < 2.9 * Math.PI; a += 0.2f)
                {
                    float x = (float)(Math.Cos(a) * r)+2;
                    float y = (float)(Math.Sin(a) * r) + 2.5f;
                    GL.glVertex2f(x, y);
                }
            }


            //Down
            if (down||sdown)
            {
                for (float a = -1.0f; a < 1.5 * Math.PI; a += 0.2f)
                {
                    float x = (float)(Math.Cos(a) * r)+2;
                    float y = (float)(Math.Sin(a) * r) + 2.5f;
                    GL.glVertex2f(x, y);
                }

            }
         

            GL.glEnd();
        }


 
        private void drawbad()
        {
            //GL.glColor4d(1.0f, 0.0f, 0.0f,0.1f);
            GL.glBegin(GL.GL_QUADS);

            GL.glTexCoord2d(0, 0);
                                   GL.glVertex2f(-0.2f, -0.2f);
            GL.glTexCoord2d(1, 0);
                                   GL.glVertex2f(0.2f, -0.2f);
            GL.glTexCoord2d(1, 1);
                                   GL.glVertex2f(0.2f, 0.2f);
            GL.glTexCoord2d(0, 1);
                                   GL.glVertex2f(-0.2f, 0.2f);
            GL.glEnd();
        }











        List<decimal> actxl = new List<decimal>();
        List<decimal> actyl = new List<decimal>();


        private void Calpo()
        {

            for (int i = 0; i < x.Length; i += 2)
            {
                decimal actx = x[i];
                decimal acty = y[i];
                decimal actx2 = x[i + 1];
                decimal acty2 = y[i + 1];
                if (actx == actx2)
                {
                    while (acty != acty2)
                    {
                        if (acty < acty2)  
                        {
                            actxl.Add(actx);
                            actyl.Add(acty);
                            acty = acty + (decimal)0.1;

                        }
                        else
                        {
                            if (acty > acty2)
                            {
                                actxl.Add(actx);
                                actyl.Add(acty2);
                                acty2 = acty2 + (decimal)0.1;
                            }
                        }
                    }
                    actxl.Add(actx);
                    actyl.Add(acty);
                }
                if (acty.ToString() == acty2.ToString())
                {
                    while (!actx.Equals(actx2))
                    {
                        if (actx < actx2)
                        {
                            actxl.Add(actx);
                            actyl.Add(acty);
                            actx = actx + (decimal)0.1;
                        }
                        else
                        {
                            if (actx > actx2)
                            {
                                actxl.Add(actx2);
                                actyl.Add(acty2);
                                actx2 = actx2 + (decimal)0.1;
                            }
                        }
                    }
                    actxl.Add(actx);
                    actyl.Add(acty);
                }
            }
        }
        private void drawMaze()
        {
            GL.glLineWidth(5f);
            GL.glBegin(GL.GL_LINES);            
            GL.glColor4f(0.0f, 1.0f, 1.0f, 1.0f);            
            for (int i = 0; i < x.Length; i++) { GL.glVertex2d((double)x[i], (double)y[i]); }
            GL.glEnd();
        }
        List<point> points = new List<point>();
        private void calculatepoints() {


            decimal y = 0;
            decimal x = 0;
            for (int i = 0; i < 27; i++)
            {
                for (decimal j=(decimal)0.3;j<(decimal)5.3;j+=(decimal)0.2) {
                    if (!exsist((double)j,(double)y))
                    {
                        points.Add(new point(j, (decimal)y, false));
                    }
                    x += (decimal)0.5f;
                }
                y+=(decimal)0.2;
            }
        }
        private void drawpoints() {


            GL.glPointSize(2f);
            GL.glColor3f(1.0f, 1.0f, 0.0f);
            GL.glBegin(GL.GL_POINTS);
            foreach (point p in points)
            {
                if (!p.eaten)
                {
                    GL.glVertex2f((float)p.x, (float)p.y);
                }
            }
            GL.glEnd();

        }
        private bool eat(double xx, double yy)
        {
            decimal roundedx = (decimal)(Math.Round((double)xx, 1));
            decimal roundedy = (decimal)(Math.Round((double)yy, 1));
            for (int i = 0; i < this.points.Count; i++) {
                if ((roundedx == points[i].x && roundedy == points[i].y)||
                    (roundedx+(decimal)0.1 == points[i].x && roundedy == points[i].y)||
                    (roundedx == points[i].x && roundedy+(decimal)0.1 == points[i].y)||
                    (roundedx + (decimal)0.1 == points[i].x && roundedy+(decimal)0.1 == points[i].y)|| 
                    (roundedx - (decimal)0.1 == points[i].x && roundedy == points[i].y) ||
                    (roundedx == points[i].x && roundedy - (decimal)0.1 == points[i].y) ||
                    (roundedx - (decimal)0.1 == points[i].x && roundedy - (decimal)0.1 == points[i].y)) { points[i].eaten = true;
                        return true; } }
            return false;
        }

        private bool exsist(double xx,double yy) {            
            decimal roundedx = (decimal)(Math.Round((double)xx, 1));
            decimal roundedy = (decimal)(Math.Round((double)yy, 1));
            for (int i = 0; i < this.actxl.Count; i++) { if (roundedx == actxl[i] && roundedy == actyl[i]) { return true; } }            
            return false; 
        }
        decimal[] x = {
        (decimal) 2.0f,
        (decimal) 2.0f,
        (decimal) 5.0f,
        (decimal) 5.5f,
        (decimal) 2.0f,
        (decimal) 2.0f,
        (decimal) 0.0f,
        (decimal) 0.5f,
        (decimal)5.5f,
        (decimal)0.0f,
        (decimal)5.5f,
        (decimal)5.5f,
        (decimal)0.0f,
        (decimal)0.0f,
        (decimal)0.0f,
        (decimal)5.5f,
        (decimal)0.0f,
        (decimal)1.5f,
        (decimal)0.5f,
        (decimal)0.5f,
        (decimal)2.0f,
        (decimal)3.0f,
        (decimal)2.5f,
        (decimal)2.5f,
        (decimal)3.5f,
        (decimal)4.5f,
        (decimal)4.0f,
        (decimal)4.0f,
        (decimal)0.5f,
        (decimal)0.5f,
        (decimal)0.0f,
        (decimal)0.5f,
        (decimal)1.0f,
        (decimal)2.0f,
        (decimal)3.0f,
        (decimal)3.0f,
        (decimal)2.5f,
        (decimal)3.5f,
        (decimal)3.5f,
        (decimal)4.0f,
        (decimal)4.5f,
        (decimal)4.5f,
        (decimal)4.5f,
        (decimal)5.0f,
        (decimal)1.5f,
        (decimal)1.5f,
        (decimal)1.0f,
        (decimal)1.5f,
        (decimal)2.5f,
        (decimal)2.5f,
        (decimal)2.5f,
        (decimal)3.5f,
        (decimal)3.5f,
        (decimal)3.5f,
        (decimal)4.0f,
        (decimal)5.0f,
        (decimal)4.5f,
        (decimal)4.5f,
        (decimal)0.5f,
        (decimal)1.5f,
        (decimal)1.0f,
        (decimal)1.0f,
        (decimal)0.5f,
        (decimal)1.5f,
        (decimal)1.0f,
        (decimal)1.0f,
        (decimal)0.5f,
        (decimal)1.5f,
        (decimal)0.5f,
        (decimal)1.5f,
        (decimal)2.5f,
        (decimal)3.5f,
        (decimal)3.0f,
        (decimal)3.0f,
        (decimal)4.0f,
        (decimal)5.0f,
        (decimal)4.5f,
        (decimal)4.5f,
        (decimal)0.5f,
        (decimal)1.5f,
        (decimal)4.0f,
        (decimal)5.0f,
        (decimal)2.5f,
        (decimal)3.5f
    };
        decimal[] y = {
       (decimal) 5.0f,
       (decimal) 5.5f,
       (decimal) 2.5f,
       (decimal) 2.5f,
       (decimal) 0.0f,
       (decimal) 0.5f,
       (decimal) 3.0f,
       (decimal) 3.0f,

      (decimal) 0.0f,
      (decimal) 0.0f,
      (decimal) 0.0f,
      (decimal) 5.5f,
      (decimal) 0.0f,
      (decimal) 5.5f,
      (decimal) 5.5f,
      (decimal) 5.5f,
      (decimal) 0.5f,
      (decimal) 0.5f,
      (decimal) 0.5f,
      (decimal) 1.0f,
      (decimal) 1.0f,
      (decimal) 1.0f,
      (decimal) 0.5f,
      (decimal) 1.0f,
      (decimal) 0.5f,
      (decimal) 0.5f,
      (decimal) 0.5f,
      (decimal) 1.0f,
      (decimal) 1.5f,
      (decimal) 2.5f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 1.5f,
      (decimal) 1.5f,
      (decimal) 1.5f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 1.5f,
      (decimal) 1.5f,
      (decimal) 1.5f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 2.0f,
      (decimal) 3.0f,
      (decimal) 2.5f,
      (decimal) 2.5f,
      (decimal) 2.5f,
      (decimal) 3.5f,
      (decimal) 2.5f,
      (decimal) 2.5f,
      (decimal) 2.5f,
      (decimal) 3.5f,
      (decimal) 3.0f,
      (decimal) 3.0f,
      (decimal) 3.0f,
      (decimal) 3.5f,
      (decimal) 3.5f,
      (decimal) 3.5f,
      (decimal) 3.5f,
      (decimal) 3.0f,
      (decimal)
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 4.5f,
      (decimal) 4.5f,
      (decimal) 4.5f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 4.5f,
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 4.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f,
      (decimal) 5.0f

    };
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
    }
	
}
