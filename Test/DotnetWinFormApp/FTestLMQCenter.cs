using System;
using System.Windows.Forms;
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
        private void FTestLMQCenter_Load(object sender, EventArgs e)
        {
            var sub = new SubscibeItem(_topic, this.Rev);
            LMQCenter.Subscibe(sub);
        }

        private void Rev(SubscibeItem sub, object obj)
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(obj.ToString());
                richTextBox1.AppendText(Environment.NewLine);
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LMQCenter.Publish(_topic, textBox1.Text);
        }
    }
}
