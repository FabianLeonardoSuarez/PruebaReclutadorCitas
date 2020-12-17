using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace PruebaReclutadorCitas.Controllers
{
    public class HomeController : Controller
    {
        private ReclutadorCitasEntities db = new ReclutadorCitasEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ConsultarCandidatos()
        {
            var ListaCitas = db.Citas.ToList();
            string htmlCandidatos = "<table style='width:100%;'>";

            var ListaTipoCitas = db.TipoCitas.ToList();
            string htmlListaTipoCita = "";
            for (int i = 0; i < ListaTipoCitas.Count; i++)
            {
                htmlListaTipoCita += "<option value=\"" + ListaTipoCitas[i].IdTipoCita + "\">" + ListaTipoCitas[i].NombreTipo + "</option>";
            }

            for (var i = 0; i < ListaCitas.Count; i++)
            {
                var url = $"http://jsonplaceholder.typicode.com/users?id=" + ListaCitas[i].IdUser;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                ObjUsuario objUsuarioWs;
                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream strReader = response.GetResponseStream())
                        {
                            using (StreamReader objReader = new StreamReader(strReader))
                            {
                                string responseBody = objReader.ReadToEnd();
                                objUsuarioWs = JsonConvert.DeserializeObject<ObjUsuario>(responseBody);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    return Json(String.Format("'Success':'false','Error':'Ha habido un error al consultar el WS.'"));
                }

                htmlCandidatos += "<tr><td><table style='border-style: solid;width:100%;'><tr><td style='width:50% '><table><tr><td>";
                htmlCandidatos += "<label for='u" + ListaCitas[i].IdCita + "'> Nombres: "
                + objUsuarioWs.Property1[0].name + ", Email: " + objUsuarioWs.Property1[0].email + ", Dirección: "
                + objUsuarioWs.Property1[0].address.ToString().Replace("{", "").Replace("}", "").Replace("\"", "") + "</label>";
                htmlCandidatos += "</td></tr></table></td><td><table><tr><td>";
                htmlCandidatos += "<label for='fl" + ListaCitas[i].IdCita
                + "'>Fecha : </label><input type='text' id='f" + ListaCitas[i].IdCita
                + "' name='f" + ListaCitas[i].IdCita + "' value='2020/01/01'>";
                htmlCandidatos += "</td></tr><tr><td>";
                htmlCandidatos += "<label for='hl" + ListaCitas[i].IdCita
                + "'>Hora : </label><input type='text' id='h" + ListaCitas[i].IdCita
                + "' name='h" + ListaCitas[i].IdCita + "' value='00:00'>";
                htmlCandidatos += "</td></tr><tr><td>";

                htmlCandidatos += "<label for='tl" + ListaCitas[i].IdCita + "'>Tipo Cita : </label>";

                htmlCandidatos += "<select id='t" + ListaCitas[i].IdCita + "' name='t" + ListaCitas[i].IdCita + "'>";
                htmlCandidatos += htmlListaTipoCita;
                htmlCandidatos += "</select>";

                htmlCandidatos += "</td></tr></table></td></tr></table></td></tr>";
            }
            htmlCandidatos += "</table>";

            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(htmlCandidatos);
            htmlCandidatos = System.Convert.ToBase64String(bytes);

            ViewBag.htmlCandidatos = htmlCandidatos;

            return View();
        }

        public ActionResult ProgramarCita()
        {
            //IEnumerable<TipoCita> ListaTipoCitas = from s in db.TipoCitas select s;

            var ListaTipoCitas = db.TipoCitas.ToList();
            string htmlListaTipoCita = "";
            for (int i = 0; i < ListaTipoCitas.Count; i++)
            {
                htmlListaTipoCita += "<option value=\"" + ListaTipoCitas[i].IdTipoCita + "\">" + ListaTipoCitas[i].NombreTipo + "</option>";
            }

            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(htmlListaTipoCita);
            htmlListaTipoCita = System.Convert.ToBase64String(bytes);

            ViewBag.htmlListaTipoCita = htmlListaTipoCita;

            //var data = from s in db.Herramientas select s;

            //string query = "SELECT IdHerramienta, NombreHerramienta FROM Herramientas";

            //IEnumerable<Herramienta> data = db.Database.SqlQuery<Herramienta>(query);

            //query = "SELECT * FROM Tecnologias ";

            //IEnumerable<Tecnologia> ListadeCheck = db.Database.SqlQuery<Tecnologia>(query);

            //Tuple<IEnumerable<Herramienta>, IEnumerable<Tecnologia>> ElementoInicial = new Tuple<IEnumerable<Herramienta>, IEnumerable<Tecnologia>>(data, ListadeCheck);
            //return View(data.ToList());
            return View();
        }

        [HttpPost]
        public JsonResult GuardarCitas(List<ObjCita> ListaCitas)
        {
            try
            {
                foreach (var Cita in ListaCitas)
                {
                    Cita objGCita = new Cita();
                    objGCita.IdTipoCita = Cita.IdTipoCita;
                    objGCita.IdUsuarioCreacion = Cita.IdUser;
                    objGCita.Fecha = DateTime.Parse(Cita.Fecha);
                    objGCita.Hora = DateTime.Parse(Cita.Hora);
                    objGCita.IdReclutador = 1;
                    objGCita.IdUsuarioCreacion = 1;
                    objGCita.FechaCreacion = DateTime.Now;
                    db.Citas.Add(objGCita);
                    db.SaveChanges();
                }

                return Json(String.Format("'Success':'true','Error':''"));
            }
            catch (Exception ex)
            {

                return Json(String.Format("'Success':'false','Error':'" + ex.Message + "'"));
            }
        }

        [HttpPost]
        public JsonResult ResultadoWS(int EsPar)
        {
            var url = $"http://jsonplaceholder.typicode.com/users";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return Json(String.Format("'Success':'false','Error':'Ha habido un error al consultar el WS.'"));
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                            return Json(responseBody);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                return Json(String.Format("'Success':'false','Error':'Ha habido un error al consultar el WS.'"));
            }
        }
    }


    public class ObjUsuario
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public Company company { get; set; }
    }

    public class Address
    {
        public string street { get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public Geo geo { get; set; }
    }

    public class Geo
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
        public string catchPhrase { get; set; }
        public string bs { get; set; }
    }

    //public class ObjUsuario
    //{
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string username { get; set; }
    //    public string email { get; set; }
    //    public string address { get; set; }
    //    public string phone { get; set; }
    //    public string website { get; set; }
    //    public string company { get; set; }
    //}

    public class ObjCita
    {
        public int IdTipoCita
        {
            get;
            set;
        }
        public int IdUser
        {
            get;
            set;
        }
        public string Fecha
        {
            get;
            set;
        }
        public string Hora
        {
            get;
            set;
        }
    }
}