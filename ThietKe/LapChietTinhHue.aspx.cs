using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using EOSCRM.Dao;
using EOSCRM.Domain;
using EOSCRM.Util;
using EOSCRM.Web.Common;
using Microsoft.Reporting.WebForms;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using System.IO;
using itextSharpPDF = iTextSharp.text.pdf;
using System.Configuration;

namespace EOSCRM.Web.Forms.ThietKe
{
    public partial class LapChietTinhHue : Authentication
    {
        public string TenTP = "";//txtTruongPhong
        public string HoTenNv = "";//LoginInfo.NHANVIEN.HOTEN
        public string TebPb = "";//LoginInfo.NHANVIEN.PHONGBAN.TENPB
        public string TenHieu = "";//LoginInfo.NHANVIEN.KHUVUC.TENHIEU
        public string DiaChi = "";//LoginInfo.NHANVIEN.KHUVUC.DIACHI
        public string DienThoai = "";//LoginInfo.NHANVIEN.KHUVUC.DIENTHOAI

        #region Import Dao
        private readonly DonDangKyDao ddkDao = new DonDangKyDao();
        private readonly ChietTinhDao ctDao = new ChietTinhDao();
        private readonly NhanVienDao nvDao = new NhanVienDao();
        private readonly ChiTietChietTinhNd117Dao ctct117Dao = new ChiTietChietTinhNd117Dao();
        private readonly GhiChuChietTinhDao gcDao = new GhiChuChietTinhDao();
        private readonly VatTuDao vtDao = new VatTuDao();
        private readonly DvtDao dvtDao = new DvtDao();
        private readonly MauBocVatTuDao mbvtDao = new MauBocVatTuDao();
        private readonly ChiPhiDao cpDao = new ChiPhiDao();
        private readonly HeSoDao hsDao = new HeSoDao();
        private readonly CTQuyetToanDao ctqtDao = new CTQuyetToanDao();
        private readonly QuyetToanDao qtDao = new QuyetToanDao();
        private readonly CTQuyetToan117Dao ctqt117Dao = new CTQuyetToan117Dao();
        private readonly ChiTietChietTinhDao ctctDao = new ChiTietChietTinhDao();
        private readonly DuongPhoDao dpDao = new DuongPhoDao();
        private readonly DuongPhoLDDao dpldDao = new DuongPhoLDDao();
        private readonly PhuongDao pDao = new PhuongDao();
        private readonly KhuVucDao kvDao = new KhuVucDao();
        private readonly QuanDao qDao = new QuanDao();
        private readonly CMADao cDao = new CMADao();
        #endregion

