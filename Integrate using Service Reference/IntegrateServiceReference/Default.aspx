<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IntegrateServiceReference.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnPay" runat="server" Text="Pay" OnClick="btnPay_Click" />
    </div>
    <asp:Label ID="lblResult" runat="server" Text="Result"></asp:Label>
    </form>
</body>
</html>
