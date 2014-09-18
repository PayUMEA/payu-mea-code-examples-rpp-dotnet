using IntegrateServiceReference.PayUAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IntegrateServiceReference
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            SetTransactionResponseMessage payInitResult = PayUService.InitialisePayment();

            if (payInitResult == null)
            {
                lblResult.ForeColor = Color.Red;
                lblResult.Text = "No response";
            }
            else if (payInitResult.successful)
            {
                Response.Redirect("https://staging.payu.co.za/rpp.do?PayUReference=" + payInitResult.payUReference);
            }
            else
            {
                lblResult.ForeColor = Color.Red;
                lblResult.Text = payInitResult.resultMessage;
            }
        }


        public class PayUService
        {
            public static SetTransactionResponseMessage InitialisePayment()
            {
                var trans = new setTransaction
                {
                    Api = "1.0",
                    Safekey = "{CE62CE80-0EFD-4035-87C1-8824C5C46E7F}",
                    TransactionType = transactionType.PAYMENT,
                    Stage = false,
                    AdditionalInformation =
                        new additionalInfo
                        {
                            merchantReference = "EasyMerchant",
                            returnUrl = "http://localhost:60939/Default.aspx?return=true",
                            cancelUrl = "http://localhost:60939/Default.aspx?cancel=true",
                            demoMode = "true",
                            secure3d = "false",
                            supportedPaymentMethods = "CREDITCARD",
                            showBudget = "false",
                            redirectChannel = "WEB"
                        },
                    Customer =
                        new customer
                        {
                            merchantUserId = "100284",
                            email = "test@test.com",
                            mobile = "0111111111"
                        },
                    Basket =
                        new basket
                        {
                            amountInCents = "100",
                            currencyCode = "ZAR",
                            description = "Purchase"
                        },
                    Customfield =
                        new[] { new customField { key = "PaymentId", value = "PaymentId" }, }
                };

                EnterpriseAPISoapClient client = new EnterpriseAPISoapClient("PayUStaging");

                //Test soap call with hi to verify access
                //string hiResponse = client.hi();

                try
                {
                    SetTransactionResponseMessage response = client.setTransaction(
                        trans.Api,
                        trans.Safekey,
                        trans.TransactionType,
                        trans.Stage,
                        trans.AdditionalInformation,
                        trans.Customer,
                        trans.Basket,
                        trans.Fraud,
                        trans.Creditcard,
                        trans.Eft,
                        trans.Loyalty,
                        trans.BankTransfer,
                        trans.Ebucks,
                        trans.Autopay,
                        trans.Soulstace,
                        trans.Globalpay,
                        trans.Customfield,
                        trans.TransactionRecord);

                    return response;

                }
                catch (Exception ex)
                {
                }

                return null;
            }
        }
    }
}