        #region Properties
        public bool isSaveLog = false;
        protected String Keyword
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_KEYWORD))
                {
                    return null;
                }

                return EncryptUtil.Decrypt(param[Constants.PARAM_KEYWORD].ToString());
            }
        }
        protected String StateCode
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_STATECODE))
                {
                    return TTCT.CT_P.ToString();
                }

                return EncryptUtil.Decrypt(param[Constants.PARAM_STATECODE].ToString());
            }
        }
        protected String AreaCode
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_AREACODE))
                {
                    return null;
                }
                return EncryptUtil.Decrypt(param[Constants.PARAM_AREACODE].ToString());
            }
        }
        protected DateTime? FromDate
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_FROMDATE))
                {
                    return null;
                }
                try
                {
                    return DateTimeUtil.GetVietNamDate(EncryptUtil.Decrypt(param[Constants.PARAM_FROMDATE].ToString()));
                }
                catch
                {
                    return null;
                }
            }
        }
        protected DateTime? ToDate
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_TODATE))
                {
                    return null;
                }
                try
                {
                    return DateTimeUtil.GetVietNamDate(EncryptUtil.Decrypt(param[Constants.PARAM_TODATE].ToString()));
                }
                catch
                {
                    return null;
                }
            }
        }
        protected bool FromReport
        {
            get
            {
                var param = ParameterWrapper.GetParams();
                if (!param.ContainsKey(Constants.PARAM_REPORTED))
                    return false;
                return true;
            }
        }
        private DONDANGKY DonDangKy
        {
            get
            {
                if (!IsDataValid())
                    return null;
                var obj = ddkDao.Get(MADDK);
                return obj;
            }
        }
        protected CHIETTINH ChietTinh
        {
            get
            {
                try { return ctDao.Get(MADDK); }
                catch { return null; }
            }
        }
        protected QUYETTOAN QuyetToan
        {
            get
            {
                try { return qtDao.Get(MADDK); }
                catch { return null; }
            }
        }
        private string MADDK
        {
            get { return Session["LAPCHIETTINH_MADDK_2"].ToString(); }
            set { Session["LAPCHIETTINH_MADDK_2"] = value; }
        }
        private Mode UpdateMode
        {
            get
            {
                try
                {
                    if (Session[SessionKey.MODE] != null)
                    {
                        var mode = Convert.ToInt32(Session[SessionKey.MODE]);
                        return (mode == Mode.Update.GetHashCode()) ? Mode.Update : Mode.Create;
                    }

                    return Mode.Create;
                }
                catch (Exception)
                {
                    return Mode.Create;
                }
            }

            set
            {
                Session[SessionKey.MODE] = value.GetHashCode();
            }
        }
        #endregion

        private bool IsDataValid()
        {
            // check MADDK
            if (string.Empty.Equals(txtTENKH.Text))
            {
                ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Tên khách hàng"), txtTENKH.ClientID);
                return false;
            }
           
            #region check id
            if (!string.Empty.Equals(ddlTenDuong.SelectedValue.Trim()))
            {
                var dp = dpldDao.Get(ddlTenDuong.SelectedValue.Trim());
                if (dp == null)
                {
                    ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Mã đường phố lắp đặt"), ddlTenDuong.ClientID);
                    return false;
                }
            }
            #endregion


            if (!string.Empty.Equals(txtCMA.Text.Trim()))
            {
                var cma = cDao.Get(txtCMA.Text.Trim());
                if (cma == null)
                {
                    ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Mã CMA"), txtCMA.ClientID);
                    return false;
                }
            }
            #region kiem tra Phường, Đường phố LD
            if (!string.Empty.Equals(ddlPhuong.SelectedValue))
            {
                var phuong = pDao.Get(ddlPhuong.SelectedValue);
                if (phuong == null)
                {
                    ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Phường không hợp lệ"), ddlPhuong.ClientID);
                    return false;
                }
            }
            #endregion

            return true;
        }
        public string MADDKStr { get; set; }
        #region Load Form
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                AjaxPro.Utility.RegisterTypeForAjax(typeof(AjaxCRM), Page);

                if (!Page.IsPostBack)
                {
                    LoadStaticReferences();
                    if (Session["LAPCHIETTINH_MADDK"] != null && FromReport)
                    {
                        var ct = ctDao.Get(Session["LAPCHIETTINH_MADDK"].ToString());

                        MADDK = Session["LAPCHIETTINH_MADDK"].ToString();
                       
                        divChietTinhInfo.Visible = true;

                        upnlCustomers.Visible = true;

                        BindSelectedVatTuGrid();
                        BindSelectedVatTu117Grid();
                        BindGhiChu();

                        divGridList.Visible = false;
                        divGridListF.Visible = false;
                        upnlCustomers.Update();
                    }
                    else
                    {
                        UpdateMode = Mode.Create;
                        BindDataForGrid();
                        BindDataForGridF();
                        upnlVatTu.Visible = false;
                        upnlCustomers.Visible = false;
                    }
                    Session["LAPCHIETTINH_MADDK"] = null;
                }

                var o = Session["LAPCHIETTINH_MADDK"];
                if (o != null)
                {
                    var objdk = new DonDangKyDao().Get(o.ToString());
                    if (objdk.MAKV.Equals("01") || objdk.MAKV.Equals("03"))
                    {
                        txtTruongPhong.Text = "";
                        upnlInfor.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        private void ChayChietTinh2(DONDANGKY ddk, NHANVIEN nv)
        {
            var ct = ctDao.Get(ddk.MADDK);
            if (ct == null)
            {
                var result = ctDao.CreateChietTinh2(ddk, ComputerName, IpAdress, LoginInfo.MANV);
                if (result != null && !result.MsgType.Equals(MessageType.Error))
                {
                    // show chiet tinh form
                    upnlVatTu.Visible = true;
                    upnlCustomers.Visible = true;

                    BindDataForGrid();
                    BindSelectedVatTuGrid();
                    BindSelectedVatTu117Grid();

                    BindGhiChu();
                    divGridList.Visible = false;
                    divGridListF.Visible = false;
                    divChietTinhInfo.Visible = true;
                    upnlVatTu.Visible = true;
                    upnlCustomers.Visible = true;
                }
                else
                {
                    // Show message
                    ShowError("Chạy chiết tính không thành công.");
                }
            }
            else
            {
                divChietTinhInfo.Visible = true;
                upnlVatTu.Visible = true;
                upnlCustomers.Visible = true;

                BindDataForGrid();
                BindSelectedVatTuGrid();
                BindSelectedVatTu117Grid();

                BindGhiChu();
                divGridList.Visible = false;
                divGridListF.Visible = false;
               

            }
            
            var qt = qtDao.Get(ddk.MADDK);
            if (qt == null)
            {
                var resultqt = qtDao.CreateChietTinh2(ddk, ComputerName, IpAdress, LoginInfo.MANV);
                if (resultqt != null && !resultqt.MsgType.Equals(MessageType.Error)) ;
            }
        }
        
        private void LoadStaticReferences()
        {
            var list = mbvtDao.GetList(LOAIMVT.LM.ToString(),LoginInfo.NHANVIEN.MAPB);
            ddlMBVT.Items.Clear();
            ddlMBVT.Items.Add(new ListItem("", ""));

            var index = -1;
            var i = 0;

            foreach (var item in list)
            {
                if (item.DUOCCHON.HasValue && item.DUOCCHON.Value)
                    index = i;
                i++;
                ddlMBVT.Items.Add(new ListItem(item.TENTK, item.MADDK));
            }

            if (index > -1)
            {
                ddlMBVT.Items.RemoveAt(0);
                ddlMBVT.SelectedIndex = index;
            }
            var heso = hsDao.GetListNC();
            CommonFunc.BindDropDownList(ddlHSNC, heso, "GIATRI", "MAHS", false);
           
            if (chkGiam.Checked)
            {
                uplGiamGia.Visible = true;
                txtGncTrc.Text = "0";
                txtGvlTrc.Text = "0";
                txtGtcTrc.Text = "50";
                txtGncSau.Text = "0";
                txtGvlSau.Text = "0";
                txtGtcSau.Text = "0";

            }
            else
            {
                txtGncTrc.Text = "0";
                txtGvlTrc.Text = "0";
                txtGtcTrc.Text = "0";
                txtGncSau.Text = "0";
                txtGvlSau.Text = "0";
                txtGtcSau.Text = "0";
                uplGiamGia.Visible = false;
            }
            upnlCustomers.Update();
            
            HoTenNv = LoginInfo.NHANVIEN.HOTEN;
            TebPb = LoginInfo.NHANVIEN.PHONGBAN.TENPB;
            TenHieu = LoginInfo.NHANVIEN.KHUVUC.TENHIEU;
            DiaChi = LoginInfo.NHANVIEN.KHUVUC.DIACHI;
            DienThoai = LoginInfo.NHANVIEN.KHUVUC.DIENTHOAI;
        }
        #endregion`

        #region Du Toan
        private void LapMoiTruoc(CHIETTINH ct)
        {
            var dulieu = (DataSet)Session["LM_DATA"];
            if (dulieu == null || dulieu.Tables.Count == 0) { CloseWaitingDialog(); return; }
            decimal tongvl = 0;
            decimal tongnc = 0;
            decimal tongdg = 0;
            foreach (DataRow row in dulieu.Tables[0].Rows)
            {
                tongvl = (decimal)row["TCTIENVT"];
                tongnc = (decimal)row["TCTIENNC"];
                tongdg = (decimal)row["TCGIAVC"];
            }
            decimal VL = Math.Round(tongvl * ct.CHSGVL.Value, 2);
            decimal NC = Math.Round(tongnc * ct.CHSNC.Value * ct.CHSGNC.Value, 2);
            decimal CPKD = ct.CKD.Value;
            decimal TTN = ct.CNUOC.Value;
            decimal G = Math.Round((VL + CPKD + TTN) * ct.CHSGIAM.Value, 2);
            decimal LamTron = CommonUtil.LamTronHue(G);

            ct.CTCDG = tongdg;
            ct.CVL = VL;
            ct.CTCNC = tongnc;
            ct.CNC = NC;
            ct.CXDS = G;
            ct.CLAMTRON = LamTron;
            ctDao.Update(ct, ComputerName, IpAdress, LoginInfo.MANV);

            var qt = qtDao.Get(MADDK);
            qt.CTCDG = tongdg;
            qt.CVL = VL;
            qt.CTCNC = tongnc;
            qt.CNC = NC;
            qt.CXDS = G;
            qt.CLAMTRON = LamTron;
            qtDao.Update(qt, ComputerName, IpAdress, LoginInfo.MANV);
        }
        private void LapMoiSau(CHIETTINH ct)
        {
            var dulieu = (DataSet)Session["LM_DATA"];
            if (dulieu == null || dulieu.Tables.Count == 0) { CloseWaitingDialog(); return; }
            decimal tongvl = 0;
            decimal tongnc = 0;
            decimal tongdg = 0;
            foreach (DataRow row in dulieu.Tables[1].Rows)
            {
                tongvl = (decimal)row["TCTIENVT"];
                tongnc = (decimal)row["TCTIENNC"];
                tongdg = (decimal)row["TCGIAVC"];
            }
            decimal VL = Math.Round(tongvl * ct.KHSGVL.Value, 2);
            decimal NC = Math.Round(tongnc * ct.KHSNC.Value * ct.KHSGNC.Value, 2);

            //binhta
            if (chkGiamAdb.Checked) ct.KHSTTK = 0;
            decimal TT = Math.Round(ct.KHSTTK.Value * (VL + NC), 2);
            
            decimal T = Math.Round(VL + NC + TT, 2);
            
            //binhta
            if (chkGiamAdb.Checked) ct.KHSCPC = 0;
            decimal C = Math.Round(ct.KHSCPC.Value * T, 2);
            
            decimal TL = Math.Round(ct.KHSTHU.Value * (T + C));
            decimal KKS = Math.Round(decimal.Parse(txtPhiKS.Text.Trim()));
            decimal G = Math.Round(T + C + TL + KKS);
            decimal GTGT = Math.Round(ct.KHSVAT.Value * G);
            decimal GXD = Math.Round((G + GTGT) * ct.KHSGIAM.Value, 2);
            decimal LamTron = CommonUtil.LamTronHue(GXD);
            decimal TienHang = Math.Round((decimal) (LamTron / (1 + hsDao.Get("HSVAT").GIATRI)), 2);
            decimal TienThue = Math.Round(LamTron - TienHang, 2);

            ct.KTCDG = tongdg;
            ct.KTCNC = tongnc;
            ct.KVL = VL;
            ct.KNC = NC;
            ct.KTTK = TT;
            ct.KTTP = T;
            ct.KCPC = C;
            ct.KTHU = TL;
            ct.KXDT = G;
            ct.KVAT = GTGT;
            ct.KXDS = GXD;
            ct.KLAMTRON = LamTron;
            ct.KTRCVAT = TienHang;
            ct.KINVAT = TienThue;
            ct.LAMTRON = LamTron;
            ct.TRUOCVAT = TienHang;
            ct.INVAT = TienThue;
            //var don = ddkDao.Get(MADDK);
            //if (don.LOAIDK == "CT")
            //{
            //    ct.LOAIDK = LOAIDK.CT.ToString();
            //    ct.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            //}
            //else
            //{
            //    ct.LOAIDK = LOAIDK.DK.ToString();
            //    ct.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            //}
            ctDao.Update(ct, ComputerName, IpAdress, LoginInfo.MANV);

            var qt = qtDao.Get(MADDK);
            qt.KTCDG = tongdg;
            qt.KTCNC = tongnc;
            qt.KVL = VL;
            qt.KNC = NC;
            qt.KTTK = TT;
            qt.KTTP = T;
            qt.KCPC = C;
            qt.KTHU = TL;
            qt.KXDT = G;
            qt.KVAT = GTGT;
            qt.KXDS = GXD;
            qt.KLAMTRON = LamTron;
            qt.KTRCVAT = TienHang;
            qt.KINVAT = TienThue;
            qt.LAMTRON = LamTron;
            qt.TRUOCVAT = TienHang;
            qt.INVAT = TienThue;
            //if (don.LOAIDK == "CT")
            //{
            //    qt.LOAIDK = LOAIDK.CT.ToString();
            //    qt.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            //}
            //else
            //{
            //    qt.LOAIDK = LOAIDK.DK.ToString();
            //    qt.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            //}
            qtDao.Update(qt, ComputerName, IpAdress, LoginInfo.MANV);
        }
        private void DuToanLapMoiSuper()
        {
            var don = ddkDao.Get(MADDK);
            Session["LM_DATA"] = new ReportClass().DuToanCT(MADDK);
            don.ISDUTOAN = true;
            don.TTCT = TTCT.CT_P.ToString();
            don.TTDCT = TTDCT.TTDCT_N.ToString();
            
            //if (chkGiam.Checked)
            //binhta
            if(chkGiam.Checked)
            {
                don.GIAM = true;
                uplGiamGia.Visible = true;
            }
            else
            {
                don.GIAM = false;
                uplGiamGia.Visible = false;
            }
            if (chkGiamAdb.Checked)
            {
                don.GIAMADB2018 = true;
                uplGiamGia.Visible = true;
            }
            else
            {
                don.GIAMADB2018 = false;
                uplGiamGia.Visible = false;
            }
            ddkDao.Update(don, ComputerName, IpAdress, LoginInfo.MANV);
            var dutoan = ctDao.Get(MADDK);
            LapMoiTruoc(dutoan);
            LapMoiSau(dutoan);

        }

        private void TruocCaiTao(CHIETTINH dutoan)
        {
            var dulieu = (DataSet)Session["LM_DATA"];
            if (dulieu == null || dulieu.Tables.Count == 0) { CloseWaitingDialog(); return; }
            decimal tongvl = 0;
            decimal tongnc = 0;
            decimal tongdg = 0;
            foreach (DataRow row in dulieu.Tables[0].Rows)
            {
                tongvl = (decimal)row["TCTIENVT"];
                tongnc = (decimal)row["TCTIENNC"];
                tongdg = (decimal)row["TCGIAVC"];
            }
            decimal VL = Math.Round(tongvl * dutoan.CHSGVL.Value, 2);
            decimal NC = Math.Round(tongnc * dutoan.CHSGNC.Value * dutoan.CHSNC.Value, 2);
            decimal TT = Math.Round(dutoan.CHSTTK.Value * (VL + NC), 2);
            decimal T = Math.Round(VL + NC + TT, 2);
            decimal C = Math.Round(dutoan.CHSCPC.Value * T, 2);
            decimal TL = Math.Round(dutoan.CHSTHU.Value * (T + C), 2);
            decimal CPKD = dutoan.CKD.Value;
            decimal TTN = dutoan.CNUOC.Value;
            decimal KS = decimal.Parse(txtPhiKS.Text.Trim());
            decimal G = Math.Round(T + C + TL + CPKD + TTN + KS, 2);
            decimal GTGT = Math.Round(dutoan.CHSVAT.Value * G, 2);
            decimal GXD = Math.Round((G + GTGT), 2);
            decimal GIAM = Math.Round(dutoan.CHSGIAM.Value * GXD, 2);
            decimal LamTron = CommonUtil.LamTronHue(GIAM);

            dutoan.CTCDG = tongdg;
            dutoan.CVL = VL;
            dutoan.CTCNC = tongnc;
            dutoan.CNC = NC;
            dutoan.CTTK = TT;
            dutoan.CTTP = T;
            dutoan.CCPC = C;
            dutoan.CTHU = TL;
            dutoan.CKD = CPKD;
            dutoan.CNUOC = TTN;
            dutoan.CKS = KS;
            dutoan.CXDT = G;
            dutoan.CVAT = GTGT;
            dutoan.CXDS = GXD;
            dutoan.CGIAM = GIAM;
            dutoan.CLAMTRON = LamTron;
            ctDao.Update(dutoan, ComputerName, IpAdress, LoginInfo.MANV);

            var qt = qtDao.Get(MADDK);
            qt.CTCDG = tongdg;
            qt.CVL = VL;
            qt.CTCNC = tongnc;
            qt.CNC = NC;
            qt.CTTK = TT;
            qt.CTTP = T;
            qt.CCPC = C;
            qt.CTHU = TL;
            qt.CKD = CPKD;
            qt.CNUOC = TTN;
            qt.CKS = KS;
            qt.CXDT = G;
            qt.CVAT = GTGT;
            qt.CXDS = GXD;
            qt.CGIAM = GIAM;
            qt.CLAMTRON = LamTron;
            qtDao.Update(qt, ComputerName, IpAdress, LoginInfo.MANV);
        }
        private void SauCaiTao(CHIETTINH dutoan)
        {
            var dulieu = (DataSet)Session["LM_DATA"];
            if (dulieu == null || dulieu.Tables.Count == 0) { CloseWaitingDialog(); return; }
            decimal tongvl = 0;
            decimal tongnc = 0;
            decimal tongdg = 0;
            foreach (DataRow row in dulieu.Tables[1].Rows)
            {
                tongvl = (decimal)row["TCTIENVT"];
                tongnc = (decimal)row["TCTIENNC"];
                tongdg = (decimal)row["TCGIAVC"];
            }
            decimal VL = Math.Round(tongvl * Math.Round(dutoan.KHSGVL.HasValue? dutoan.KHSGVL.Value:1, 2), 2);
            decimal NC = Math.Round(
                tongnc * (dutoan.KHSNC.HasValue? dutoan.KHSNC.Value:1) * (dutoan.KHSNC1.Value == 0 ? 1 : dutoan.KHSNC1.Value) * (dutoan.KHSGNC.HasValue? dutoan.KHSGNC.Value:0), 2);
            decimal TT = Math.Round(dutoan.KHSTTK.Value * (VL + NC), 2);
            decimal T = Math.Round(VL + NC + TT, 2);
            decimal C = Math.Round(dutoan.KHSCPC.Value * T, 2);
            decimal TL = Math.Round(dutoan.KHSTHU.Value * (T + C));
            decimal G = Math.Round(T + C + TL);
            decimal GTGT = Math.Round(dutoan.KHSVAT.Value * G);
            decimal GXD = Math.Round((G + GTGT) * dutoan.KHSGIAM.Value, 2);
            decimal LamTron = CommonUtil.LamTronHue(GXD);
            decimal TongVL = Math.Round(VL+(dutoan.CVL.HasValue?dutoan.CVL.Value:0) );
            decimal TongNC = Math.Round(NC + (dutoan.CNC.HasValue ? dutoan.CNC.Value : 0));
            decimal TongKP = Math.Round(dutoan.CGIAM.Value + GXD, 2);
            decimal TongLamTron = CommonUtil.LamTronHue(TongKP);
            decimal TienHang = Math.Round(TongLamTron / (1 + (decimal)hsDao.Get("HSVAT").GIATRI), 2);
            decimal TienThue = Math.Round(TongLamTron - TienHang, 2);

            dutoan.KTCDG = tongdg;
            dutoan.KTCNC = tongnc;
            dutoan.KVL = VL;
            dutoan.KNC = NC;
            dutoan.KTTK = TT;
            dutoan.KTTP = T;
            dutoan.KCPC = C;
            dutoan.KTHU = TL;
            dutoan.KXDT = G;
            dutoan.KVAT = GTGT;
            dutoan.KXDS = GXD;
            dutoan.KLAMTRON = LamTron;
            dutoan.TCVL = TongVL;
            dutoan.TCNC = TongNC;
            dutoan.TCCP = TongKP;
            dutoan.LAMTRON = TongLamTron;
            dutoan.TRUOCVAT = TienHang;
            dutoan.INVAT = TienThue;

            var don = ddkDao.Get(MADDK);
            if (don.LOAIDK == "CT")
            {
                dutoan.LOAIDK = LOAIDK.CT.ToString();
                dutoan.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            }
            else
            {
                dutoan.LOAIDK = LOAIDK.DK.ToString();
                dutoan.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            }
            ctDao.Update(dutoan, ComputerName, IpAdress, LoginInfo.MANV);

            var qt = qtDao.Get(MADDK);
            qt.KTCDG = tongdg;
            qt.KTCNC = tongnc;
            qt.KVL = tongvl;
            qt.KNC = NC;
            qt.KTTK = TT;
            qt.KTTP = T;
            qt.KCPC = C;
            qt.KTHU = TL;
            qt.KXDT = G;
            qt.KVAT = GTGT;
            qt.KXDS = GXD;
            qt.KLAMTRON = LamTron;
            qt.TCVL = TongVL;
            qt.TCNC = TongNC;
            qt.TCCP = TongKP;
            qt.LAMTRON = TongLamTron;
            qt.TRUOCVAT = TienHang;
            qt.INVAT = TienThue;

            if (don.LOAIDK == "CT")
            {
                qt.LOAIDK = LOAIDK.CT.ToString();
                qt.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            }
            else
            {
                qt.LOAIDK = LOAIDK.DK.ToString();
                qt.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            }
            qtDao.Update(qt, ComputerName, IpAdress, LoginInfo.MANV);
        }
        private void DuToanCaiTaoSuper()
        {
            var don = ddkDao.Get(MADDK);
            Session["LM_DATA"] = new ReportClass().DuToanCT(MADDK);
            don.ISDUTOAN = true;
            //don.TTCT = TTCT.CT_RA.ToString(); old
            don.TTCT = TTCT.CT_P.ToString(); // New code update by ThanhDV (20/122022) - Fix lỗi chuyển trạng thái chiết tính về đang lập lại dự toán
            if (chkGiam.Checked)
            {
                don.GIAM = true;
                uplGiamGia.Visible = true;
            }
            else
            {
                don.GIAM = false;
                uplGiamGia.Visible = false;
            }
            ddkDao.Update(don, ComputerName, IpAdress, LoginInfo.MANV);
            var dutoan = ctDao.Get(MADDK);
            TruocCaiTao(dutoan);
            SauCaiTao(dutoan);
        }
        #endregion

        #region DS N Du Toan
        private void BindDataForGrid()
        {
            try
            {
                var objList = ddkDao.GetListLapCT(Keyword, FromDate, ToDate, AreaCode, LoginInfo.MANV);
                gvList.DataSource = objList;
                gvList.PagerInforText = objList.Count.ToString();
                gvList.DataBind();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            var id = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "EditItem":
                    var nhanvien = nvDao.Get(LoginInfo.Username);
                    if (nhanvien == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }

                    MADDK = id;
                    var ddk = ddkDao.Get(id);
                    if (ddk == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }

                    ChayChietTinh2(ddk, nhanvien);
                 
                    SetDDKToForm(ddk);

                    var dutoan = ctDao.Get(id);
                    if (dutoan != null)
                        SetCTToForm(dutoan);


                    CloseWaitingDialog();

                    break;
            }

        }
        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvList.PageIndex = e.NewPageIndex;
                BindDataForGrid();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;

            var lnkBtnID = e.Row.FindControl("lnkBtnID") as LinkButton;
            if (lnkBtnID == null) return;
            lnkBtnID.Attributes.Add("onclick", "onClientClickGridItem('" + CommonFunc.UniqueIDWithDollars(lnkBtnID) + "')");

            var lnkBtnID2 = e.Row.FindControl("lnkBtnID2") as LinkButton;
            if (lnkBtnID2 == null) return;
            lnkBtnID2.Attributes.Add("onclick", "onClientClickGridItem('" + CommonFunc.UniqueIDWithDollars(lnkBtnID2) + "')");
        }
        #endregion

        #region DS FDuToan
        private void SetDDKToForm(DONDANGKY ddk)
        {
            var loaidk = ddlLoaiDon.Items.FindByValue(ddk.LOAIDK);
            if (ddk.LOAIDK != null)
                ddlLoaiDon.SelectedIndex = ddlLoaiDon.Items.IndexOf(loaidk);
            if (ddlLoaiDon.SelectedValue.Trim() == "CT")
            {
                SetControlValue(txtTenHM.ClientID, "CẢI TẠO HỆ THỐNG CẤP NƯỚC");
                SetControlValue(txtHSNCphu.ClientID, "70");
                btnCT.Visible = true;
                btnFree.Visible = false;
            }
            else if (ddlLoaiDon.SelectedValue.Trim() == LOAIDK.DK.ToString())
            {
                SetControlValue(txtTenHM.ClientID, "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC");
                SetControlValue(txtGhiChu.ClientID, ddk.GHICHU??"");
                if (ddlLoaiDon.SelectedValue == "BX")
                {
                    SetControlValue(txtTenHM.ClientID, "BỔ SUNG DỰ TOÁN HỆ THỐNG CẤP NƯỚC");
                    SetControlValue(txtGhiChu.ClientID, " Bổ xung dự toán hệ thống cấp nước");
                }
                btnCT.Visible = false;
                btnFree.Visible = true;
            }
            else if (ddlLoaiDon.SelectedValue.Trim() ==LOAIDK.DKP.ToString())
            {
                SetControlValue(txtTenHM.ClientID, "LĂP ĐẶT HỆ THỐNG CẤP NƯỚC");
                SetControlValue(txtHSNCphu.ClientID, "0");
                btnCT.Visible = true;
                btnFree.Visible = false;
            }
           
        
            var p = pDao.Get(ddk.MAPHUONG);

            SetControlValue(txtTatPhuong.ClientID,p!=null? p.PTAT:"");
            SetControlValue(txtCMA.ClientID,p!=null?p.MACMA??"":"");
            var listP = pDao.GetListTat(p.PTAT);
            if (listP != null)
                CommonFunc.BindDropDownList(ddlPhuong, listP, "TENPHUONG", "MAPHUONG", false);
            var phuong = ddlPhuong.Items.FindByValue(ddk.MAPHUONG);
            if (phuong != null)
                ddlPhuong.SelectedIndex = ddlPhuong.Items.IndexOf(phuong);


            SetControlValue(txtTENKH.ClientID, ddk.TENKH);
            SetControlValue(txtDIENTHOAI.ClientID, ddk.DIENTHOAI);
            SetControlValue(txtDiDong.ClientID, ddk.DIDONG);
            SetControlValue(txtMaLoTrinh.ClientID, ddk.MADP);
            var duongPho = dpDao.Get(ddk.MADP);
            if (duongPho != null)
                SetControlValue(txtTenLT.ClientID, duongPho.TENDP);
            
            var d = dpldDao.Get(ddk.MADPLD);
            if (d!=null)
            {
                SetControlValue(txtMaDp.ClientID, d.VIETTAT ?? "");
            }
           
            var listd = dpldDao.GetListVietTat(d!=null?d.VIETTAT:null);
            if (listd != null)
                CommonFunc.BindDropDownList(ddlTenDuong, listd, "TENDUONGLD", "MADPLD", false);
            var duong = ddlTenDuong.Items.FindByValue(ddk.MADPLD);
            if (duong != null)
                ddlTenDuong.SelectedIndex = ddlTenDuong.Items.IndexOf(duong);


            //SetControlValue(txtCMA.ClientID, ddk.MACMA);
            SetControlValue(txtSoNha.ClientID, ddk.SONHA);

            //BINHTA- Lấy chi phí khảo sát từ PHIEUT
            decimal tienKsDaVat = new PhieuThuDao().Get("KSTK").GIATIEN ?? 0;
            string tienKsChuaVat = (tienKsDaVat / (1 + (decimal)hsDao.Get("HSVAT").GIATRI)).ToString(CultureInfo.InvariantCulture);
            SetControlValue(txtPhiKS.ClientID, ddk.ISTHUTIEN == true ? "0" : tienKsChuaVat);

            var heso = hsDao.Get(MAHS.HSNCP);
            decimal hs = heso.GIATRI.Value;
            if (ddk.LOAIDK == "CT")
            {
                SetControlValue(txtHSNCphu.ClientID, (Math.Round(hs * 100, 0)).ToString());
            }
            else
            {
                SetControlValue(txtHSNCphu.ClientID, "0");
            }
            if (ddk.GIAM != null)
            {
                var giam = ddk.GIAM.Value;
                if (giam == true)
                {
                    chkGiam.Checked = true;
                    uplGiamGia.Visible = true;
                   
                }
                else
                {
                    chkGiam.Checked = false;
                    uplGiamGia.Visible = false;
                }
            }

            //binhta
            if (ddk.GIAMADB2018 != null)
            {
                var giamAdb = ddk.GIAMADB2018.Value;
                if (giamAdb)
                {
                    chkGiamAdb.Checked = true;
                    uplGiamGia.Visible = true;

                }
                else
                {
                    chkGiamAdb.Checked = false;
                    uplGiamGia.Visible = false;
                }
            }
            
            SetControlValue(txtTruongPhong.ClientID, LoginInfo.NHANVIEN.KHUVUC.GIAMDOCCN);
            var valueindex = ddk.KHUVUC.HESONC;
            if(!String.IsNullOrEmpty(valueindex))
            {
                var index = ddlHSNC.Items.FindByValue(valueindex);
                ddlHSNC.SelectedIndex = ddlHSNC.Items.IndexOf(index);
            }
            upnlInfor.Update();            
        }
        private void SetCTToForm(CHIETTINH ct)
        {
            //SetControlValue(txtTenHM.ClientID, ct.TENHM ?? "");

            txtGhiChu.Text = ct.DONDANGKY.GHICHU != null ? ct.DONDANGKY.GHICHU.Trim() : "" ;

            var gdxn = ChietTinh.MANVDCT != null
                           ? nvDao.Get(ChietTinh.MANVDCT).HOTEN
                           : ct.DONDANGKY.KHUVUC.GIAMDOCCN ?? "";
            SetControlValue(txtTruongPhong.ClientID, gdxn);
            if (ct.CHSNC.HasValue)
            {
                var hs = ddlHSNC.Items.FindByText(ct.CHSNC.Value.ToString("0.0000"));
                if (ct.CHSNC != null)
                    ddlHSNC.SelectedIndex = ddlHSNC.Items.IndexOf(hs);
            }
            //BINHTA - Lấy động tiền KS
            decimal tienKsDaVat = new PhieuThuDao().Get("KSTK").GIATIEN ?? 0;
            var a = ct.CHSVAT;
            var b = hsDao.Get("HSVAT").GIATRI;
            decimal thuesuatVat = (decimal) (ct.CHSVAT ?? hsDao.Get("HSVAT").GIATRI);
            string tienKsChuaVat = (Math.Round(tienKsDaVat / (1+thuesuatVat), 0, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);
           
            SetControlValue(txtPhiKS.ClientID, ct.CKS.HasValue ? ct.CKS.Value.ToString() : ct.DONDANGKY.ISTHUTIEN == true ? "0" : tienKsChuaVat);
           
            SetControlValue(txtGvlTrc.ClientID, ct.CHSGVL.HasValue ? Math.Round((1 - ct.CHSGVL.Value) * 100, 0).ToString() : "0");
            SetControlValue(txtGncTrc.ClientID, ct.CHSGNC.HasValue ? Math.Round((1 - ct.CHSGNC.Value) * 100, 0).ToString() : "0");
            if (ct.DONDANGKY.LOAIDK == "CT")
            {
                SetControlValue(txtHSNCphu.ClientID, ct.CHSNC1.HasValue ? Math.Round(ct.CHSNC1.Value * 100, 0).ToString() : "70");
                SetControlValue(txtGtcTrc.ClientID, ct.CHSGIAM.HasValue ? Math.Round((1 - ct.CHSGIAM.Value) * 100, 0).ToString() : "50");
            }
            else
            {
                SetControlValue(txtHSNCphu.ClientID, ct.CHSNC1.HasValue ? Math.Round(ct.CHSNC1.Value * 100, 0).ToString() : "0");
                SetControlValue(txtGtcTrc.ClientID, ct.CHSGIAM.HasValue ? Math.Round((1 - ct.CHSGIAM.Value) * 100, 0).ToString() : "0");
            }
            SetControlValue(txtGvlSau.ClientID, ct.KHSGVL.HasValue ? Math.Round((1 - ct.KHSGVL.Value) * 100, 0).ToString() : "0");
            SetControlValue(txtGncSau.ClientID, ct.KHSGNC.HasValue ? Math.Round((1 - ct.KHSGNC.Value) * 100, 0).ToString() : "0");
            SetControlValue(txtGtcSau.ClientID, ct.KHSGIAM.HasValue ? Math.Round((1 - ct.KHSGIAM.Value) * 100, 0).ToString() : "0");
            changeloaidk();
            uplCaiTao.Update();
            uplGiamGia.Update();
        }
        private void BindDataForGridF()
        {
            try
            {
                var objList = ddkDao.GetListFLapCT(Keyword, FromDate, ToDate, AreaCode, LoginInfo.MANV);
                gvListF.DataSource = objList;
                gvListF.PagerInforText = objList.Count.ToString();
                gvListF.DataBind();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvListF_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            var id = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "EditItem":
                    var nhanvien = nvDao.Get(LoginInfo.Username);
                    if (nhanvien == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }

                    MADDK = id;
                    var ddk = ddkDao.Get(id);
                    if (ddk == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }
                    var ct = ctDao.Get(id);
                    if (ct == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }
                    ChayChietTinh2(ddk, nhanvien);
                    SetDDKToForm(ddk);
                    SetCTToForm(ct);
                    upnlInfor.Update();
                    upnlCustomers.Update();
                    CloseWaitingDialog();
                    break;
            }

        }
        protected void gvListF_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                // Update page index
                gvListF.PageIndex = e.NewPageIndex;

                // Bind data for grid
                BindDataForGridF();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvListF_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;
            var lnkBtnDT1F = e.Row.FindControl("lnkBtnDT1F") as LinkButton;
            if (lnkBtnDT1F == null) return;
            lnkBtnDT1F.Attributes.Add("onclick", "onClientClickGridItem('" + CommonFunc.UniqueIDWithDollars(lnkBtnDT1F) + "')");

        }
        #endregion

        #region Chose VT
        protected void btnFilterVatTu_Click(object sender, EventArgs e)
        {
            BindVatTu();
            CloseWaitingDialog();
        }
        protected void btnBrowseVatTu_Click(object sender, EventArgs e)
        {
            BindVatTu();
            upnlVatTu.Update();
            UpdateMode = Mode.Create;
            UnblockDialog("divVatTu");
        }
        protected void btnBrowseVatTu117_Click(object sender, EventArgs e)
        {
            BindVatTu();
            upnlVatTu.Update();
            UpdateMode = Mode.Update;
            UnblockDialog("divVatTu");
        }
        private void BindVatTu()
        {
            var list = vtDao.Search(txtFilterVatTu.Text.Trim());
            gvVatTu.DataSource = list;
            gvVatTu.PagerInforText = list.Count.ToString();
            gvVatTu.DataBind();
        }
        protected void gvVatTu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                var id = e.CommandArgument.ToString();
                switch (e.CommandName)
                {
                    case "EditItem":
                        CloseWaitingDialog();
                        switch (UpdateMode)
                        {
                            case Mode.Create:
                                SetControlValue(txtMAVT.ClientID, id);
                                SetFocus(txtKHOILUONG);
                                break;
                            case Mode.Update:
                                SetControlValue(txtMAVT117.ClientID, id);
                                SetFocus(txtMAVT117);
                                break;
                        }
                        HideDialog("divVatTu");

                        break;

                }
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvVatTu_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                // Update page index
                gvVatTu.PageIndex = e.NewPageIndex;

                // Bind data for grid
                BindVatTu();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        protected void gvVatTu_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;
            var lnkBtnID = e.Row.FindControl("lnkBtnID") as LinkButton;
            if (lnkBtnID == null) return;
            lnkBtnID.Attributes.Add("onclick", "onClientClickGridItem('" + CommonFunc.UniqueIDWithDollars(lnkBtnID) + "')");
        }
        protected void ChoseMauBocVatTu()
        {
            var mbvt = mbvtDao.Get(ddlMBVT.SelectedValue);
            if (mbvt == null)
            {
                CloseWaitingDialog();
                ShowError("Vui lòng chọn mẫu bốc vật tư");
                return;
            }
            ctDao.ChangeFromMBVT(ChietTinh, mbvt);
            qtDao.ChangeFromMBVT(QuyetToan, mbvt);
            BindSelectedVatTuGrid();
            BindSelectedVatTu117Grid();
            upnlCustomers.Update();
            upnlVatTu.Update();
            upnlInfor.Update();
        }
        #endregion

        #region Part CTy
        private void BindSelectedVatTuGrid()
        {
            if (ChietTinh == null) return;
            var list = ctctDao.GetList(MADDK);
            gvSelectedVatTu.DataSource = list;
            gvSelectedVatTu.PagerInforText = list.Count.ToString();
            gvSelectedVatTu.DataBind();
        }
        protected void gvSelectedVatTu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var mavt = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "DeleteVatTu":
                    if (ChietTinh == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }
                    var deletingCTCT = ctctDao.Get(ChietTinh.MADDK, mavt);
                    if (deletingCTCT == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }
                    ctctDao.Delete(deletingCTCT);

                    if (QuyetToan == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }
                    var deletingCTQT = ctqtDao.Get(ChietTinh.MADDK, mavt);
                    if (deletingCTQT == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }
                    ctqtDao.Delete(deletingCTQT);
                    BindSelectedVatTuGrid();
                    upnlCustomers.Update();

                    //CloseWaitingDialog();

                    break;
            }
        }
        protected void gvSelectedVatTu_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;
            var txtSL = e.Row.FindControl("txtSOLUONG") as TextBox;
            var txtSLKH = e.Row.FindControl("txtSLKH") as TextBox;
            var txtGIAVT = e.Row.FindControl("txtGIAVT") as TextBox;
            var txtGIANC = e.Row.FindControl("txtGIANC") as TextBox;
            var lblTIENVT = e.Row.FindControl("lblTIENVT") as Label;
            var lblTIENNC = e.Row.FindControl("lblTIENNC") as Label;
            var txtSTT = e.Row.FindControl("txtSTT") as TextBox;
            if (txtSTT == null) return;
            if (txtSL == null || txtSLKH == null || txtGIAVT == null || txtGIANC == null ||
                lblTIENNC == null || lblTIENVT == null) return;
            var source = gvSelectedVatTu.DataSource as List<CTCHIETTINH>;
            if (source == null) return;
            var mavt = source[e.Row.RowIndex].MAVT;
            var maddk = source[e.Row.RowIndex].MADDK;
            var script = "javascript:updateCTCT(\"" + maddk + "\", \"" + mavt +
                                                        "\", \"" + txtSL.ClientID +
                                                        "\", \"" + txtSLKH.ClientID +
                                                        "\", \"" + txtGIAVT.ClientID +
                                                         "\", \"" + txtGIANC.ClientID +
                                                        "\", \"" + lblTIENVT.ClientID +
                                                        "\", \"" + lblTIENNC.ClientID +
                                                         "\", \"" + txtSTT.ClientID +
                                                        "\")";
            txtSL.Attributes.Add("onblur", script);
            txtSLKH.Attributes.Add("onblur", script);
            txtGIAVT.Attributes.Add("onblur", script);
            txtGIANC.Attributes.Add("onblur", script);
            txtSTT.Attributes.Add("onblur", script);
        }
        protected void linkBtnChangeMAVT_Click(object sender, EventArgs e)
        {
            if (txtMAVT.Text.Trim() == "")
            {
                CloseWaitingDialog();
                return;
            }

            var vt = vtDao.Get(txtMAVT.Text.Trim());
            if (vt == null)
            {
                CloseWaitingDialog();
                ShowError("Vật tư không có trong cơ sở dữ liệu. Vui lòng chọn lại.", txtMAVT.ClientID);
                SetFocus(txtMAVT.ClientID);
                return;
            }

            txtKHOILUONG.Text = "1";
            FocusAndSelect(txtKHOILUONG.ClientID);
            upnlCustomers.Update();
            CloseWaitingDialog();
        }
        protected void linkBtnChangeKhoiLuong_Click(object sender, EventArgs e)
        {
            if (ChietTinh == null)
            {
                CloseWaitingDialog();
                return;
            }

            if (txtMAVT.Text.Trim() == "")
            {
                CloseWaitingDialog();
                ShowError("Vui lòng nhập mã vật tư", txtMAVT.ClientID);
                return;
            }

            var vt = vtDao.Get(txtMAVT.Text.Trim());
            if (vt == null)
            {
                CloseWaitingDialog();
                ShowError("Vật tư không có trong cơ sở dữ liệu. Vui lòng chọn lại.", txtMAVT.ClientID);
                return;
            }

            try
            {
                decimal.Parse(txtKHOILUONG.Text.Trim());
            }
            catch
            {
                CloseWaitingDialog();
                ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Khối lượng"), txtKHOILUONG.ClientID);
                return;
            }

            // add to grid
            var ctct = new CTCHIETTINH
            {
                MADDK = ChietTinh.MADDK,
                MAVT = vt.MAVT,
                LOAICT = CT.CT.ToString(),
                LOAICV = "---***---",
                SLCTY = decimal.Parse(txtKHOILUONG.Text.Trim()),
                SLKH = decimal.Parse(txtKLKhachHang.Text.Trim()),
                DGIAVC = vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI,
                GIAVT = vt.GIAVT,
                TIENVT = ((vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI) + vt.GIAVT) * decimal.Parse(txtKHOILUONG.Text.Trim()),
                GIANC = vt.GIANC,
                TIENNC = (decimal.Parse(txtKHOILUONG.Text.Trim()) + decimal.Parse(txtKLKhachHang.Text.Trim())) * vt.GIANC,
                ISCTYDTU = true,
                STT = ctctDao.GetSTT(ChietTinh.MADDK),
                ISVATTU = vt.ISVATTU
            };

            ctctDao.Insert(ctct);

            var ctqt = new CTQUYETTOAN
                           {
                               MADDK = QuyetToan.MADDK,
                               MAVT = vt.MAVT,
                               LOAICT = CT.QT.ToString(),
                               LOAICV = "---***---",
                               SLCTY = decimal.Parse(txtKHOILUONG.Text.Trim()),
                               SLKH = decimal.Parse(txtKLKhachHang.Text.Trim()),
                               DGIAVC = vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI,
                               GIAVT = vt.GIAVT,
                               TIENVT =
                                   ((vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI) + vt.GIAVT) *
                                   decimal.Parse(txtKHOILUONG.Text.Trim()),
                               GIANC = vt.GIANC,
                               TIENNC =
                                   (decimal.Parse(txtKHOILUONG.Text.Trim()) + decimal.Parse(txtKLKhachHang.Text.Trim())) *
                                   vt.GIANC,
                               ISCTYDTU = true,
                               STT = ctctDao.GetSTT(QuyetToan.MADDK),
                               ISVATTU = vt.ISVATTU
                           };
            ctqtDao.Insert(ctqt);

            BindSelectedVatTuGrid();

            txtMAVT.Text = "";
            txtKHOILUONG.Text = "";
            FocusAndSelect(txtMAVT.ClientID);
            Focus(txtMAVT.ClientID);
            upnlCustomers.Update();

            CloseWaitingDialog();
        }
        #endregion

        #region Part KH
        private void BindSelectedVatTu117Grid()
        {
            if (ChietTinh == null) return;

            var list = ctct117Dao.GetList(MADDK);

            gvSelectedVatTu117.DataSource = list;
            gvSelectedVatTu117.PagerInforText = list.Count.ToString();
            gvSelectedVatTu117.DataBind();
        }
        protected void gvSelectedVatTu117_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var mavt = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "DeleteVatTu":
                    if (ChietTinh == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }

                    var deletingCTCT = ctct117Dao.Get(ChietTinh.MADDK, mavt);
                    if (deletingCTCT == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }
                    ctct117Dao.Delete(deletingCTCT);
                    if (QuyetToan == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }

                    var deletingCTQT = ctqt117Dao.Get(ChietTinh.MADDK, mavt);
                    if (deletingCTQT == null)
                    {
                        //CloseWaitingDialog();
                        return;
                    }

                    ctqt117Dao.Delete(deletingCTQT);
                    BindSelectedVatTu117Grid();
                    upnlCustomers.Update();

                    //CloseWaitingDialog();

                    break;
            }
        }
        protected void gvSelectedVatTu117_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;

            var txtSL = e.Row.FindControl("txtSOLUONG") as TextBox;
            var txtSLKH = e.Row.FindControl("txtSLKH") as TextBox;
            var txtGIAVT = e.Row.FindControl("txtGIAVT") as TextBox;
            var txtGIANC = e.Row.FindControl("txtGIANC") as TextBox;
            var lblTIENVT = e.Row.FindControl("lblTIENVT") as Label;
            var lblTIENNC = e.Row.FindControl("lblTIENNC") as Label;
            var txtSTT = e.Row.FindControl("txtSTT") as TextBox;
            if (txtSTT == null) return;
            if (txtSL == null || txtSLKH == null || txtGIAVT == null || txtGIANC == null ||
                lblTIENNC == null || lblTIENVT == null) return;

            var source = gvSelectedVatTu117.DataSource as List<CTCHIETTINH_ND117>;
            if (source == null) return;

            var mavt = source[e.Row.RowIndex].MAVT;
            var maddk = source[e.Row.RowIndex].MADDK;

            var script = "javascript:updateCTCT117(\"" + maddk + "\", \"" + mavt +
                                                        "\", \"" + txtSL.ClientID +
                                                        "\", \"" + txtSLKH.ClientID +
                                                        "\", \"" + txtGIAVT.ClientID +
                                                        "\", \"" + txtGIANC.ClientID +
                                                        "\", \"" + lblTIENVT.ClientID +
                                                        "\", \"" + lblTIENNC.ClientID +
                                                         "\", \"" + txtSTT.ClientID +
                                                        "\")";
            txtSL.Attributes.Add("onblur", script);
            txtSLKH.Attributes.Add("onblur", script);
            txtGIAVT.Attributes.Add("onblur", script);
            txtGIANC.Attributes.Add("onblur", script);
            txtSTT.Attributes.Add("onblur", script);
        }
        protected void linkBtnChangeMAVT117_Click(object sender, EventArgs e)
        {
            if (txtMAVT117.Text.Trim() == "")
            {
                CloseWaitingDialog();
                return;
            }

            var vt = vtDao.Get(txtMAVT117.Text.Trim());
            if (vt == null)
            {
                CloseWaitingDialog();
                ShowError("Vật tư không có trong cơ sở dữ liệu. Vui lòng chọn lại.", txtMAVT117.ClientID);
                return;
            }


            txtKHOILUONG117.Text = "1";
            txtKLKhachHang117.Text = "0";
            FocusAndSelect(txtKHOILUONG117.ClientID);

            CloseWaitingDialog();
        }
        protected void linkBtnChangeKhoiLuong117_Click(object sender, EventArgs e)
        {
            if (ChietTinh == null)
            {
                CloseWaitingDialog();
                return;
            }

            if (txtMAVT117.Text.Trim() == "")
            {
                CloseWaitingDialog();
                ShowError("Vui lòng nhập mã vật tư", txtMAVT117.ClientID);
                return;
            }

            var vt = vtDao.Get(txtMAVT117.Text.Trim());
            if (vt == null)
            {
                CloseWaitingDialog();
                ShowError("Vật tư không có trong cơ sở dữ liệu. Vui lòng chọn lại.", txtMAVT117.ClientID);
                return;
            }

            try
            {
                decimal.Parse(txtKHOILUONG117.Text.Trim());
            }
            catch
            {
                CloseWaitingDialog();
                ShowError(String.Format(Resources.Message.E_INVALID_DATA, "Khối lượng"), txtKHOILUONG117.ClientID);
                return;
            }

            // add to grid
            var ctct = new CTCHIETTINH_ND117
                           {
                               MADDK = ChietTinh.MADDK,
                               MAVT = vt.MAVT,
                               LOAICT = CT.CT.ToString(),
                               LOAICV = "---***---",
                               SLCTY = decimal.Parse(txtKHOILUONG117.Text.Trim()),
                               SLKH = decimal.Parse(txtKLKhachHang117.Text.Trim()),
                               DGIAVC = vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI,
                               GIAVT = vt.GIAVT,
                               TIENVT =
                                   ((vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI) + vt.GIAVT) *
                                   decimal.Parse(txtKHOILUONG117.Text.Trim()),
                               GIANC = vt.GIANC,
                               TIENNC =
                                   (decimal.Parse(txtKHOILUONG117.Text.Trim()) +
                                    decimal.Parse(txtKLKhachHang117.Text.Trim())) * vt.GIANC,
                               STT = ctct117Dao.GetSTT(ChietTinh.MADDK),
                               ISVATTU = vt.ISVATTU
                           };

            ctct117Dao.Insert(ctct);

            var ctqt117 = new CTQUYETTOAN_ND117
                              {
                                  MADDK = QuyetToan.MADDK,
                                  MAVT = vt.MAVT,
                                  LOAICT = CT.QT.ToString(),
                                  LOAICV = "---***---",
                                  SLCTY = decimal.Parse(txtKHOILUONG117.Text.Trim()),
                                  SLKH = decimal.Parse(txtKLKhachHang117.Text.Trim()),
                                  DGIAVC = vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI,
                                  GIAVT = vt.GIAVT,
                                  TIENVT =
                                      ((vt.GIAVT * hsDao.Get(MAHS.HSVC).GIATRI) + vt.GIAVT) *
                                      decimal.Parse(txtKHOILUONG117.Text.Trim()),
                                  GIANC = vt.GIANC,
                                  TIENNC =
                                      (decimal.Parse(txtKHOILUONG117.Text.Trim()) +
                                       decimal.Parse(txtKLKhachHang117.Text.Trim())) *
                                      vt.GIANC,
                                  STT = ctctDao.GetSTT(QuyetToan.MADDK),
                                  ISVATTU = vt.ISVATTU
                              };
            ctqt117Dao.Insert(ctqt117);

            ctDao.UpdateChiPhiForChietTinh(ChietTinh.MADDK);
            BindSelectedVatTu117Grid();

            txtMAVT117.Text = "";

            txtKHOILUONG117.Text = "";
            FocusAndSelect(txtMAVT117.ClientID);
            Focus(txtMAVT117.ClientID);
            upnlCustomers.Update();

            CloseWaitingDialog();
        }
        #endregion

        #region Button Click
        protected void btnChange_Click(object sender, EventArgs e)
        {
            var mbvt = mbvtDao.Get(ddlMBVT.SelectedValue);

            if (mbvt == null)
            {
                CloseWaitingDialog();
                ShowError("Vui lòng chọn mẫu bốc vật tư");
                return;
            }
            ctDao.ChangeFromMBVT(ChietTinh, mbvt);
            qtDao.ChangeFromMBVT(QuyetToan, mbvt);
            BindSelectedVatTuGrid();
            BindSelectedVatTu117Grid();
            upnlCustomers.Update();
            upnlVatTu.Update();
            upnlInfor.Update();
            CloseWaitingDialog();
        }
        private void SaveCT()
        {
            DONDANGKY don = DonDangKy;
            if (don == null)
            {
                CloseWaitingDialog();
                return;
            }
            var objUi = ctDao.Get(MADDK);
            objUi.TENCT = txtTENKH.Text.Trim();
            objUi.LOAIDK = ddlLoaiDon.SelectedValue.Trim();
            objUi.DIACHIHM = "";
            objUi.TENHM = txtTenHM.Text.Trim();
            objUi.GHICHU = txtGhiChu.Text.Trim();
            objUi.CHSNC = decimal.Parse(ddlHSNC.SelectedItem.Text.Trim());
            objUi.KHSNC = decimal.Parse(ddlHSNC.SelectedItem.Text.Trim());
            objUi.CHSNC1 = Math.Round(decimal.Parse(txtHSNCphu.Text.Trim()) / decimal.Parse("100"), 2);
            objUi.KHSNC1 = Math.Round(decimal.Parse(txtHSNCphu.Text.Trim()) / decimal.Parse("100"), 2);
            objUi.CHSGNC = Math.Round((decimal.Parse("100") - decimal.Parse(txtGncTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.CHSGVL = Math.Round((decimal.Parse("100") - decimal.Parse(txtGvlTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.CHSGIAM = Math.Round((decimal.Parse("100") - decimal.Parse(txtGtcTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGNC = Math.Round((decimal.Parse("100") - decimal.Parse(txtGncSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGVL = Math.Round((decimal.Parse("100") - decimal.Parse(txtGvlSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGIAM = Math.Round((decimal.Parse("100") - decimal.Parse(txtGtcSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KKS = decimal.Parse(txtPhiKS.Text.Trim());
            objUi.CKS = decimal.Parse(txtPhiKS.Text.Trim());

            objUi.CHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
            objUi.CHSTTK = hsDao.Get(MAHS.HSTTK).GIATRI;
            objUi.CHSCPC = hsDao.Get(MAHS.HSCPC).GIATRI;
            objUi.CHSTHU = hsDao.Get(MAHS.HSTHU).GIATRI;
            objUi.CHSVAT = hsDao.Get(MAHS.HSVAT).GIATRI;
            objUi.CKD = hsDao.GetCP(MACP.CKD).GIATRICP;
            objUi.CNUOC = hsDao.GetCP(MACP.CNUOC).GIATRICP;

            objUi.KHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
            objUi.KHSTTK = hsDao.Get(MAHS.HSTTK).GIATRI;
            objUi.KHSCPC = hsDao.Get(MAHS.HSCPC).GIATRI;
            objUi.KHSTHU = hsDao.Get(MAHS.HSTHU).GIATRI;
            objUi.KHSVAT = hsDao.Get(MAHS.HSVAT).GIATRI;
            objUi.KHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
           // Message msg = ctDao.Update(objUi, ComputerName, IpAdress, LoginInfo.MANV);

            //if (don.LOAIDK == "CT")
            //{
            //    objUi.LOAIDK = LOAIDK.CT.ToString();
            //    //objUi.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            //}
            //else if (don.LOAIDK == "BX")
            //{
            //    objUi.LOAIDK = LOAIDK.BX.ToString();
            //    //objUi.TENHM = "BỔ SUNG DỰ TOÁN HỆ THỐNG CẤP NƯỚC";
            //}
            //else if (don.LOAIDK == li)
            //{
            //    objUi.LOAIDK = LOAIDK.DK.ToString();
            //    //objUi.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            //}
            //objUi.LOAIDK = don.LOAIDK;
            Message chiet = ctDao.Update(objUi, ComputerName, IpAdress, LoginInfo.MANV);

            if (chiet == null) return;
            if (chiet.MsgType != MessageType.Error)
            {
                ShowInfor("Cập nhật thông tin thành công.");             
            }
            else
            {
                ShowError("Cập nhật thông tin không thành công.");
            }
            CloseWaitingDialog();
        }
        private void SaveQT()
        {
            var objUi = qtDao.Get(MADDK);
            objUi.TENCT = txtTENKH.Text.Trim();
            objUi.LOAIDK = ddlLoaiDon.SelectedValue.Trim();
            objUi.DIACHIHM = "";
            objUi.TENHM = txtTenHM.Text.Trim();
            objUi.GHICHU = txtGhiChu.Text.Trim();
            objUi.CHSNC = decimal.Parse(ddlHSNC.SelectedItem.Text.Trim());
            objUi.KHSNC = decimal.Parse(ddlHSNC.SelectedItem.Text.Trim());
            objUi.CHSNC1 = Math.Round(decimal.Parse(txtHSNCphu.Text.Trim()) / decimal.Parse("100"), 2);
            objUi.KHSNC1 = Math.Round(decimal.Parse(txtHSNCphu.Text.Trim()) / decimal.Parse("100"), 2);
            objUi.CHSGNC = Math.Round((decimal.Parse("100") - decimal.Parse(txtGncTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.CHSGVL = Math.Round((decimal.Parse("100") - decimal.Parse(txtGvlTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.CHSGIAM = Math.Round((decimal.Parse("100") - decimal.Parse(txtGtcTrc.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGNC = Math.Round((decimal.Parse("100") - decimal.Parse(txtGncSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGVL = Math.Round((decimal.Parse("100") - decimal.Parse(txtGvlSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KHSGIAM = Math.Round((decimal.Parse("100") - decimal.Parse(txtGtcSau.Text.Trim())) / decimal.Parse("100"), 2);
            objUi.KKS = decimal.Parse(txtPhiKS.Text.Trim());
            objUi.CKS = decimal.Parse(txtPhiKS.Text.Trim());

            objUi.CHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
            objUi.CHSTTK = hsDao.Get(MAHS.HSTTK).GIATRI;
            objUi.CHSCPC = hsDao.Get(MAHS.HSCPC).GIATRI;
            objUi.CHSTHU = hsDao.Get(MAHS.HSTHU).GIATRI;
            objUi.CHSVAT = hsDao.Get(MAHS.HSVAT).GIATRI;
            objUi.CKD = hsDao.GetCP(MACP.CKD).GIATRICP;
            objUi.CNUOC = hsDao.GetCP(MACP.CNUOC).GIATRICP;

            objUi.KHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
            objUi.KHSTTK = hsDao.Get(MAHS.HSTTK).GIATRI;
            objUi.KHSCPC = hsDao.Get(MAHS.HSCPC).GIATRI;
            objUi.KHSTHU = hsDao.Get(MAHS.HSTHU).GIATRI;
            objUi.KHSVAT = hsDao.Get(MAHS.HSVAT).GIATRI;
            objUi.KHSVC = hsDao.Get(MAHS.HSVC).GIATRI;
            //Message msg = qtDao.Update(objUi, ComputerName, IpAdress, LoginInfo.MANV);
            DONDANGKY don = DonDangKy;
            if (don == null)
            {
                CloseWaitingDialog();
                return;
            }
            if (don.LOAIDK == "CT")
            {
                objUi.LOAIDK = LOAIDK.CT.ToString();
                objUi.TENHM = "CẢI TẠO HỆ THỐNG CẤP NƯỚC";
            }
            else if (don.LOAIDK == "BX")
            {
                objUi.LOAIDK = LOAIDK.BX.ToString();
                objUi.TENHM = "BỔ SUNG DỰ TOÁN HỆ THỐNG CẤP NƯỚC";
            }
            else if (don.LOAIDK == "DKP")
            {
                objUi.LOAIDK = LOAIDK.DKP.ToString();
                objUi.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            }
            else
            {
                objUi.LOAIDK = LOAIDK.DK.ToString();
                objUi.TENHM = "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC";
            }
            Message chiet = qtDao.Update(objUi, ComputerName, IpAdress, LoginInfo.MANV);


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveCT();
            SaveQT();

            DONDANGKY don = DonDangKy;
            if (don == null)
            {
                CloseWaitingDialog();
                return;
            }
            var ddk = ddkDao.Get(MADDK);
            ddk.MACMA = txtCMA.Text.Trim();
            ddk.MAPHUONG = ddlPhuong.SelectedValue.Trim();
            var sonha = txtSoNha.Text.Trim();
            var p = pDao.Get(ddlPhuong.SelectedValue.Trim());
            var tenphuong = p.TENPHUONG;
            var d = dpldDao.Get(ddlTenDuong.SelectedValue.Trim());
            var tenduong = d.TENDUONGLD??"";
            ddk.DIACHILD = string.Format("{0}, {1}, {2}", sonha, tenduong, tenphuong);
            ddk.MAQUAN = pDao.Get(ddlPhuong.SelectedValue.Trim()).MAQUAN;
            ddk.TENKH = txtTENKH.Text.Trim();
            ddk.DIENTHOAI = txtDIENTHOAI.Text.Trim();
            ddk.DIDONG = txtDiDong.Text.Trim();
            ddk.SONHA = txtSoNha.Text.Trim();
            ddk.MADPLD = ddlTenDuong.SelectedValue.Trim();
            ddk.LOAIDK = ddlLoaiDon.SelectedValue.Trim();
            ddk.MACMA = txtCMA.Text.Trim();
            ddk.GHICHU = txtGhiChu.Text.Trim();
            ddk.MADP = txtMaLoTrinh.Text.Trim();
            if (chkGiam.Checked)
            {
                ddk.GIAM = true;
            }
            else
            {
                ddk.GIAM = false;
            }
            ddk.MANVSUA = LoginInfo.MANV;
            ddk.NGAYSUA = DateTime.Now;
            //ddk.TTCT = TTCT.CT_P.ToString();
            //ddk.TTDCT = TTDCT.TTDCT_N.ToString();
            //ddk.ISDUTOAN = true;
            Message dangky = ddkDao.Update(ddk, ComputerName, IpAdress, LoginInfo.MANV);
            if (dangky == null) return;
            if (dangky.MsgType != MessageType.Error)
            {
                ShowInfor("Cập nhật thông tin thành công.");
                // Lưu log nội dung
                if (isSaveLog && !string.IsNullOrEmpty(ddk.GHICHU))
                {
                    CONTENT_LOG log = new CONTENT_LOG()
                    {
                        MADON = ddk.MADDK,
                        TACVU = "Lập dự toán",
                        NOIDUNG = "",
                        GHICHU = ddk.GHICHU,
                        NGAYTHUCHIEN = DateTime.Now
                    };
                    var msg = ddkDao.saveContentLog(log);
                    if(msg.MsgType == MessageType.Info)
                    {
                        isSaveLog = false;
                    }
                }
            }
            else
            {
                ShowError("Cập nhật thông tin không thành công.");
            }
        }
        // KY SO -----------------------------------------------------------------
        private class Base64DuToanRes{
            public string DuToanTruoc{get;set;}
            public string NghiemThu{get;set;}
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ResultLoginVnptCA LoginVnpt(string loginEntityStr)
        {
            var loginEntity = JsonConvert.DeserializeObject<LoginEntity>(loginEntityStr);
            var result = new VnptHashDao().LoginVnptCA(loginEntity);
            if (result.token == null)
            {
                throw new Exception("Sai password");
            }
            return result;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string Sign(string docSignStr)
        {
            var signObj = JsonConvert.DeserializeObject<doc_sign>(docSignStr);
            var result = new VnptHashDao().Sign(signObj);
            if (result != null && result != "")
            {
                return result;
            }
            return "200";
        }
        private class Base64DuToan
        {
            public string MADDK { get; set; }
            public string TenPB { get; set; }
            public string TenTP { get; set; }
            public string TenHM { get; set; }
            public string TenNguoiLap { get; set; }
            public string TenHieu { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetBase64RDLC(string base64DuToanStr)
        {
            var base64DuToan = JsonConvert.DeserializeObject<Base64DuToan>(base64DuToanStr);
            var lapChietTinhHue = new LapChietTinhHue();
            //var objUi = lapChietTinhHue.ctDao.Get(MADDK);
            var objUi = lapChietTinhHue.ctDao.Get(base64DuToan.MADDK);
            var check = objUi.LOAIDK;
            if (check == null)
            {
                //ShowError("Chưa lưu thông tin chiết tính", txtTENKH.ClientID);
                //HideDialog("divInFree");
                //CloseWaitingDialog();
                return null;
            }

            //if (!lapChietTinhHue.IsDataValid())
            //    return new Base64DuToanRes();
            //DuToanLapMoiSuper();
            //UnblockDialog("divInFree");

            //var myds = new ReportClass().DuToanIn(MADDK);
            var myds = new ReportClass().DuToanIn(base64DuToan.MADDK);

            //if (myds == null || myds.Tables.Count == 0)
            //{
            //    CloseWaitingDialog();
            //    return;
            //}
            var chiettinh = new ReportDataSource("LDM_CHIETTINH", myds.Tables[0]);
            var ctct = new ReportDataSource("CTCHIETTINH", myds.Tables[1]);
            var ctct117 = new ReportDataSource("CTCHIETTINH117", myds.Tables[2]);
            var rpViewer = new ReportViewer();

            rpViewer.LocalReport.DataSources.Clear();
            rpViewer.LocalReport.ReportPath = "Reports/DonLapDatMoi/DuToanTruoc.rdlc";
            rpViewer.LocalReport.DataSources.Add(chiettinh);
            rpViewer.LocalReport.DataSources.Add(ctct);
            rpViewer.LocalReport.DataSources.Add(ctct117);
            //rpViewer.DataBind();
            
            //var tenpb = LoginInfo.NHANVIEN.PHONGBAN.TENPB.ToUpper();
            var tenpb = base64DuToan.TenPB;
            var prPhongBan = new ReportParameter("prPhongBan", "GIÁM ĐỐC "+ tenpb);
            //var prTruongPhong = new ReportParameter("prTruongPhong", txtTruongPhong.Text.Trim());
            var prTruongPhong = new ReportParameter("prTruongPhong", base64DuToan.TenTP);
            //var prNguoiLap = new ReportParameter("prNguoiLap", LoginInfo.NHANVIEN.HOTEN);
            var prNguoiLap = new ReportParameter("prNguoiLap", base64DuToan.TenNguoiLap);

            rpViewer.LocalReport.SetParameters(new[] { prTruongPhong, prNguoiLap, prPhongBan });
            rpViewer.LocalReport.Refresh();
            //divCR.Visible = true;
            //uplInFree.Update();

            //var pdfViewer = Convert.ToBase64String(rpViewer.LocalReport.Render("PDF"));
            var base64DuToanRes = new Base64DuToanRes();
            
            var DuToanTruocBase64 = Convert.ToBase64String(rpViewer.LocalReport.Render("PDF"));

            var rpViewer2 = new ReportViewer();

            //var myds2 = new ReportClass().NghiemThu(MADDK);
            var myds2 = new ReportClass().NghiemThu(base64DuToan.MADDK);
            if (myds2 == null || myds2.Tables.Count == 0) { return null; }
            var ds2 = new ReportDataSource("LDM_NGHIEMTHU", myds2.Tables[0]);
            rpViewer2.LocalReport.DataSources.Clear();
            rpViewer2.LocalReport.ReportPath = "Reports/DonLapDatMoi/NghiemThu.rdlc";
            rpViewer2.LocalReport.DataSources.Add(ds2);

            //rpViewer2.DataBind();

            //var rpXiNghiep = new ReportParameter("rpXiNghiep", LoginInfo.NHANVIEN.KHUVUC.TENHIEU.ToUpper());
            var rpXiNghiep = new ReportParameter("rpXiNghiep", base64DuToan.TenHieu.ToUpper());
            var prTieuDe = new ReportParameter("prTieuDe", base64DuToan.TenHM.Trim());
            //var diachi = "Địa chỉ :" + LoginInfo.NHANVIEN.KHUVUC.DIACHI + "   Điện thoại :" +
            //             LoginInfo.NHANVIEN.KHUVUC.DIENTHOAI;
            var diachi = "Địa chỉ :" + base64DuToan.DiaChi + "   Điện thoại :" +
                         base64DuToan.DienThoai;
            var rpDiaChi = new ReportParameter("rpDiaChi", diachi);
            rpViewer2.LocalReport.SetParameters(new[] { prTieuDe, rpXiNghiep, rpDiaChi });
            rpViewer2.LocalReport.Refresh();
            //base64DuToanRes.NghiemThu = Convert.ToBase64String(rpViewer2.LocalReport.Render("PDF"));
            var nghiemThuBase64 = Convert.ToBase64String(rpViewer2.LocalReport.Render("PDF"));
            var listBase64 = new List<string>
            {
                DuToanTruocBase64,
                nghiemThuBase64
            };
            return GhepFilePDF(listBase64);
            //return base64DuToanRes;
            //divCR2.Visible = true;

            //divCR.Visible = true;
            //divCR2.Visible = true;
            //uplInFree.Update();
            //Session["LM_DATA"] = null;
            //CloseWaitingDialog();
        }
        public class LuuKySoReq
        {
            public string Base64Pdf { get; set; }
            public string MaDDK { get; set; }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static int LuuFileKySo(string luuKySoReqStr)
        {
            var luuKySoReq = JsonConvert.DeserializeObject<LuuKySoReq>(luuKySoReqStr);
            var rootFolder = ConfigurationManager.AppSettings["PathKySo"];
            var folderKySoLapDat = rootFolder + "\\LapDatNuoc\\";
            if (!Directory.Exists(folderKySoLapDat))
            {
                Directory.CreateDirectory(folderKySoLapDat);
            }
            var folderKySoLapDatDuToan = rootFolder + "\\LapDatNuoc\\DuToan\\";
            if (!Directory.Exists(folderKySoLapDatDuToan))
            {
                Directory.CreateDirectory(folderKySoLapDatDuToan);
            }
            var ddkDao = new DonDangKyDao();
            var tenFilePdf = String.Format("Duyet_Du_Toan_Lap_Dat_{0}.pdf", luuKySoReq.MaDDK);
            var pathfileKySo = folderKySoLapDatDuToan + "\\" + tenFilePdf;
            if (File.Exists(pathfileKySo))
            {
                File.Delete(pathfileKySo);
            }
            byte[] pdfBytes = Convert.FromBase64String(luuKySoReq.Base64Pdf);
            File.WriteAllBytes(pathfileKySo, pdfBytes);
            return ddkDao.UpdateKySoDuToanDuyetDon("\\LapDatNuoc\\DuToan\\"+tenFilePdf, luuKySoReq.MaDDK);
        }
        public static string GhepFilePDF(List<string> listBase64)
        {
            if (listBase64.Count > 0)
            {
                using (var outputPdfStream = new MemoryStream())
                {
                    var pdfDocument = new iTextSharp.text.Document();

                    var pdfCopy = new itextSharpPDF.PdfCopy(pdfDocument, outputPdfStream);

                    pdfDocument.Open();

                    foreach (var base64Str in listBase64)
                    {
                        byte[] pdfBytes = Convert.FromBase64String(base64Str);

                        var inputPdf = new itextSharpPDF.PdfReader(pdfBytes);
                        int pageCount = inputPdf.NumberOfPages;
                        for (int i = 1; i <= pageCount; i++)
                        {
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(inputPdf, i));
                        }
                    }

                    pdfDocument.Close();

                    byte[] mergedPdfBytes = outputPdfStream.ToArray();
                    return Convert.ToBase64String(mergedPdfBytes);
                }
            }
            else
            {
                return JsonConvert.SerializeObject(listBase64);
            }
        }
        //KY-SO----------------------------------------------------------------------
        protected void btnProfile_Click(object sender, EventArgs e)
        {
            var ct = ChietTinh;
            if (ct == null)
            {
                CloseWaitingDialog();
                ShowError("Đơn đăng ký không tồn tại.");
                return;
            }

            Session[SessionKey.TK_HOSOTHIETKE_MADDK] = ct.MADDK;
            ReDirect("/Forms/ThietKe/HoSoThietKe.aspx");

            CloseWaitingDialog();
        }
     
        protected void btnFree_Click(object sender, EventArgs e)
        {
            var objUi = ctDao.Get(MADDK);
            var check = objUi.LOAIDK;
            if (check == null)
            {
                ShowError("Chưa lưu thông tin chiết tính", txtTENKH.ClientID);
                HideDialog("divInFree");
                CloseWaitingDialog();
                return;
            }

            if (!IsDataValid())
                return;
            DuToanLapMoiSuper();
            UnblockDialog("divInFree");
            var myds = new ReportClass().DuToanIn(MADDK);
            if (myds == null || myds.Tables.Count == 0)
            {
                CloseWaitingDialog();
                return;
            }
            var chiettinh = new ReportDataSource("LDM_CHIETTINH", myds.Tables[0]);
            var ctct = new ReportDataSource("CTCHIETTINH", myds.Tables[1]);
            var ctct117 = new ReportDataSource("CTCHIETTINH117", myds.Tables[2]);

            rpViewer.LocalReport.DataSources.Clear();
            rpViewer.LocalReport.ReportPath = "Reports/DonLapDatMoi/DuToanTruoc.rdlc";
            rpViewer.LocalReport.DataSources.Add(chiettinh);
            rpViewer.LocalReport.DataSources.Add(ctct);
            rpViewer.LocalReport.DataSources.Add(ctct117);
            rpViewer.DataBind();
            
            var tenpb = LoginInfo.NHANVIEN.PHONGBAN.TENPB.ToUpper();
            var prPhongBan = new ReportParameter("prPhongBan", "GIÁM ĐỐC "+ tenpb);
            var prTruongPhong = new ReportParameter("prTruongPhong", txtTruongPhong.Text.Trim());
            var prNguoiLap = new ReportParameter("prNguoiLap", LoginInfo.NHANVIEN.HOTEN);

           
            rpViewer.LocalReport.SetParameters(new[] { prTruongPhong, prNguoiLap, prPhongBan });
            rpViewer.LocalReport.Refresh();
            divCR.Visible = true;
            uplInFree.Update();

            //var pdfViewer = Convert.ToBase64String(rpViewer.LocalReport.Render("PDF"));

            var myds2 = new ReportClass().NghiemThu(MADDK);
            if (myds2 == null || myds2.Tables.Count == 0) { CloseWaitingDialog(); return; }
            var ds2 = new ReportDataSource("LDM_NGHIEMTHU", myds2.Tables[0]);
            rpViewer2.LocalReport.DataSources.Clear();
            rpViewer2.LocalReport.ReportPath = "Reports/DonLapDatMoi/NghiemThu.rdlc";
            rpViewer2.LocalReport.DataSources.Add(ds2);
            rpViewer2.DataBind();
            var rpXiNghiep = new ReportParameter("rpXiNghiep", LoginInfo.NHANVIEN.KHUVUC.TENHIEU.ToUpper());
            var prTieuDe = new ReportParameter("prTieuDe", txtTenHM.Text.Trim());
            var diachi = "Địa chỉ :" + LoginInfo.NHANVIEN.KHUVUC.DIACHI + "   Điện thoại :" +
                         LoginInfo.NHANVIEN.KHUVUC.DIENTHOAI;
            var rpDiaChi = new ReportParameter("rpDiaChi", diachi);
            rpViewer2.LocalReport.SetParameters(new[] { prTieuDe, rpXiNghiep, rpDiaChi });
            rpViewer2.LocalReport.Refresh();
            divCR2.Visible = true;

            divCR.Visible = true;
            divCR2.Visible = true;
            uplInFree.Update();
            Session["LM_DATA"] = null;
            CloseWaitingDialog();
        }

        protected void btnCT_Click(object sender, EventArgs e)
        {
            var objUi = ctDao.Get(MADDK);
            var check = objUi.LOAIDK;
            if (check == null)
            {
                ShowError("Chưa lưu thông tin chiết tính", txtTENKH.ClientID);
                HideDialog("divInCT");
                CloseWaitingDialog();
                return;
            }

            if (!IsDataValid())
                return;
            DuToanCaiTaoSuper();
            UnblockDialog("divInCT");
            var myds = new ReportClass().DuToanIn(MADDK);
            if (myds == null || myds.Tables.Count == 0)
            {
                CloseWaitingDialog();
                return;
            }
            var chiettinh = new ReportDataSource("LDM_CHIETTINH", myds.Tables[0]);
            var ctct = new ReportDataSource("CTCHIETTINH", myds.Tables[1]);
            var ctct117 = new ReportDataSource("CTCHIETTINH117", myds.Tables[2]);
            rpViewer3.LocalReport.DataSources.Clear();
            rpViewer3.LocalReport.ReportPath = "Reports/DonLapDatMoi/DuToanCT.rdlc";
            rpViewer3.LocalReport.DataSources.Add(chiettinh);
            rpViewer3.LocalReport.DataSources.Add(ctct);
            rpViewer3.LocalReport.DataSources.Add(ctct117);
            rpViewer3.DataBind();
         
            var tenpb = LoginInfo.NHANVIEN.PHONGBAN.TENPB;
            var prPhongBan = new ReportParameter("prPhongBan", tenpb);
            var prTruongPhong = new ReportParameter("prTruongPhong", txtTruongPhong.Text.Trim());
            var prNguoiLap = new ReportParameter("prNguoiLap", LoginInfo.NHANVIEN.HOTEN);
            var prChiPhiXayDungTruocThue = new ReportParameter("prChiPhiXayDungTruocThue", (objUi.DONDANGKY.LOAIDK == "DKP" || objUi.DONDANGKY.LOAIDK == "CT")?"T+C+TL":"T+C+TL+KD");
            rpViewer3.LocalReport.SetParameters(new[] { prTruongPhong, prNguoiLap, prPhongBan, prChiPhiXayDungTruocThue });
          
            rpViewer3.LocalReport.Refresh();

            divCR3.Visible = true;
            uplInCT.Update();

            var myds2 = new ReportClass().NghiemThu(MADDK);
            if (myds2 == null || myds2.Tables.Count == 0) { CloseWaitingDialog(); return; }
            var ds2 = new ReportDataSource("LDM_NGHIEMTHU", myds2.Tables[0]);
            rpViewer4.LocalReport.DataSources.Clear();
            rpViewer4.LocalReport.ReportPath = "Reports/DonLapDatMoi/NghiemThu.rdlc";
            rpViewer4.LocalReport.DataSources.Add(ds2);
            rpViewer4.DataBind();
            var rpXiNghiep = new ReportParameter("rpXiNghiep", LoginInfo.NHANVIEN.KHUVUC.TENHIEU.ToUpper());
            var prTieuDe = new ReportParameter("prTieuDe", txtTenHM.Text.Trim());
            var diachi = "Địa chỉ :" + LoginInfo.NHANVIEN.KHUVUC.DIACHI + "   Điện thoại :" +
                         LoginInfo.NHANVIEN.KHUVUC.DIENTHOAI;
            var rpDiaChi = new ReportParameter("rpDiaChi", diachi);
           
            rpViewer4.LocalReport.SetParameters(new[] { prTieuDe,rpDiaChi,rpXiNghiep,prTieuDe });
            rpViewer4.LocalReport.Refresh();
            divCR4.Visible = true;

            divCR3.Visible = true;
            divCR4.Visible = true;
            uplInCT.Update();
            Session["LM_DATA"] = null;
            CloseWaitingDialog();
        }
        #endregion

        #region Note
        private void BindGhiChu()
        {
            if (ChietTinh == null) return;

            var list = gcDao.GetList(MADDK);

            gvGhiChu.DataSource = list;
            gvGhiChu.PagerInforText = list.Count.ToString();
            gvGhiChu.DataBind();
        }
        protected void gvGhiChu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var magc = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "DeleteGhiChu":
                    var deletingGC = gcDao.Get(Int32.Parse(magc));
                    if (deletingGC == null)
                    {
                        CloseWaitingDialog();
                        return;
                    }

                    gcDao.Delete(deletingGC);
                    BindGhiChu();

                    CloseWaitingDialog();

                    break;
            }
        }
        protected void gvGhiChu_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!e.Row.RowType.Equals(DataControlRowType.DataRow)) return;

            var txtSL = e.Row.FindControl("txtNOIDUNG") as TextBox;
            if (txtSL == null) return;

            var source = gvGhiChu.DataSource as List<GHICHU>;
            if (source == null) return;

            var magc = source[e.Row.RowIndex].MAGC;

            var script = "javascript:updateGCCT(\"" + magc + "\", \"" + txtSL.ClientID + "\")";
            txtSL.Attributes.Add("onblur", script);

            var btnDeleteItem = e.Row.FindControl("btnDelete") as LinkButton;
            if (btnDeleteItem == null) return;
            btnDeleteItem.Attributes.Add("onclick", "onClientClickGridDelete('" + CommonFunc.UniqueIDWithDollars(btnDeleteItem) + "')");
        }
        protected void btnAddGhiChu_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChietTinh == null)
                {
                    CloseWaitingDialog();
                    return;
                }

                var gcct = new GHICHU
                {
                    MADDK = ChietTinh.MADDK,
                    NOIDUNG = ""
                };

                gcDao.Insert(gcct);
                BindGhiChu();

                CloseWaitingDialog();
            }
            catch (Exception ex)
            {
                DoError(new Message(MessageConstants.E_EXCEPTION, MessageType.Error, ex.Message, ex.StackTrace));
            }
        }
        #endregion

        #region Selected Changed

         public   void changeloaidk()
         {
             if (ddlLoaiDon.SelectedValue == LOAIDK.CT.ToString())
             {
                
                 SetControlValue(txtTenHM.ClientID, "CẢI TẠO HỆ THỐNG CẤP NƯỚC");
                 chkGiam.Checked = true;
                 btnCT.Visible = true;
                 btnFree.Visible = false;
                 uplGiamGia.Visible = true;
            
                 upnlCustomers.Update();
             }
             else if (ddlLoaiDon.SelectedValue == LOAIDK.DKP.ToString())
             {
                
                 SetControlValue(txtTenHM.ClientID, "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC");
                 chkGiam.Checked = false;
                 btnCT.Visible = true;
                 btnFree.Visible = false;
                 uplGiamGia.Visible = false;
            
                 upnlCustomers.Update();
             }
           
             else
             {
                 chkGiam.Checked = false;
                 uplGiamGia.Visible = false;
                
                 SetControlValue(txtTenHM.ClientID, "LẮP ĐẶT HỆ THỐNG CẤP NƯỚC");
                 if (ddlLoaiDon.SelectedValue == "BX")
                 {
                     SetControlValue(txtTenHM.ClientID, "BỔ SUNG DỰ TOÁN HỆ THỐNG CẤP NƯỚC");
                 }
                 btnCT.Visible = false;
                 btnFree.Visible = true;
                 upnlCustomers.Update();
             }
         }
        protected void ddlLoaiDon_SelectedIndexChanged(object sender, EventArgs e)
        {

            changeloaidk();
            CloseWaitingDialog();
        }
        protected void chkGiam_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGiam.Checked)
            {
                uplGiamGia.Visible = true;
                SetControlValue(txtGncTrc.ClientID, "0");
                SetControlValue(txtGvlTrc.ClientID, "0");
                SetControlValue(txtGtcTrc.ClientID, "50");
                SetControlValue(txtGncSau.ClientID, "0");
                SetControlValue(txtGvlSau.ClientID, "0");
                SetControlValue(txtGtcSau.ClientID, "0");

            }
            else
            {
                SetControlValue(txtGncTrc.ClientID, "0");
                SetControlValue(txtGvlTrc.ClientID, "0");
                SetControlValue(txtGtcTrc.ClientID, "0");
                SetControlValue(txtGncSau.ClientID, "0");
                SetControlValue(txtGvlSau.ClientID, "0");
                SetControlValue(txtGtcSau.ClientID, "0");
                uplGiamGia.Visible = false;
            }
            upnlCustomers.Update();
            CloseWaitingDialog();
        }
        #endregion

        protected void linkCMA_Click(object sender, EventArgs e)
        {
            if (txtCMA.Text.Trim() == "")
            {
                CloseWaitingDialog();
                return;
            }

            var cma = cDao.Get(txtCMA.Text.Trim());
            if (cma != null)
            {
                lblCMA.Text = cma.TENCMA;
                CloseWaitingDialog();
            }
            else
            {
                CloseWaitingDialog();
                ShowWarning("Mã CMA không hợp lệ");
            }
        }
        protected void linkDp_Click(object sender, EventArgs e)
        {
            if (txtMaDp.Text.Trim() == "")
            {
                var list = dpldDao.GetListVietTat("000");
                CommonFunc.BindDropDownList(ddlTenDuong, list, "TENDUONGLD", "MADPLD", false);
                ddlTenDuong.SelectedIndex = 0;
                CloseWaitingDialog();
                return;
            }

            var listDp = dpldDao.GetListVietTat(txtMaDp.Text.Trim());
            if (listDp != null)
            {
                CommonFunc.BindDropDownList(ddlTenDuong, listDp, "TENDUONGLD", "MADPLD", false);
                CloseWaitingDialog();
            }
            else
            {
                CloseWaitingDialog();
                ShowWarning("Mã đường phố không hợp lệ");
            }

            SetFocus(txtMaDp);
            upnlInfor.Update();
        }
        protected void linkPhuong_Click(object sender, EventArgs e)
        {
            var listP = pDao.GetListTat(txtTatPhuong.Text.Trim());
            if (listP != null)
            {
                CommonFunc.BindDropDownList(ddlPhuong, listP, "TENPHUONG", "MAPHUONG", false);
                CloseWaitingDialog();
            }
            else
            {
                CloseWaitingDialog();
                ShowWarning("Mã phường không hợp lệ");
            }
            SetFocus(txtTatPhuong);
            upnlInfor.Update();
        }

        protected void btnSave1_Click1(object sender, EventArgs e)
        {
            SaveCT();
            SaveQT();

            DONDANGKY don = DonDangKy;
            if (don == null)
            {
                CloseWaitingDialog();
                return;
            }
            var ddk = ddkDao.Get(MADDK);
            ddk.MACMA = txtCMA.Text.Trim();
            ddk.MAPHUONG = ddlPhuong.SelectedValue.Trim();
            var sonha = txtSoNha.Text.Trim();
            var p = pDao.Get(ddlPhuong.SelectedValue.Trim());
            var tenphuong = p.TENPHUONG;
            var d = dpldDao.Get(ddlTenDuong.SelectedValue.Trim());
            var tenduong = d.TENDUONGLD;
            ddk.DIACHILD = string.Format("{0}, {1}, {2}", sonha, tenduong, tenphuong);
            ddk.MAQUAN = pDao.Get(ddlPhuong.SelectedValue.Trim()).MAQUAN;
            ddk.TENKH = txtTENKH.Text.Trim();
            ddk.DIENTHOAI = txtDIENTHOAI.Text.Trim();
            ddk.DIDONG = txtDiDong.Text.Trim();
            ddk.SONHA = txtSoNha.Text.Trim();
            ddk.MADPLD = ddlTenDuong.SelectedValue.Trim();
            ddk.LOAIDK = ddlLoaiDon.SelectedValue.Trim();
            ddk.MACMA = txtCMA.Text.Trim();
            
            if (chkGiam.Checked)
            {
                ddk.GIAM = true;
            }
            else
            {
                ddk.GIAM = false;
            }


            //binhta
            if (chkGiamAdb.Checked)
            {
                ddk.GIAMADB2018 = true;
            }
            else
            {
                ddk.GIAMADB2018 = false;
            }

            ddk.MANVSUA = LoginInfo.MANV;
            ddk.NGAYSUA = DateTime.Now;
            //ddk.TTCT = TTCT.CT_P.ToString();
            //ddk.TTDCT = TTDCT.TTDCT_N.ToString();
            //ddk.ISDUTOAN = true;
            Message dangky = ddkDao.Update(ddk, ComputerName, IpAdress, LoginInfo.MANV);
            if (dangky == null) return;
            if (dangky.MsgType != MessageType.Error)
            {
                ShowInfor("Cập nhật thông tin thành công.");
                 // Lưu log nội dung
                if (isSaveLog && !string.IsNullOrEmpty(ddk.GHICHU))
                {
                    CONTENT_LOG log = new CONTENT_LOG()
                    {
                        MADON = ddk.MADDK,
                        TACVU = "Lập dự toán",
                        NOIDUNG = "",
                        GHICHU = ddk.GHICHU,
                        NGAYTHUCHIEN = DateTime.Now
                    };
                    var msg = ddkDao.saveContentLog(log);
                    if (msg.MsgType == MessageType.Info)
                    {
                        isSaveLog = false;
                    }
                }
            }
            else
            {
                ShowError("Cập nhật thông tin không thành công.");
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            ReDirectFromParent("/Forms/ThietKe/LapChietTinhHue.aspx");
        }

        protected void chkGiamAdb_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGiamAdb.Checked)
            {
                uplGiamGia.Visible = true;
                SetControlValue(txtGncTrc.ClientID, "0");
                SetControlValue(txtGvlTrc.ClientID, "0");
                SetControlValue(txtGtcTrc.ClientID, "0");
                SetControlValue(txtGncSau.ClientID, "50");
                SetControlValue(txtGvlSau.ClientID, "0");
                SetControlValue(txtGtcSau.ClientID, "0");

            }
            else
            {
                SetControlValue(txtGncTrc.ClientID, "0");
                SetControlValue(txtGvlTrc.ClientID, "0");
                SetControlValue(txtGtcTrc.ClientID, "0");
                SetControlValue(txtGncSau.ClientID, "0");
                SetControlValue(txtGvlSau.ClientID, "0");
                SetControlValue(txtGtcSau.ClientID, "0");
                txtGncSau.Text = "0";
                uplGiamGia.Visible = false;
            }
            upnlCustomers.Update();
            CloseWaitingDialog();
        }

        protected void txtGhiChu_TextChanged(object sender, EventArgs e)
        {
            isSaveLog = true;
        }

        protected void txtMaLoTrinh_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMaLoTrinh.Text.Trim()))
            {
                var duongPho = dpDao.Get(txtMaLoTrinh.Text.Trim());
                if (duongPho != null)
                    SetControlValue(txtTenLT.ClientID, duongPho.TENDP);
            }
            CloseWaitingDialog();
        }
    }
}
