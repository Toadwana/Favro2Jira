using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FavroToJira
{
  public class JiraData
  {
    public class JiraUser
    {
      public string name;
      public bool active;
      public string[] groups;
      public string email;
      public string fullname;
    }

    public class JiraVersion
    {
      public string name;
    }

    public class JiraProject
    {
      public string name;
      public string key;
      public IList<JiraVersion> versions = new List<JiraVersion>();
      public IList<JiraIssue> issues = null;
    }

    public class JiraContentList
    {
      public IList<JiraUser> users = new List<JiraUser>();
      public IList<JiraProject> projects = new List<JiraProject>();
    }

    private JiraContentList contentList = new JiraContentList();

    public JiraData(FavroData favroData, string projectName, string projectKey)
    {
      // users
      foreach (FavroData.FavroUser user in favroData.users)
      {
        contentList.users.Add(new JiraUser() { name = user.name.Split(' ')[0], active = true, groups = new string[0], email = user.email, fullname = user.name });
      }

      // project
      contentList.projects.Add(new JiraProject() { name = projectName, key = projectKey });

      // versions
      contentList.projects[0].versions.Add(new JiraVersion() { name = "Milestone 1" });
      contentList.projects[0].versions.Add(new JiraVersion() { name = "Milestone 2" });

      // issues
      GetIssues(favroData);

      // write json file
      string json = JsonConvert.SerializeObject(contentList, Formatting.Indented);

      using (StreamWriter sw = new StreamWriter("..\\..\\..\\Jira.json"))
      {
        sw.Write(json);
      }
    }

    private void GetIssues(FavroData favroData)
    {
      IList<JiraIssue> issues = new List<JiraIssue>();

      foreach (FavroData.FavroCard favroCard in favroData.cards)
      {
        issues.Add(new JiraIssue()
        {
          description = favroCard.detailedDescription,
          status = "To Do",
          issueType = favroData.tags.Where(t => favroCard.tags.Contains(t.tagId)).Select(t => t.name).FirstOrDefault(),
          summary = favroCard.name,
          assignee = "Ingo",
          fixedVersions = string.IsNullOrEmpty(favroCard.milestone) ? new string[0] : new string[1] { favroCard.milestone },
          externalId = favroCard.sequentialId.ToString(),
          position = favroCard.position,
        });

        List<FavroData.FavroComment> comments = favroData.comments.Where(c => c.cardCommonId == favroCard.cardCommonId).ToList();
        if (comments.Count > 0)
        {
          List<JiraComment> jiraComments = new List<JiraComment>();
          foreach (FavroData.FavroComment favroComment in comments)
          {
            jiraComments.Add(new JiraComment() { body = favroComment.comment, author = "MyName", created = new DateTime(favroComment.created.Year, favroComment.created.Month, favroComment.created.Day, favroComment.created.Hour, favroComment.created.Minute, favroComment.created.Second, DateTimeKind.Utc) });
          }

          issues.Last().comments = jiraComments.ToArray();
        }
        else
        {
          issues.Last().comments = new JiraComment[0];
        }
      }

      contentList.projects[0].issues = issues.OrderBy(p => p.position).ToList();
    }
  }
}
