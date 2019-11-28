using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using MWMS2.Dao;

namespace MWMS2.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using MWMS2.Dao;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<WL>("WLs");
    builder.EntitySet<WL_FILE_PATH>("WL_FILE_PATH"); 
    builder.EntitySet<WL_TYPE_OF_OFFENSE>("WL_TYPE_OF_OFFENSE"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class WLsController : ODataController
    {
        private Entities db = new Entities();


        // GET: odata/WLs
        [EnableQuery]
        public IQueryable<WL> GetWLs()
        {
            return db.WL;
        }

        // GET: odata/WLs(5)
        [EnableQuery]
        public SingleResult<WL> GetWL([FromODataUri] string key)
        {
            return SingleResult.Create(db.WL.Where(wL => wL.UUID == key));
        }

        // PUT: odata/WLs(5)
        public IHttpActionResult Put([FromODataUri] string key, Delta<WL> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WL wL = db.WL.Find(key);
            if (wL == null)
            {
                return NotFound();
            }

            patch.Put(wL);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WLExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(wL);
        }

        // POST: odata/WLs
        public IHttpActionResult Post(WL wL)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WL.Add(wL);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (WLExists(wL.UUID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(wL);
        }

        // PATCH: odata/WLs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] string key, Delta<WL> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WL wL = db.WL.Find(key);
            if (wL == null)
            {
                return NotFound();
            }

            patch.Patch(wL);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WLExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(wL);
        }

        // DELETE: odata/WLs(5)
        public IHttpActionResult Delete([FromODataUri] string key)
        {
            WL wL = db.WL.Find(key);
            if (wL == null)
            {
                return NotFound();
            }

            db.WL.Remove(wL);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WLExists(string key)
        {
            return db.WL.Count(e => e.UUID == key) > 0;
        }
    }
}
