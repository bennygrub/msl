using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    public interface IPicRepository
    {
        void Add(ISocialData data);

        bool IsNew(long id);
    }
}
