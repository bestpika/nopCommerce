﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tax model factory implementation
    /// </summary>
    public partial class TaxModelFactory : ITaxModelFactory
    {
        #region Fields

        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public TaxModelFactory(
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager)
        {
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tax configuration model
        /// </summary>
        /// <param name="taxConfigurationModel">Tax configuration model</param>
        /// <returns>Tax configuration model</returns>
        public virtual async Task<TaxConfigurationModel> PrepareTaxConfigurationModel(TaxConfigurationModel taxConfigurationModel)
        {
            if (taxConfigurationModel == null)
                throw new ArgumentNullException(nameof(taxConfigurationModel));

            //prepare nested search models
            await PrepareTaxProviderSearchModel(taxConfigurationModel.TaxProviders);
            await PrepareTaxCategorySearchModel(taxConfigurationModel.TaxCategories);

            return taxConfigurationModel;
        }

        /// <summary>
        /// Prepare tax provider search model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>Tax provider search model</returns>
        public virtual Task<TaxProviderSearchModel> PrepareTaxProviderSearchModel(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tax provider list model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>Tax provider list model</returns>
        public virtual Task<TaxProviderListModel> PrepareTaxProviderListModel(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax providers
            var taxProviders = _taxPluginManager.LoadAllPlugins().ToPagedList(searchModel);

            //prepare grid model
            var model = new TaxProviderListModel().PrepareToGrid(searchModel, taxProviders, () =>
            {
                return taxProviders.Select(provider =>
                {
                    //fill in model values from the entity
                    var taxProviderModel = provider.ToPluginModel<TaxProviderModel>();

                    //fill in additional values (not existing in the entity)
                    taxProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                    taxProviderModel.IsPrimaryTaxProvider = _taxPluginManager.IsPluginActive(provider);

                    return taxProviderModel;
                });
            });

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare tax category search model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category search model</returns>
        public virtual Task<TaxCategorySearchModel> PrepareTaxCategorySearchModel(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>Tax category list model</returns>
        public virtual async Task<TaxCategoryListModel> PrepareTaxCategoryListModel(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax categories
            var taxCategories = (await _taxCategoryService.GetAllTaxCategories()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TaxCategoryListModel().PrepareToGrid(searchModel, taxCategories, () =>
            {
                //fill in model values from the entity
                return taxCategories.Select(taxCategory => taxCategory.ToModel<TaxCategoryModel>());
            });

            return model;
        }

        #endregion
    }
}