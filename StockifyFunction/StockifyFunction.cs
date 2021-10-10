using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using StockifyFunction.Stockify.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace StockifyFunction
{
  public static class StockifyFunction
  {
    [FunctionName("StockifyFunction")]
    public static async Task RunAsync([TimerTrigger("0 0 9 * * *", RunOnStartup = true)] TimerInfo timerInfo, ILogger log)
    {
      var phoneNumberToNotify = Environment.GetEnvironmentVariable("SubscriberPhoneNumber");
      var twilioUserName = "AC4af05c594927aedaf3ec1f8d39fa1ef0"; // Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
      var twilioPassword = "6b5de686ec8affa07aa415621446af5a"; // Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
      var twilioPhoneNumber = "+18783484311"; // Environment.GetEnvironmentVariable("TwilioPhoneNumber");

      TwilioClient.Init(twilioUserName, twilioPassword);

      IList<ProductPage> products = new List<ProductPage>()
            {
                new BestBuyXboxPage(),
               // new GameStopXboxPage(),
                //new TargetXboxPage(),
            };

      DateTime nextNotification = DateTime.Now.AddMinutes(-1);
      foreach (var productPage in products)
      {


        try // try because FindElementByCssSelector throws exception if it can't be found
        {

          using var playwright = await Playwright.CreateAsync();
          await using var browser = await playwright.Chromium.LaunchAsync();
          var page = await browser.NewPageAsync();
          await page.GotoAsync(productPage.Url);

          var addToCart = await page.QuerySelectorAsync(productPage.AddToCartButtonSelector);
          if (addToCart != null && ! await addToCart.IsDisabledAsync())
          {
            if (nextNotification < DateTime.Now)
            {
              var message = MessageResource.Create(
                body: $"{productPage.Name} is available - {DateTime.Now} - {productPage.Url}",
                from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumberToNotify)
                );
              nextNotification = DateTime.Now.AddMinutes(5);
            }
          }
        }
        catch (Exception ex)
        {
          var test = ex;
        }

      }

     //   Thread.Sleep(6000);
    }
   }
 }
