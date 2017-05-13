using NeedsApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedsApp.Core.ViewModels
{
   public class SpotListViewModel:BaseViewModel
    {
        private IHttpService _httpService;
        public IHttpService HttpService {
            get { return _httpService; }
            set { _httpService = value; }
        }

        public SpotListViewModel(IHttpService httpService)
        {
            HttpService = httpService;
        }


    }
}
