<%@ Page Language="C#" MasterPageFile="~/Shared/EOS.Master" AutoEventWireup="True"
    EnableEventValidation="true" CodeBehind="LapChietTinhHue.aspx.cs" Inherits="EOSCRM.Web.Forms.ThietKe.LapChietTinhHue" %>

<%@ Register Src="../../UserControls/FilterPanel.ascx" TagName="FilterPanel" TagPrefix="bwaco" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="EOSCRM.Util" %>
<%@ Import Namespace="EOSCRM.Web.Common" %>
<%@ Register Assembly="EOSCRM.Controls" Namespace="EOSCRM.Controls" TagPrefix="eoscrm" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" %>
<asp:Content ID="head" ContentPlaceHolderID="headCPH" runat="server">
    
    <script type="text/javascript">
        $(document).ready(function () {
            $("#divVatTu").dialog({
                autoOpen: false,
                modal: true,
                minHeight: 20,
                height: 'auto',
                width: 'auto',
                resizable: false,
                open: function (event, ui) {
                    $(this).parent().appendTo("#divVatTuDlgContainer");
                }
            });

        });
        $(document).ready(function () {
            $("#divInFree").dialog({
                autoOpen: false,
                modal: true,
                minHeight: 50,
                height: 500,
                width: 800,
                resizable: true,
                open: function (event, ui) {
                    $(this).parent().appendTo("#divInFreeContainer");
                }

            });
        });

        $(document).ready(function () {
            $("#divInCT").dialog({
                autoOpen: false,
                modal: true,
                minHeight: 50,
                height: 500,
                width: 800,
                resizable: true,
                open: function (event, ui) {
                    $(this).parent().appendTo("#divInCTContainer");
                }

            });
        });

        function formatCurrency(num) {
            num = num.toString().replace(/$|,/g, '');
            if (isNaN(num))
                num = "0";
            sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            num = Math.floor(num / 100).toString();
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + '.' +
            num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num);
        }
        /* phần vật tư miễn phí */

        function CheckFormMAVTKeyPress(e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            jQuery.fn.exists = function () { return jQuery(this).length > 0; };
            if (code == 13 || code == 9) {
                openWaitingDialog();
                unblockWaitingDialog();
                window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(linkBtnChangeMAVT) %>', '');
            }
        }

        function CheckFormKhoiLuongKeyPress(e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            jQuery.fn.exists = function () { return jQuery(this).length > 0; };
            if (code == 13 || code == 9) {
                openWaitingDialog();
                unblockWaitingDialog();
                window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(linkBtnChangeKhoiLuong) %>', '');
            }
        }

        /* phần vật tư khách hàng thanh toán */

        function CheckFormMAVT117KeyPress(e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            jQuery.fn.exists = function () { return jQuery(this).length > 0; };
            if (code == 13 || code == 9) {
                openWaitingDialog();
                unblockWaitingDialog();
                window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(linkBtnChangeMAVT117) %>', '');
            }
        }

        function CheckFormKhoiLuong117KeyPress(e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            jQuery.fn.exists = function () { return jQuery(this).length > 0; };
            if (code == 13 || code == 9) {
                openWaitingDialog();
                unblockWaitingDialog();
                window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(linkBtnChangeKhoiLuong117) %>', '');
            }
        }

        function CheckFormAddGhiChu() {
            openWaitingDialog();
            unblockWaitingDialog();

            window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(btnAddGhiChu) %>', '');

            return false;
        }

        function CheckFormFilterVatTu() {
            openWaitingDialog();
            unblockWaitingDialog();

            window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(btnFilterVatTu) %>', '');

            return false;
        }

        function CheckFormSave() {
            openWaitingDialog();
            unblockWaitingDialog();

            window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(btnSave) %>', '');

            return false;
        }

        function CheckFormProfile() {
            openWaitingDialog();
            unblockWaitingDialog();

            window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(btnProfile) %>', '');

            return false;
        }

        function checkChange() {

            if (confirm('Tất cả vật tư có sẵn sẽ bị thay thế bởi mẫu bốc vật tư. Đổi?')) {
                openWaitingDialog();
                unblockWaitingDialog();

                window.__doPostBack('<%= CommonFunc.UniqueIDWithDollars(btnChange) %>', '');
            }
            return false;
        }

        function updateCTCT(maDDK, maVT, txtSLId, txtSLKH, txtGIAVTId, txtGIANCId, lblTIENVT, lblTIENNC, sttClientId) {
            openWaitingDialog();
            unblockWaitingDialog();
            var sl = document.getElementById(txtSLId).value;
            var slkh = document.getElementById(txtSLKH).value;
            var gvt = document.getElementById(txtGIAVTId).value;
            var gnc = document.getElementById(txtGIANCId).value;
            var stt = document.getElementById(sttClientId).value;
            var msg = window.EOSCRM.Web.Common.AjaxCRM.UpdateCTCT(maDDK, maVT, sl, slkh, gvt, gnc, stt);

            if (msg.value != "<%= DELIMITER.Passed %>" && msg.value != "<%= DELIMITER.Failed %>") {
                var idArr = msg.value.split("<%= DELIMITER.Delimiter %>");
                if (idArr.length == 4) {
                    document.getElementById(txtSLId).value = idArr[0];
                    document.getElementById(txtSLKH).value = idArr[1];
                    document.getElementById(txtGIAVTId).value = idArr[2];
                    document.getElementById(txtGIANCId).value = idArr[3];
                }
            }
            if (msg.value == "<%= DELIMITER.Passed %>") {
                document.getElementById(lblTIENVT).innerHTML = formatCurrency(parseInt(sl) * gvt);
                document.getElementById(lblTIENNC).innerHTML = formatCurrency((parseInt(sl) + parseInt(slkh)) * gnc);
            }
            closeWaitingDialog();
        }

        function updateCTCT117(maDDK, maVT, txtSLId, txtSLKH, txtGIAVTId, txtGIANCId, lblTIENVT, lblTIENNC, sttClientId) {
            openWaitingDialog();
            unblockWaitingDialog();
            var sl = document.getElementById(txtSLId).value;
            var slkh = document.getElementById(txtSLKH).value;
            var gvt = document.getElementById(txtGIAVTId).value;
            var gnc = document.getElementById(txtGIANCId).value;
            var stt = document.getElementById(sttClientId).value;
            var msg = window.EOSCRM.Web.Common.AjaxCRM.UpdateCTCT117(maDDK, maVT, sl, slkh, gvt, gnc, stt);
            if (msg.value != "<%= DELIMITER.Passed %>" && msg.value != "<%= DELIMITER.Failed %>") {
                var idArr = msg.value.split("<%= DELIMITER.Delimiter %>");
                if (idArr.length == 4) {
                    document.getElementById(txtSLId).value = idArr[0];
                    document.getElementById(txtSLKH).value = idArr[1];
                    document.getElementById(txtGIAVTId).value = idArr[2];
                    document.getElementById(txtGIANCId).value = idArr[3];
                }
            }
            if (msg.value == "<%= DELIMITER.Passed %>") {
                document.getElementById(lblTIENVT).innerHTML = formatCurrency(parseInt(sl) * gvt);
                document.getElementById(lblTIENNC).innerHTML = formatCurrency((parseInt(sl) + parseInt(slkh)) * gnc);
            }
            closeWaitingDialog();
        }

        function updateGCCT(maGC, clientId) {
            var noidung = document.getElementById(clientId).value;
            var msg = window.EOSCRM.Web.Common.AjaxCRM.UpdateGCCT(maGC, noidung);

            if (msg.value != "<%= DELIMITER.Passed %>" && msg.value != "<%= DELIMITER.Failed %>") {
                document.getElementById(clientId).value = msg.value;
            }
        }

        closeWaitingDialog();
        
    </script>
