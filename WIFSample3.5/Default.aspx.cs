using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace WIFSample3._5
{
    public partial class _Default : System.Web.UI.Page
    {
        #region Commented
        //protected void Page_Load(object sender, EventArgs e)
        //{

        //    if (Request.Form.Get(WSFederationConstants.Parameters.Result) != null)
        //    {
        //        SignInResponseMessage message =
        //            WSFederationMessage.CreateFromFormPost(System.Web.HttpContext.Current.Request) as SignInResponseMessage;

        //        XmlDocument signInResponseXml = new XmlDocument();
        //        signInResponseXml.LoadXml(message.Result);

        //        var securityTokenHandlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();

        //        string samlTokenXml = signInResponseXml
        //       .DocumentElement  // <trust:RequestSecurityTokenResponseCollection>
        //       .ChildNodes[0] // <trust:RequestSecurityTokenResponse>
        //       .ChildNodes[2] // <trust:RequestedSecurityToken>
        //       .InnerXml; // <Assertion>

        //        var xmlTextReader = new XmlTextReader(new StringReader(samlTokenXml));

        //        // read the token
        //        SecurityToken securityToken = securityTokenHandlers.ReadToken(xmlTextReader);

        //        string str = string.Empty;
        //        str += "Valid From: " + securityToken.ValidFrom;
        //        str += "Valid Till: " + securityToken.ValidTo;
        //        str += "Id: " + securityToken.Id;

        //    }


        //} 
        #endregion

        /// <summary>
        /// The Page_Load method implementation for this page.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        protected void Page_Load(object sender, EventArgs e)
        {

            IClaimsIdentity claimsIdentity = ((IClaimsPrincipal)Thread.CurrentPrincipal).Identities[0];

            //
            // For illustrative purposes this sample application simply shows all the parameters of 
            // claims (i.e. claim types and claim values), which are issued by a security token 
            // service (STS), to its clients. In production code, security implications of echoing 
            // the properties of claims to the clients should be carefully considered. For example, 
            // some of the security considerations are: (i) accepting the only claim types that are 
            // expected by relying party applications; (ii) sanitizing the claim parameters before 
            // using them; and (iii) filtering out claims that contain sensitive personal information). 
            // DO NOT use this sample code ‘as is’ in production code.
            //
            ShowName(claimsIdentity);
            ShowClaimsIdentityAsIIdentity(claimsIdentity);
            ShowClaimsFromClaimsIdentity(claimsIdentity);
        }


        /// <summary>
        /// Display a page heading welcoming the user.
        /// </summary>
        /// <param name="claimsIdentity">Identity of user</param>
        private void ShowName(IClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity != null)
            {
                WelcomeMessage.Text = "Welcome : " + claimsIdentity.Name;
            }
            else
            {
                WelcomeMessage.Text = "Please sign in.";
            }
        }


        /// <summary>
        /// Build a table showing the name and authentication status of the current identity.
        /// </summary>
        /// <param name="ci">Current Identity</param>
        public void ShowClaimsIdentityAsIIdentity(IIdentity identity)
        {
            // create the table and labels
            Table table = new Table();
            table.BorderWidth = 1;
            table.Font.Name = "Franklin Gothic Book";
            table.Font.Size = 10;
            table.CellPadding = 3;
            table.CellSpacing = 3;
            table.BorderColor = System.Drawing.Color.Chocolate;

            Label labelTitle = new Label();
            labelTitle.Text = "Values from IIdentity";
            this.Controls.Add(labelTitle);
            this.Controls.Add(table);

            // build a row with the identity data
            TableCell tc1 = new TableCell();
            tc1.Text = "";
            tc1.BorderWidth = 1;

            TableCell tc2 = new TableCell();
            tc2.Text = "IsAuthenticated:" + identity.IsAuthenticated.ToString();
            tc2.BorderWidth = 1;

            TableCell tc3 = new TableCell();
            tc3.Text = "Name:" + identity.Name;
            tc3.BorderWidth = 1;

            TableRow tr = new TableRow();
            tr.Controls.Add(tc1);
            tr.Controls.Add(tc2);
            tr.Controls.Add(tc3);

            // add the row to the table
            table.Controls.Add(tr);
        }

        /// <summary>
        /// Write a claim out in a row of the table.
        /// </summary>
        /// <param name="claim"> claim value to write out </param>
        /// <param name="table"> the table to write the row out to </param>
        private void WriteClaim(Claim claim, Table table)
        {
            TableCell tc1 = new TableCell();
            tc1.Text = claim.ClaimType;

            TableCell tc2 = new TableCell();
            tc2.Text = claim.Value;

            TableCell tc3 = new TableCell();
            tc3.Text = claim.ValueType.Substring(claim.ValueType.IndexOf('#') + 1);

            TableCell tc4 = new TableCell();
            if ((claim.Subject) != null && (String.IsNullOrEmpty(claim.Subject.Name)) == false)
            {
                tc4.Text = claim.Subject.Name;
            }
            else
            {
                tc4.Text = "Null";
            }

            TableCell tc5 = new TableCell();
            if (!String.IsNullOrEmpty(claim.Issuer))
            {
                tc5.Text = claim.Issuer;
            }
            else
            {
                tc5.Text = "Null";
            }

            TableRow tr = new TableRow();

            tr.Controls.Add(tc1);
            tr.Controls.Add(tc2);
            tr.Controls.Add(tc3);
            tr.Controls.Add(tc4);
            tr.Controls.Add(tc5);

            tr.BorderColor = System.Drawing.Color.Chocolate;

            table.Controls.Add(tr);
        }

        /// <summary>
        /// Build a table listing the claims accepted about the identity.
        /// </summary>
        /// <param name="claimsIdentity">Given identity</param>
        private void ShowClaimsFromClaimsIdentity(IClaimsIdentity claimsIdentity)
        {
            // build table and header row
            Table table = new Table();
            TableHeaderCell thc1 = new TableHeaderCell();
            thc1.Text = "Claim Type";

            TableHeaderCell thc2 = new TableHeaderCell();
            thc2.Text = "Claim Value";

            TableHeaderCell thc3 = new TableHeaderCell();
            thc3.Text = "Value Type";

            TableHeaderCell thc4 = new TableHeaderCell();
            thc4.Text = "Subject Name";

            TableHeaderCell thc5 = new TableHeaderCell();
            thc5.Text = "Issuer Name";

            TableHeaderRow th = new TableHeaderRow();
            th.Controls.Add(thc1);
            th.Controls.Add(thc2);
            th.Controls.Add(thc3);
            th.Controls.Add(thc4);
            th.Controls.Add(thc5);
            th.BorderWidth = 1;
            th.BorderColor = System.Drawing.Color.Chocolate;

            // add a row for each claim
            table.Controls.Add(th);
            foreach (Claim claim in claimsIdentity.Claims)
            {
                // Show all the claims
                WriteClaim(claim, table);
            }

            // add the table to the returned page
            table.BorderWidth = 1;
            table.Font.Name = "Franklin Gothic Book";
            table.Font.Size = 10;
            table.CellPadding = 3;
            table.CellSpacing = 3;
            table.BorderColor = System.Drawing.Color.Chocolate;
            Label labelTitle = new Label();
            labelTitle.Text = "Claims from IClaimsIdentity";
            this.Controls.Add(labelTitle);
            this.Controls.Add(table);
        }

        public void FederatedSignOut(string reply = null)
        {
            WSFederationAuthenticationModule fam = FederatedAuthentication.WSFederationAuthenticationModule;

            // Native FederatedSignOut doesn't seem to have a way for finding/registering realm for singout, get it from the FAM
            string wrealm = string.Format("wtrealm={0}", fam.Realm);

            // Create basic url for signout (wreply is set by native FederatedSignOut)
            string signOutUrl = WSFederationAuthenticationModule.GetFederationPassiveSignOutUrl(fam.Issuer, null, wrealm);

            // Check where to return, if not set ACS will use Reply address configured for the RP
            string wreply = !string.IsNullOrEmpty(reply) ? reply : (!string.IsNullOrEmpty(fam.Reply) ? fam.Reply : null);

            //WSFederationAuthenticationModule.FederatedSignOut(new Uri(signOutUrl), !string.IsNullOrEmpty(wreply) ? new Uri(wreply) : null);

            WSFederationAuthenticationModule.FederatedSignOut(new Uri(signOutUrl), new Uri(wreply));

            // Remarks! Native FederatedSignout has an option for setting signOutUrl to null, even if the documentation tells otherwise.
            // If set to null the method will search for signoutUrl in Session token, but I couldn't find any information about how to set this. Found some Sharepoint code that use this
            // Michele Leroux Bustamante had a code example (from 2010) that also uses this form.
            // Other examples creates the signout url manually and calls redirect.

            // FAM has support for wsignoutcleanup1.0 right out of the box, there is no need for code to handle this.
            // That makes it even harder to understand why there are no complete FederatedSignOut method in FAM

            // When using native FederatedSignOut() no events for signout will be called, if you need this use the FAM SignOut methods instead.
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            FederatedSignOut();
        }
    }
}