using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PaintVisual
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Determinando el tamaño por defecto de la forma y
            //para dibujar en mapa de bits
            this.Width = 800;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
        }

        //Declarando variables en función a pixeles, espacios de dibujo y coordenadas
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        //Creamos una variable de la clase Pen para borrar
        Pen erase = new Pen(Color.White, 10);
        int index;
        //Creando variables nombre
        int x, y, sX, sY, cX, cY;

        //Método para enviar y recibir un punto de la imagen de paleta de colores
        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        //Método para validar el pixel anterior antes de rellenar la figura  con el nuevo color
        private void validate(Bitmap bm, Stack<Point> sp, int x, int y,Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        //Método para el relleno de las figuras
        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;
            //Obteniendo el pixel del color anterior y rellena con el nuevo color al momento
            //de ca clic en el punto, el contador del stack count >0
            //si no, si el color es igual al nuevo color entnces lo regresa
            while (pixel.Count > 0)
            {
                Point pt = pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height -
                1)
                {
                    //Primero se realizará una validación, despues rellenará el stack point
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);

                }
            }
        }

        //Creando un metodo para guardar nuestro dibujo
        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg|(*.*|";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width,
                pic.Height), bm.PixelFormat);
                //Para activar la clase ImageFormar debemos llamar al  espacio de nombres
                //System.Drawing.Imaging;
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Image Saved Successfully...");
            }
        }
        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void bnt_ellipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void btn_rec_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            //Se dibuja el indice seleccionado y despliega las posiciones actuales de dibujo si la variable paint es verdadera
            Graphics g = e.Graphics;
            if (paint)
            {
                //Copiamos las condicionales que tenemos en el evento mouse_up, lo que logramos con esto es ver en tiempo real el tamaño de la forma seleccionada
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cY, cY, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            //Reiniciando el area de dibujo
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            //Si el boton de color es presionado, entonces se abre el
           // cuadro de dialogo de color y seleccionamos un color, el cual se
            //almacena en la variable new_color, en el lapiz y en el pic
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(color_picker, e.Location);
            //Si el usuario selecciona cualquiero color de la imagen color_picker, entonces
            //ese color lo toma new_color, pen color y pic_color
            Point point1 = set_point(color_picker, e.Location);
            pic_color.BackColor =
            ((Bitmap)color_picker.Image).GetPixel(point1.X, point1.Y);
            new_color = pic_color.BackColor;
            p.Color = pic_color.BackColor;
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 7)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            //Si el mouse se presiona entonces asigna a la variable booleana Paint el valor de falso
            paint = false;
            sX = x - cX;
            sY = y - cY;
            //Se dibuja la elipse si el mouse se presiona y el valor de  paint es falso, además de tener el índice 3
            if (index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            //Se dibuja el rectángulo si el mouse esta arriba y si la variable booleana Paint es falsa y el índice es igual a 4
            if (index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            //Se dibujan lineas si el mouse esta arriba y la variable booleana esta en falso el indice es 5
            if (index == 5)
            {
                g.DrawLine(p, cY, cY, x, y);
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            //En esta parte se permite dibujar de forma libre en linea, si la variable paint está en verdadero y el index == 1, el usuario dio  clic y el mouse se mueve
            if (paint)
            {
                if (index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;

                }
                //Esta parte permite borrar al habilitar el índice 2, esta parte se implementa en el botón eraser
                if (index == 2)
                {
                    px = e.Location;

                    g.DrawLine(erase, px, py);
                    py = px;

                }
            }
            pic.Refresh();
            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        //Creamos una variable que representa un cuadro de dialogo con para poder definir
        //colores
        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            //Si el usuario da clic en el área de diibujo entonces la variable paint pasara a verdadero y asigna el clic en el punto pY
            paint = true;
            py = e.Location;
            //si el mouse se baja entonces toma las coordenadas X,Y para dibujar
            cX = e.X;
            cY = e.Y;
        }
    }
}
