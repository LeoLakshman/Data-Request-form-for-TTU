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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateSubmitted.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtDateNeeded.Text = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
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

                string body = $@"
                    <h3>New Data Request Submitted</h3>
                    <p><strong>Requested By:</strong> {HttpUtility.HtmlEncode(txtRequestedBy.Text)}</p>
                    <p><strong>College/Company:</strong> {HttpUtility.HtmlEncode(txtCollegeCompany.Text)}</p>
                    <p><strong>Department:</strong> {HttpUtility.HtmlEncode(txtDepartment.Text)}</p>
                    <p><strong>Phone:</strong> {HttpUtility.HtmlEncode(txtPhone.Text)}</p>
                    <p><strong>Email:</strong> {HttpUtility.HtmlEncode(txtEmail.Text)}</p>
                    <p><strong>Date Submitted:</strong> {txtDateSubmitted.Text}</p>
                    <p><strong>Date Report Needed:</strong> {txtDateNeeded.Text}</p>
                    <hr />
                    <p><strong>Request Title:</strong> {HttpUtility.HtmlEncode(txtRequestTitle.Text)}</p>
                    <p><strong>IR Contact:</strong> {HttpUtility.HtmlEncode(txtIRContact.Text)}</p>
                    <p><strong>Request Details:</strong><br />{safeDetails}</p>
                ";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("onyiiukaegbu@gmail.com"); // CHANGE to valid sender (e.g. irim@ttu.edu)
                mail.To.Add("onyiiukaegbu@gmail.com");                 // CHANGE to real recipient (e.g. irim@ttu.edu)
                mail.Subject = "New Online Data Request: " + txtRequestTitle.Text;
                mail.Body = body;
                mail.IsBodyHtml = true;

                // Attachment handling
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
                    string tempPath = Path.Combine(Server.MapPath("~/App_Data/TempUploads/"), safeName);

                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                    fuAttachment.SaveAs(tempPath);

                    try
                    {
                        Attachment attachment = new Attachment(tempPath);
                        attachment.Name = fuAttachment.FileName; // show original name
                        mail.Attachments.Add(attachment);

                        SendEmail(mail);
                    }
                    finally
                    {
                        if (File.Exists(tempPath)) File.Delete(tempPath);
                    }
                }
                else
                {
                    SendEmail(mail);
                }

                lblMessage.Text = "Your request has been submitted successfully. Thank you!";
                lblMessage.Visible = true;

                ClearForm();
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
                    "onyiiukaegbu@gmail.com",        // ← Your full Gmail address
                    "vgdhaiyetnwuuusy"  // ← The App Password you generated (NO spaces!)
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