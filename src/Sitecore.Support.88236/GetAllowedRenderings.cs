namespace Sitecore.Support.Pipelines.GetPlaceholderRenderings
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Pipelines.GetPlaceholderRenderings;
    using Sitecore.Text;
    using Sitecore.Web;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class GetAllowedRenderings
    {
        protected virtual List<Item> GetRenderings(Item placeholderItem, out bool allowedControlsSpecified)
        {
            Assert.ArgumentNotNull(placeholderItem, "placeholderItem");
            allowedControlsSpecified = false;
            ListString str = new ListString(placeholderItem["Allowed Controls"]);
            if (str.Count <= 0)
            {
                return null;
            }
            allowedControlsSpecified = true;
            List<Item> list = new List<Item>();
            Language clientContentLanguage = WebEditUtil.GetClientContentLanguage();
            foreach (string str2 in str)
            {
                Item item = placeholderItem.Database.GetItem(str2, clientContentLanguage);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        [Obsolete("Deprecated")]
        protected virtual List<Item> GetRenderings(string placeholderKey, string layoutDefinition, Database contentDatabase)
        {
            bool flag;
            Assert.IsNotNull(placeholderKey, "placeholder");
            Assert.IsNotNull(contentDatabase, "database");
            Assert.IsNotNull(layoutDefinition, "layout");
            Item placeholderItem = Client.Page.GetPlaceholderItem(placeholderKey, contentDatabase, layoutDefinition);
            if (placeholderItem == null)
            {
                return null;
            }
            return this.GetRenderings(placeholderItem, out flag);
        }

        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");
            Item placeholderItem = null;
            if (ID.IsNullOrEmpty(args.DeviceId))
            {
                placeholderItem = Client.Page.GetPlaceholderItem(args.PlaceholderKey, args.ContentDatabase, args.LayoutDefinition);
            }
            else
            {
                using (new DeviceSwitcher(args.DeviceId, args.ContentDatabase))
                {
                    placeholderItem = Client.Page.GetPlaceholderItem(args.PlaceholderKey, args.ContentDatabase, args.LayoutDefinition);
                }
            }
            List<Item> collection = null;
            if (placeholderItem != null)
            {
                bool flag3;
                args.HasPlaceholderSettings = true;
                collection = this.GetRenderings(placeholderItem, out flag3);
                if (flag3)
                {
                    args.Options.ShowTree = false;
                }
            }
            if (collection != null)
            {
                if (args.PlaceholderRenderings == null)
                {
                    args.PlaceholderRenderings = new List<Item>();
                }
                args.PlaceholderRenderings.AddRange(collection);
            }
        }
    }
}
