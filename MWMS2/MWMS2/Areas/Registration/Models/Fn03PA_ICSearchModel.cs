using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn03PA_ICSearchModel: DisplayGrid
    {
        public String Year { get; set; }
        public string InterviewDate{ get; set; }
        //public Group Groupg { get; set; }
        public String Group { get; set; }
        public string Type { get; set; }


    }
    //public enum Group
    //{
    //    l,
    //    B,
    //    C,
    //    D,
    //    E,
    //    F,
    //    G,
    //    H,
    //    I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z
    //}

}