using System.Collections.Generic;

namespace Emby.Plugins.Douban.Response
{
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string loc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string kind { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string followed { get; set; }
        /// <summary>
        /// 丹尼斯·维伦纽瓦
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int verify_type { get; set; }
        /// <summary>
        /// 导演 丹尼斯·维伦纽瓦  Denis Villeneuve 
        /// </summary>
        public string @abstract { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reg_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> medal_groups { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string in_blacklist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int followers_count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_banned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar_side_icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uid { get; set; }
    }

    public class Avatar
    {
        /// <summary>
        /// 
        /// </summary>
        public string large { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string normal { get; set; }
    }

    public class CelebritiesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public User user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> roles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string latin_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 丹尼斯·维伦纽瓦生于加拿大魁北克，具有法国血统。曾凭借2000年的《迷情漩涡》、2009年的《理工学院》...
        /// </summary>
        public string @abstract { get; set; }
        /// <summary>
        /// 丹尼斯·维伦纽瓦（同名）加拿大,魁北克省,三河城影视演员
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 导演 Director
        /// </summary>
        public string character { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cover_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Avatar avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sharing_url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 丹尼斯·维伦纽瓦
        /// </summary>
        public string name { get; set; }
    }

    public class CreditsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CelebritiesItem> celebrities { get; set; }
        /// <summary>
        /// 导演
        /// </summary>
        public string title { get; set; }
    }

    public class SubjectCredits
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CreditsItem> credits { get; set; }
    }
}
