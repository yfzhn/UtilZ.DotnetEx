using System;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.LMQ;

namespace DotnetWinFormApp
{
    public partial class FTestLMQCenter : Form
    {
        public FTestLMQCenter()
        {
            InitializeComponent();
        }

        private readonly string _topic = "xzxx";
        private AsynQueue<string> _pushQueue;

        private void FTestLMQCenter_Load(object sender, EventArgs e)
        {
            LMQConfigManager.AddLMQConfig(new LMQConfig(_topic) { ParallelPublish = true, SyncPublish = true });

            LMQCenter.Subscibe(new SubscibeItem(_topic, this.Rev));
            LMQCenter.Subscibe(new SubscibeItem(_topic, this.Rev2));

            this._pushQueue = new AsynQueue<string>(this.Push);
            this._pushQueue.Start();
        }

        private void Push(string message)
        {
            LMQCenter.Publish(_topic, textBox1.Text);
        }

        private void Rev(SubscibeItem sub, object obj)
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText("111_" + obj.ToString());
                richTextBox1.AppendText(Environment.NewLine);
            }));
        }

        private void Rev2(SubscibeItem sub, object obj)
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText("222_" + obj.ToString());
                richTextBox1.AppendText(Environment.NewLine);
            }));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this._pushQueue.Enqueue(textBox1.Text);            
        }
    }
}
