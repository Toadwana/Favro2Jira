using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FavroToJira
{
  public class JiraComment
  {
    public string body;
    public string author;
    public DateTime created;
  }

  public class JiraIssue
  {
    public string description;
    public string status;
    public string issueType;
    public string summary;
    public string assignee;
    public string[] fixedVersions;
    public string externalId;
    public JiraComment[] comments;

    [JsonIgnore]
    public float position;
  }
}
