using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// 扩展TreeView类
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.TreeView))]//定义工具栏中的图标
    public class TreeViewZ : System.Windows.Forms.TreeView
    {
        #region TreeView 展开深度扩展方法
        /// <summary>
        /// 递归展开结点
        /// </summary>
        /// <param name="treeNodes">TreeView.TreeNodeCollection</param>        
        /// <param name="currentDepth">当前已展开的深度</param>
        /// <param name="depth">要展开的深度</param>
        private static void ExpandDepth(TreeNodeCollection treeNodes, int currentDepth, int depth)
        {
            if (currentDepth > depth)
            {
                return;
            }

            foreach (TreeNode node in treeNodes)
            {
                node.Expand();
                TreeViewZ.ExpandDepth(node.Nodes, ++currentDepth, depth);
                currentDepth--;
            }
        }

        /// <summary>
        /// 展开树深度
        /// </summary>
        /// <param name="tree">当前的树</param>
        /// <param name="depth">要展开的深度</param>
        public static void ExpandDepth(TreeView tree, int depth)
        {
            TreeViewZ.ExpandDepth(tree.Nodes, 0, depth - 1);
        }
        #endregion

        /// <summary>
        /// 设置选中的结点的背景颜色
        /// </summary>
        /// <param name="treeView">当前的树</param>
        /// <param name="lastSelectedNode">上一次选中的结点</param>
        /// <param name="backColor">选中节点背景色</param>
        /// <param name="foreColor">选中节点前景色</param>
        public static void SetSelectedTreeNodeBackcolor(TreeView treeView, ref TreeNode lastSelectedNode, System.Drawing.Color backColor, System.Drawing.Color foreColor)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException(nameof(treeView), string.Empty);
            }

            if (treeView.SelectedNode == null)
            {
                throw new ArgumentNullException(nameof(treeView.SelectedNode), string.Empty);
            }

            if (treeView.SelectedNode == lastSelectedNode)
            {
                return;
            }

            if (lastSelectedNode != null)
            {
                lastSelectedNode.BackColor = treeView.BackColor;
                lastSelectedNode.ForeColor = treeView.ForeColor;
            }

            treeView.SelectedNode.BackColor = backColor;
            treeView.SelectedNode.ForeColor = foreColor;
            lastSelectedNode = treeView.SelectedNode;
        }

        #region 根据TreeView选中结点的全路径设置选中结点方法
        /// <summary>
        /// 根据TreeView选中结点的全路径设置选中结点
        /// </summary>
        /// <param name="tree">当前的树</param>
        /// <param name="fullPath">全路径[@"Earth/Assi/Yindu/jiazi2"]</param>
        /// <returns>设置成功返回true,失败返回false</returns>
        public static bool SetSelectedNodeByFullPath(TreeView tree, string fullPath)
        {
            string[] paths = fullPath.Split('/');
            TreeNode selectedNode = null;

            if (paths.Length == 0)
            {
                throw new Exception(string.Format("全路径{0}不是合法的TreeView选中结点全路径", fullPath));
            }

            for (int i = 0; i < tree.Nodes.Count; i++)
            {
                if (!tree.Nodes[i].Text.Equals(paths[0]))
                {
                    continue;
                }

                //此处第二个参数传入固定值1，是因为这儿的for循环对应的路径索引为0,所以下一层的值为从1开始                
                selectedNode = TreeViewZ.FindSelectedNode(paths, 1, tree.Nodes[i]);
                if (selectedNode != null)
                {
                    tree.SelectedNode = selectedNode;
                    return true;
                }
                else
                {
                    continue;
                }
            }

            return false;
        }

        /// <summary>
        /// 递归查找选中项结点
        /// </summary>
        /// <param name="paths">路径集合</param>
        /// <param name="index">当前路径索引</param>
        /// <param name="parentNode">父级节点</param>
        /// <returns>找到的选中节点</returns>
        private static TreeNode FindSelectedNode(string[] paths, int index, TreeNode parentNode)
        {
            for (int i = 0; i < parentNode.Nodes.Count; i++)
            {
                if (parentNode.Nodes[i].Text == paths[index])
                {
                    index++;
                    if (parentNode.Nodes[i].Nodes.Count > 0 && paths.Length > index)
                    {
                        return TreeViewZ.FindSelectedNode(paths, index, parentNode.Nodes[i]);
                    }
                    else if (parentNode.Nodes[i].Nodes.Count == 0 && paths.Length == index)
                    {
                        return parentNode.Nodes[i];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }
        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TreeViewZ()
            : base()
        {
            this.EnabledDoubleClick = false;
            this.AutoExpandedCollapse = true;
            this.AutoChecked = true;
        }

        /// <summary>
        /// 是否响应鼠标双击事件[true:响应;false:不响应;默认false]
        /// </summary>
        [BrowsableAttribute(true)]
        [DescriptionAttribute("是否响应鼠标双击事件")]
        [DefaultValueAttribute(false)]
        [CategoryAttribute("扩展")]
        public bool EnabledDoubleClick { get; set; }

        /// <summary>
        /// 是否双击自动展开或折叠树节点[true:自动展开或折叠;false:不自动展开或折叠;默认true]
        /// </summary>
        [BrowsableAttribute(true)]
        [DescriptionAttribute("是否双击自动展开或折叠树节点[true:自动展开或折叠;false:不自动展开或折叠;默认true]")]
        [DefaultValueAttribute(true)]
        [CategoryAttribute("扩展")]
        public bool AutoExpandedCollapse { get; set; }

        /// <summary>
        /// 显示复选框时,当一外复选框勾选状态改变后,是否自动勾选其低级及子节点[true:自动勾选;false:不自动勾选;默认true]
        /// </summary>
        [BrowsableAttribute(true)]
        [DescriptionAttribute("显示复选框时,当一外复选框勾选状态改变后,是否自动勾选其低级及子节点[true:自动勾选;false:不自动勾选;默认true]")]
        [DefaultValueAttribute(true)]
        [CategoryAttribute("扩展")]
        public bool AutoChecked { get; set; }

        /// <summary>
        /// 重写WndProc
        /// </summary>
        /// <param name="m">消息</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203 && !this.EnabledDoubleClick)
            {
                //屏蔽WM_LBUTTONDBLCLK=0x0203消息
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 重写OnNodeMouseDoubleClick
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseDoubleClick(e);

            if (this.AutoExpandedCollapse)
            {
                if (e.Node.IsExpanded)
                {
                    e.Node.Collapse();
                }
                else
                {
                    e.Node.Expand();
                }
            }
        }

        /// <summary>
        /// 重写OnAfterCheck
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            if (e.Action != TreeViewAction.Unknown && this.AutoChecked)
            {
                this.SetChildNodes(e.Node, e.Node.Checked);
                this.SetParentNodes(e.Node, e.Node.Checked);
            }
        }

        /// <summary>
        /// 设置父节点选中
        /// </summary>
        /// <param name="curNode">当前节点</param>
        /// <param name="isChecked">选中状态</param>
        private void SetParentNodes(TreeNode curNode, bool isChecked)
        {
            if (curNode.Parent != null)
            {
                if (isChecked)
                {
                    curNode.Parent.Checked = isChecked;
                    SetParentNodes(curNode.Parent, isChecked);
                }
                else
                {
                    bool ParFlag = false;
                    foreach (TreeNode tmp in curNode.Parent.Nodes)
                    {
                        if (tmp.Checked)
                        {
                            ParFlag = true;
                            break;
                        }
                    }
                    curNode.Parent.Checked = ParFlag;
                    SetParentNodes(curNode.Parent, ParFlag);
                }
            }
        }

        /// <summary>
        /// 设置子节点选中状态
        /// </summary>
        /// <param name="curNode">当前节点</param>
        /// <param name="isChecked">选中状态</param>
        private void SetChildNodes(TreeNode curNode, bool isChecked)
        {
            if (curNode.Nodes != null)
            {
                foreach (TreeNode tmpNode in curNode.Nodes)
                {
                    tmpNode.Checked = isChecked;
                    SetChildNodes(tmpNode, isChecked);
                }
            }
        }
    }
}
