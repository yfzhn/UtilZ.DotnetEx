using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V1
{
    /// <summary>
    /// Metro等待样式框
    /// </summary>
    public partial class MetroShadeControl : UserControl, IPartAsynWaitWinform
    {
        /// <summary>
        /// 获取UI是否处于设计器模式
        /// </summary>
        public bool UIDesignMode
        {
            get { return this.DesignMode; }
        }

        /// <summary>
        /// 获取或设置提示标题
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示标题")]
        [Browsable(true)]
        [Description("获取或设置提示标题")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get
            {
                return this.GetCaption();
            }
            set
            {
                this.SetCaption(value);
            }
        }

        private void SetCaption(string caption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.SetCaption(caption);
                }));
            }
            else
            {
                labelControlCaption.Text = caption;
                int captionX = (int)((labelControlTitle.Parent.Width - labelControlCaption.Width) / 2);
                int captionY = (int)((labelControlTitle.Parent.Height - labelControlCaption.Height) / 2);
                labelControlCaption.Location = new Point(captionX, captionY);
            }
        }

        private string GetCaption()
        {
            string caption = null;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    caption = labelControlCaption.Text;
                }));
            }
            else
            {
                caption = labelControlCaption.Text;
            }

            return caption;
        }

        /// <summary>
        /// 获取或设置提示内容
        /// </summary>
        [Category("异步等待")]
        [DisplayName("提示内容")]
        [Browsable(true)]
        [Description("获取或设置提示内容")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Message
        {
            get
            {
                return this.GetHint();
            }
            set
            {
                this.SetHint(value);
            }
        }

        private void SetHint(string hint)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.SetHint(hint);
                }));
            }
            else
            {
                labelControlTitle.Text = hint;
                int titleX = (int)((labelControlTitle.Parent.Width - labelControlTitle.Width) / 2);
                int titleY = (int)((labelControlTitle.Parent.Height - labelControlTitle.Height) / 2);
                labelControlTitle.Location = new Point(titleX, titleY);
            }
        }

        private string GetHint()
        {
            string hint = null;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    hint = labelControlTitle.Text;
                }));
            }
            else
            {
                hint = labelControlTitle.Text;
            }

            return hint;
        }

        /// <summary>
        /// 是否已经取消
        /// </summary>
        private bool _isCanceled = false;

        /// <summary>
        /// 获取是否已经取消
        /// </summary>
        [Browsable(false)]
        public bool Canceled
        {
            get { return this._isCanceled; }
        }

        /// <summary>
        /// 获取或设置是否显示取消按钮
        /// </summary>
        [Category("异步等待")]
        [DisplayName("是否显示取消按钮")]
        [Browsable(true)]
        [Description("获取或设置是否显示取消按钮")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowCancel
        {
            get { return btnCancell.Visible; }
            set { btnCancell.Visible = value; }
        }

        /// <summary>
        /// 获取或设置动画背景色
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("获取或设置动画背景色")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object ShadeBackground
        {
            get { return this.BackColor; }
            set
            {
                var color = (Color)value;
                if (this.BackColor == color)
                {
                    return;
                }

                this.BackColor = color;
                //this.metroRotaionIndicator.AnimalBackground = System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
            }

            //get
            //{
            //    var animalBackground = this.metroRotaionIndicator.AnimalBackground;
            //    return Color.FromArgb(animalBackground.A, animalBackground.R, animalBackground.G, animalBackground.B);
            //}
            //set
            //{
            //    if (this.AsynWaitBackground.Equals(value))
            //    {
            //        return;
            //    }

            //    this.metroRotaionIndicator.AnimalBackground = System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
            //}
        }

        /// <summary>
        /// 取消通知事件
        /// </summary>
        public event EventHandler CanceledNotify;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            if (this._isCanceled)
            {
                return;
            }

            lock (this._monitor)
            {
                if (this._isCanceled)
                {
                    return;
                }

                this._isCanceled = true;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    btnCancell.Text = "正在取消";
                    btnCancell.Enabled = false;
                }));
            }
            else
            {
                btnCancell.Text = "正在取消";
                btnCancell.Enabled = false;
            }

            var handler = this.CanceledNotify;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void StartAnimation()
        {
            //metroRotaionIndicator.StartAnimal();
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void StopAnimation()
        {
            //metroRotaionIndicator.StopAnimal();
        }

        /// <summary>
        /// 设置信息(保留接口)
        /// </summary>
        /// <param name="para">参数</param>
        public void SetInfo(object para)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重置异步等待框
        /// </summary>
        public void Reset()
        {
            this._isCanceled = false;
            btnCancell.Text = "取消";
            btnCancell.Enabled = true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MetroShadeControl()
        {
            InitializeComponent();
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            this.Cancel();
        }
    }
}
