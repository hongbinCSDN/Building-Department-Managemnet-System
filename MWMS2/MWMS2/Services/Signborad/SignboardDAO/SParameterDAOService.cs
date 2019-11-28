using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SParameterDAOService : BaseDAOService
    {
        public List<B_S_PARAMETER> FindByProperty()
        {
            string queryString = @"SELECT * FROM B_S_PARAMETER";
            return GetObjectData<B_S_PARAMETER>(queryString).ToList();
        }
    }
}