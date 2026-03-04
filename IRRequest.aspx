<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IRRequest.aspx.cs" Inherits="YourNamespace.IRRequest" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Online Data Request Form - Institutional Research</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        body { font-family: Arial, sans-serif; max-width: 960px; margin: 0 auto; padding: 20px; background: #f8f8f8; }
        .header { background: #cc0000; color: white; padding: 15px; margin-bottom: 20px; border-radius: 4px; }
        .header a { color: white; text-decoration: underline; font-weight: bold; }
        .header a:hover { text-decoration: none; }
        h1 { color: #cc0000; margin: 0; }
        h2 { color: #333; }
        table { width: 100%; border-collapse: collapse; margin-bottom: 25px; background: white; }
        td, th { padding: 10px; border: 1px solid #ddd; vertical-align: top; }
        .label { font-weight: bold; width: 40%; background: #f0f0f0; }
        .required { color: #cc0000; font-weight: bold; }
        input[type="text"], input[type="email"], input[type="tel"], input[type="date"], textarea, select {
            width: 100%; padding: 8px; box-sizing: border-box; border: 1px solid #ccc; border-radius: 4px;
        }
        input:focus, textarea:focus { border-color: #cc0000; outline: none; }
        .error { border: 2px solid #cc0000 !important; }
        input[type="submit"] { background: #cc0000; color: white; padding: 12px 30px; border: none; cursor: pointer; font-weight: bold; font-size: 1.1em; border-radius: 4px; }
        input[type="submit"]:hover { background: #a80000; }
        .message { color: green; font-weight: bold; margin: 20px 0; padding: 10px; background: #e0ffe0; border: 1px solid green; border-radius: 4px; }
        .error-summary { color: #cc0000; font-weight: bold; margin: 20px 0; padding: 10px; background: #ffe0e0; border: 1px solid red; border-radius: 4px; }
        .validators { color: #cc0000; font-size: 0.9em; }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="header">
            <a href="http://www.ttu.edu">Texas Tech University</a> // 
            <a href="https://www.depts.ttu.edu/irim/">Institutional Research</a> 

            <h1 style="color: white;">Data Request Form</h1>
        </div>

        <p><em>(Fields marked with <span class="required">*</span> are required)</em></p>

        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="error-summary" HeaderText="Please correct the following errors:" DisplayMode="BulletList" />

        <h2>Online Data Request Form</h2>
        <table>
            <tr>
                <td class="label">Requested By <span class="required">*</span>:</td>
                <td>
                    <asp:TextBox ID="txtRequestedBy" runat="server" CssClass="required-input"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqRequestedBy" runat="server" ControlToValidate="txtRequestedBy" ErrorMessage="Requested By is required." CssClass="validators" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label">College/Company:</td>
                <td><asp:TextBox ID="txtCollegeCompany" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="label">Department:</td>
                <td><asp:TextBox ID="txtDepartment" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="label">Phone (xxx-xxx-xxxx):</td>
                <td>
                <asp:TextBox ID="txtPhone" runat="server" 
                    placeholder="(123) 456-7890"
                    oninput="fmt(this)"
                    MaxLength="14">
                    </asp:TextBox>                    
                    <asp:RegularExpressionValidator ID="regPhone" runat="server" 
                    ControlToValidate="txtPhone"
                    ValidationExpression="^\(\d{3}\)\s\d{3}-\d{4}$"
                    ErrorMessage="Phone must be in (xxx) xxx-xxxx format (exactly 10 digits)."
                    CssClass="validators" 
                    Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label">Email <span class="required">*</span>:</td>
                <td>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="validators" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="regEmail" runat="server" ControlToValidate="txtEmail" 
                        ValidationExpression="^[\w\.-]+@[\w\.-]+\.\w+$" ErrorMessage="Invalid email format." CssClass="validators" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label">Date Submitted <span class="required">*</span>:</td>
                <td>
                    <asp:TextBox ID="txtDateSubmitted" runat="server" TextMode="Date" ReadOnly="true"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqDateSubmitted" runat="server" ControlToValidate="txtDateSubmitted" ErrorMessage="Date Submitted is required." CssClass="validators" />
                </td>
            </tr>
            <tr>
                <td class="label">Date Report Needed <span class="required">*</span>:</td>
                <td>
                    <asp:TextBox ID="txtDateNeeded" runat="server" TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqDateNeeded" runat="server" ControlToValidate="txtDateNeeded" ErrorMessage="Date Report Needed is required." CssClass="validators" Display="Dynamic" />
                    <asp:CompareValidator ID="cmpDateNeeded" runat="server" ControlToValidate="txtDateNeeded" ControlToCompare="txtDateSubmitted" 
                        Operator="GreaterThanEqual" Type="Date" ErrorMessage="Date Needed must be at least 14 days from today." CssClass="validators" Display="Dynamic" />
                </td>
            </tr>
        </table>

        <h2>Request Details</h2>
        <table>
            <tr>
                <td class="label">Request Title <span class="required">*</span>:</td>
                <td>
                    <asp:TextBox ID="txtRequestTitle" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqTitle" runat="server" ControlToValidate="txtRequestTitle" ErrorMessage="Request Title is required." CssClass="validators" />
                </td>
            </tr>
            <tr>
                <td class="label">Have you contacted IR Staff? If so, name of IR Contact:</td>
                <td><asp:TextBox ID="txtIRContact" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="label">Attachment (pdf, word, excel, txt, jpeg, png):</td>
                <td>
                    <asp:FileUpload ID="fuAttachment" runat="server" />
                    <asp:CustomValidator ID="valFileType" runat="server" OnServerValidate="valFileType_ServerValidate" ErrorMessage="Invalid file type or size (max 10MB). Allowed: pdf, doc/docx, xls/xlsx, txt, jpg/jpeg, png." CssClass="validators" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label" valign="top">Request Details <span class="required">*</span>:<br /><small>(Provide detailed information about the data needed.)</small></td>
                <td>
                    <asp:TextBox ID="txtRequestDetails" runat="server" TextMode="MultiLine" Rows="10"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="reqDetails" runat="server" ControlToValidate="txtRequestDetails" ErrorMessage="Request Details is required." CssClass="validators" />
                </td>
            </tr>
        </table>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit Request" OnClick="btnSubmit_Click" />

        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>
        <asp:Label ID="lblError" runat="server" CssClass="error-summary" Visible="false"></asp:Label>

        <hr />
        <p style="text-align:center; font-size:0.9em;">
            © Texas Tech University | All Rights Reserved
        </p>
        <p style="text-align:center;">
            For additional information, contact us at <a href="mailto:IRIM@ttu.edu">IRIM@ttu.edu</a> or (806) 742-2166.
        </p>
    </form>
<!-- Real-time US Phone Formatting: (123) 456-7890 -->
<script type="text/javascript">
    function fmt(el) {
        let d = el.value.replace(/\D/g, '').slice(0, 10);
        el.value = d.length < 1 ? '' :
            '(' + d.slice(0, 3) + (d.length > 3 ? ') ' + d.slice(3, 6) : '') +
            (d.length > 6 ? '-' + d.slice(6) : '');
    }
</script>
</body>
</html>
