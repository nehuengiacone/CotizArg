using Cotiza.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using API;

namespace Cotiza
{
    public partial class frmPrincipal : Form
    {
        private DolarApi dolarApi;

        public frmPrincipal()
        {
            dolarApi = new DolarApi();
            InitializeComponent();
        }

        //cierre del formulario
        private void btnCloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //minimizar formulario
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //AboutMe
        private void btmAboutMe_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GitHub del proyecto: https://github.com/nehuengiacone/CotizArg\n\nCotización:\nDatos de cotizaciones brindados por DolarApi\nhttps://dolarapi.com/docs", "Acerca de CotizArg", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //eventos de transicion de btnCloseWindow
        private void btnCloseWindow_MouseEnter(object sender, EventArgs e)
        {
            btnCloseWindow.BackgroundImage = global::Cotiza.Properties.Resources.delete_remove_close_icon_181533;
        }

        private void btnCloseWindow_MouseLeave(object sender, EventArgs e)
        {
            btnCloseWindow.BackgroundImage = global::Cotiza.Properties.Resources.closewindowapplication_cerca_ventan_2874;
        }




        //eventos de transicion de btnAboutMe
        private void btnAboutMe_MouseEnter(object sender, EventArgs e)
        {
            btnAboutMe.BackgroundImage = global::Cotiza.Properties.Resources.acerca_de;
        }

        private void btnAboutMe_MouseLeave(object sender, EventArgs e)
        {
            btnAboutMe.BackgroundImage = global::Cotiza.Properties.Resources.info;
        }




        //eventos de transicion de btnMinimizar
        private void btnMinimizar_MouseLeave(object sender, EventArgs e)
        {
            btnMinimizar.BackgroundImage = global::Cotiza.Properties.Resources.minimizar;
        }

        private void btnMinimizar_MouseEnter(object sender, EventArgs e)
        {
            btnMinimizar.BackgroundImage = global::Cotiza.Properties.Resources.minimizar_hover;
        }




        //cargas de propiedades al iniciar el formulario
        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            //redondeo de esquinas de formulario principal y tarjetas
            formPrincipal_Paint();
            cardCotiza_Paint(cardCotiza1);
            cardCotiza_Paint(cardCotiza2);
            cardCotiza_Paint(cardCotiza3);

            //llamada a la API para obtener la informacion de cada moneda
            Moneda dBlue = dolarApi.getRequest("blue");
            Moneda dOficial = dolarApi.getRequest("oficial");
            Moneda dTarjeta = dolarApi.getRequest("tarjeta");

            //Actualizo el contenido de las tarjetas con las monedas generadas.
            //Si algo falla, la excepción mostrara información del fallo.
            try
            {
                this.lblCotiCompra1.Text = $"US$ {dOficial.compra.ToString()}";
                this.lblCotiVenta1.Text = $"US$ {dOficial.venta.ToString()}";
                this.lblActuaDato1.Text = dOficial.parseFechaActualizacion();
            }
            catch (Exception ex)
            {
                this.lblTitulo1.ForeColor = System.Drawing.Color.Red;
                this.cardCotiza1.Visible = false;
            }

            try
            {
                this.lblCotiCompra2.Text = $"US$ {dBlue.compra.ToString()}";
                this.lblCotiVenta2.Text = $"US$ {dBlue.venta.ToString()}";
                this.lblActuaDato2.Text = dBlue.parseFechaActualizacion();
            }
            catch(Exception ex)
            {
                this.lblTitulo2.ForeColor = System.Drawing.Color.Red;
                this.cardCotiza2.Visible = false;
            }

            try
            {
                this.lblCotiCompra3.Text = $"US$ {dTarjeta.compra.ToString()}";
                this.lblCotiVenta3.Text = $"US$ {dTarjeta.venta.ToString()}";
                this.lblActuaDato3.Text = dTarjeta.parseFechaActualizacion();
            }
            catch(Exception ex)
            {
                this.lblTitulo3.ForeColor = System.Drawing.Color.Red;
                this.cardCotiza3.Visible = false;
            }

        }

