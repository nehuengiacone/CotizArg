using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CodeDom;

namespace API
{
    class DolarApi
    {
        private string urlDolarApi = "https://dolarapi.com/v1/dolares/";

        public Moneda getRequest(string urlMoneda)
        {
            string contenido = string.Empty;

            using (var cliente = new HttpClient())
            {
                string statuscode = "";
                var watch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    cliente.DefaultRequestHeaders.Clear();

                    var response = cliente.GetAsync(urlDolarApi + urlMoneda).Result;

                    contenido = response.Content.ReadAsStringAsync().Result;
                    statuscode = response.StatusCode.ToString();
                    watch.Stop();

                    Moneda moneda = JsonSerializer.Deserialize<Moneda>(contenido);


                    if (response.IsSuccessStatusCode)
                    {
                        return moneda;
                    }


                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrio un problema con la petición a:\n{urlDolarApi + urlMoneda}\n{contenido}", $"Fallo la petición: {statuscode}", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Moneda moneda = new Moneda();
                    return moneda;
                }

            }

        }
    }

    class Moneda
    {
        public float? compra { get; set; }
        public float? venta { get; set; }
        public string casa { get; set; }
        public string moneda { get; set; }
        public string fechaActualizacion { get; set; }

        public string parseFechaActualizacion()
        {
            try
            {
                // Fecha y hora en formato UTC
                DateTime fecha = DateTime.Parse(fechaActualizacion);

                // Convertir a la zona horaria local
                DateTime tiempoLocal = fecha.ToLocalTime();

                // Formato personalizado para la cadena de fecha
                string formatoFecha = "dd/MM/yyyy, HH:mm tt";

                // Obtener la cadena de fecha en el formato deseado
                string fechaFormateada = tiempoLocal.ToString(formatoFecha);
                return fechaFormateada;
            }
            catch(Exception ex)
            {
                return "../../..";
            } 


        }
    }
}
