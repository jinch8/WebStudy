using FlyPig.HttpHelper.Enum;
using System;

namespace FlyPig.HttpHelper.Item
{
    public class AItem
    {
        public string Href
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public ImgItem Img
        {
            get;
            set;
        }

        public string Html
        {
            get;
            set;
        }

        public AType Type
        {
            get;
            set;
        }
    }
}
