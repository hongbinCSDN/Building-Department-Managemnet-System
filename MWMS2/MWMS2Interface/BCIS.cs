using MWMS2Interface.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2Interface
{
    class BCIS
    {
        static void Main(string[] args)
        {
            using (EntitiesProcessing db = new EntitiesProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {


                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_ADDRESS_META_DATA");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_BLK");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_BLK_SL");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_DISTRICT");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_ST_LOC");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_ST_NAME");
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE MWMS_UNIT");
                    db.SaveChanges();

                    db.Database.ExecuteSqlCommand("Insert into MWMS_ADDRESS_META_DATA (select * from V_MWMS_ADDRESS_META_DATA@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_BLK (select * from MWMS_BLK@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_BLK_SL (select * from MWMS_BLK_SL@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_DISTRICT (select * from MWMS_DISTRICT@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_ST_LOC (select * from MWMS_ST_LOC@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_ST_NAME (select * from MWMS_ST_NAME@BCISdbLink ) ");
                    db.Database.ExecuteSqlCommand("Insert into MWMS_UNIT (select * from MWMS_UNIT@BCISdbLink ) ");
                    db.SaveChanges();
                    tran.Commit();
                }
            }
         }
    }
}
