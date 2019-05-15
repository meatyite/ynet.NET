using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace ynet
{
    public class Article
    {
        public string Article_url;
        public string Article_id;
        public string Article_uri;
        public Article(string article_url)
        {
            Uri parsed_url = new Uri(article_url);
            this.Article_id = Path.GetFileName(parsed_url.AbsolutePath).Split(',')[2];
            this.Article_uri = parsed_url.Scheme + "://" + parsed_url.Host + "/";
        }

        public Comment[] GetComments()
        {
            List<Comment> comments = new List<Comment>();

            string raw_json = new WebClient().DownloadString(this.Article_uri + 
                "Ext/Comp/ArticleLayout/Proc/ShowTalkBacksAjax/v2/0,12990," + 
                this.Article_id + "-desc-68-0-1,00.html");
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            dynamic comment_json = jsonSerializer.Deserialize<dynamic>(raw_json);
            foreach (dynamic comment_obj in comment_json["rows"])
            {
                comments.Add(
                    new Comment(
                        this,
                        comment_obj["name"],
                        string.Empty,
                        comment_obj["location"],
                        comment_obj["title"],
                        comment_obj["text"],
                        comment_obj["ts"],
                        comment_obj["tc"],
                        comment_obj["id"],
                        comment_obj["parent_id"]
                    ).SetToRetrievedCommentFormat());
            }
            return comments.ToArray();
        }

        public Comment GetCommentByCommentNum(int commentNum)
        {
            Comment[] all_comments = this.GetComments();
            foreach(Comment comment in all_comments)
            {
                if (comment.CommentNum == commentNum)
                {
                    return comment;
                }
            }
            return null;
        }

        public Comment[] GetCommentsByWriter(string WriterName)
        {
            List<Comment> commentsByWriter = new List<Comment>();

            Comment[] all_comments = this.GetComments();
            foreach(Comment comment in all_comments)
            {
                if (comment.Name == WriterName)
                {
                    commentsByWriter.Add(comment);
                }
            }

            return commentsByWriter.ToArray();
        }

        public bool HasCommentsByWriter(string WriterName)
        {
            Comment[] commentsByWriter = this.GetCommentsByWriter(WriterName);
            if (commentsByWriter == null || commentsByWriter.Length == 0)
            {
                return false;
            }
            return true;
        }

        public Comment GetCommentById(int id)
        {
            foreach (Comment comment in this.GetComments())
            {
                if (comment.CommentId == id)
                {
                    return comment;
                }
            }
            return null;
        }
    }
}
