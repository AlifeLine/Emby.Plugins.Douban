using System;
using System.Collections.Generic;
using System.Text;

namespace Emby.Plugins.Douban.Response
{
    //如果好用，请收藏地址，帮忙分享。
    public class Extra
    {
        /// <summary>
        /// 
        /// </summary>
        public List<List<string>> info { get; set; }
        /// <summary>
        /// 演员 美术 配音 其它 / 一出好戏 非诚勿扰2 全民目击
        /// </summary>
        public string short_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string header_img { get; set; }
    }

    public class Cover_img
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class Rating_group
    {
        /// <summary>
        /// 
        /// </summary>
        public Rating rating { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string null_rating_reason { get; set; }
    }

    public class Large
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class Normal
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int height { get; set; }
    }

    public class Cover
    {
        /// <summary>
        /// 
        /// </summary>
        public Large large { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Normal normal { get; set; }
    }

    public class Color_scheme
    {
        /// <summary>
        /// 
        /// </summary>
        public string is_dark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primary_color_light { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<double> _base_color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secondary_color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<double> _avg_color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primary_color_dark { get; set; }
    }
    public class WorksItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> roles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Subject subject { get; set; }
    }

    public class CollectionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<WorksItem> works { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 影视
        /// </summary>
        public string title { get; set; }
    }

    public class Payload
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CollectionsItem> collections { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 作品集
        /// </summary>
        public string title { get; set; }
    }

    public class ModulesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Payload payload { get; set; }
    }


    public class ElessarSubject 
    {
        /// <summary>
        /// 
        /// </summary>
        public Extra extra { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Cover_img cover_img { get; set; }
        /// <summary>
        /// 孙红雷
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Cover cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string latin_title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// 简介
        /// </summary>
        public string desc { get; set; }
        }
}
