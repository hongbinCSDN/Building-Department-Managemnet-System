using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using System.Data.Entity;
using MWMS2.Controllers;

namespace MWMS2.Services
{
    public class SMMSCUTeamManBLService
    {
        private static volatile SMMSCUTeamManDAOService _DAO;
        private static readonly object locker = new object();
        private static SMMSCUTeamManDAOService DAO { get { if (_DAO == null) lock (locker) if (_DAO == null) _DAO = new SMMSCUTeamManDAOService(); return _DAO; } }


        public Dictionary<string, object> AjaxTeamData()
        {
            return DAO.AjaxTeamData();
        }

        public IEnumerable<object> AddressStreetName(AtcpModel model)
        {
            return DAO.StructuralAddress(
                new string[] { "ST_NAME" }, new string[] { "ST_NAME" }
                , true, null
                , true, null
                , true, null
                , true, null
                , false, model.term
                , true, null
                , null
                , null).Select(o => new { code = o["ST_NAME"] });
        }
        public IEnumerable<object> AddressStreetNumber(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["ST_NAME"])) return null;
            return DAO.StructuralAddress(
                new string[] { "STREETNO", "BUILDING" }, new string[] { "STREETNO", "BUILDING" }
                , true, null
                , true, null
                , true, null
                , false, model.term
                , true, model.atcpParameters["ST_NAME"] 
                , true, null
                , null
                , null).Select(o => new { code = o["STREETNO"], building = o["BUILDING"] });
        }
        public IEnumerable<object> AddressBuilding(AtcpModel model)
        {
            return DAO.StructuralAddress(
                new string[] { "ST_NAME", "STREETNO", "BUILDING" }, new string[] { "ST_NAME", "STREETNO", "BUILDING" }
                , true, null
                , true, null
                , false, model.term
                , true, model.atcpParameters["STREETNO"]
                , true, model.atcpParameters["ST_NAME"] 
                , true, null
                , null
                , null).Select(o => new { code = o["BUILDING"], STREETNO = o["STREETNO"], ST_NAME = o["ST_NAME"] });
        }
        public IEnumerable<object> AddressFloor(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["BUILDING"])) return null;
            return DAO.StructuralAddress(
                new string[] { "FLOOR" }, new string[] { "FLOOR" }
                , true, null
                , false, model.term
                , true, model.atcpParameters["BUILDING"] 
                , true, model.atcpParameters["STREETNO"] 
                , true, model.atcpParameters["ST_NAME"] 
                , true, null
                , null
                , null).Select(o => new { code = o["FLOOR"] });
        }
        public IEnumerable<object> AddressUnit(AtcpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.atcpParameters["BUILDING"]) || string.IsNullOrWhiteSpace(model.atcpParameters["FLOOR"])) return null;
            return DAO.StructuralAddress(
                new string[] { "UNIT" }, new string[] { "UNIT" }
                , false, model.term
                , true, model.atcpParameters["FLOOR"] 
                , true, model.atcpParameters["BUILDING"] 
                , true, model.atcpParameters["STREETNO"] 
                , true, model.atcpParameters["ST_NAME"] 
                , true, null
                , null
                , null).Select(o => new { code = o["UNIT"] });
        }




        public IEnumerable<object> Rank(AtcpModel model)
        {
            return DAO.Rank(model)
                .Select(o => new { uuid = o["UUID"], code = o["CODE"], description = o["DESCRIPTION"] });
        }
        public IEnumerable<object> Unit(AtcpModel model)
        {
            return DAO.Unit(model)
                .Select(o => new { uuid = o["UUID"], section = o["SECTION"], unit = o["UNIT"], description = o["DESCRIPTION"] });
        }
        
    }
}