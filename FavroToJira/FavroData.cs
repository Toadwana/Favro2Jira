using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FavroToJira
{
  public class FavroData
  {
    public class FavroUser
    {
      public string userId;
      public string name;
      public string email;
    }

    public class FavroComment
    {
      public string commentId;
      public string cardCommonId;
      public string organizationId;
      public string userId;
      public string comment;
      public DateTime created;
    }

    public class FavroTag
    {
      public string tagId;
      public string name;
      public string color;
      public string organizationId;
    }

    public class FavroCard
    {
      public string cardId;
      public string cardCommonId;
      public string organizationId;
      public bool archived;
      public float position;
      public string name;
      public string widgetCommonId;
      public string columnId;
      public bool isLane;
      public string parentCardId;
      public string detailedDescription;
      public string[] tags;
      public int sequentialId;
      public int numComments;
      public int tasksTotal;
      public int tasksDone;
      public string milestone = "";
    }

    public IList<FavroUser> users = new List<FavroUser>();
    public IList<FavroComment> comments = new List<FavroComment>();
    public IList<FavroTag> tags = new List<FavroTag>();
    public IList<FavroCard> cards = new List<FavroCard>();

    public FavroData()
    {
      GetUsers();
      GetComments();
      GetTags();

      GetCards("Milestone 1");
      GetCards("Milestone 2");
      GetCards("Backlog");

      Console.ReadLine();
    }

    private void GetUsers()
    {
      string usersFile = GetFile("..\\..\\..\\Users.txt");

      JObject userObjects = JObject.Parse(usersFile);
      IList<JToken> userTokens = userObjects["entities"].Children().ToList();

      foreach (JToken userToken in userTokens)
      {
        // JToken.ToObject is a helper method that uses JsonSerializer internally
        FavroUser user = userToken.ToObject<FavroUser>();
        users.Add(user);

        Console.WriteLine(user.name);
        //Console.WriteLine(user.userId);
        //Console.WriteLine(user.email);
      }
    }

    private void GetComments()
    {
      string commentsFile = GetFile("..\\..\\..\\Comments.txt");
      string[] commentStrings = commentsFile.Split(new string[1] { "{\n  \"entities" }, StringSplitOptions.RemoveEmptyEntries);

      foreach (string commentString in commentStrings)
      {
        string fixedCommentString = "{\n  \"entities" + commentString;
        JObject commentsObjects = JObject.Parse(fixedCommentString);
        IList<JToken> commentsTokens = commentsObjects["entities"].Children().ToList();

        foreach (JToken commentToken in commentsTokens)
        {
          // JToken.ToObject is a helper method that uses JsonSerializer internally
          FavroComment comment = commentToken.ToObject<FavroComment>();
          comments.Add(comment);

          Console.WriteLine(comment.created);
        }
      }
    }

    private void GetTags()
    {
      string tagsFile = GetFile("..\\..\\..\\Tags.txt");

      JObject tagObjects = JObject.Parse(tagsFile);
      IList<JToken> tagTokens = tagObjects["entities"].Children().ToList();

      foreach (JToken tagToken in tagTokens)
      {
        // JToken.ToObject is a helper method that uses JsonSerializer internally
        FavroTag tag = tagToken.ToObject<FavroTag>();
        tags.Add(tag);

        Console.WriteLine(tag.name);
      }
    }

    private void GetCards(string milestoneName)
    {
      string cardsFile = GetFile("..\\..\\..\\" + milestoneName.Replace(" ", "") + ".txt");

      JObject cardObjects = JObject.Parse(cardsFile);
      IList<JToken> cardTokens = cardObjects["entities"].Children().ToList();

      foreach (JToken cardToken in cardTokens)
      {
        // JToken.ToObject is a helper method that uses JsonSerializer internally
        FavroCard card = cardToken.ToObject<FavroCard>();
        if (!milestoneName.Equals("Backlog"))
        {
          card.milestone = milestoneName;
        }
        cards.Add(card);

        Console.WriteLine(card.name);
      }
    }

    private string GetFile(string fileName)
    {
      string content = "";

      try
      {
        // Open the text file using a stream reader.
        using (StreamReader sr = new StreamReader(fileName))
        {
          // Read the stream to a string, and write the string to the console.
          content = sr.ReadToEnd();
          //Console.WriteLine(content);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
      }

      return content;
    }
  }
}
