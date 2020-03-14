using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.UITypeEditors
{
    /// <summary>
    /// 属性编辑窗口目录编辑
    /// </summary>
    [Serializable]
    public class PropertyGridDirectoryEditor : UITypeEditor
    {
        /// <summary>
        /// 获取由 EditValue 方法使用的编辑器样式
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <returns>UITypeEditorEditStyle  值，指示 EditValue 方法使用的编辑器样式。 如果 UITypeEditor 不支持该方法，则 GetEditStyle 将返回 None</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// 使用 GetEditStyle 方法所指示的编辑器样式编辑指定对象的值
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <param name="provider">IServiceProvider ，此编辑器可用其来获取服务</param>
        /// <param name="value">要编辑的对象</param>
        /// <returns>新的对象值。 如果对象的值尚未更改，则它返回的对象应与传递给它的对象相同</returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // 可以打开任何特定的对话框  
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (value != null)
                {
                    fbd.SelectedPath = value.ToString();
                }

                if (string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    if (context.Instance.GetType().GetInterface(typeof(IPropertyGridDirectory).FullName) != null)
                    {
                        IPropertyGridDirectory ipropertyGridFile = (IPropertyGridDirectory)context.Instance;
                        fbd.SelectedPath = ipropertyGridFile.GetInitialSelectedPath(context.PropertyDescriptor.Name);
                    }
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return fbd.SelectedPath;
                }
            }

            return value;
        }
    }
}
