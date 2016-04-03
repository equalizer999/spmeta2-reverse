﻿using SPMeta2.Definitions;
using SPMeta2.Reverse.CSOM.Foundation.ReverseHandlers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using SPMeta2.Models;
using SPMeta2.Reverse.CSOM.Foundation.ReverseHosts;
using SPMeta2.Reverse.ReverseHosts;
using SPMeta2.Reverse.Services;
using SPMeta2.Utils;
using SPMeta2.Syntax.Default;
using SPMeta2.ModelHosts;

namespace SPMeta2.Reverse.CSOM.Foundation.ReverseHandlers
{
    public class FieldReverseHandler : CSOMReverseHandlerBase
    {
        #region properties
        public override Type ReverseType
        {
            get { return typeof(FieldDefinition); }
        }

        public override IEnumerable<Type> ReverseParentTypes
        {
            get
            {
                return new[]
                {
                    typeof(SiteDefinition)
                };
            }
        }
        #endregion

        #region methods



        #endregion

        #region methods

        public override IEnumerable<ReverseHostBase> ReverseHosts(ReverseHostBase parentHost, ReverseOptions options)
        {
            var result = new List<FieldReverseHost>();

            var typedHost = parentHost.WithAssertAndCast<SiteReverseHost>("reverseHost", value => value.RequireNotNull());

            var site = typedHost.HostSite;
            var context = typedHost.HostClientContext;

            var items = site.RootWeb.Fields;

            context.Load(items);
            context.ExecuteQuery();

            result.AddRange(items.ToArray().Select(i =>
            {
                return ModelHostBase.Inherit<FieldReverseHost>(parentHost, h =>
                {
                    h.Field = i;
                });
            }));

            return result;
        }

        public override ModelNode ReverseSingleHost(object reverseHost, ReverseOptions options)
        {
            var item = (reverseHost as FieldReverseHost).Field;

            var def = new FieldDefinition();

            def.Title = item.Title;
            def.InternalName = item.InternalName;
            def.Id = item.Id;

            def.DefaultValue = item.DefaultValue;

            def.Required = item.Required;
            def.Group = item.Group;

            return new FieldModelNode
            {
                Options = { RequireSelfProcessing = true },
                Value = def
            };
        }

        #endregion
    }
}