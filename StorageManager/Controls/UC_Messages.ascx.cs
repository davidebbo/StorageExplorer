﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using StorageManager.Helpers;
using StorageHelper;

namespace StorageManager.Controls
{
	public partial class UC_Messages : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				btnMessage.Enabled = Request.Params["queue"] != null;

				if (Request.Params["queue"] != null)
				{
					string queueName = Request.Params["queue"];
					btnMessage.Text = string.Format("Add message on queue '{0}'", queueName);
				}
			}

		}

		protected void btnMessage_Click(object sender, EventArgs e)
		{
			try
			{
				UCHelper.CleanUpLabels(new Label[] { lblMessage, lblError });

				Queue.CreateMessage(Request.Cookies[SiteHelper.ACCOUNT].Value,
									Request.Cookies[SiteHelper.KEY].Value,
									Request.QueryString["queue"],
									txtMessage.Text);

			}
			catch (Exception ex)
			{
				lblError.Text = ex.Message;
			}
			finally
			{
				grdMessages.DataBind();
			}
		}

		protected void grdMessages_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				UCHelper.CleanUpLabels(new Label[] { lblMessage, lblError });

				if (e.CommandName == "DeleteMessage")
				{
					int index = int.Parse(e.CommandArgument.ToString());
					MessageHelper.Delete(Request.Cookies[SiteHelper.ACCOUNT].Value,
									Request.Cookies[SiteHelper.KEY].Value,
									Request.QueryString["queue"],
									grdMessages.DataKeys[index].Value.ToString());
				}
			}
			catch (Exception ex)
			{
				lblError.Text = ex.Message;
			}
			finally
			{
				grdMessages.DataBind();
			}

		}
	}
}