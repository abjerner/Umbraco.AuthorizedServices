using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.AuthorizedServices.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;

namespace Umbraco.AuthorizedServices.Trees;

[Authorize(Policy = Cms.Web.Common.Authorization.AuthorizationPolicies.SectionAccessSettings)]
[Tree(Cms.Core.Constants.Applications.Settings, Constants.Trees.AuthorizedServices, TreeTitle = "Authorized Services")]
[PluginController(Constants.PluginName)]
public class AuthorizedServicesTreeController : TreeController
{
    private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
    private readonly AuthorizedServiceSettings _authorizedServiceSettings;

    public AuthorizedServicesTreeController(
        ILocalizedTextService textService,
        UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
        IEventAggregator eventAggregator,
        IMenuItemCollectionFactory menuItemCollectionFactory,
        IOptionsMonitor<AuthorizedServiceSettings> authorizedServiceSettings)
        : base(textService, umbracoApiControllerTypeCollection, eventAggregator)
    {
        _menuItemCollectionFactory = menuItemCollectionFactory;
        _authorizedServiceSettings = authorizedServiceSettings.CurrentValue;
    }

    protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
    {
        MenuItemCollection menu = _menuItemCollectionFactory.Create();

        if (id == Cms.Core.Constants.System.RootString)
        {
            menu.Items.Add(new RefreshNode(LocalizedTextService, true));
        }

        return menu;
    }

    protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
    {
        var nodes = new TreeNodeCollection();
        foreach (ServiceDetail service in _authorizedServiceSettings.Services)
        {
            TreeNode node = CreateTreeNode(service.Alias, "-1", queryStrings, service.DisplayName, service.Icon, false);
            nodes.Add(node);
        }

        return nodes;
    }
}
