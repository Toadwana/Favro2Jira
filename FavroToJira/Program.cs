using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavroToJira
{
  class Program
  {
    static void Main(string[] args)
    {
      FavroData favro = new FavroData();
      JiraData jira = new JiraData(favro, "MyProject", "MP");
    }
  }
}
