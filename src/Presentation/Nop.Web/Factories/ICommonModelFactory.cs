﻿using System.Threading.Tasks;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the common models factory
    /// </summary>
    public partial interface ICommonModelFactory
    {
        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        Task<LogoModel> PrepareLogoModel();

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        Task<LanguageSelectorModel> PrepareLanguageSelectorModel();

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>Currency selector model</returns>
        Task<CurrencySelectorModel> PrepareCurrencySelectorModel();

        /// <summary>
        /// Prepare the tax type selector model
        /// </summary>
        /// <returns>Tax type selector model</returns>
        Task<TaxTypeSelectorModel> PrepareTaxTypeSelectorModel();

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        Task<HeaderLinksModel> PrepareHeaderLinksModel();

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModel();

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>Social model</returns>
        Task<SocialModel> PrepareSocialModel();

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        Task<FooterModel> PrepareFooterModel();

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        Task<ContactUsModel> PrepareContactUsModel(ContactUsModel model, bool excludeProperties);

        /// <summary>
        /// Prepare the contact vendor model
        /// </summary>
        /// <param name="model">Contact vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact vendor model</returns>
        Task<ContactVendorModel> PrepareContactVendorModel(ContactVendorModel model, Vendor vendor,
            bool excludeProperties);

        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>Sitemap model</returns>
        Task<SitemapModel> PrepareSitemapModel(SitemapPageModel pageModel);

        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        Task<string> PrepareSitemapXml( int? id);

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        Task<StoreThemeSelectorModel> PrepareStoreThemeSelectorModel();

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModel();

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        Task<string> PrepareRobotsTextFile();
    }
}
