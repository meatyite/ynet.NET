using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ynet
{
    public class Comment
    {
        public Article _Article;
        public string Name;
        public string Email;
        public string Location;
        public string Title;
        public string Text;
        public int Likes;
        public int CommentNum;
        public DateTime CommentDate;
        public int CommentId;
        public int ParentId;
        public bool isReply;

        public Comment(Article _article,
            string Name,
            string Email,
            string Location,
            string Title,
            string Text,
            int Likes,
            int CommentNum,
            int CommentId,
            int ParentId)
        {
            this.Name = Name;
            this.Email = Email;
            this.Location = Location;
            this.Title = Title;
            this.Text = Text;
            this.Likes = Likes;
            this.CommentNum = CommentNum;
            this.CommentId = CommentId;
            this.ParentId = ParentId;
            if (ParentId == 0)
            {
                this.isReply = false;
            } else
            {
                this.isReply = true;
            }
        }

        public Comment[] GetReplies()
        {
            List<Comment> replies = new List<Comment>();

            Comment[] all_comments = this._Article.GetComments();
            foreach (Comment comment in all_comments)
            {
                if (comment.ParentId == this.CommentId)
                {
                    replies.Add(comment);
                }
            }

            return replies.ToArray();
        }

        public Comment SetToRetrievedCommentFormat()
        {
            this.Name = this.Name.Replace("&quot;", "\"").Replace("&#39;", "'");
            this.Title = this.Title.Replace("&quot;", "\"").Replace("&#39;", "'");
            this.Text = this.Text.Replace("&quot;", "\"").Replace("&#39;", "'").Replace("<br/>", "\n");
            return this;
        }

        public Comment GetParentComment()
        {
            Comment[] all_comments = this._Article.GetComments();
            foreach(Comment comment in all_comments)
            {
                if (this.ParentId == comment.CommentId)
                {
                    return comment;
                }
            }
            return null;
        }

        public string Reply(Comment comment)
        {
            using (WebClient wc = new WebClient())
            {
                string post_url = this._Article.Article_uri +
                    "YediothPortal/Ext/TalkBack/CdaTalkBackTrans/0,2499," + 
                    this._Article.Article_id +
                    "-0-68-13108-0---" + 
                    this.CommentId.ToString() + 
                    ",00";
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("WSGBRWSR", "FF");
                reqparm.Add("name", comment.Name);
                reqparm.Add("email", comment.Email);
                reqparm.Add("Location", comment.Location);
                reqparm.Add("title", comment.Title);
                reqparm.Add("description", comment.Text);
                byte[] responsebytes = wc.UploadValues(post_url, "POST", reqparm);
                return Encoding.UTF8.GetString(responsebytes);
            }
        }

        public string Post()
        {
            using (WebClient wc = new WebClient())
            {
                string post_url = this._Article.Article_uri +
                    "YediothPortal/Ext/TalkBack/CdaTalkBackTrans/0,2499," +
                    this._Article.Article_id +
                    "-0-68-546-0---0,00.html";
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("WSGBRWSR", "FF");
                reqparm.Add("name", this.Name);
                reqparm.Add("email", this.Email);
                reqparm.Add("Location", this.Location);
                reqparm.Add("title", this.Title);
                reqparm.Add("description", this.Text);
                byte[] responsebytes = wc.UploadValues(post_url, "POST", reqparm);
                return Encoding.UTF8.GetString(responsebytes);
            }
        }
    }
}
