using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BreadBoards.Core.Web.Pages {
    public class AboutModel : PageModel {
        public string Message { set; get; }

        public void OnGet() {
            this.Message = "Your application description page.";
        }
    }
}
