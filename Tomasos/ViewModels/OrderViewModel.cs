using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tomasos.Models;

namespace Tomasos.ViewModels
{
    public class ViewModelOrder
    {
        public List<Matratt> DishList { get; set; }
        public List<MatrattTyp> CategoryList { get; set; }
        public Matratt Order { get; set; }
    }
}