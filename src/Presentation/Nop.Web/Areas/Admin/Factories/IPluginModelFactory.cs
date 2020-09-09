﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the plugin model factory
    /// </summary>
    public partial interface IPluginModelFactory
    {
        /// <summary>
        /// Prepare plugin search model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>Plugin search model</returns>
        Task<PluginSearchModel> PreparePluginSearchModel(PluginSearchModel searchModel);

        /// <summary>
        /// Prepare paged plugin list model
        /// </summary>
        /// <param name="searchModel">Plugin search model</param>
        /// <returns>Plugin list model</returns>
        Task<PluginListModel> PreparePluginListModel(PluginSearchModel searchModel);

        /// <summary>
        /// Prepare plugin model
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Plugin model</returns>
        Task<PluginModel> PreparePluginModel(PluginModel model, PluginDescriptor pluginDescriptor, bool excludeProperties = false);

        /// <summary>
        /// Prepare search model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>Search model of plugins of the official feed</returns>
        Task<OfficialFeedPluginSearchModel> PrepareOfficialFeedPluginSearchModel(OfficialFeedPluginSearchModel searchModel);

        /// <summary>
        /// Prepare paged list model of plugins of the official feed
        /// </summary>
        /// <param name="searchModel">Search model of plugins of the official feed</param>
        /// <returns>List model of plugins of the official feed</returns>
        Task<OfficialFeedPluginListModel> PrepareOfficialFeedPluginListModel(OfficialFeedPluginSearchModel searchModel);

        /// <summary>
        /// Prepare plugins configuration model
        /// </summary>
        /// <param name="configModel">Plugins configuration model</param>
        /// <returns>Plugins configuration model</returns>
        Task<PluginsConfigurationModel> PreparePluginsConfigurationModel(PluginsConfigurationModel configModel);

        /// <summary>
        /// Prepare plugin models for admin navigation
        /// </summary>
        /// <returns>List of models</returns>
        Task<IList<AdminNavigationPluginModel>> PrepareAdminNavigationPluginModels();
    }
}