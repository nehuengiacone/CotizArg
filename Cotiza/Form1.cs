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
using System.Threading;

namespace Cotiza
{
    public partial class frmPrincipal : Form
    {
        private DolarApi dolarApi;
        private Moneda dBlue;
        private Moneda dOficial;
        private Moneda dTarjeta;
        private static string ultFechaActualizada = default;
        private Thread thrActualizacion;
        private bool loop = true;

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
            string legales = $"Aviso Legal: Información de CotizArg\r\n\r\nEl sistema de cotización del dólar en Argentina proporcionado en esta aplicación tiene un carácter meramente informativo y no debe considerarse como asesoramiento financiero o una recomendación para realizar transacciones comerciales o inversiones.\r\n\r\nLa información presentada aquí se obtiene de fuentes consideradas confiables, pero no podemos garantizar su exactitud, integridad o actualidad en todo momento. Nos esforzamos por mantener la información actualizada y precisa, pero los datos pueden estar sujetos a cambios sin previo aviso.\r\n\r\nLos usuarios deben tener en cuenta que las fluctuaciones en los mercados financieros pueden ocurrir rápidamente y pueden ser influenciadas por una variedad de factores, incluyendo pero no limitado a eventos económicos, políticos o sociales, así como cambios en las políticas gubernamentales.\r\n\r\nPor lo tanto, no nos hacemos responsables por ninguna pérdida o daño directo, indirecto, incidental, consecuente o de otro tipo que pueda surgir como resultado de la confianza en la información proporcionada en este sistema de cotización del dólar en Argentina.\r\n\r\nSe recomienda encarecidamente a los usuarios que consulten a profesionales financieros calificados antes de tomar decisiones basadas en la información presentada aquí.\r\n\r\nAl utilizar este sistema de cotización del dólar en Argentina, usted acepta que lo hace bajo su propio riesgo y responsabilidad.\r\n\r\nÚltima actualización: {DateTime.Now}";
            MessageBox.Show($"GitHub del proyecto: https://github.com/nehuengiacone/CotizArg\n\nCotización:\nDatos de cotizaciones brindados por DolarApi\nhttps://dolarapi.com/docs\n\n{legales}", "Acerca de CotizArg", MessageBoxButtons.OK, MessageBoxIcon.Information);
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




        // Actualización de las tarjetas y subproceso de peticiones a DolarApi
        private void setMonedas()
        {
            //llamada a la API para obtener la informacion de cada moneda
            dBlue = dolarApi.getRequest("blue");
            dOficial = dolarApi.getRequest("oficial");
            dTarjeta = dolarApi.getRequest("tarjeta");
        }
        
        private void setCardsInfo()
        {
            //Actualizo el contenido de las tarjetas con las monedas generadas.
            //Si algo falla, la excepción mostrara información del fallo.
            try
            {
                this.Invoke(new Action(() =>
                {
                    this.lblCotiCompra1.Text = $"US$ {dOficial.compra.ToString()}";
                    this.lblCotiVenta1.Text = $"US$ {dOficial.venta.ToString()}";
                    this.lblActuaDato1.Text = dOficial.parseFechaActualizacion();
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    this.lblTitulo1.ForeColor = System.Drawing.Color.Red;
                    this.cardCotiza1.Visible = false;
                }));
            }

            try
            {
                this.Invoke(new Action(() =>
                {
                    this.lblCotiCompra2.Text = $"US$ {dBlue.compra.ToString()}";
                    this.lblCotiVenta2.Text = $"US$ {dBlue.venta.ToString()}";
                    this.lblActuaDato2.Text = dBlue.parseFechaActualizacion();
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    this.lblTitulo2.ForeColor = System.Drawing.Color.Red;
                    this.cardCotiza2.Visible = false;
                }));
            }

            try
            {
                this.Invoke(new Action(() =>
                {
                    this.lblCotiCompra3.Text = $"US$ {dTarjeta.compra.ToString()}";
                    this.lblCotiVenta3.Text = $"US$ {dTarjeta.venta.ToString()}";
                    this.lblActuaDato3.Text = dTarjeta.parseFechaActualizacion();
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    this.lblTitulo3.ForeColor = System.Drawing.Color.Red;
                    this.cardCotiza3.Visible = false;
                }));
            }
        }

        //logica del subproceso
        private async void actualizarCards()
        {
            while (loop)
            {
                //Seteo la información de DolarApi en los laberls de las tarjetas
                setMonedas();

                if (ultFechaActualizada == default)
                {
                    ultFechaActualizada = dOficial.parseFechaActualizacion();
                    //Metodo asincrono para mostrar un MessageBox en un hilo sin suspender su ejecución
                    await Task.Run(() =>
                    {
                        MessageBox.Show($"Se abrió el CotizArg.\nSe visualizará la cotización actual.\nFecha: {ultFechaActualizada}", "CotizArg: Actualización", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    });

                    setCardsInfo();
                }   
                else if (ultFechaActualizada != dOficial.parseFechaActualizacion())
                {
                    //Metodo asincrono para mostrar un MessageBox en un hilo sin suspender su ejecución
                    await Task.Run(() =>
                    {
                        MessageBox.Show($"Hay una nueva cotización.\nLa misma se modificará.\nFecha: {ultFechaActualizada}", "CotizArg: Actualización", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    });
                    
                    setCardsInfo();
                    ultFechaActualizada = dOficial.parseFechaActualizacion();
                }
                Thread.Sleep(60000);
            }
        }

        //En el formulario activo se ejecuta el subproceso de actualización de tarjetas
        private void frmPrincipal_Activated(object sender, EventArgs e)
        {
            thrActualizacion = new Thread(actualizarCards);
            thrActualizacion.Start();
        }

        //Al cerrar el formulario, se detiene el subproceso de actualización de tarjetas
        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            loop = false;
            if (thrActualizacion != null && thrActualizacion.IsAlive)
            {
                //Si el subproceso no finaliza en 1s, se Aborta agresivamente el thread
                if (!thrActualizacion.Join(100))
                {
                    thrActualizacion.Abort();
                }
            }
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
