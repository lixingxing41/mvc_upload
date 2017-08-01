using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MVCBASE.Models;


namespace MVCBASE.ViewModels
{
    public class EmployeeListViewModel
    {
        public EmployeeSearchModel SearchParameter { get; set; }

        public IPagedList<Employees> Employees { get; set; }
        

        public int PageIndex { get; set; }

    }
}