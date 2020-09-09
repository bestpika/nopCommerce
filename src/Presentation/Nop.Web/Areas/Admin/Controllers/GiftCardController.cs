﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class GiftCardController : BaseAdminController
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardModelFactory _giftCardModelFactory;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public GiftCardController(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardModelFactory giftCardModelFactory,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardModelFactory = giftCardModelFactory;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardSearchModel(new GiftCardSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GiftCardList(GiftCardSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModel(new GiftCardModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var giftCard = model.ToEntity<GiftCard>();
                giftCard.CreatedOnUtc = DateTime.UtcNow;
                await _giftCardService.InsertGiftCard(giftCard);

                //activity log
                await _customerActivityService.InsertActivity("AddNewGiftCard",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.GiftCards.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = giftCard.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardModel(null, giftCard);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(GiftCardModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardById(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            var order = await _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);

            model.PurchasedWithOrderId = order?.Id;
            model.RemainingAmountStr = await _priceFormatter.FormatPrice(await _giftCardService.GetGiftCardRemainingAmount(giftCard), true, false);
            model.AmountStr = await _priceFormatter.FormatPrice(giftCard.Amount, true, false);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;

            if (ModelState.IsValid)
            {
                giftCard = model.ToEntity(giftCard);
                await _giftCardService.UpdateGiftCard(giftCard);

                //activity log
                await _customerActivityService.InsertActivity("EditGiftCard",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditGiftCard"), giftCard.GiftCardCouponCode), giftCard);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.GiftCards.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = giftCard.Id });
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModel(model, giftCard, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateCouponCode()
        {
            return Json(new { CouponCode = _giftCardService.GenerateGiftCardCode() });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notifyRecipient")]
        public virtual async Task<IActionResult> NotifyRecipient(GiftCardModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardById(model.Id);
            if (giftCard == null)
                return RedirectToAction("List");

            try
            {
                if (!CommonHelper.IsValidEmail(giftCard.RecipientEmail))
                    throw new NopException("Recipient email is not valid");

                if (!CommonHelper.IsValidEmail(giftCard.SenderEmail))
                    throw new NopException("Sender email is not valid");

                var languageId = 0;
                var order = await _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);
                
                if (order != null)
                {
                    var customerLang = await _languageService.GetLanguageById(order.CustomerLanguageId);
                    if (customerLang == null)
                        customerLang = (await _languageService.GetAllLanguages()).FirstOrDefault();
                    if (customerLang != null)
                        languageId = customerLang.Id;
                }
                else
                {
                    languageId = _localizationSettings.DefaultAdminLanguageId;
                }

                var queuedEmailIds = await _workflowMessageService.SendGiftCardNotification(giftCard, languageId);
                if (queuedEmailIds.Any())
                {
                    giftCard.IsRecipientNotified = true;
                    await _giftCardService.UpdateGiftCard(giftCard);
                    model.IsRecipientNotified = true;
                }
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = await _giftCardModelFactory.PrepareGiftCardModel(model, giftCard);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedView();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardById(id);
            if (giftCard == null)
                return RedirectToAction("List");

            await _giftCardService.DeleteGiftCard(giftCard);

            //activity log
            await _customerActivityService.InsertActivity("DeleteGiftCard",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteGiftCard"), giftCard.GiftCardCouponCode), giftCard);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.GiftCards.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(GiftCardUsageHistorySearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageGiftCards))
                return AccessDeniedDataTablesJson();

            //try to get a gift card with the specified id
            var giftCard = await _giftCardService.GetGiftCardById(searchModel.GiftCardId)
                ?? throw new ArgumentException("No gift card found with the specified id");

            //prepare model
            var model = await _giftCardModelFactory.PrepareGiftCardUsageHistoryListModel(searchModel, giftCard);

            return Json(model);
        }

        #endregion
    }
}