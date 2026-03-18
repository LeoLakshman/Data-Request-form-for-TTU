using System;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Linq;                  
using System.Web.UI.WebControls;    

namespace YourNamespace
{
    public partial class IRRequest : Page
    {
        private string tempPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSubmitted.Text = DateTime.Today.ToString("yyyy-MM-dd");
                DateTime minDate = DateTime.Today.AddDays(14);
                txtDateNeeded.Text = minDate.ToString("yyyy-MM-dd");
                txtDateNeeded.Attributes["min"] = minDate.ToString("yyyy-MM-dd");
                // Show success message from previous submission (if any)
                if (Session["SuccessMessage"] != null)
                {
                    lblMessage.Text = Session["SuccessMessage"].ToString();
                    lblMessage.Visible = true;
                    Session.Remove("SuccessMessage");  
                    ClearForm();                       
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            if (!Page.IsValid) return;  // Stop if validators fail

            try
            {
                // Build safe HTML email body
                string safeDetails = HttpUtility.HtmlEncode(txtRequestDetails.Text).Replace("\n", "<br />");

                string attachmentNote = fuAttachment.HasFile
                    ? HttpUtility.HtmlEncode(fuAttachment.FileName)
                    : "(none)";

                string body = $@"
                    <!DOCTYPE html>
                        <html>
                        <body style=""font-family: Arial, sans-serif; font-size: 14px; color: #000;"">

                          <p style=""font-size: 20px; font-weight: bold; color: #CC0000;"">Online Data Request Form</p>
                          <!-- DEMOGRAPHICS TABLE -->
                          <table border=""1"" cellpadding=""8"" cellspacing=""0""
                                 style=""border-collapse: collapse; width: 600px; border: 2px solid #000;"">

                            <tr>
                              <td colspan=""2""
                                  style=""background-color: #000; color: #fff; font-weight: bold;
                                          font-size: 14px; padding: 8px;"">
                                DEMOGRAPHICS:
                              </td>
                            </tr>

                            <tr>
                              <td style=""width: 200px; font-weight: bold; border: 1px solid #000; padding: 8px;"">Requested By *:</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtRequestedBy.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">College/Company:</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtCollegeCompany.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Department:</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtDepartment.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Phone (xxx-xxx-xxxx):</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtPhone.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Email *:</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtEmail.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Date Submitted:*</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{txtDateSubmitted.Text}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Date Report Needed:*</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{txtDateNeeded.Text}</td>
                            </tr>

                          </table>

                          <br />

                          <!-- REQUEST DETAILS TABLE -->
                          <table border=""1"" cellpadding=""8"" cellspacing=""0""
                                 style=""border-collapse: collapse; width: 600px; border: 2px solid #000;"">

                            <tr>
                              <td colspan=""2""
                                  style=""background-color: #000; color: #fff; font-weight: bold;
                                          font-size: 14px; padding: 8px;"">
                                REQUEST DETAILS:
                              </td>
                            </tr>

                            <tr>
                              <td style=""width: 200px; font-weight: bold; border: 1px solid #000; padding: 8px;"">Request Title:*</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtRequestTitle.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">
                                Have you contacted IR Staff regarding this request?
                                If so, please type name of <strong>IR Contact:*</strong>
                              </td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{HttpUtility.HtmlEncode(txtIRContact.Text)}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px;"">Attachments:</td>
                              <td style=""border: 1px solid #000; padding: 8px;"">{attachmentNote}</td>
                            </tr>
                            <tr>
                              <td style=""font-weight: bold; border: 1px solid #000; padding: 8px; vertical-align: top;"">
                                Request Details *:<br />
                                <span style=""font-weight: normal; font-style: italic;"">(Please provide detailed information for data needed.)</span>
                              </td>
                              <td style=""border: 1px solid #000; padding: 8px; vertical-align: top;"">{safeDetails}</td>
                            </tr>

                          </table>

                        </body>
                        </html>
                ";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("lakshmanpukhraj@gmail.com");
                    mail.To.Add("lakshmanpukhraj@gmail.com");
                    mail.Subject = "New Online Data Request: " + txtRequestTitle.Text;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    if (fuAttachment.HasFile)
                    {
                        string ext = Path.GetExtension(fuAttachment.FileName).ToLowerInvariant();
                        string[] allowed = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };

                        if (!allowed.Contains(ext) || fuAttachment.PostedFile.ContentLength > 10 * 1024 * 1024)
                        {
                            lblError.Text = "Invalid file: only pdf, doc/docx, xls/xlsx, txt, jpg/jpeg, png allowed. Max 10 MB.";
                            lblError.Visible = true;
                            return;
                        }

                        string safeName = Guid.NewGuid().ToString() + ext;
                        tempPath = Path.Combine(Server.MapPath("~/App_Data/TempUploads/"), safeName);
                        Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                        fuAttachment.SaveAs(tempPath);

                        Attachment attachment = new Attachment(tempPath);
                        attachment.Name = fuAttachment.FileName;
                        mail.Attachments.Add(attachment);
                    }

                    SendEmail(mail);

                    Session["SuccessMessage"] = "Your request has been submitted successfully. Thank you!";  // temporary storage
                    Response.Redirect(Request.RawUrl);   // or Response.Redirect("~/IRRequest.aspx");  // redirect to same page
                }

                // Cleanup — now tempPath is in scope
                if (tempPath != null && File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch (Exception)
                    {
                        // Optional: log the error silently (don't show to user)
                        // You can write to a log file, or just swallow it
                        // Example:
                        // System.Diagnostics.Debug.WriteLine("Could not delete temp file: " + deleteEx.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error sending request: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void valFileType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (fuAttachment.HasFile)
            {
                string ext = Path.GetExtension(fuAttachment.FileName).ToLowerInvariant();
                string[] allowed = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };
                args.IsValid = allowed.Contains(ext) && fuAttachment.PostedFile.ContentLength <= 10 * 1024 * 1024;
            }
            else
            {
                args.IsValid = true;
            }
        }

        private void SendEmail(MailMessage mail)
        {
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;                  // Use 587 for TLS (recommended)
                smtp.EnableSsl = true;            // Required for Gmail
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(
                    "lakshmanpukhraj@gmail.com",        // ← Your full Gmail address
                    "bjyvxadldkrhvgwr"  // ← The App Password you generated (NO spaces!)
                );

                smtp.Send(mail);
            }
            catch (SmtpException ex)
            {
                throw new Exception("Gmail SMTP failed: " + ex.Message +
                    (ex.InnerException != null ? " Inner: " + ex.InnerException.Message : ""));
            }
        }

        private void ClearForm()
        {
            txtRequestedBy.Text = "";
            txtCollegeCompany.Text = "";
            txtDepartment.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtDateSubmitted.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtDateNeeded.Text = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
            txtRequestTitle.Text = "";
            txtIRContact.Text = "";
            txtRequestDetails.Text = "";
        }
    }
}
