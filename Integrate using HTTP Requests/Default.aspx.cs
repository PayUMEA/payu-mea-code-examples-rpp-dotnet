using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.Text;
using System.IO;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string _url = "https://staging.payu.co.za"+ serviceAPI; //web service url       

        string soapContent = ""; //soap content

        soapContent = "<ns0:setTransaction>";
        soapContent += "<Api>ONE_ZERO</Api>";
        soapContent += "<Safekey>{45D5C765-16D2-45A4-8C41-8D6F84042F8C}</Safekey>";

        //Payment type   
        soapContent += "<TransactionType>RESERVE</TransactionType>";
        soapContent += "<Stage>'True'</Stage>";

        // Additional
        soapContent += "<AdditionalInformation>";
        
        // The cancel url for redirect shuold be configured accordingly
        soapContent += "<cancelUrl>http://197.84.206.122/dev/integration/demo/developer/payu-redirect-payment-page/cancel-page.php</cancelUrl>";
        
        soapContent += "<merchantReference>1351079862</merchantReference>";

        // The return url for redirect shuold be configured accordingly
        soapContent += "<returnUrl>http://197.84.206.122/dev/integration/demo/developer/payu-redirect-payment-page/send-getTransaction-via-soap.php</returnUrl>";
        soapContent += "<supportedPaymentMethods>CREDITCARD</supportedPaymentMethods>";
        soapContent += "</AdditionalInformation>";

        //Customer
        soapContent += "<Customer>";
        soapContent += "<merchantUserId>1351079862</merchantUserId>";
        soapContent += "<email>test@gmail.com</email>";
        soapContent += "<firstName>firstName_1349862936</firstName>";
        soapContent += "<lastName>lastName_1349862936</lastName>";
        soapContent += "<mobile>27123456789</mobile>";
        soapContent += "</Customer>";

        // Basket
        soapContent += "<Basket>";
        soapContent += "<amountInCents>9394</amountInCents>";
        soapContent += "<currencyCode>ZAR</currencyCode>";
        soapContent += "<description>Test Store Order: 1351079862</description>";
        soapContent += "</Basket>";
        soapContent += "</ns0:setTransaction>";
        // construct soap object

        XmlDocument soapEnvelopeXml = CreateSoapEnvelope(soapContent);

        // create username and password namespace
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(soapEnvelopeXml.NameTable);
        nsmgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");


        // set soap username
        XmlNode userName = soapEnvelopeXml.SelectSingleNode("//wsse:Username",nsmgr);
        userName.InnerText = "Staging Integration Store 1";

        // set soap password
        XmlNode userPassword = soapEnvelopeXml.SelectSingleNode("//wsse:Password",nsmgr);
        userPassword.InnerText = "78cXrW1W";

        // construct web request object
        HttpWebRequest webRequest = CreateWebRequest(_url);

        // insert soap envelope into web request
        InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

        // get the PayU reference number from the completed web request.
        string soapResult;

        using (WebResponse webResponse = webRequest.GetResponse())
        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
        {
            soapResult = rd.ReadToEnd();
        }

        // create an empty soap result object
        XmlDocument soapResultXml = new XmlDocument();
        soapResultXml.LoadXml(soapResult.ToString());

        string success = soapResultXml.SelectSingleNode("//successful").InnerText;
            if (success == "true") {
                StringBuilder builder = new StringBuilder();

                builder.Append("https://staging.payu.co.za" + redirectAPI);

                // retrieve payU reference & build url with payU reference query string
                string payURef = soapResultXml.SelectSingleNode("//payUReference").InnerText;
                builder.Append(payURef);

                // redirect to payU site
                Response.Redirect(builder.ToString()); 
            }
    }

    static string serviceAPI = "/service/PayUAPI?wsdl";
    static string redirectAPI = "/rpp.do?PayUReference=";

    static string _soapEnvelope =
                @"<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/'
                        xmlns:ns0='http://soap.api.controller.web.payjar.com/'
                        xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'
                        xmlns:SOAP-ENC='http://www.w3.org/2003/05/soap-encoding'
                        xmlns:ns1='http://schemas.xmlsoap.org/soap/envelope/'
                        xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                    <SOAP-ENV:Header>
                        <wsse:Security SOAP-ENV:mustUnderstand='1' xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
                            <wsse:UsernameToken wsu:Id='UsernameToken-9' xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
                                <wsse:Username></wsse:Username>
                                <wsse:Password Type='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText'></wsse:Password>
                            </wsse:UsernameToken>
                        </wsse:Security>
                    </SOAP-ENV:Header>
                    <ns1:Body>
                    </ns1:Body>
                </SOAP-ENV:Envelope>";

    /// <summary>
    /// Creates the HttpWebRequest object
    /// </summary>
    /// <returns>webRequest</returns>        
    private static HttpWebRequest CreateWebRequest(string url)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Headers.Add(@"SOAP:Action");
        webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        webRequest.Accept = "text/xml";
        webRequest.Method = "POST";
        return webRequest;
    }


    /// <summary>
    /// Creates the SOAP envelope object
    /// </summary>
    /// <returns>soapEnvelopeXml</returns> 
    private static XmlDocument CreateSoapEnvelope(string content)
    {
        StringBuilder sb = new StringBuilder(_soapEnvelope);
        sb.Insert(sb.ToString().IndexOf("</ns1:Body>"), content);

        // create an empty soap envelope
        XmlDocument soapEnvelopeXml = new XmlDocument();
        soapEnvelopeXml.LoadXml(sb.ToString());

        return soapEnvelopeXml;
    }

    /// <summary>
    /// Insert soap envelope into web request
    /// </summary>
    /// <returns></returns>
    private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
    {
        using (Stream stream = webRequest.GetRequestStream())
        {
            soapEnvelopeXml.Save(stream);
        }
    }
}