</asp:Content>
<asp:Content ID="content" ContentPlaceHolderID="mainCPH" runat="server">
    <bwaco:FilterPanel ID="filterPanel" runat="server" />
    <br />
    
    <div id="divVatTuDlgContainer">
        <div id="divVatTu" style="display: none">
            <asp:UpdatePanel ID="upnlVatTu" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table cellpadding="3" cellspacing="1" style="width: 700px;">
                        <tr>
                            <td class="crmcontainer">
                                <table class="crmtable">
                                    <tbody>
                                        <tr>
                                            <td class="crmcell right">
                                                Từ khóa
                                            </td>
                                            <td class="crmcell">
                                                <div class="left">
                                                    <asp:TextBox ID="txtFilterVatTu" onchange="return CheckFormFilterVatTu();" runat="server" />
                                                </div>
                                                <div class="left">
                                                    <asp:Button ID="btnFilterVatTu" OnClientClick="return CheckFormFilterVatTu();" runat="server"
                                                        CssClass="filter" UseSubmitBehavior="false" OnClick="btnFilterVatTu_Click" />
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="ptop-10">
                                <div class="crmcontainer">
                                    <eoscrm:Grid ID="gvVatTu" runat="server" UseCustomPager="true" OnRowDataBound="gvVatTu_RowDataBound"
                                        OnRowCommand="gvVatTu_RowCommand" OnPageIndexChanging="gvVatTu_PageIndexChanging">
                                        <PagerSettings FirstPageText="vật tư" PageButtonCount="2" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Mã VT" HeaderStyle-Width="15%">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkBtnID" runat="server" CommandArgument='<%# Eval("MAVT") %>'
                                                        CommandName="EditItem" Text='<%# Eval("MAVT") %>'></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="Mã hiệu" DataField="MAHIEU" HeaderStyle-Width="10%" />
                                            <asp:BoundField HeaderText="Tên vật tư" DataField="TENVT" HeaderStyle-Width="45%" />
                                            <asp:BoundField HeaderText="Đơn vị tính" DataField="MADVT" HeaderStyle-Width="10%" />
                                            <asp:CheckBoxField HeaderText="Vật tư" DataField="ISVATTU" HeaderStyle-Width="5%" />
                                            <asp:BoundField HeaderText="Giá VT" DataField="GIAVT" HeaderStyle-Width="10%" />
                                            <asp:BoundField HeaderText="Giá NC" DataField="GIANC" HeaderStyle-Width="10%" />
                                        </Columns>
                                    </eoscrm:Grid>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="divInCTContainer">
        <div id="divInCT" style="display: none">
            <asp:UpdatePanel ID="uplInCT" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="crmcontainer" id="divCR3" runat="server">
                        <rsweb:ReportViewer ID="rpViewer3" runat="server" Font-Names="Verdana" Font-Size="8pt"
                            InteractiveDeviceInfos="(Collection)" ShowParameterPrompts="true" AsyncRendering="false"
                            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" SizeToReportContent="True">
                        </rsweb:ReportViewer>
                    </div>
                    <div class="crmcontainer" id="divCR4" runat="server">
                        <rsweb:ReportViewer ID="rpViewer4" runat="server" Font-Names="Verdana" Font-Size="8pt"
                            InteractiveDeviceInfos="(Collection)" ShowParameterPrompts="true" AsyncRendering="false"
                            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="1000px" SizeToReportContent="True"
                            Width="800px">
                        </rsweb:ReportViewer>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="divInFreeContainer">
        <div id="divInFree" style="display: none">
            <asp:UpdatePanel ID="uplInFree" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="crmcontainer" id="divCR" runat="server">
                        <rsweb:ReportViewer ID="rpViewer" runat="server" Font-Names="Verdana" Font-Size="8pt"
                            InteractiveDeviceInfos="(Collection)" ShowParameterPrompts="true" AsyncRendering="false"
                            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" SizeToReportContent="True">
                        </rsweb:ReportViewer>
                    </div>
                    <div class="crmcontainer" id="divCR2" runat="server">
                        <rsweb:ReportViewer ID="rpViewer2" runat="server" Font-Names="Verdana" Font-Size="8pt"
                            InteractiveDeviceInfos="(Collection)" ShowParameterPrompts="true" AsyncRendering="false"
                            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="1000px" SizeToReportContent="True"
                            Width="800px">
                        </rsweb:ReportViewer>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="crmcontainer" id="divGridList" runat="server">
        <eoscrm:Grid ID="gvList" runat="server" UseCustomPager="true" OnRowDataBound="gvList_RowDataBound"
            OnRowCommand="gvList_RowCommand" OnPageIndexChanging="gvList_PageIndexChanging"
            PageSize="20">
            <PagerSettings FirstPageText="thiết kế" PageButtonCount="2" />
            <Columns>
                <asp:TemplateField HeaderText="Mã đơn" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkBtnID" runat="server" CommandArgument='<%# Eval("MADDK") %>'
                            CommandName="EditItem" Text='<%# Eval("MADDK") %>'></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Font-Bold="True" />
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="25%" HeaderText="Tên khách hàng" DataField="TENKH" />
                <%--<asp:BoundField HeaderStyle-Width="75px" HeaderText="Điện thoại" DataField="DIENTHOAI" />--%>
                <asp:TemplateField HeaderStyle-Width="75px" HeaderText="Điện thoại">
                     <ItemTemplate>
                            <%# Eval("DIDONG") != "" && Eval("DIDONG") != null ? Eval("DIDONG") : Eval("DIENTHOAI") %>
                     </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="35%" HeaderText="Địa chỉ lắp đặt" DataField="DIACHILD" />
                <asp:TemplateField HeaderStyle-Width="75px" HeaderText="Ngày khảo sát">
                    <ItemTemplate>
                        <%# (Eval("NGAYKS") != null) ?
                                String.Format("{0:dd/MM/yyyy}", Eval("NGAYKS"))
                                : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="30px" HeaderText="Loại" DataField="LOAIDK" />
                <asp:TemplateField HeaderText="Hoạt động" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkBtnID2" CssClass="a-maddk" runat="server" CommandArgument='<%# Eval("MADDK") %>'
                            CommandName="EditItem" Text='Dự toán'><div class="div-maddk <%# Eval("MADDK") %>"></div></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </eoscrm:Grid>
    </div>
    <div class="crmcontainer" id="divGridListF" runat="server">
        <eoscrm:Grid ID="gvListF" runat="server" UseCustomPager="true" OnRowDataBound="gvListF_RowDataBound"
            OnRowCommand="gvListF_RowCommand" OnPageIndexChanging="gvListF_PageIndexChanging"
            PageSize="20">
            <PagerSettings FirstPageText="thiết kế" PageButtonCount="2" />
            <Columns>
                <asp:TemplateField HeaderText="Mã đơn" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkBtnDT1F" runat="server" CommandArgument='<%# Eval("MADDK") %>'
                            CommandName="EditItem" Text='<%# Eval("MADDK") %>'></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Font-Bold="True" />
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="25%" HeaderText="Tên khách hàng" DataField="TENKH" />
                <%--<asp:BoundField HeaderStyle-Width="75px" HeaderText="Điện thoại" DataField="DIENTHOAI" />--%>
                <asp:TemplateField HeaderStyle-Width="75px" HeaderText="Điện thoại">
                     <ItemTemplate>
                            <%# Eval("DIDONG") != "" && Eval("DIDONG") != null ? Eval("DIDONG") : Eval("DIENTHOAI") %>
                     </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="35%" HeaderText="Địa chỉ lắp đặt" DataField="DIACHILD" />
                <asp:TemplateField HeaderStyle-Width="75px" HeaderText="Ngày khảo sát">
                    <ItemTemplate>
                        <%# (Eval("NGAYKS") != null) ? String.Format("{0:dd/MM/yyyy}", Eval("NGAYKS"))
                                : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderStyle-Width="30px" HeaderText="Loại" DataField="LOAIDK" />
                <asp:TemplateField HeaderText="Hoạt động" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkBtnDTF" CssClass="a-maddk" runat="server" CommandArgument='<%# Eval("MADDK") %>'
                            CommandName="EditItem" Text='Dự toán lại'><div class="div-maddk <%# Eval("MADDK") %>"></div></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </eoscrm:Grid>
    </div>
    <asp:UpdatePanel ID="upnlInfor" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div id="divChietTinhInfo" class="crmcontainer" runat="server" visible="false">
                <div class="crmcontainer">
                    <table class="crmtable">
                        <tbody>
                            <tr>
                                <td class="crmcell right width-75 ">
                                    Loại đơn
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:DropDownList AutoPostBack="True" ID="ddlLoaiDon" runat="server" OnSelectedIndexChanged="ddlLoaiDon_SelectedIndexChanged">
                                            <asp:ListItem Text="Đơn đăng ký" Value="DK" />
                                            <asp:ListItem Text="Đơn cải tạo" Value="CT" />
                                             <asp:ListItem Text="Bổ sung dự toán" Value="BX" />
                                             <asp:ListItem Text="Lắp đặt có phí" Value="DKP" />
                                        </asp:DropDownList>
                                    </div>
                                </td>
                                <td class="crmcell right width-75">
                                    Họ tên
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtTENKH" runat="server" Width="150px" MaxLength="200" TabIndex="22" />
                                    </div>
                                </td>
                                <td class="crmcell right width-75">
                                    Điện thoại
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtDIENTHOAI" MaxLength="15" runat="server" TabIndex="71" Width="100px" />
                                    </div>
                                </td>
                                <td class="crmcell right width-75">
                                    Di động
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtDiDong" MaxLength="15" runat="server" TabIndex="71" Width="100px" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right">
                                    Số nhà
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtSoNha" runat="server" Width="150px" TabIndex="9">
                                        </asp:TextBox>
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    Đường
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <eoscrm:TextBox ID="txtMaDp" PostBackControlId="linkDp" runat="server" MaxLength="9"
                                            Width="40px" TabIndex="4" />
                                        <asp:LinkButton ID="linkDp" CausesValidation="false" Style="display: none" OnClick="linkDp_Click"
                                            runat="server">Update MADP</asp:LinkButton>
                                        <asp:DropDownList AutoPostBack="True" ID="ddlTenDuong" runat="server" Width="150px" />
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    Phường
                                </td>
                                <td class="crmcell" colspan="2">
                                    <div class="left">
                                        <eoscrm:TextBox ID="txtTatPhuong" PostBackControlId="linkPhuong" runat="server" MaxLength="4"
                                            Width="30px" TabIndex="4" />
                                        <asp:LinkButton ID="linkPhuong" CausesValidation="false" Style="display: none" OnClick="linkPhuong_Click"
                                            runat="server">Update MADP</asp:LinkButton>
                                    </div>
                                    <div class="left">
                                        <asp:DropDownList ID="ddlPhuong" runat="server" TabIndex="1" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right">
                                    Công tác
                                </td>
                                <td class="crmcell" colspan="3">
                                    <div class="left">
                                        <asp:TextBox ID="txtTenHM" runat="server" MaxLength="500" Width="250px"></asp:TextBox>
                                    </div>
                                    <div class="left"><strong>Lộ trình</strong></div>
                                     <div class="left">
                                        <asp:TextBox ID="txtMaLoTrinh" runat="server" MaxLength="70" Width="70px" OnTextChanged="txtMaLoTrinh_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        <asp:TextBox ID="txtTenLT" runat="server" ReadOnly="true" disabled="true"  style="color:#0075ff; font-weight: bold;"></asp:TextBox>
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    CMA
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <eoscrm:TextBox ID="txtCMA" PostBackControlId="linkCMA" runat="server" MaxLength="5"
                                            Width="40px" TabIndex="10" />
                                        <asp:LinkButton ID="linkCMA" CausesValidation="false" Style="display: none" OnClick="linkCMA_Click"
                                            runat="server">Update CMA</asp:LinkButton>
                                    </div>
                                    <div class="left">
                                        <asp:Label ID="lblCMA" runat="server" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right">
                                    Ghi chú
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtGhiChu" runat="server" TextMode="MultiLine" Rows="2" Width="800px" OnTextChanged="txtGhiChu_TextChanged"></asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right">
                                    Giảm giá
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:CheckBox ID="chkGiam" runat="server" AutoPostBack="true" OnCheckedChanged="chkGiam_CheckedChanged"
                                            onchange="openWaitingDialog()" Checked="false" />
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    Giảm ADB
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:CheckBox ID="chkGiamAdb" runat="server" AutoPostBack="true" OnCheckedChanged="chkGiamAdb_CheckedChanged"
                                            onchange="openWaitingDialog()" Checked="false" />
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    Hệ số NC
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:DropDownList ID="ddlHSNC" runat="server" Width="80px" TabIndex="9">
                                        </asp:DropDownList>
                                    </div>
                                </td>
                                <td class="crmcell right width-100">
                                    Phí KS
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:TextBox ID="txtPhiKS" runat="server" Width="70px" TabIndex="9">
                                        </asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right" colspan="6">
                                    <asp:UpdatePanel ID="uplCaiTao" UpdateMode="Conditional" runat="server">
                                        <ContentTemplate>
                                            <div class="crmcontainer">
                                                <table class="crmtable">
                                                    <tbody>
                                                        <tr>
                                                            <td class="crmcell right width-100">
                                                                Hệ số NC phụ
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtHSNCphu" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                                <div class="left">
                                                                    <asp:Label ID="lblHsnc" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right" colspan="6">
                                    <asp:UpdatePanel ID="uplGiamGia" UpdateMode="Conditional" runat="server" Visible="false">
                                        <ContentTemplate>
                                            <div class="crmcontainer">
                                                <table class="crmtable">
                                                    <tbody>
                                                        <tr>
                                                            <td class="crmcell right width-100">
                                                                Giảm NC trước
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGncTrc" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                            <td class="crmcell right width-100">
                                                                Giảm VL trước
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGvlTrc" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                            <td class="crmcell right width-100">
                                                                Giảm TC trước
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGtcTrc" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="crmcell right">
                                                                Giảm NC sau
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGncSau" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                            <td class="crmcell right">
                                                                Giảm VL sau
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGvlSau" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                            <td class="crmcell right">
                                                                Giảm TC sau
                                                            </td>
                                                            <td class="crmcell">
                                                                <div class="left">
                                                                    <asp:TextBox ID="txtGtcSau" runat="server" Width="30px" TabIndex="9">
                                                                    </asp:TextBox>%
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="crmcell right">
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <eoscrm:Button ID="btnSave" runat="server" CssClass="save" OnClick="btnSave_Click"
                                            TabIndex="16" UseSubmitBehavior="false" />
                                    </div>
                                </td>
                                <td class="crmcell">
                                    <div class="left">
                                        <asp:Button ID="btnProfile" runat="server" OnClientClick="return CheckFormProfile();"
                                            CssClass="profile" OnClick="btnProfile_Click" TabIndex="16" UseSubmitBehavior="false" />
                                    </div>
                                </td>
                                <td class="crmcell right">
                                    Trưởng phòng
                                </td>
                                <td class="crmcell" colspan="2">
                                    <div class="left">
                                        <asp:TextBox ID="txtTruongPhong" Width="190px" runat="server" Text=""></asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <asp:UpdatePanel ID="upnlCustomers" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="crmcontainer">
                <table class="crmtable">
                    <tbody>
                        <tr>
                            <td class="crmcell right">
                                Mẫu bốc vật tư:
                            </td>
                            <td class="crmcell">
                                <div class="left">
                                    <asp:DropDownList ID="ddlMBVT" Width="300px" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </td>
                            <td class="crmcell right">
                            </td>
                            <td class="crmcell">
                                <div class="left">
                                    <asp:Button ID="btnChange" runat="server" CommandArgument="Change" CssClass="change"
                                        OnClientClick="return checkChange();" OnClick="btnChange_Click" TabIndex="16"
                                        UseSubmitBehavior="false" />
                                </div>
                            </td>
                            <td class="crmcell">
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <br />
            <div class="crmcontainer">
                <table class="crmcontainer">
                    <tr>
                        <td>
                            <table class="crmtable">
                                <tr>
                                    <td class="crmcell right width-100">
                                        <strong>Mã vật tư </strong>
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:TextBox ID="txtMAVT" Width="60px" runat="server" onkeypress="return CheckFormMAVTKeyPress(event);" />
                                            <asp:LinkButton ID="linkBtnChangeMAVT" CausesValidation="false" Style="display: none"
                                                OnClick="linkBtnChangeMAVT_Click" runat="server">Change MAVT</asp:LinkButton>
                                        </div>
                                        <div class="left">
                                            <asp:Button ID="btnBrowseVatTu" runat="server" CssClass="pickup" OnClick="btnBrowseVatTu_Click"
                                                CausesValidation="false" UseSubmitBehavior="false" OnClientClick="openDialogAndBlock('Chọn từ danh sách vật tư', 700, 'divVatTu')" />
                                        </div>
                                    </td>
                                    <td class="crmcell right width-50">
                                        KL Cty
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:TextBox ID="txtKHOILUONG" Width="45px" runat="server" onkeypress="return CheckFormKhoiLuongKeyPress(event);"></asp:TextBox>
                                            <asp:LinkButton ID="linkBtnChangeKhoiLuong" CausesValidation="false" Style="display: none"
                                                OnClick="linkBtnChangeKhoiLuong_Click" runat="server">Change KL</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td class="crmcell right width-50">
                                        KL KH
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:TextBox ID="txtKLKhachHang" Width="45px" runat="server" onkeypress="return CheckFormKhoiLuongKeyPress(event);"
                                                Text="0"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td class="crmcell right width-150">
                                    </td>
                                    <td class="crmcell">
                                    </td>
                                </tr>
                            </table>
                            <div class="crmcontainer" style="border-left: none !important; border-right: none !important;">
                                <eoscrm:Grid ID="gvSelectedVatTu" runat="server" UseCustomPager="true" PageSize="2000"
                                    OnRowCommand="gvSelectedVatTu_RowCommand" OnRowDataBound="gvSelectedVatTu_RowDataBound">
                                    <PagerSettings FirstPageText="vật tư" PageButtonCount="2" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" HeaderStyle-CssClass="checkbox">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="STT" HeaderStyle-Width="20px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSTT" Text='<%# Eval("STT") ?? "1" %>' Width="30px" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mã VT" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("MAVT") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mã hiệu" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.MAHIEU") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nội dung công việc">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.TENVT") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ĐVT" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.DVT.TENDVT")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="KL CTY" HeaderStyle-Width="40px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSOLUONG" Text='<%# Bind("SLCTY") %>' Width="40px"  runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="KL KH" HeaderStyle-Width="40px">
                                            <ItemTemplate>
                                               
                                                <asp:TextBox ID="txtSLKH" Text='<%# Bind("SLKH") %>' Width="40px" runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" HeaderStyle-Width="0px">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="hidden" ID="txtGIAVT" Text='<%# Bind("GIAVT") %>' Width="0px"
                                                    runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tiền vật tư" HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                            
                                                <asp:Label ID="lblTIENVT" Text='<%# String.Format(new CultureInfo("en-US"), "{0:0,0}", Eval("TIENVT")).Replace(",",".")%>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                         <ItemStyle HorizontalAlign="Right" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" HeaderStyle-Width="0px" >
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtGIANC" CssClass="hidden" Text='<%# Bind("GIANC") %>' Width="0px"
                                                    runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tiền NC" HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTIENNC" Text=' <%# String.Format(new CultureInfo("en-US"), "{0:0,0}", Eval("TIENNC")).Replace(",", ".")%>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Right" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hoạt động" HeaderStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDelete" Text="Xóa" CommandName="DeleteVatTu" CausesValidation="false"
                                                    CommandArgument='<%# Eval("MAVT")%>' runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </eoscrm:Grid>
                            </div>
                        </td>
                    </tr>
                    <tr style="display: none">
                        <td class="header">
                            Ghi chú
                        </td>
                    </tr>
                    <tr style="display: none">
                        <td>
                            <table class="crmtable">
                                <tr>
                                    <td class="crmcell right">
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:Button ID="btnAddGhiChu" runat="server" CssClass="addnew" OnClientClick="return CheckFormAddGhiChu();"
                                                OnClick="btnAddGhiChu_Click" CausesValidation="false" UseSubmitBehavior="false" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <div class="crmcontainer" style="border-left: none !important; border-right: none !important;">
                                <eoscrm:Grid ID="gvGhiChu" runat="server" UseCustomPager="true" PageSize="2000" OnRowCommand="gvGhiChu_RowCommand"
                                    OnRowDataBound="gvGhiChu_RowDataBound">
                                    <PagerSettings FirstPageText="ghi chú" PageButtonCount="2" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" HeaderStyle-CssClass="checkbox">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ghi chú">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtNOIDUNG" Text='<%# Bind("NOIDUNG") %>' Width="98%" TextMode="MultiLine"
                                                    runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hoạt động" HeaderStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDelete" Text="Xóa" CommandName="DeleteGhiChu" CausesValidation="false"
                                                    CommandArgument='<%# Eval("MAGC")%>' runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </eoscrm:Grid>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <div class="crmcontainer">
                <table class="crmcontainer">
                    <tr>
                        <td>
                            <table class="crmtable">
                                <tr>
                                    <td class="crmcell right width-100">
                                        <strong>Mã vật tư </strong>
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:TextBox ID="txtMAVT117" Width="60px" runat="server" onkeypress="return CheckFormMAVT117KeyPress(event);" />
                                            <asp:LinkButton ID="linkBtnChangeMAVT117" CausesValidation="false" Style="display: none"
                                                OnClick="linkBtnChangeMAVT117_Click" runat="server">Change MAVT 117</asp:LinkButton>
                                        </div>
                                        <div class="left">
                                            <asp:Button ID="btnBrowseVatTu117" runat="server" CssClass="pickup" OnClick="btnBrowseVatTu117_Click"
                                                CausesValidation="false" UseSubmitBehavior="false" OnClientClick="openDialogAndBlock('Chọn từ danh sách vật tư', 700, 'divVatTu')" />
                                        </div>
                                    </td>
                                    <td class="crmcell right width-50">
                                        KL Cty
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:TextBox ID="txtKHOILUONG117" Width="45px" runat="server" onkeypress="return CheckFormKhoiLuong117KeyPress(event);"></asp:TextBox>
                                            <asp:LinkButton ID="linkBtnChangeKhoiLuong117" CausesValidation="false" Style="display: none"
                                                OnClick="linkBtnChangeKhoiLuong117_Click" runat="server">Change KL 117</asp:LinkButton>
                                        </div>
                                    </td>
                                    <td class="crmcell right width-50">
                                        KL KH
                                    </td>
                                    <td class="crmcell width-150">
                                        <div class="left">
                                            <asp:TextBox ID="txtKLKhachHang117" Width="45px" runat="server" Text="0" onkeypress="return CheckFormKhoiLuong117KeyPress(event);"></asp:TextBox>
                                        </div>
                                        <div class="left">
                                            <!-- <input onclick="setData()" type="button" style="background-image:url(/content/images/common/buttons.png); width: 60px; background-position: 0px -308px; " /> -->
                                            <div>
                                               <eoscrm:Button ID="btnSave1" runat="server" CssClass="save" OnClick="btnSave1_Click1"
                                                UseSubmitBehavior="false" />
                                            </div>
                                            
                                            <%-- <asp:Button ID="btnSave1" runat="server" Text="Button" onclick="btnSave1_Click1" />--%>
                                        </div>
                                    </td>
                                    <td class="crmcell right width-100">
                                        <div class="left">
                                            <button onclick="setData()" type="button" style="width: 60px">Ký số</button>
                                        </div>
                                    </td>
                                    <td class="crmcell right width-100">
                                        <div class="left">
                                        In dự toán
                                    </td>
                                    <td class="crmcell">
                                        <div class="left">
                                            <asp:Button ID="btnFree" runat="server" CssClass="report" OnClick="btnFree_Click"
                                                Visible="false" CausesValidation="false" UseSubmitBehavior="false" OnClientClick="openDialogAndBlock('Chọn in miễn phí', 1000, 'divInFree')" />
                                        </div>
                                        <div class="left">
                                            <asp:Button ID="btnCT" runat="server" CssClass="report" OnClick="btnCT_Click" CausesValidation="false"
                                                Visible="false" UseSubmitBehavior="false" OnClientClick="openDialogAndBlock('Chọn in dự toán cải tạo', 1000, 'divInCT')" />
                                        </div>
                                        <div class="left">
                                            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click" CssClass="back" ToolTip="Trở về danh sách dự toán" />
                                        </div>
                                    </td>
                                    <td class="crmcell right width-100">
                                    </td>
                                </tr>
                            </table>
                            <div class="crmcontainer" style="border-left: none !important; border-right: none !important;">
                                <eoscrm:Grid ID="gvSelectedVatTu117" runat="server" UseCustomPager="true" PageSize="2000"
                                    OnRowCommand="gvSelectedVatTu117_RowCommand" OnRowDataBound="gvSelectedVatTu117_RowDataBound">
                                    <PagerSettings FirstPageText="vật tư" PageButtonCount="2" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" HeaderStyle-CssClass="checkbox">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="STT" HeaderStyle-Width="20px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSTT" Text='<%# Eval("STT") ?? "1" %>' Width="30px" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mã VT" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("MAVT") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mã hiệu" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.MAHIEU") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nội dung công việc">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.TENVT") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ĐVT" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Eval("VATTU.DVT.TENDVT")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="KL CTY" HeaderStyle-Width="40px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSOLUONG" Text='<%# Bind("SLCTY") %>' Width="40px" runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="KL KH" HeaderStyle-Width="40px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSLKH" Text='<%# Bind("SLKH") %>' Width="40px" runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" HeaderStyle-Width="0px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtGIAVT" CssClass="hidden" Text='<%# Bind("GIAVT") %>' Width="0px"
                                                    runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tiền vật tư" HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTIENVT" Text='<%# String.Format(new CultureInfo("en-US"), "{0:0,0}", Eval("TIENVT")).Replace(",",".")%>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" HeaderStyle-Width="0px">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtGIANC" CssClass="hidden" Text='<%# Bind("GIANC") %>' Width="0px"
                                                    runat="server"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tiền NC" HeaderStyle-Width="70px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTIENNC" Text=' <%# String.Format(new CultureInfo("en-US"), "{0:0,0}", Eval("TIENNC")).Replace(",", ".")%>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hoạt động" HeaderStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnDelete" Text="Xóa" CommandName="DeleteVatTu" CausesValidation="false"
                                                    CommandArgument='<%# Eval("MAVT")%>' runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </eoscrm:Grid>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div class="crmcontainer p-5">
        <a href="DuyetChietTinh.aspx">Chuyển sang bước kế tiếp</a>
    </div>
    <script>
        window.addEventListener('pageshow', function(event) {
            let trangThaiKySo = JSON.parse(localStorage.getItem("isKySoSuccess"))
            console.log(trangThaiKySo)
            if(trangThaiKySo!=null && trangThaiKySo==true){
                $("#ctl00_mainCPH_btnSave1").click()
                localStorage.setItem("isKySoSuccess", false)
            }
        });
        let HoTenNv = "<%=HoTenNv%>"
        let TebPb = "<%=TebPb%>"
        let TenHieu = "<%=TenHieu%>"
        let DiaChi = "<%=DiaChi%>"
        let DienThoai = "<%=DienThoai%>"
        if(HoTenNv!="") localStorage.setItem('HoTenNv', HoTenNv);
        if(TebPb!="") localStorage.setItem("TebPb", TebPb);
        if(TenHieu!="") localStorage.setItem("TenHieu", TenHieu);
        if(DiaChi!="") localStorage.setItem("DiaChi", DiaChi);
        if(DienThoai!="") localStorage.setItem("DienThoai", DienThoai);
        
        const listAMaddk = $(".a-maddk");
        for (let i = 0; i < listAMaddk.length; i++) {
            $(listAMaddk[i]).on("click", function () {
                let maddk = $($(".div-maddk")[i]).attr("class").split(" ")[1]
                localStorage.setItem("maddk", maddk)
            })
        }
        
        function setData() {
            $("#loginvnpt").dialog("open");
            $("#userName-vnpt").val(localStorage.getItem("keyUser"));
            $("#password-vnpt").val(localStorage.getItem("keyPass"));
            let objStr = {
                MADDK : localStorage.getItem("maddk"),
                TenPB: localStorage.getItem("TebPb"),
                TenTP: $("#ctl00_mainCPH_txtTruongPhong").val(),
                TenHM: $("#ctl00_mainCPH_txtTenHM").val(),
                TenNguoiLap: localStorage.getItem("HoTenNv"),
                TenHieu: localStorage.getItem("TenHieu"),
                DiaChi: localStorage.getItem("DiaChi"),
                DienThoai: localStorage.getItem("DienThoai")
            }
            let url = window.location.href;
            localStorage.setItem("listKeyBase64", JSON.stringify(["base64DuToanStr"]))
            localStorage.setItem("listValueBase64", JSON.stringify([objStr]))
            
            localStorage.setItem("apiSetBase64", "/Forms/ThietKe/LapChietTinhHue.aspx/GetBase64RDLC")
            localStorage.setItem("apiLoginVNPT", "/Forms/ThietKe/LapChietTinhHue.aspx/LoginVnpt")
            localStorage.setItem("apiSign", "/Forms/DongMoNuocOnline/Manager_DuyetTNCN.aspx/Sign")
            localStorage.setItem("apiLuuKySo", "/Forms/ThietKe/LapChietTinhHue.aspx/LuuFileKySo")
            
            localStorage.setItem("listKeyLuuKySo", JSON.stringify(["luuKySoReqStr"]))
            let objKySoParam = {
                MaDDK: objStr.MADDK
            }
            localStorage.setItem("listValueLuuKySo", JSON.stringify([objKySoParam]))

            localStorage.setItem("preUrl", url)
            localStorage.setItem("tenVb", "Duyet_Du_Toan_Lap_Dat_"+objStr.MADDK)
            
            window.location.href = "/Forms/KySo/KySo.aspx"
        }
        
        
    </script>
</asp:Content>
