using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2_Data_Conversion
{
    public class Conversion_BL
    {
        private Conversion_DA _DA;
        protected Conversion_DA DA
        {
            get
            {
                return _DA ?? (_DA = new Conversion_DA());
            }
        }
        public DataSet SelectDemo()
        {
            return DA.SelectDemo();
        }
    }
}
