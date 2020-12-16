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
        public JsonResult GuardarCitas(List<Cita> ListaCitas)
        {
            try
            {
                foreach (var Cita in ListaCitas)
                {
                    db.Citas.Add(Cita);
                    db.SaveChanges();
                }

                return Json(String.Format("'Success':'true','Error':''"));
            }
            catch (Exception ex)
            {

                return Json(String.Format("'Success':'false','Error':'" + ex.Message +"'"));
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
    public class JsonEntrada
    {
        public int EsPar { get; set; }
    }

    public class CheckModel
    {
        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool Checked
        {
            get;
            set;
        }
    }
}