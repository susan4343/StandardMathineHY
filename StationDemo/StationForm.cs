using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionIoLib;
using System.Threading.Tasks;
using System.Threading;
using HalconDotNet;
using CameraLib;

using CommonTools;
using System.Diagnostics;
using BaseDll;
using VisionProcess;

//using HalconLib;
namespace StationDemo
{
    public delegate void StationPosChangedHandle(string strStationName, string posName);


    public partial class StationForm : Form, IUserRightSwitch
    {
        public static event StationPosChangedHandle eventStationPosChanged;
        Stationbase m_Stationbase = null;
        //整个IO列表
        Dictionary<string, IOMgr.IoDefine> m_dicInput;
        Dictionary<string, IOMgr.IoDefine> m_dicOutput;
        //工站IO-->datagridview 第一列名
        //Dictionary<string, int> m_dicNameIndexInput = new Dictionary<string, int>();
        //Dictionary<string, int> m_dicNameIndexOutput = new Dictionary<string, int>();

        public UserRight userRight { get; set; }

        public StationForm()
        {
            InitializeComponent();
        }
        public void SetBtnStartEnable(bool bEnable)
        {
            button_start.Visible = bEnable;
        }
        void ChangeState(Label labelControl, bool bval)
        {
            labelControl.Text = bval ? "ON" : "OFF";
            labelControl.BackColor = bval ? Color.LightGreen : Color.LightBlue;
        }
        void ChangeStateSeverOnBtn(Button button, bool bval)
        {
            button.Text = bval ? "伺服ON" : "伺服OFF";
            button.BackColor = bval ? Color.LightGreen : Color.LightBlue;
        }
        void ChangeAxisPos(Label label, double pos)
        {
            label.Text = pos.ToString();
        }
        public void ChangeMotionIoStateAndPos(int index, bool[] bChangeBitGet, AxisIOState axisIOState, AxisPos axisPos)
        {
            bool[] bChangeBit = new bool[bChangeBitGet.Length];
            for (int i = 0; i < bChangeBit.Length; i++)
                bChangeBit[i] = bChangeBitGet[i];
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ChangeMotionIoStateAndPos(index, bChangeBit, axisIOState, axisPos)));
            }
            else
            {
                Stationbase pbase = StationMgr.GetInstance().GetStation(this);
                if (index == pbase.AxisX)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnX, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmX, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPX, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNX, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORIX, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGX, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosX, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosX, axisPos._lCmdPos);

                }
                if (index == pbase.AxisY)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnY, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmY, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPY, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNY, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORIY, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGY, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosY, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosY, axisPos._lCmdPos);
                }
                if (index == pbase.AxisZ)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnZ, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmZ, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPZ, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNZ, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORIZ, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGZ, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosZ, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosZ, axisPos._lCmdPos);

                }
                if (index == pbase.AxisU)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnU, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmU, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPU, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNU, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORIU, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGU, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosU, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosU, axisPos._lCmdPos);

                }
                if (index == pbase.AxisTx)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnTx, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmTx, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPTx, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNTx, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORITx, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGTx, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosTx, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosTx, axisPos._lCmdPos);
                }
                if (index == pbase.AxisTy)
                {
                    if (bChangeBit[0])
                        ChangeStateSeverOnBtn(button_ServoOnTy, axisIOState._bSeverOn);
                    if (bChangeBit[1])
                        ChangeState(labelControl_AlarmTy, axisIOState._bAlarm);
                    if (bChangeBit[2])
                        ChangeState(labelControl_LimtPTy, axisIOState._bLimtP);
                    if (bChangeBit[3])
                        ChangeState(labelControl_LimtNTy, axisIOState._bLimtN);
                    if (bChangeBit[4])
                        ChangeState(labelControl_ORITy, axisIOState._bOrg);
                    if (bChangeBit[5])
                        ChangeState(labelControl_EMGTy, axisIOState._bEmg);
                    if (bChangeBit[6])
                        ChangeAxisPos(label_ActPosTy, axisPos._lActPos);
                    if (bChangeBit[7])
                        ChangeAxisPos(label_CmdPosTy, axisPos._lCmdPos);
                }

            }

        }
        public void OutIoWhenClickBtn(string str)
        {
            bool bState = IOMgr.GetInstace().ReadIoOutBit(str);
            IOMgr.GetInstace().WriteIoBit(str, !bState);
        }
        public void OutIoWhenClickDownBtn(string str)
        {
            // bool bState = IOMgr.GetInstace().ReadIoOutBit(str);
            //  IOMgr.GetInstace().WriteIoBit(str, !bState);
        }

        public void OutIoWhenClickUpBtn(string str)
        {
            //  bool bState = IOMgr.GetInstace().ReadIoOutBit(str);
            //   IOMgr.GetInstace().WriteIoBit(str, !bState);
        }
        void UpdatadataGridView_PointInfo()
        {
            dataGridView_PointInfo.Rows.Clear();
            Dictionary<string, PointInfo> tempdic = StationMgr.GetInstance().GetStation(this.Name).GetStationPointDic();
            foreach (var temp in tempdic)
            {
                dataGridView_PointInfo.Rows.Add(temp.Key, temp.Value.pointX.ToString(),
                    temp.Value.pointY.ToString(), temp.Value.pointZ.ToString(), temp.Value.pointU.ToString(),
                     temp.Value.pointTx.ToString(), temp.Value.pointTy.ToString());
            }
        }



        private void StationForm_Load(object sender, EventArgs e)
        {
            int width = 0; int height = 0;
            m_Stationbase = StationMgr.GetInstance().GetStation(this);
            comboBox_SelMotionType.SelectedIndex = 0;
            UpdatadataGridView_PointInfo();
            dataGridView_PointInfo.AllowUserToDeleteRows = true;
            m_dicInput = IOMgr.GetInstace().GetInputDic();
            m_dicOutput = IOMgr.GetInstace().GetOutputDic();

            int indexInput = 0;
            foreach (var tem in m_Stationbase.m_listIoInput)
            {
                if (m_dicInput.ContainsKey(tem))
                {
                    //dataGridView_ioInput.Rows.Add(tem, "OFF");
                    // dataGridView_ioInput.Rows[indexInput].Cells[1].Style.BackColor = Color.Blue;
                    // m_dicNameIndexInput[tem] = indexInput;
                    //indexInput++;
                    userPanel_Input.AddFlag(tem);
                    userPanel_Input.SetLebalState(tem, IOMgr.GetInstace().ReadIoInBit(tem));
                }
                else
                {
                    MessageBox.Show(tem + "不在IO 表中，请检查工站IO配置", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
#if false
            for (int n = 0; n < dataGridView_ioInput.Columns.Count; n++)
                width += dataGridView_ioInput.Columns[n].Width;
            dataGridView_ioInput.Width = width;
            for (int i = 0; i < dataGridView_ioInput.Rows.Count; i++)
                height += dataGridView_ioInput.Rows[i].Height;
            dataGridView_ioInput.Height = height + dataGridView_ioInput.ColumnHeadersHeight + 2;
#endif
            if (userPanel_Input.Count == 0)
            {
                // dataGridView_ioInput.Visible = false;
                // dataGridView_ioInput.Width = 0;
                userPanel_Input.Visible = false;
            }
            userPanel_Input.m_nNumPerRow = 2;
            userPanel_Input.Update();
            int indexOutput = 0;
            foreach (var tem in m_Stationbase.m_listIoOutput)
            {

                if (m_dicOutput.ContainsKey(tem))
                {
                    // dataGridView_IoOutput.Rows.Add(tem, "OFF", "OFF");
                    // dataGridView_IoOutput.Rows[indexOutput].Cells[2].Style.BackColor = Color.Blue;
                    // m_dicNameIndexOutput[tem] = indexOutput;

                    indexOutput++;
                    userBtnPanel_Output.AddFlag(tem);
                    userBtnPanel_Output.SetBtnClickEvent(tem, OutIoWhenClickBtn);
                    userBtnPanel_Output.SetBtnClickDownEvent(tem, OutIoWhenClickDownBtn);
                    userBtnPanel_Output.SetBtnClickUpEvent(tem, OutIoWhenClickUpBtn);
                    userBtnPanel_Output.SetBtnState(tem, IOMgr.GetInstace().ReadIoOutBit(tem));
                }

                else
                {
                    MessageBox.Show(tem + "不在IO 表中，请检查工站IO配置", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
#if false
            width = 0; height = 0;
            for (int n = 0; n < dataGridView_IoOutput.Columns.Count; n++)
                width += dataGridView_IoOutput.Columns[n].Width;
            dataGridView_IoOutput.Width = width;
            for (int i = 0; i < dataGridView_IoOutput.Rows.Count; i++)
                height += dataGridView_IoOutput.Rows[i].Height;
            dataGridView_IoOutput.Height = height + dataGridView_IoOutput.ColumnHeadersHeight + 2;
#endif
            if (userBtnPanel_Output.Count == 0)
            {
                //  dataGridView_IoOutput.Visible = false;
                // dataGridView_IoOutput.Width = 0;
                userBtnPanel_Output.Visible = false;
            }
            userBtnPanel_Output.m_nNumPerRow = 1;
            userBtnPanel_Output.Update();

            sys.g_eventRightChanged += ChangedUserRight;
            sys.g_User = sys.g_User;

            visionControl1.InitWindow();
            Thread.Sleep(10);
            List<string> camname = CameraMgr.GetInstance().GetCameraNameArr();
            foreach (var temp in camname)
            {
                comboBox_SelCamera.Items.Add(temp.ToString());
            }
            if (camname != null && camname.Count > 0)
            {
                comboBox_SelCamera.Visible = true;
                comboBox_SelCamera.SelectedIndex = 0;
            }
            else
            {
                visionControl1.Visible = false;
                button_ContinuousSnap.Visible = false;
                button2.Visible = false;
                comboBox_SelVisionPR.Visible = false;
                roundButton_VisionPrTest.Visible = false;
                comboBox_SelCamera.Visible = false;
            }
            UpdataPointDataGridView();
            VisionMgr.GetInstance().PrItemChangedEvent += ChagedPrItem;
            ChagedPrItem("");
            ParamSetMgr.GetInstance().m_eventLoadProductFileUpadata += UpdatadataGridView_PointInfo;
            HOperatorSet.SetDraw(visionControl1.GetHalconWindow(), "margin");
            // dataGridView_ioInput.DefaultCellStyle.SelectionBackColor = Color.White;
            // dataGridView_ioInput.DefaultCellStyle.SelectionForeColor = Color.Black;
        }
        public void ChagedPrItem(string name)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { ChagedPrItem(name); }));
            }
            else
            {
                comboBox_SelVisionPR.Items.Clear();
                foreach (var tem in VisionMgr.GetInstance().GetItemNamesAndTypes())
                {
                    comboBox_SelVisionPR.Items.Add(tem.Key);
                }
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }
        private void UpdataMovePnBtnText()
        {
            m_Stationbase = StationMgr.GetInstance().GetStation(this);
            button_Xnegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisX) + "-";
            button_Ynegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisY) + "-";
            button_Znegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisZ) + "-";
            button_Unegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisU) + "-";
            button_Txnegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTx) + "-";
            button_Tynegtive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTy) + "-";

            button_Xpositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisX) + "+";
            button_Ypositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisY) + "+";
            button_Zpositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisZ) + "+";
            button_Upositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisU) + "+";
            button_Txpositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTx) + "+";
            button_Typositive.Text = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTy) + "+";


        }
        void UpdataPointDataGridView()
        {
            UpdataMovePnBtnText();
            Point[] points = { button_Xpositive .Location, button_Ypositive.Location, button_Zpositive.Location, button_Upositive.Location ,
            button_Txpositive.Location, button_Typositive.Location};
            //dataGridView_PointInfo.Columns[0].
            int nCount = 0;
            m_Stationbase = StationMgr.GetInstance().GetStation(this);
            string HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisX);
            if (HeaderText != "" && m_Stationbase.AxisX != -1)
            {
                dataGridView_PointInfo.Columns[1].HeaderText = HeaderText;
                nCount++;
            }

            else
            {

                dataGridView_PointInfo.Columns[1].Width = 0;
                dataGridView_PointInfo.Columns[1].Visible = false;
                button_homeX.Visible = false;
                button_ServoOnX.Visible = false;
                label_ActPosX.Visible = false;
                label_CmdPosX.Visible = false;
                labelControl_AlarmX.Visible = false;
                labelControl_LimtPX.Visible = false;
                labelControl_LimtNX.Visible = false;
                labelControl_ORIX.Visible = false;
                labelControl_EMGX.Visible = false;
                button_Xnegtive.Visible = false;
                button_Xpositive.Visible = false;


            }
            HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisY);
            if (HeaderText != "" && m_Stationbase.AxisY != -1)
            {
                dataGridView_PointInfo.Columns[2].HeaderText = HeaderText;
                button_homeY.Location = new Point(button_homeY.Location.X, points[nCount].Y);
                button_ServoOnY.Location = new Point(button_ServoOnY.Location.X, points[nCount].Y);
                label_ActPosY.Location = new Point(label_ActPosY.Location.X, points[nCount].Y); ;
                label_CmdPosY.Location = new Point(label_CmdPosY.Location.X, points[nCount].Y); ;
                labelControl_AlarmY.Location = new Point(labelControl_AlarmY.Location.X, points[nCount].Y);
                labelControl_LimtPY.Location = new Point(labelControl_LimtPY.Location.X, points[nCount].Y);
                labelControl_LimtNY.Location = new Point(labelControl_LimtNY.Location.X, points[nCount].Y);
                labelControl_ORIY.Location = new Point(labelControl_ORIY.Location.X, points[nCount].Y);
                labelControl_EMGY.Location = new Point(labelControl_EMGY.Location.X, points[nCount].Y);
                button_Ynegtive.Location = new Point(button_Ynegtive.Location.X, points[nCount].Y);
                button_Ypositive.Location = new Point(button_Ypositive.Location.X, points[nCount].Y); ;
                nCount++;


            }
            else
            {

                dataGridView_PointInfo.Columns[2].Width = 0;
                dataGridView_PointInfo.Columns[2].Visible = false;

                ///// 隐藏按钮
                button_homeY.Visible = false;
                button_ServoOnY.Visible = false;
                label_ActPosY.Visible = false;
                label_CmdPosY.Visible = false;
                labelControl_AlarmY.Visible = false;
                labelControl_LimtPY.Visible = false;
                labelControl_LimtNY.Visible = false;
                labelControl_ORIY.Visible = false;
                labelControl_EMGY.Visible = false;
                button_Ynegtive.Visible = false;
                button_Ypositive.Visible = false;

            }
            HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisZ);
            if (HeaderText != "" && m_Stationbase.AxisZ != -1)
            {
                dataGridView_PointInfo.Columns[3].HeaderText = HeaderText;

                //调整位置
                button_homeZ.Location = new Point(button_homeZ.Location.X, points[nCount].Y);
                button_ServoOnZ.Location = new Point(button_ServoOnZ.Location.X, points[nCount].Y);
                label_ActPosZ.Location = new Point(label_ActPosZ.Location.X, points[nCount].Y); ;
                label_CmdPosZ.Location = new Point(label_CmdPosZ.Location.X, points[nCount].Y); ;
                labelControl_AlarmZ.Location = new Point(labelControl_AlarmZ.Location.X, points[nCount].Y);
                labelControl_LimtPZ.Location = new Point(labelControl_LimtPZ.Location.X, points[nCount].Y);
                labelControl_LimtNZ.Location = new Point(labelControl_LimtNZ.Location.X, points[nCount].Y);
                labelControl_ORIZ.Location = new Point(labelControl_ORIZ.Location.X, points[nCount].Y);
                labelControl_EMGZ.Location = new Point(labelControl_EMGZ.Location.X, points[nCount].Y);
                button_Znegtive.Location = new Point(button_Znegtive.Location.X, points[nCount].Y);
                button_Zpositive.Location = new Point(button_Zpositive.Location.X, points[nCount].Y); ;
                nCount++;
            }

            else
            {

                dataGridView_PointInfo.Columns[3].Width = 0;
                dataGridView_PointInfo.Columns[3].Visible = false;

                ///// 隐藏按钮
                button_homeZ.Visible = false;
                button_ServoOnZ.Visible = false;
                label_ActPosZ.Visible = false;
                label_CmdPosZ.Visible = false;
                labelControl_AlarmZ.Visible = false;
                labelControl_LimtPZ.Visible = false;
                labelControl_LimtNZ.Visible = false;
                labelControl_ORIZ.Visible = false;
                labelControl_EMGZ.Visible = false;
                button_Znegtive.Visible = false;
                button_Zpositive.Visible = false;
            }
            HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisU);
            if (HeaderText != "" && m_Stationbase.AxisU != -1)
            {
                dataGridView_PointInfo.Columns[4].HeaderText = HeaderText;

                //调整位置
                button_homeU.Location = new Point(button_homeU.Location.X, points[nCount].Y);
                button_ServoOnU.Location = new Point(button_ServoOnU.Location.X, points[nCount].Y);
                label_ActPosU.Location = new Point(label_ActPosU.Location.X, points[nCount].Y); ;
                label_CmdPosU.Location = new Point(label_CmdPosU.Location.X, points[nCount].Y); ;
                labelControl_AlarmU.Location = new Point(labelControl_AlarmU.Location.X, points[nCount].Y);
                labelControl_LimtPU.Location = new Point(labelControl_LimtPU.Location.X, points[nCount].Y);
                labelControl_LimtNU.Location = new Point(labelControl_LimtNU.Location.X, points[nCount].Y);
                labelControl_ORIU.Location = new Point(labelControl_ORIU.Location.X, points[nCount].Y);
                labelControl_EMGU.Location = new Point(labelControl_EMGU.Location.X, points[nCount].Y);
                button_Unegtive.Location = new Point(button_Unegtive.Location.X, points[nCount].Y);
                button_Upositive.Location = new Point(button_Upositive.Location.X, points[nCount].Y); ;
                nCount++;

            }
            else
            {

                dataGridView_PointInfo.Columns[4].Width = 0;
                dataGridView_PointInfo.Columns[4].Visible = false;

                ///// 隐藏按钮
                button_homeU.Visible = false;
                button_ServoOnU.Visible = false;
                label_ActPosU.Visible = false;
                label_CmdPosU.Visible = false;
                labelControl_AlarmU.Visible = false;
                labelControl_LimtPU.Visible = false;
                labelControl_LimtNU.Visible = false;
                labelControl_ORIU.Visible = false;
                labelControl_EMGU.Visible = false;
                button_Unegtive.Visible = false;
                button_Upositive.Visible = false;
            }
            HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTx);
            if (HeaderText != "" && m_Stationbase.AxisTx != -1)
            {
                dataGridView_PointInfo.Columns[5].HeaderText = HeaderText;
                //调整位置
                button_homeTx.Location = new Point(button_homeTx.Location.X, points[nCount].Y);
                button_ServoOnTx.Location = new Point(button_ServoOnTx.Location.X, points[nCount].Y);
                label_ActPosTx.Location = new Point(label_ActPosTx.Location.X, points[nCount].Y); ;
                label_CmdPosTx.Location = new Point(label_CmdPosTx.Location.X, points[nCount].Y); ;
                labelControl_AlarmTx.Location = new Point(labelControl_AlarmTx.Location.X, points[nCount].Y);
                labelControl_LimtPTx.Location = new Point(labelControl_LimtPTx.Location.X, points[nCount].Y);
                labelControl_LimtNTx.Location = new Point(labelControl_LimtNTx.Location.X, points[nCount].Y);
                labelControl_ORITx.Location = new Point(labelControl_ORITx.Location.X, points[nCount].Y);
                labelControl_EMGTx.Location = new Point(labelControl_EMGTx.Location.X, points[nCount].Y);
                button_Txnegtive.Location = new Point(button_Txnegtive.Location.X, points[nCount].Y);
                button_Txpositive.Location = new Point(button_Txpositive.Location.X, points[nCount].Y); ;
                nCount++;

            }

            else
            {

                dataGridView_PointInfo.Columns[5].Width = 0;
                dataGridView_PointInfo.Columns[5].Visible = false;

                ///// 隐藏按钮
                button_homeTx.Visible = false;
                button_ServoOnTx.Visible = false;
                label_ActPosTx.Visible = false;
                label_CmdPosTx.Visible = false;
                labelControl_AlarmTx.Visible = false;
                labelControl_LimtPTx.Visible = false;
                labelControl_LimtNTx.Visible = false;
                labelControl_ORITx.Visible = false;
                labelControl_EMGTx.Visible = false;
                button_Txnegtive.Visible = false;
                button_Txpositive.Visible = false;
            }
            HeaderText = MotionMgr.GetInstace().GetAxisName(m_Stationbase.AxisTy);
            if (HeaderText != "" && m_Stationbase.AxisTy != -1)
            {
                dataGridView_PointInfo.Columns[6].HeaderText = HeaderText;

                //调整位置
                button_homeTy.Location = new Point(button_homeTy.Location.X, points[nCount].Y);
                button_ServoOnTy.Location = new Point(button_ServoOnTy.Location.X, points[nCount].Y);
                label_ActPosTy.Location = new Point(label_ActPosTy.Location.X, points[nCount].Y); ;
                label_CmdPosTy.Location = new Point(label_CmdPosTy.Location.X, points[nCount].Y); ;
                labelControl_AlarmTy.Location = new Point(labelControl_AlarmTy.Location.X, points[nCount].Y);
                labelControl_LimtPTy.Location = new Point(labelControl_LimtPTy.Location.X, points[nCount].Y);
                labelControl_LimtNTy.Location = new Point(labelControl_LimtNTy.Location.X, points[nCount].Y);
                labelControl_ORITy.Location = new Point(labelControl_ORITy.Location.X, points[nCount].Y);
                labelControl_EMGTy.Location = new Point(labelControl_EMGTy.Location.X, points[nCount].Y);
                button_Tynegtive.Location = new Point(button_Tynegtive.Location.X, points[nCount].Y);
                button_Typositive.Location = new Point(button_Typositive.Location.X, points[nCount].Y); ;
                nCount++;

            }

            else
            {

                dataGridView_PointInfo.Columns[6].Width = 0;
                dataGridView_PointInfo.Columns[6].Visible = false;

                ///// 隐藏按钮
                button_homeTy.Visible = false;
                button_ServoOnTy.Visible = false;
                label_ActPosTy.Visible = false;
                label_CmdPosTy.Visible = false;
                labelControl_AlarmTy.Visible = false;
                labelControl_LimtPTy.Visible = false;
                labelControl_LimtNTy.Visible = false;
                labelControl_ORITy.Visible = false;
                labelControl_EMGTy.Visible = false;
                button_Tynegtive.Visible = false;
                button_Typositive.Visible = false;
            }
            int nH = nCount;
            if (nH >= 1)
                nH = nCount - 1;
            else
                nH = 0;
            button_stop.Height = points[nH].Y + labelControl_EMGTy.Height - button_stop.Location.Y;

            int width = 0;
            for (int i = 0; i < dataGridView_PointInfo.ColumnCount; i++)
                width += dataGridView_PointInfo.Columns[i].Width;
            dataGridView_PointInfo.Width = width;

            button_Save.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 1, dataGridView_PointInfo.Location.Y);
            Button button = button_Save;
            button_RecordPoint.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 1, button.Location.Y + button.Height + 5);

            button = button_RecordPoint;
            button_SingleAxisMove.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 1, button.Location.Y + button.Height + 5);

            button = button_SingleAxisMove;
            button_AllAxisMove.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 1, button.Location.Y + button.Height + 5);

            button = button_AllAxisMove;
            btn_Del.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 1, button.Location.Y + button.Height + 5);


            userPanel_Input.Location = new Point(button_Save.Location.X + button_Save.Width + 5, dataGridView_PointInfo.Location.Y);
            userBtnPanel_Output.Location = new Point(userPanel_Input.Location.X + userPanel_Input.Width + 1, dataGridView_PointInfo.Location.Y);

            dataGridView_ioInput.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, dataGridView_PointInfo.Location.Y);
            dataGridView_IoOutput.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + dataGridView_ioInput.Width + 5, dataGridView_PointInfo.Location.Y);


            dataGridView_PointInfo.Columns[0].Width = (int)(0.22 * width);
            for (int i = 1; i < 6; i++)
            {
                if (dataGridView_PointInfo.Columns[i].Width == 0)
                    continue;
                else
                    dataGridView_PointInfo.Columns[i].Width = (int)(0.78 * width / nCount);
            }
        }


        private void button_homeX_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisX;
            button_homeX.Text = "回0中...";
            button_homeX.BackColor = Color.LightBlue;
            button_homeX.Enabled = false;
            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                // if (axiscofig.motorType == MotorType.SEVER)
                //     MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDX;
                    }

                }
                Thread.Sleep(800);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDX:
                button_homeX.Invoke(
                      new Action(() =>
                      {
                          button_homeX.Enabled = true;
                          button_homeX.Text = "回原点";
                          button_homeX.BackColor = Color.LightGreen;
                          button_ServoOnX.Text = "伺服OFF";
                      })
                 );

            });

        }
        private void button_homeY_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisY;
            button_homeY.Text = "回0中...";
            button_homeY.BackColor = Color.LightBlue;
            button_homeY.Enabled = false;
            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                // if (axiscofig.motorType == MotorType.SEVER)
                //     MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDY;
                    }
                }
                Thread.Sleep(50);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDY:
                button_homeY.Invoke(
                        new Action(() =>
                        {
                            button_homeY.Enabled = true;
                            button_homeY.Text = "回原点";
                            button_homeY.BackColor = Color.LightGreen;
                            button_ServoOnY.Text = "伺服OFF";
                        })
                   );

            });

        }
        private void button_homeZ_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisZ;

            button_homeZ.Text = "回0中...";
            button_homeZ.BackColor = Color.LightBlue;
            button_homeZ.Enabled = false;
            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                //  if (axiscofig.motorType == MotorType.SEVER)
                //      MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDZ;
                    }
                }
                Thread.Sleep(300);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDZ:
                button_homeZ.Invoke(
                           new Action(() =>
                           {
                               button_homeZ.Enabled = true;
                               button_homeZ.BackColor = Color.LightGreen;
                               button_homeZ.Text = "回原点";
                               button_ServoOnZ.Text = "伺服OFF";
                           })
                      );


            });
        }
        private void button_homeU_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisU;
            //  bool isori = MotionMgr.GetInstace().isOrgTrig(nAxisNo);
            button_homeU.Text = "回0中...";
            button_homeU.BackColor = Color.LightBlue;
            button_homeU.Enabled = false;

            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                //   if (axiscofig.motorType == MotorType.SEVER)
                //       MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDU;
                    }
                }
                Thread.Sleep(300);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDU:
                button_homeU.Invoke(
                         new Action(() =>
                         {
                             button_homeU.Enabled = true;
                             button_homeU.BackColor = Color.LightGreen;
                             button_homeU.Text = "回原点";
                             button_ServoOnU.Text = "伺服OFF";
                         })
                    );

            });

        }
        private void button_homeTx_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisTx;
            button_homeTx.Text = "回0中...";
            button_homeTx.BackColor = Color.LightBlue;
            button_homeTx.Enabled = false;

            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                //  if (axiscofig.motorType == MotorType.SEVER)
                //      MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDTX;
                    }
                }
                Thread.Sleep(300);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDTX:
                button_homeTx.Invoke(
                       new Action(() =>
                       {
                           button_homeTx.Enabled = true;
                           button_homeTx.Text = "回原点";
                           button_homeTx.BackColor = Color.LightGreen;
                           button_ServoOnTx.Text = "伺服OFF";
                       })
                  );

            });
        }
        private void button_homeTy_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisTy;
            button_homeTy.Text = "回0中...";
            button_homeTy.BackColor = Color.LightBlue;
            button_homeTy.Enabled = false;

            button_homeTy.Enabled = false;
            Task ts = new Task(delegate ()
            {
                AxisConfig axiscofig = new AxisConfig();
                MotionMgr.GetInstace().GetAxisConfig(nAxisNo, ref axiscofig);
                //   if (axiscofig.motorType == MotorType.SEVER)
                //       MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                MotionMgr.GetInstace().Home(nAxisNo, 0);
            });
            ts.Start();
            ts.ContinueWith((ts1) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                while (!MotionMgr.GetInstace().IsHomeNormalStop(nAxisNo))
                {
                    if (stopwatch.ElapsedMilliseconds > 90 * 1000 || bStopMove)
                    {
                        MotionMgr.GetInstace().StopAxis(nAxisNo);
                        MotionMgr.GetInstace().ServoOff((short)nAxisNo);
                        MotionMgr.GetInstace().ReasetAxisHomeFinishFlag(nAxisNo);
                        MessageBox.Show(MotionMgr.GetInstace().GetAxisName(nAxisNo) + "90秒超时" + "回原点失败", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        goto Exception_ENDTY;
                    }
                }
                Thread.Sleep(300);
                MotionMgr.GetInstace().SetAxisActualPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisCmdPos(nAxisNo, 0);
                MotionMgr.GetInstace().SetAxisHomeFinishFlag(nAxisNo);
                Exception_ENDTY:
                button_homeTy.Invoke(
                   new Action(() =>
                   {
                       button_homeTy.Enabled = true;
                       button_homeTy.BackColor = Color.LightGreen;
                       button_homeTy.Text = "回原点";
                       button_ServoOnTy.Text = "伺服OFF";
                   })
              );

            });
        }

        private void JogStart(object sender, MouseEventArgs e)
        {

            if (comboBox_SelMotionType.SelectedItem != null && comboBox_SelMotionType.SelectedItem.ToString() != "Jog")
                return;
            int nAxisNo = 1;
            switch (((Button)sender).Name)
            {
                case "button_Xpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
                    if (nAxisNo < 0) return;
                    // MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Xnegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
                    if (nAxisNo < 0) return;
                    //   MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;
                case "button_Ypositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
                    if (nAxisNo < 0) return;
                    //   MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Ynegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
                    if (nAxisNo < 0) return;
                    //    MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;
                case "button_Zpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
                    if (nAxisNo < 0) return;
                    //     MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Znegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
                    if (nAxisNo < 0) return;
                    //    MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;
                case "button_Upositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
                    if (nAxisNo < 0) return;
                    //     MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Unegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
                    if (nAxisNo < 0) return;
                    //      MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;
                case "button_Txpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
                    if (nAxisNo < 0) return;
                    //     MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Txnegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
                    if (nAxisNo < 0) return;
                    //      MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;
                case "button_Typositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
                    if (nAxisNo < 0) return;
                    //       MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, true, 0, 2);
                    break;
                case "button_Tynegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
                    if (nAxisNo < 0) return;
                    //      MotionMgr.GetInstace().ServoOn((short)nAxisNo);
                    MotionMgr.GetInstace().JogMove(nAxisNo, false, 0, 2);
                    break;

            }
        }
        private void JogEnd(object sender, MouseEventArgs e)
        {
            if (comboBox_SelMotionType.SelectedItem != null && comboBox_SelMotionType.SelectedItem.ToString() != "Jog")
                return;

            int nAxisNo;
            switch (((Button)sender).Name)
            {
                case "button_Xpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Xnegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Ypositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Ynegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Zpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Znegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Upositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Unegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Txpositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Txnegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Typositive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;
                case "button_Tynegtive":
                    nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
                    if (nAxisNo < 0) return;
                    MotionMgr.GetInstace().StopAxis(nAxisNo);
                    break;

            }
        }
        private void SelMoveType(int nAxisNo, bool bpostive, int speed)
        {
            if (comboBox_SelMotionType.SelectedItem == null || comboBox_SelMotionType.SelectedItem.ToString() == "Jog")
                return;
            if (nAxisNo < 0)
                return;
            if (comboBox_SelMotionType.SelectedItem == null || comboBox_SelMotionType.Text == null || comboBox_SelMotionType.Text == "")
                return;
            int k = bpostive ? 1 : -1;
            MotionMgr.GetInstace().RelativeMove(nAxisNo, k * comboBox_SelMotionType.Text.ToDouble(), speed);
            //switch (comboBox_SelMotionType.SelectedItem.ToString())
            //{
            //    case "微动1":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 1, speed);
            //        break;
            //    case "微动5":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 5, speed);
            //        break;
            //    case "微动10":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 10, speed);
            //        break;
            //    case "微动50":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 50, speed);
            //        break;
            //    case "微动100":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 100, speed);
            //        break;
            //    case "微动1000":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 1000, speed);
            //        break;
            //    case "微动10000":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 10000, speed);
            //        break;
            //    case "微动100000":
            //        MotionMgr.GetInstace().RelativeMove(nAxisNo, k * 100000, speed);
            //        break;
            //}

        }
        private void button_Xpositive_Click(object sender, EventArgs e)
        {

            bStopMove = false;
            string str = (this.Parent).Parent.Parent.Name;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
            SelMoveType(nAxisNo, true, 2);

        }
        private void button_Xnegtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
            SelMoveType(nAxisNo, false, 2);
        }
        private void button_Ypostive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
            SelMoveType(nAxisNo, true, 2);
        }
        private void button_Ynegtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
            SelMoveType(nAxisNo, false, 2);
        }
        private void button_Zpostive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
            SelMoveType(nAxisNo, true, 2);
        }
        private void button_Znegtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
            SelMoveType(nAxisNo, false, 2);
        }
        private void button_Upostive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
            SelMoveType(nAxisNo, true, 2);
        }
        private void buttonU_negtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
            SelMoveType(nAxisNo, false, 2);
        }
        private void button_Txpositive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
            SelMoveType(nAxisNo, true, 2);
        }

        private void button_Txnegtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
            SelMoveType(nAxisNo, false, 2);
        }

        private void button_Typositive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
            SelMoveType(nAxisNo, true, 2);
        }

        private void button_Tynegtive_Click(object sender, EventArgs e)
        {
            bStopMove = false;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
            SelMoveType(nAxisNo, false, 2);
        }
        bool bStopMove = false;
        private void button_stop_Click(object sender, EventArgs e)
        {
            bStopMove = true;
            int nAxisNo = StationMgr.GetInstance().GetStation(this).AxisX;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            nAxisNo = StationMgr.GetInstance().GetStation(this).AxisY;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            nAxisNo = StationMgr.GetInstance().GetStation(this).AxisZ;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            nAxisNo = StationMgr.GetInstance().GetStation(this).AxisU;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTx;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            nAxisNo = StationMgr.GetInstance().GetStation(this).AxisTy;
            if (nAxisNo > 0)
                MotionMgr.GetInstace().StopAxis(nAxisNo);
            button_homeX.Enabled = true;
            button_homeX.BackColor = Color.LightGreen;
            button_homeX.Text = "回原点";

            button_homeY.Enabled = true;
            button_homeY.BackColor = Color.LightGreen;
            button_homeY.Text = "回原点";

            button_homeZ.Enabled = true;
            button_homeZ.BackColor = Color.LightGreen;
            button_homeZ.Text = "回原点";

            button_homeU.Enabled = true;
            button_homeU.BackColor = Color.LightGreen;
            button_homeU.Text = "回原点";

            button_homeTx.Enabled = true;
            button_homeTx.BackColor = Color.LightGreen;
            button_homeTx.Text = "回原点";

            button_homeTy.Enabled = true;
            button_homeTy.BackColor = Color.LightGreen;
            button_homeTy.Text = "回原点";
        }
        private void CloseForm(object sender, FormClosedEventArgs e)
        {

        }

        private async void button_AllAxisMove_Click(object sender, EventArgs e)
        {
            From_OKCancel tempInfoForm = new From_OKCancel();
            System.Drawing.Point p = this.Location;
            p.X = this.Size.Width / 2;
            p.Y = this.Size.Height / 2;
            tempInfoForm.Location = p;
            tempInfoForm.m_OperateInfoText = "是否进行联动，注意设置速度，点位。请注意安全";
            tempInfoForm.ShowDialog();
            if (!tempInfoForm.OperateContiue)
                return;
            int indexRow = dataGridView_PointInfo.CurrentCell.RowIndex;
            int indexCol = dataGridView_PointInfo.CurrentCell.ColumnIndex;
            int nAxisX = -1, nAxisY = -1, nAxisZ = -1, nAxisU = -1, nAxisTx = -1, nAxisTy = -1; ;
            nAxisX = StationMgr.GetInstance().GetStation(this.Name).AxisX;
            nAxisY = StationMgr.GetInstance().GetStation(this.Name).AxisY;
            nAxisZ = StationMgr.GetInstance().GetStation(this.Name).AxisZ;
            nAxisU = StationMgr.GetInstance().GetStation(this.Name).AxisU;
            nAxisTx = StationMgr.GetInstance().GetStation(this.Name).AxisTx;
            nAxisTy = StationMgr.GetInstance().GetStation(this.Name).AxisTy;

            int index = 1;
            double posx = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double posy = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double posz = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double posu = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double postx = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double posty = Convert.ToDouble(dataGridView_PointInfo.Rows[indexRow].Cells[index++].Value);
            double SafeZ = 0;
            for (int saveI = 0; saveI < dataGridView_PointInfo.Rows.Count; saveI++)
            {
                if (dataGridView_PointInfo.Rows[saveI].Cells[0].Value != null)
                {
                    if (dataGridView_PointInfo.Rows[saveI].Cells[0].Value.ToString().Trim() == "安全位置")
                    {
                        SafeZ = Convert.ToDouble(dataGridView_PointInfo.Rows[saveI].Cells[3].Value);
                    }
                }
            }
            await Task.Run(() =>
            {
                //Z先到安全位置
                if (MotionMgr.GetInstace().IsAxisNormalStop(nAxisZ) != AxisState.NormalStop)
                {
                    MessageBox.Show($"Z轴未正常停止，请等待该轴运动完成或者重新复位后继续", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                if (nAxisZ != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisZ) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisZ, SafeZ, 2);
                }
                Stopwatch stopwatch = new Stopwatch();
                while (true)
                {
                    if (stopwatch.ElapsedMilliseconds > 30000)
                    {
                        MessageBox.Show("Z轴运动超时", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    if (MotionMgr.GetInstace().IsAxisNormalStop(nAxisZ) == AxisState.NormalStop)
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }

                if (nAxisX != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisX) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisX, posx, 2);
                }
                if (nAxisU != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisU) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisU, posu, 2);
                }
                if (nAxisTx != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisTx) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisTx, postx, 2);
                }
                if (nAxisTy != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisTy) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisTy, posty, 2);
                }
                if (nAxisY != -1)
                {
                    if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxisY) != AxisHomeFinishFlag.Homed)
                    {
                        MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                    MotionMgr.GetInstace().AbsMove(nAxisY, posy, 2);
                };//AA工位最后移动Y 比较安全
                stopwatch.Restart();
                while (true)
                {
                    AxisState axisState = 0;
                    axisState |= MotionMgr.GetInstace().IsAxisNormalStop(nAxisX);
                    axisState |= MotionMgr.GetInstace().IsAxisNormalStop(nAxisU);
                    axisState |= MotionMgr.GetInstace().IsAxisNormalStop(nAxisTx);
                    axisState |= MotionMgr.GetInstace().IsAxisNormalStop(nAxisTy);
                    axisState |= MotionMgr.GetInstace().IsAxisNormalStop(nAxisY);
                    if (axisState == AxisState.NormalStop)
                    {
                        break;
                    }
                    if (stopwatch.ElapsedMilliseconds > 60000)
                    {
                        MessageBox.Show("轴运动超时", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                }
                //最后移动Z轴
                if (nAxisZ != -1)
                    MotionMgr.GetInstace().AbsMove(nAxisZ, posz, 2);
            });
        }
        private async void button_SingleAxisMove_Click(object sender, EventArgs e)
        {

            From_OKCancel tempInfoForm = new From_OKCancel();
            System.Drawing.Point p = this.Location;
            p.X = this.Size.Width / 2;
            p.Y = this.Size.Height / 2;
            tempInfoForm.Location = p;

            tempInfoForm.m_OperateInfoText = "是否进行单轴运动，请注意安全";
            tempInfoForm.ShowDialog();
            if (!tempInfoForm.OperateContiue)
                return;
            if (dataGridView_PointInfo.CurrentCell == null)
                return;
            int indexRow = dataGridView_PointInfo.CurrentCell.RowIndex;
            int indexCol = dataGridView_PointInfo.CurrentCell.ColumnIndex;
            int nAxis = -1;

            switch (indexCol)
            {
                case 1:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisX;
                    break;
                case 2:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisY;
                    break;
                case 3:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisZ;
                    break;
                case 4:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisU;
                    break;
                case 5:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisTx;
                    break;
                case 6:
                    nAxis = StationMgr.GetInstance().GetStation(this).AxisTy;
                    break;
                default:
                    //tempInfoForm.m_OperateInfoText = "轴号选择错误";
                    // tempInfoForm.ShowDialog();

                    break;
            }
            if (MotionMgr.GetInstace().GetHomeFinishFlag(nAxis) != AxisHomeFinishFlag.Homed)
            {
                MessageBox.Show("该轴未回到原点，禁止移动", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            if (MotionMgr.GetInstace().IsAxisNormalStop(nAxis) != AxisState.NormalStop)
            {
                MessageBox.Show("该轴未正常停止，请等待该轴运动完成或者重新复位后继续", "Err", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            double pos = Convert.ToDouble(dataGridView_PointInfo.CurrentCell.Value.ToString());
            await Task.Run(() =>
            {
                if (nAxis != -1)
                    MotionMgr.GetInstace().AbsMove(nAxis, pos, 2);
            });
        
        }
        private bool CheckReName(string[] staPointName)
        {
            if (staPointName == null)
                return false;
            if (staPointName.Length <= 0)
                return false;
            for (int i = 0; i < staPointName.Length; i++)
            {
                for (int j = i + 1; j < staPointName.Length; j++)
                {
                    if (staPointName[i] == staPointName[j])
                        return true;
                }
            }
            return false;
        }
        private void button_Save_Click(object sender, EventArgs e)
        {
            From_OKCancel tempInfoForm = new From_OKCancel();
            System.Drawing.Point p = this.Location;
            p.X = this.Size.Width / 2;
            p.Y = this.Size.Height / 2;
            tempInfoForm.Location = p;

            tempInfoForm.m_OperateInfoText = "是否保存所有点位";
            tempInfoForm.ShowDialog();
            if (!tempInfoForm.OperateContiue)
                return;
            string[] strPointName = new string[dataGridView_PointInfo.Rows.Count];
            for (int i = 0; i < dataGridView_PointInfo.Rows.Count; i++)
            {
                if (dataGridView_PointInfo.Rows[i].Cells[0].Value != null && dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString() != "")
                    strPointName[i] = dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString();
            }
            if (CheckReName(strPointName))
            {
                tempInfoForm.m_OperateInfoText = "点位有重名，请更正";
                tempInfoForm.ShowDialog();
                return;
            }

            Dictionary<string, PointInfo> staPoint = new Dictionary<string, PointInfo>();
            StationMgr.GetInstance().GetStation(this.Name).GetStationPointDic(ref staPoint);
            string strPonitName = "";
            PointInfo temp;
            int index = 1;
            List<string> PointArrIndataGridView_PointInfo = new List<string>();
            PointArrIndataGridView_PointInfo.Clear();
            for (int i = 0; i < dataGridView_PointInfo.Rows.Count; i++)
            {
                index = 1;
                if (dataGridView_PointInfo.Rows[i].Cells[0].Value != null && dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString() != "")
                {
                    strPonitName = dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString();
                    temp.pointX = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.pointY = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.pointZ = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.pointU = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.pointTx = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.pointTy = Convert.ToDouble(dataGridView_PointInfo.Rows[i].Cells[index++].Value);
                    temp.handedSystem = true;
                    if (staPoint.ContainsKey(strPonitName))
                    {
                        staPoint[strPonitName] = temp;
                    }
                    else
                    {
                        staPoint.Add(strPonitName, temp);
                    }
                    PointArrIndataGridView_PointInfo.Add(strPonitName);
                }

            }
            List<string> DeletePointName = new List<string>();
            DeletePointName.Clear();
            foreach (var tem in staPoint)
            {
                if (!PointArrIndataGridView_PointInfo.Contains(tem.Key))
                    DeletePointName.Add(tem.Key);
            }
            foreach (var tem in DeletePointName)
            {
                staPoint.Remove(tem);
            }
            ConfigToolMgr.GetInstance().SavePoint(this.Name, staPoint);
            foreach (var tem in staPoint)
                eventStationPosChanged?.Invoke(this.Name, tem.Key);

            //重新加载
            Dictionary<string, PointInfo> dicPonit = new Dictionary<string, PointInfo>();
            foreach (var tem in StationMgr.GetInstance().GetAllStationName())
            {
                ConfigToolMgr.GetInstance().ReadPoint(tem, out dicPonit);
                StationMgr.GetInstance().GetStation(tem).SetStationPointDic(dicPonit);
            }
        }

        private void button_RecordPoint_Click(object sender, EventArgs e)
        {
            From_OKCancel tempInfoForm = new From_OKCancel();
            System.Drawing.Point p = this.Location;
            p.X = this.Size.Width / 2;
            p.Y = this.Size.Height / 2;
            tempInfoForm.Location = p;

            tempInfoForm.m_OperateInfoText = "是否记录此点到点位表格";
            tempInfoForm.ShowDialog();
            if (!tempInfoForm.OperateContiue)
                return;
            int nAxisNot = StationMgr.GetInstance().GetStation(this).AxisX;

            if (dataGridView_PointInfo.CurrentCell != null)
            {
                int indexRow = dataGridView_PointInfo.CurrentCell.RowIndex;
                int indexCol = dataGridView_PointInfo.CurrentCell.ColumnIndex;
                string strPointName = dataGridView_PointInfo.Rows[indexRow].Cells[0].Value.ToString();
                string[] strPointNameArr = new string[dataGridView_PointInfo.Rows.Count];
                for (int i = 0; i < dataGridView_PointInfo.Rows.Count; i++)
                {
                    if (dataGridView_PointInfo.Rows[i].Cells[0].Value != null && dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString() != "")
                        strPointNameArr[i] = dataGridView_PointInfo.Rows[i].Cells[0].Value.ToString();
                }
                if (CheckReName(strPointNameArr))
                {
                    tempInfoForm.m_OperateInfoText = "点位有重名，请更正";
                    tempInfoForm.ShowDialog();
                    return;
                }
                PointInfo tempPoint;
                int nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisX;
                if (nAxisNo >= 0)
                    tempPoint.pointX = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointX = 0;
                nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisY;
                if (nAxisNo >= 0)
                    tempPoint.pointY = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointY = 0;
                nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisZ;
                if (nAxisNo >= 0)
                    tempPoint.pointZ = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointZ = 0;
                nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisU;
                if (nAxisNo >= 0)
                    tempPoint.pointU = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointU = 0;
                nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisTx;
                if (nAxisNo >= 0)
                    tempPoint.pointTx = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointTx = 0;
                nAxisNo = StationMgr.GetInstance().GetStation(this.Name).AxisTy;
                if (nAxisNo >= 0)
                    tempPoint.pointTy = MotionMgr.GetInstace().GetAxisPos(nAxisNo);
                else
                    tempPoint.pointTy = 0;
                dataGridView_PointInfo.Rows[indexRow].Cells[1].Value =Math.Round( tempPoint.pointX,3);
                dataGridView_PointInfo.Rows[indexRow].Cells[2].Value = Math.Round( tempPoint.pointY, 3);
                dataGridView_PointInfo.Rows[indexRow].Cells[3].Value = Math.Round( tempPoint.pointZ, 3);
                dataGridView_PointInfo.Rows[indexRow].Cells[4].Value = Math.Round( tempPoint.pointU, 3);
                dataGridView_PointInfo.Rows[indexRow].Cells[5].Value = Math.Round( tempPoint.pointTx, 3);
                dataGridView_PointInfo.Rows[indexRow].Cells[6].Value = Math.Round(tempPoint.pointTy, 3);
                tempPoint.handedSystem = true;
                StationMgr.GetInstance().GetStation(this.Name).AddPoint(strPointName, tempPoint);
            }
        }

        private void button_ContinuousSnap_Click(object sender, EventArgs e)
        {
            if (comboBox_SelCamera.Text == null)
                return;
            CameraBase pCamer = CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.Text);
            pCamer.SetTriggerMode(CameraModeType.Software);
            Thread.Sleep(100);
            CameraMgr.GetInstance().BindWindow(comboBox_SelCamera.Text, visionControl1);
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.Text).SetAcquisitionMode();
            //  CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).wnd = hWindowControl1.HalconID;
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.Text).StartGrab();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox_SelCamera.SelectedItem == null)
                return;
            for (int i = 0; i < comboBox_SelCamera.Items.Count; i++)
            {
                CameraMgr.GetInstance().ClaerPr(comboBox_SelCamera.Items[i].ToString());
                CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.Text).StopGrap();
                CameraMgr.GetInstance().SetTriggerSoftMode(comboBox_SelCamera.Items[i].ToString());
            }
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).SetTriggerMode(CameraModeType.Software);
            Thread.Sleep(100);
            //  CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).wnd = hWindowControl1.HalconID;
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).StartGrab();
            CameraMgr.GetInstance().GetCamera(comboBox_SelCamera.SelectedItem.ToString()).GarbBySoftTrigger();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            string[] CameraNameArr = StationMgr.GetInstance().GetStation(this.Name).GetCameraArr();
            if (CameraNameArr != null && CameraNameArr.Length > 0)
            {
                for (int i = 0; i < CameraNameArr.Length; i++)
                {
                    comboBox_SelCamera.Items.Add(CameraNameArr[i]);
                    //   CameraMgr.GetInstance().GetCamera(CameraNameArr[i]).wnd = hWindowControl1.HalconID;//       *****
                }
                CameraMgr.GetInstance().GetCamera(CameraNameArr[0]).SetTriggerMode(CameraModeType.Software);
                CameraMgr.GetInstance().GetCamera(CameraNameArr[0]).StartGrab();
            }
            HTuple ModelID = new HTuple();
            HTuple ResultRow = new HTuple();
            HTuple ResultCol = new HTuple();
            HTuple ResultAngle = new HTuple();
            // HalconExternFunExport.Read_ncc_Model(out ModelID);
            //CameraMgr.GetInstance().GetCamera(CameraNameArr[0]).ProcessImageEvent +=
            //    delegate (HObject Img)
            //    {
            //        // HTuple row,col,angle,sorce;
            //        //  HalconExternFunExport.FindProduct(hWindowControl1.HalconID, Img,   ModelID, out row, out col, out angle, out sorce);
            //        //  HalconExternFunExport.disp_message(hWindowControl1.HalconID, "x=" + col, "image", 20, 20, "green", "false");
            //        //  HalconExternFunExport.disp_message(hWindowControl1.HalconID, "y=" + row, "image", 20, 120, "green", "false");
            //        //  HalconExternFunExport.disp_message(hWindowControl1.HalconID, "a=" + angle, "image", 20, 220, "green", "false");
            //        //if (row.Length > 0)
            //        //{
            //        //    ResultRow.TupleAdd(row);
            //        //    ResultCol.TupleAdd(col);
            //        //    ResultAngle.TupleAdd(angle);

            //        //}
            //        return true;
            //    };
            string strName = this.Name;
            Stationbase sta = StationMgr.GetInstance().GetStation(strName);
            Task sTask = new Task(
              delegate ()
              {
                  sta.ProcessWork();
              });
            sTask.Start();
        }

        private void ShowFirist(object sender, EventArgs e)
        {
            string[] CameraNameArr = StationMgr.GetInstance().GetStation(this.Name).GetCameraArr();
            List<string> camname = CameraMgr.GetInstance().GetCameraNameArr();
            if (camname != null && camname.Count > 0)
            {
                //for (int i = 0; i < CameraNameArr.Length; i++)
                //{
                //    comboBox_SelCamera.Items.Add(CameraNameArr[i]);
                //    //  CameraMgr.GetInstance().GetCamera(CameraNameArr[i]).wnd = hWindowControl1.HalconID;
                //}
                comboBox_SelCamera.Visible = true;
                comboBox_SelCamera.SelectedIndex = 0;


            }
            else
            {
                comboBox_SelCamera.Visible = false;
            }

        }


        private void PointDel(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //    int indexRow = 0;
            //  DataGridViewRow selectedRow = dataGridView_PointInfo.CurrentRow;
            //  int s=  selectedRow.Index;
            //    string strPointName = dataGridView_PointInfo.Rows[indexRow].Cells[0].ToString();
            //    ConfigToolMgr.GetInstance().DelPoint(this.Name, strPointName);
        }

        private void OnVisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdataMotion();
                MotionMgr.GetInstace().m_eventChangeMotionIoOrPos += ChangeMotionIoStateAndPos;
                MotionMgr.GetInstace().m_eventChangeMotionConfig += ChangeMotionConfig;
                IOMgr.GetInstace().m_eventIoInputChanageByName += ChangedIoInState;
                IOMgr.GetInstace().m_eventIoOutputChanageByName += ChangedIoOutState;
                ChagedPrItem("");


            }
            else
            {
                MotionMgr.GetInstace().m_eventChangeMotionIoOrPos -= ChangeMotionIoStateAndPos;
                MotionMgr.GetInstace().m_eventChangeMotionConfig -= ChangeMotionConfig;
                IOMgr.GetInstace().m_eventIoInputChanageByName -= ChangedIoInState;
                IOMgr.GetInstace().m_eventIoOutputChanageByName -= ChangedIoOutState;
            }
        }

        void ChangeMotionConfig(int nAxis, AxisConfig axisConfig)
        {
            if (m_Stationbase.AxisX == nAxis)
                button_ServoOnX.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);

            if (m_Stationbase.AxisY == nAxis)
                button_ServoOnY.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);

            if (m_Stationbase.AxisZ == nAxis)
                button_ServoOnZ.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);

            if (m_Stationbase.AxisU == nAxis)
                button_ServoOnU.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);

            if (m_Stationbase.AxisTx == nAxis)
                button_ServoOnTx.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);

            if (m_Stationbase.AxisTy == nAxis)
                button_ServoOnTy.Enabled = ((axisConfig.motorType == MotorType.SEVER) ? true : false);
        }
        void UpdataAxisIoAndPos(int nAxisNo, AxisIOState axisIOState, AxisPos pos)
        {
            if (nAxisNo == -1)
                return;
            Stationbase pbase = StationMgr.GetInstance().GetStation(this);
            if (nAxisNo == pbase.AxisX)
            {
                ChangeStateSeverOnBtn(button_ServoOnX, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmX, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPX, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNX, axisIOState._bLimtN);
                ChangeState(labelControl_ORIX, axisIOState._bOrg);
                ChangeState(labelControl_EMGX, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosX, pos._lActPos);
                ChangeAxisPos(label_CmdPosX, pos._lCmdPos);


            }
            if (nAxisNo == pbase.AxisY)
            {
                ChangeStateSeverOnBtn(button_ServoOnY, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmY, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPY, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNY, axisIOState._bLimtN);
                ChangeState(labelControl_ORIY, axisIOState._bOrg);
                ChangeState(labelControl_EMGY, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosY, pos._lActPos);
                ChangeAxisPos(label_CmdPosY, pos._lCmdPos);

            }
            if (nAxisNo == pbase.AxisZ)
            {
                ChangeStateSeverOnBtn(button_ServoOnZ, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmZ, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPZ, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNZ, axisIOState._bLimtN);
                ChangeState(labelControl_ORIZ, axisIOState._bOrg);
                ChangeState(labelControl_EMGZ, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosZ, pos._lActPos);
                ChangeAxisPos(label_CmdPosZ, pos._lCmdPos);
            }
            if (nAxisNo == pbase.AxisU)
            {
                ChangeStateSeverOnBtn(button_ServoOnU, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmU, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPU, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNU, axisIOState._bLimtN);
                ChangeState(labelControl_ORIU, axisIOState._bOrg);
                ChangeState(labelControl_EMGU, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosU, pos._lActPos);
                ChangeAxisPos(label_CmdPosU, pos._lCmdPos);
            }
            if (nAxisNo == pbase.AxisTx)
            {
                ChangeStateSeverOnBtn(button_ServoOnTx, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmTx, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPTx, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNTx, axisIOState._bLimtN);
                ChangeState(labelControl_ORITx, axisIOState._bOrg);
                ChangeState(labelControl_EMGTx, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosTx, pos._lActPos);
                ChangeAxisPos(label_CmdPosTx, pos._lCmdPos);
            }
            if (nAxisNo == pbase.AxisTy)
            {
                ChangeStateSeverOnBtn(button_ServoOnTy, axisIOState._bSeverOn);
                ChangeState(labelControl_AlarmTy, axisIOState._bAlarm);
                ChangeState(labelControl_LimtPTy, axisIOState._bLimtP);
                ChangeState(labelControl_LimtNTy, axisIOState._bLimtN);
                ChangeState(labelControl_ORITy, axisIOState._bOrg);
                ChangeState(labelControl_EMGTy, axisIOState._bEmg);
                ChangeAxisPos(label_ActPosTy, pos._lActPos);
                ChangeAxisPos(label_CmdPosTy, pos._lCmdPos);
            }
        }

        private void UpdataMotion()
        {
            Stationbase psb = StationMgr.GetInstance().GetStation(this);
            AxisIOState axisIOState = new AxisIOState();
            AxisPos axisPos = new AxisPos();
            AxisConfig axisConfig = new AxisConfig();

            MotionMgr.GetInstace().GetAxisIOState(psb.AxisX, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisX, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisX, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisX, ref axisConfig);
            button_ServoOnX.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisX == -1)
            {
                label_ActPosX.Enabled = false;
                label_CmdPosX.Enabled = false;
                labelControl_AlarmX.Enabled = false;
                labelControl_LimtPX.Enabled = false;
                labelControl_LimtNX.Enabled = false;
                labelControl_ORIX.Enabled = false;
                labelControl_EMGX.Enabled = false;

            }


            MotionMgr.GetInstace().GetAxisIOState(psb.AxisY, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisY, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisY, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisY, ref axisConfig);
            button_ServoOnY.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisY == -1)
            {
                button_ServoOnY.Visible = false;
                label_ActPosY.Enabled = false;
                label_CmdPosY.Enabled = false;
                labelControl_AlarmY.Enabled = false;
                labelControl_LimtPY.Enabled = false;
                labelControl_LimtNY.Enabled = false;
                labelControl_ORIY.Enabled = false;
                labelControl_EMGY.Enabled = false;
            }

            MotionMgr.GetInstace().GetAxisIOState(psb.AxisZ, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisZ, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisZ, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisZ, ref axisConfig);
            button_ServoOnZ.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisZ == -1)
            {
                label_ActPosZ.Enabled = false;
                label_CmdPosZ.Enabled = false;
                labelControl_AlarmZ.Enabled = false;
                labelControl_LimtPZ.Enabled = false;
                labelControl_LimtNZ.Enabled = false;
                labelControl_ORIZ.Enabled = false;
                labelControl_EMGZ.Enabled = false;
            }


            MotionMgr.GetInstace().GetAxisIOState(psb.AxisU, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisU, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisU, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisU, ref axisConfig);
            button_ServoOnU.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisU == -1)
            {
                label_ActPosU.Enabled = false;
                label_CmdPosU.Enabled = false;
                labelControl_AlarmU.Enabled = false;
                labelControl_LimtPU.Enabled = false;
                labelControl_LimtNU.Enabled = false;
                labelControl_ORIU.Enabled = false;
                labelControl_EMGU.Enabled = false;
            }

            MotionMgr.GetInstace().GetAxisIOState(psb.AxisTx, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisTx, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisTx, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisTx, ref axisConfig);
            button_ServoOnTx.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisTx == -1)
            {

                label_ActPosTx.Enabled = false;
                label_CmdPosTx.Enabled = false;
                labelControl_AlarmTx.Enabled = false;
                labelControl_LimtPTx.Enabled = false;
                labelControl_LimtNTx.Enabled = false;
                labelControl_ORITx.Enabled = false;
                labelControl_EMGTx.Enabled = false;
            }

            MotionMgr.GetInstace().GetAxisIOState(psb.AxisTy, ref axisIOState);
            MotionMgr.GetInstace().GetAxisPos(psb.AxisTy, ref axisPos);
            UpdataAxisIoAndPos(psb.AxisTy, axisIOState, axisPos);
            MotionMgr.GetInstace().GetAxisConfig(psb.AxisTy, ref axisConfig);
            button_ServoOnTy.Enabled = axisConfig.motorType == MotorType.SEVER ? true : false;
            if (psb.AxisTy == -1)
            {

                label_ActPosTy.Enabled = false;
                label_CmdPosTy.Enabled = false;
                labelControl_AlarmTy.Enabled = false;
                labelControl_LimtPTy.Enabled = false;
                labelControl_LimtNTy.Enabled = false;
                labelControl_ORITy.Enabled = false;
                labelControl_EMGTy.Enabled = false;
            }
            //工站IO 更新
            foreach (var tem in m_Stationbase.m_listIoInput)
                if (m_dicInput.ContainsKey(tem))
                    userPanel_Input.SetLebalState(tem, IOMgr.GetInstace().ReadIoInBit(tem));


            foreach (var tem in m_Stationbase.m_listIoOutput)
                if (m_dicOutput.ContainsKey(tem))
                    userBtnPanel_Output.SetBtnState(tem, IOMgr.GetInstace().ReadIoOutBit(tem));

