using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using StorageManager.Helpers;
using StorageHelper;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
//using Microsoft.WindowsAzure.ServiceRuntime;

namespace StorageManager
{
	public partial class Login : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// If we can get the account info from an env variable, use that
			if (GetAccountFromEnvironment())
			{
				Response.Redirect("default.aspx");
				return;
			}

			if (!IsPostBack)
			{
				txtAccount.Focus();
				lblError.Text = string.Empty;
			}
		}

		public bool GetAccountFromEnvironment()
		{
			const string pattern =
				"(DefaultEndpointsProtocol)(=)((?:[a-z][a-z]+))(;)(AccountName)(=)((?:[a-z][a-z0-9_]*))(;)(AccountKey)(=)(.*)";
			foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
			{
				if (entry.Key.ToString().StartsWith("APPSETTING"))
					continue;
				var match = Regex.Match(entry.Value.ToString(), pattern);
				if (match.Success)
				{
					SaveCookies(match.Groups[7].ToString(), match.Groups[11].ToString());
					return true;
				}
			}

			return false;
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(txtAccount.Text) || string.IsNullOrEmpty(txtKey.Text))
					return;

				CheckSecurity();

				SaveCookies(txtAccount.Text, txtKey.Text);

				Response.Redirect("default.aspx");
			}
			catch (Exception ex)
			{
				lblError.Text = ex.Message;
			}
		}

		void SaveCookies(string account, string key)
		{
			HttpCookie accountCookie = new HttpCookie(SiteHelper.ACCOUNT, account);
			HttpCookie keyCookie = new HttpCookie(SiteHelper.KEY, key);

			Response.Cookies.Add(accountCookie);
			Response.Cookies.Add(keyCookie);
		}

		void CheckSecurity()
		{
			CloudBlobClient client = Client.GetBlobClient(txtAccount.Text, txtKey.Text);
			foreach (CloudBlobContainer container in client.ListContainers())
			{
				string name = container.Name;
				break;
			}
		}
	}
}