        private void formPrincipal_Paint()
        {
            ////redondear las puntas del formulario 
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 15; //adjust the value to change the roundness of the corners

            ////create a rounded rectangle path using the form's size and radius
            path.AddArc(0, 0, radius, radius, 180, 90); //top-left corner
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90); //top-right corner
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90); //bottom-right corner
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90); //bottom-left corner

            ////create a region with the rounded rectangle path and apply it to the form
            this.Region = new Region(path);
        }

        private void cardCotiza_Paint(Panel card)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 15; //adjust the value to change the roundness of the corners

            //create a rounded rectangle path using the form's size and radius
            path.AddArc(0, 0, radius, radius, 180, 90); //top-left corner
            path.AddArc(card.Width - radius, 0, radius, radius, 270, 90); //top-right corner
            path.AddArc(card.Width - radius, card.Height - radius, radius, radius, 0, 90); //bottom-right corner
            path.AddArc(0, card.Height - radius, radius, radius, 90, 90); //bottom-left corner

            //create a region with the rounded rectangle path and apply it to the form
            card.Region = new Region(path);
        }



        //Evento de expandir tarjetas
        string nombreInvocador = "";    

        // Evento para label de titulos de tarjetas
        private void lblTitulos_Click(object sender, EventArgs e)
        {
            Label lblInvocador = (Label)sender;
            serDetectarInvocador(ref nombreInvocador, lblInvocador.Name);
            timerCards.Start();
        }
        
        // Detección del control que ejecutó el evento
        private void serDetectarInvocador(ref string lblInvocador, string nameInvocador)
        {
            lblInvocador = nameInvocador;
        }

        // Bifurcación para afectar una determinada tarjeta según control invocador
        private void timerCards_Tick(object sender, EventArgs e)
        {
            switch (nombreInvocador)
            {
                case "lblTitulo1":
                    expandCards(cardCotiza1);
                    break;
                case "lblTitulo2":
                    expandCards(cardCotiza2);
                    break;
                case "lblTitulo3":
                    expandCards(cardCotiza3);
                    break;
            }
        }

        // Logica de expansión de las tarjetas
        bool[] cardExpand = {true, true, true};
        private void expandCards(Panel card)
        {
            if (cardExpand[0] && card.Name=="cardCotiza1")
            {
                card.Height -= 10;

                if (card.Height == card.MinimumSize.Height)
                {
                    cardExpand[0] = false;
                    timerCards.Stop();
                }
            }
            else if(cardExpand[1] && card.Name == "cardCotiza2")
            {
                card.Height -= 10;

                if (card.Height == card.MinimumSize.Height)
                {
                    cardExpand[1] = false;
                    timerCards.Stop();
                }
            }
            else if (cardExpand[1] && card.Name == "cardCotiza3")
            {
                card.Height -= 10;

                if (card.Height == card.MinimumSize.Height)
                {
                    cardExpand[1] = false;
                    timerCards.Stop();
                }
            }
            else
            {
                if(!cardExpand[0] && card.Name == "cardCotiza1")
                {
                    card.Height += 10;

                    if (card.Height == card.MaximumSize.Height)
                    {
                        cardExpand[0] = true;
                        timerCards.Stop();
                    }
                }
                else if(!cardExpand[1] && card.Name == "cardCotiza2")
                {
                    card.Height += 10;

                    if (card.Height == card.MaximumSize.Height)
                    {
                        cardExpand[1] = true;
                        timerCards.Stop();
                    }
                }
                else if (!cardExpand[1] && card.Name == "cardCotiza3")
                {
                    card.Height += 10;

                    if (card.Height == card.MaximumSize.Height)
                    {
                        cardExpand[1] = true;
                        timerCards.Stop();
                    }
                }

            }
        }




        // Evento de arrastre del formulario
        int m, mx, my;
        private void barControl_MouseDown(object sender, MouseEventArgs e)
        {
            //indico con 1 que el mouse se poso en la barra y seteo las coordenadas con las del evento
            m = 1;
            mx = e.X;
            my = e.Y;
        }



        private void barControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (m == 1)
            {
                //muevo la ubicacion del formulario por las coordenadas enviadas
                //mouseposition refiere a la posicion del mouse en movimiento
                this.SetDesktopLocation(MousePosition.X - mx, MousePosition.Y - my);   
            }
        }

        private void barControl_MouseUp(object sender, MouseEventArgs e)
        {
            m = 0;
        }
    }
}