#if false
            //工站IO 更新
            bool bval = false;
            foreach (var temp in m_dicNameIndexInput)
            {
                bval = IOMgr.GetInstace().ReadIoInBit(temp.Key);
                dataGridView_ioInput.Rows[temp.Value].Cells[1].Value = bval ? "ON" : "OFF";
                dataGridView_ioInput.Rows[temp.Value].Cells[1].Style.BackColor = bval ? Color.LightGreen : Color.LightBlue;
            }
            foreach (var temp in m_dicNameIndexOutput)
            {
                bval = IOMgr.GetInstace().ReadIoOutBit(temp.Key);
                dataGridView_IoOutput.Rows[temp.Value].Cells[1].Value = bval ? "ON" : "OFF";
                dataGridView_IoOutput.Rows[temp.Value].Cells[1].Style.BackColor = bval ? Color.LightGreen : Color.LightBlue;
                dataGridView_IoOutput.Rows[temp.Value].Cells[2].Value = bval ? "ON" : "OFF";
            }
#endif
        }
        private void button_ServoOnX_Click(object sender, EventArgs e)
        {
            if (button_ServoOnX.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisX);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisX);
            }
        }
        private void button_ServoOnY_Click(object sender, EventArgs e)
        {
            if (button_ServoOnY.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisY);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisY);
            }

        }

        private void button_ServoOnZ_Click(object sender, EventArgs e)
        {
            if (button_ServoOnZ.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisZ);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisZ);
            }
        }

        private void button_ServoOnU_Click(object sender, EventArgs e)
        {
            if (button_ServoOnU.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisU);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisU);
            }
        }

        private void button_ServoOnTx_Click(object sender, EventArgs e)
        {
            if (button_ServoOnTx.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisTx);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisTx);
            }
        }

        private void button_ServoOnTy_Click(object sender, EventArgs e)
        {
            if (button_ServoOnTy.Text == "伺服OFF")
            {
                MotionMgr.GetInstace().ServoOn((short)m_Stationbase.AxisTy);
            }
            else
            {
                MotionMgr.GetInstace().ServoOff((short)m_Stationbase.AxisTy);
            }

        }



        public void ChangedIoInState(string IoName, bool bStateCurrent)
        {
            int nRow = 0;
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ChangedIoInState(IoName, bStateCurrent)));
            }
            else
            {
                //   if (m_dicNameIndexInput.TryGetValue(IoName, out nRow))
                //  {
                userPanel_Input.SetLebalState(IoName, bStateCurrent);
                // dataGridView_ioInput.Rows[nRow].Cells[1].Value = bStateCurrent ? "ON" : "OFF";
                // dataGridView_ioInput.Rows[nRow].Cells[1].Style.BackColor = bStateCurrent ? Color.Green : Color.Blue;
                //  }

            }
        }
        public void ChangedIoOutState(string IoName, bool bStateCurrent)
        {
            int nRow = 0;
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ChangedIoOutState(IoName, bStateCurrent)));
            }
            else
            {
                //  if (m_dicNameIndexOutput.TryGetValue(IoName, out nRow))
                // dataGridView_IoOutput.Rows[nRow].Cells[1].Value = bStateCurrent ? "ON" : "OFF";
                //dataGridView_IoOutput.Rows[nRow].Cells[1].Style.BackColor = bStateCurrent ? Color.LightGreen : Color.LightBlue;
                // dataGridView_IoOutput.Rows[nRow].Cells[2].Value = bStateCurrent ? "ON" : "OFF";

                userBtnPanel_Output.SetBtnState(IoName, bStateCurrent);
            }
        }



        private void OnSelectionDataGridView(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            dgv.ClearSelection();
        }

        private void dataGridView_IoOutput_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                string ioname = dataGridView_IoOutput.Rows[e.RowIndex].Cells[0].Value.ToString();
                bool bCurrentVal = IOMgr.GetInstace().ReadIoOutBit(ioname);
                IOMgr.GetInstace().WriteIoBit(ioname, !bCurrentVal);
            }
        }

        public void ChangedUserRight(User CurrentUser)
        {
            if (InvokeRequired)
                this.BeginInvoke(new Action(() => ChangedUserRight(CurrentUser)));
            else
            {
                btn_Del.Enabled = false;
                switch ((int)CurrentUser._userRight)
                {
                    case (int)UserRight.客户操作员:
                        button_RecordPoint.Enabled = false;
                        button_Save.Enabled = false;
                        button_SingleAxisMove.Enabled = false;
                        button_AllAxisMove.Enabled = false;
                        dataGridView_PointInfo.AllowUserToAddRows = false;
                        dataGridView_PointInfo.AllowUserToDeleteRows = false;
                        dataGridView_PointInfo.ReadOnly = true;

                        break;
                    case (int)UserRight.调试工程师:
                        button_RecordPoint.Enabled = true;
                        button_Save.Enabled = true;
                        button_SingleAxisMove.Enabled = true;
                        button_AllAxisMove.Enabled = true;
                        dataGridView_PointInfo.AllowUserToAddRows = false;
                        dataGridView_PointInfo.AllowUserToDeleteRows = false;
                        dataGridView_PointInfo.ReadOnly = true;
                        dataGridView_PointInfo.Columns[0].ReadOnly = true;
                        break;
                    case (int)UserRight.软件工程师:
                        button_RecordPoint.Enabled = true;
                        button_Save.Enabled = true;
                        button_SingleAxisMove.Enabled = true;
                        button_AllAxisMove.Enabled = true;
                        dataGridView_PointInfo.AllowUserToAddRows = true;
                        dataGridView_PointInfo.AllowUserToDeleteRows = true;
                        btn_Del.Enabled = true;
                        dataGridView_PointInfo.ReadOnly = false;
                        break;
                    case (int)UserRight.超级管理员:
                        button_RecordPoint.Enabled = true;
                        button_Save.Enabled = true;
                        button_SingleAxisMove.Enabled = true;
                        button_AllAxisMove.Enabled = true;
                        dataGridView_PointInfo.AllowUserToAddRows = true;
                        dataGridView_PointInfo.AllowUserToDeleteRows = true;
                        dataGridView_PointInfo.ReadOnly = false;
                        break;
                }
                bool bEnable = false;
                if ((int)CurrentUser._userRight >= (int)UserRight.调试工程师)
                {
                    bEnable = true;
                }
                button_homeX.Enabled = bEnable;
                button_ServoOnX.Enabled = bEnable;
                button_homeY.Enabled = bEnable;
                button_ServoOnY.Enabled = bEnable;
                button_homeZ.Enabled = bEnable;
                button_ServoOnZ.Enabled = bEnable;
                button_homeU.Enabled = bEnable;
                button_ServoOnU.Enabled = bEnable;
                button_homeTx.Enabled = bEnable;
                button_ServoOnTx.Enabled = bEnable;
                button_homeTy.Enabled = bEnable;
                button_ServoOnTy.Enabled = bEnable;
                button_Xpositive.Enabled = bEnable;
                button_Ypositive.Enabled = bEnable;
                button_Zpositive.Enabled = bEnable;
                button_Upositive.Enabled = bEnable;
                button_Txpositive.Enabled = bEnable;
                button_Typositive.Enabled = bEnable;
                button_Xnegtive.Enabled = bEnable;
                button_Ynegtive.Enabled = bEnable;
                button_Znegtive.Enabled = bEnable;
                button_Unegtive.Enabled = bEnable;
                button_Txnegtive.Enabled = bEnable;
                button_Tynegtive.Enabled = bEnable;

            }

        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            button_Save.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, dataGridView_PointInfo.Location.Y);
            button_RecordPoint.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, button_RecordPoint.Location.Y);
            button_SingleAxisMove.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, button_SingleAxisMove.Location.Y);
            button_AllAxisMove.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, button_AllAxisMove.Location.Y);
            btn_Del.Location = new Point(dataGridView_PointInfo.Location.X + dataGridView_PointInfo.Width + 5, btn_Del.Location.Y);
            dataGridView_ioInput.Location = new Point(button_Save.Location.X + button_Save.Width + 5, dataGridView_PointInfo.Location.Y);
            dataGridView_IoOutput.Location = new Point(button_Save.Location.X + button_Save.Width + dataGridView_ioInput.Width + 5, dataGridView_PointInfo.Location.Y);

        }
        private void btn_Del_Click(object sender, EventArgs e)
        {
            Dictionary<string, PointInfo> tempdic = StationMgr.GetInstance().GetStation(this.Name).GetStationPointDic();
            if (dataGridView_PointInfo.SelectedCells.Count <= 0)
                return;

            int CellSelectedRow = dataGridView_PointInfo.SelectedCells[0].RowIndex;
            if (dataGridView_PointInfo.Rows[CellSelectedRow].Cells[0].Value == null)
                return;
            string strPointName = dataGridView_PointInfo.Rows[CellSelectedRow].Cells[0].Value.ToString();

            DataGridViewRow row = dataGridView_PointInfo.Rows[CellSelectedRow];
            dataGridView_PointInfo.Rows.Remove(row);
            PointInfo pointInfo = new PointInfo();
            tempdic.TryGetValue(strPointName, out pointInfo);
            tempdic.Remove(strPointName);
            ConfigToolMgr.GetInstance().DeletePoint(this.Name, strPointName);
            ConfigToolMgr.GetInstance().SavePoint(this.Name, tempdic);
        }

        private void comboBox_SelCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox_SelCamera.Items.Count; i++)
                CameraMgr.GetInstance().SetTriggerSoftMode(comboBox_SelCamera.Items[i].ToString());

            CameraMgr.GetInstance().BindWindow(comboBox_SelCamera.Text, visionControl1);
            CameraMgr.GetInstance().SetAcquisitionMode(comboBox_SelCamera.Text);
        }

        private void roundButton_Test_Click(object sender, EventArgs e)
        {
            if (comboBox_SelVisionPR.SelectedIndex == -1)
                return;
            string strVisionPrName = comboBox_SelVisionPR.Items[comboBox_SelVisionPR.SelectedIndex].ToString();

            Action action = new Action(() =>
            {
                string camname = VisionMgr.GetInstance().GetCamName(strVisionPrName);
                double? Expouse = VisionMgr.GetInstance().GetExpourseTime(strVisionPrName);
                double? Gain = VisionMgr.GetInstance().GetGain(strVisionPrName);

                CameraMgr.GetInstance().SetCamExposure(camname, (double)Expouse);
                CameraMgr.GetInstance().SetCamGain(camname, (double)Gain);
                CameraMgr.GetInstance().GetCamera(camname).SetTriggerMode(CameraModeType.Software);

                CameraMgr.GetInstance().BindWindow(camname, visionControl1);
                // CameraMgr.GetInstance().GetCamera(camname).StartGrab();

                CameraMgr.GetInstance().GetCamera(camname).GarbBySoftTrigger();
                HObject Img = CameraMgr.GetInstance().GetImg(camname);

                CameraMgr.GetInstance().GetCamera(camname).ClearAllPr();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                // VisionSetpBase visionSetpBase = VisionMgr.GetInstance().GetItem(strVisionPrName);
                //  Action action2 = new Action(() => { visionSetpBase.Process_image(visionControl1.Img, visionControl1); });
                DoWhile doWhile = new DoWhile((time, dowhile, bmanual2, obj) =>
                {
                    if (Img != null && Img.IsInitialized())
                        return WaranResult.Run;
                    else if (stopwatch.ElapsedMilliseconds > 300)
                        return WaranResult.Failture;
                    else
                    {
                        Img = CameraMgr.GetInstance().GetImg(camname);
                        return WaranResult.CheckAgain;
                    }

                }, 3000
                );
                doWhile.doSomething(null, doWhile, true, null);
                if (Img != null && Img.IsInitialized())
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        VisionMgr.GetInstance().ProcessImage(strVisionPrName, Img, visionControl1);
                        if (Img != null && Img.IsInitialized())
                            Img.Dispose();
                    }));
                }


            }
            );

            action.BeginInvoke((ar) => { }, null);
        }


    }
}
