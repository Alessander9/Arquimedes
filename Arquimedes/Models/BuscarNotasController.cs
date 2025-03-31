using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Arquimedes.Models
{
    public class BuscarNotasController : Controller
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

      



    }
}