using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Base
{
    /// <summary>
    /// PropertyGrid集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [EditorAttribute(typeof(CollectionEditorEx), typeof(System.Drawing.Design.UITypeEditor))]

    [TypeConverter(typeof(TypeConverters.PropertyGridCollectionConverter))]
    public class PropertyGridCollection<T> : CollectionBase, ICollection<T>, ICustomTypeDescriptor
    {
        /// <summary>
        /// 内部集合列表
        /// </summary>
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyGridCollection()
        {

        }

        /// <summary>
		/// Returns an employee object at index position.
		/// </summary>
		public T this[int index]
        {
            get
            {
                return (T)this.List[index];
            }
        }

        /// <summary>
        /// 集合是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">要添加的项</param>
        public void Add(T item)
        {
            this.List.Add(item);
            this._items.Add(item);
        }

        /// <summary>
        /// 是否包含指定项
        /// </summary>
        /// <param name="item">指定项</param>
        /// <returns>返回包含结果</returns>
        public bool Contains(T item)
        {
            return this.List.Contains(item);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始，将整个 System.Collections.Generic.List`1 复制到兼容的一维数组
        /// </summary>
        /// <param name="array">一维 System.Array，它是从 System.Collections.Generic.List`1 复制的元素的目标。 System.Array必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 移除与指定的谓词所定义的条件相匹配的所有元素
        /// </summary>
        /// <param name="item">System.Predicate`1 委托，用于定义要移除的元素应满足的条件</param>
        /// <returns>从 System.Collections.Generic.List`1 中移除的元素数</returns>
        public bool Remove(T item)
        {
            this.List.Remove(item);
            return this._items.Remove(item);
        }

        /// <summary>
        /// 返回循环访问 System.Collections.Generic.List`1 的枚举数
        /// </summary>
        /// <returns>用于 System.Collections.Generic.List`1.Enumerator 的 System.Collections.Generic.List`1</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        #region ICustomTypeDescriptor接口
        /// <summary>
        /// 返回此组件实例的自定义属性的集合
        /// </summary>
        /// <returns>包含此对象的属性的 System.ComponentModel.AttributeCollection。</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// 返回此组件实例的类名称
        /// </summary>
        /// <returns>对象的类名称，如果该类没有名称，则为 null</returns>
        public string GetClassName()
        {
            //return TypeDescriptor.GetClassName(this, true);
            return typeof(T).FullName;
        }

        /// <summary>
        /// 返回某个组件的此实例的名称
        /// </summary>
        /// <returns>对象的名称，如果该对象不具有名称，则为 null</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// 返回此组件实例的类型转换器
        /// </summary>
        /// <returns>作为此对象转换器的 System.ComponentModel.TypeConverter，或为 null（如果此对象不存在任何 System.ComponentModel.TypeConverter）。</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// 返回某个组件的此实例的默认事件
        /// </summary>
        /// <returns>表示此对象的默认事件的 System.ComponentModel.EventDescriptor；如果该对象没有事件，则为 null。</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// 返回此组件实例的默认属性
        /// </summary>
        /// <returns>表示此对象的默认属性的 System.ComponentModel.PropertyDescriptor；如果该对象没有属性，则为 null。</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// 返回指定类型的此实例的一个组件的编辑器
        /// </summary>
        /// <param name="editorBaseType">一个 System.Type ，它表示此对象的编辑器</param>
        /// <returns>System.Object 是此对象的编辑器的指定类型或 null 如果找不到编辑器中。</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// 返回此组件实例的事件
        /// </summary>
        /// <returns>一个 System.ComponentModel.EventDescriptorCollection，表示此组件实例的事件。</returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// 使用指定的属性数组作为筛选器，返回此组件实例的事件。
        /// </summary>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组</param>
        /// <returns>表示此组件实例的已筛选事件的 System.ComponentModel.EventDescriptorCollection。</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// 返回此组件实例的属性
        /// </summary>
        /// <returns>表示此组件实例的属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            var pds = new PropertyDescriptorCollection(null);
            for (int i = 0; i < this.Count; i++)
            {
                pds.Add(new PropertyGridCollectionPropertyDescriptor<T>((T)this.List[i], i));
            }

            return pds;
        }

        /// <summary>
        /// 使用特性数组作为筛选器，返回此组件实例的属性
        /// </summary>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组</param>
        /// <returns>一个 System.ComponentModel.PropertyDescriptorCollection，它表示此组件实例的已筛选属性</returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        /// <summary>
        /// 返回一个对象，该对象包含指定的属性描述符所描述的属性
        /// </summary>
        /// <param name="pd">一个 System.ComponentModel.PropertyDescriptor，表示要查找其所有者的属性</param>
        /// <returns>一个 System.Object，表示指定属性的所有者</returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }

    internal class PropertyGridCollectionPropertyDescriptor<T> : PropertyDescriptor
    {
        private readonly T _item;
        public PropertyGridCollectionPropertyDescriptor(T item, int index) :
            base(index.ToString(), null)
        {
            this._item = item;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override Type ComponentType
        {
            get
            {
                return typeof(PropertyGridCollection<T>);
                //return this._collection.GetType();
            }
        }

        public override string DisplayName
        {
            get
            {
                return this._item.ToString();
            }
        }

        public override string Description
        {
            get
            {
                //Employee emp = this.collection[index];
                //StringBuilder sb = new StringBuilder();
                //sb.Append(emp.FirstName);
                //sb.Append(",");
                //sb.Append(emp.Age);
                //sb.Append(" years old, working for ");
                //return sb.ToString();
                //return this._collection.ElementAt(_index).ToString();

                return "Employy.Description";
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(T); }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            return this._item;
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {

        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

    internal class CollectionEditorEx : CollectionEditor
    {
        //public static event EventHandler OnValueChange;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">Type</param>
        public CollectionEditorEx(Type type) : base(type)
        {

        }

        //private CollectionForm _frm;

        /// <summary>
        /// 集合编辑列表控件
        /// </summary>
        private ListBox _listbox;

        /// <summary>
        /// 创建一个新的窗体来显示和编辑当前集合
        /// </summary>
        /// <returns>一个 System.ComponentModel.Design.CollectionEditor.CollectionForm 作为用户界面提供用于编辑集合</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm frm = base.CreateCollectionForm();
            FieldInfo fileinfo = frm.GetType().GetField("propertyBrowser", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fileinfo != null)
            {
                (fileinfo.GetValue(frm) as System.Windows.Forms.PropertyGrid).HelpVisible = true;
            }

            frm.Width = 700;
            frm.Height = 600;

            FieldInfo listboxFieldInfo = frm.GetType().GetField("listbox", BindingFlags.NonPublic | BindingFlags.Instance);
            this._listbox = listboxFieldInfo.GetValue(frm) as ListBox;

            //注册按钮事件
            this.ReflectButtonEvent(frm);

            //_frm = frm;
            return frm;
        }

        #region 反射事件
        private void ReflectButtonEvent(CollectionForm frm)
        {
            FieldInfo okButton = frm.GetType().GetField("okButton", BindingFlags.NonPublic | BindingFlags.Instance);
            if (okButton != null)
            {
                (okButton.GetValue(frm) as Button).Click += Ok_Click; ;
            }

            //FieldInfo removeButton = frm.GetType().GetField("removeButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (removeButton != null)
            //{
            //    (removeButton.GetValue(frm) as Button).Click += ExtendCollectionEditor_RemoveClick;
            //}

            //FieldInfo cancelButton = frm.GetType().GetField("cancelButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (cancelButton != null)
            //{
            //    (cancelButton.GetValue(frm) as Button).Click += ExtendCollectionEditor_ValueChange;
            //}

            //FieldInfo okButton = frm.GetType().GetField("okButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (okButton != null)
            //{

            //}
            //FieldInfo upButton = frm.GetType().GetField("upButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (upButton != null)
            //{

            //}
            //FieldInfo downButton = frm.GetType().GetField("downButton", BindingFlags.NonPublic | BindingFlags.Instance);
            //if (downButton != null)
            //{

            //}
        }

        /// <summary>
        /// 集合编辑器点击OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, EventArgs e)
        {
            if (this.Context.PropertyDescriptor.ComponentType.GetInterface(typeof(IPropertyGridCollection).FullName) == null)
            {
                return;
            }

            ((IPropertyGridCollection)this.Context.Instance).CollectionEditCompletedNotify(this.Context.PropertyDescriptor.PropertyType.Name);
        }
        #endregion

        /// <summary>
        /// 获取此集合编辑器可以包含的数据类型
        /// </summary>
        /// <returns>此集合可以包含的数据类型的数组</returns>
        protected override Type[] CreateNewItemTypes()
        {
            var context = this.Context;
            if (context == null || context.Instance == null)
            {
                return base.CreateNewItemTypes();
            }

            if (context.PropertyDescriptor.ComponentType.GetInterface(typeof(IPropertyGridCollection).FullName) != null)
            {
                return ((IPropertyGridCollection)context.Instance).GetCreateNewItemTypes();
            }

            return base.CreateNewItemTypes();
        }

        /// <summary>
        /// 指示是否可以删除原始集合的成员
        /// </summary>
        /// <param name="value">要移除的值</param>
        /// <returns>true 如果允许此值从集合中删除;否则为 false。 默认实现始终返回 true</returns>
        protected override bool CanRemoveInstance(object value)
        {
            var context = this.Context;
            if (context == null || context.Instance == null)
            {
                return base.CanRemoveInstance(value);
            }

            Type type = context.Instance.GetType();
            if (type.GetInterface(typeof(IPropertyGridCollection).FullName) != null)
            {
                return ((IPropertyGridCollection)context.Instance).GetCanRemoveInstance(context.PropertyDescriptor.PropertyType.Name, value);
            }

            return base.CanRemoveInstance(value);
        }

        /// <summary>
        /// 返回包含给定的对象的列表
        /// </summary>
        /// <param name="instance">System.Collections.ArrayList 作为对象返回</param>
        /// <returns>System.Collections.ArrayList 其中包含要创建的各个对象</returns>
        protected override IList GetObjectsFromInstance(object instance)
        {
            if (this.Context.PropertyDescriptor.ComponentType.GetInterface(typeof(IPropertyGridCollection).FullName) == null)
            {
                //未实现对应接口,不限制
                return base.GetObjectsFromInstance(instance);
            }

            int maxCount = ((IPropertyGridCollection)this.Context.Instance).GetObjectsInstanceMaxCount(this.Context.PropertyDescriptor.PropertyType.Name);
            if (maxCount < 1)
            {
                //小于1不限制
                return base.GetObjectsFromInstance(instance);
            }

            if (this._listbox.Items.Count < maxCount)
            {
                return base.GetObjectsFromInstance(instance);
            }

            return null;
        }

        /* 限制集合数量-备份
        protected int _instanceCount = 0;
        protected override IList GetObjectsFromInstance(object instance)
        {
            Type type = this.Context.Instance.GetType();
            var collection = this._frm.EditValue as ICollection;
            if (collection == null)
            {
                return base.GetObjectsFromInstance(instance);
            }

            if (type.GetInterface(typeof(IPropertyGridCollection).FullName) == null)
            {
                return base.GetObjectsFromInstance(instance);
            }

            int maxCount = ((IPropertyGridCollection)this.Context.Instance).GetObjectsInstanceMaxCount(this.Context.PropertyDescriptor.PropertyType.Name);
            if (maxCount < 1)
            {
                //小于1不限制
                return base.GetObjectsFromInstance(instance);
            }

            if ((collection.Count < maxCount || this._instanceCount < maxCount))
            {
                this._instanceCount++;
                return base.GetObjectsFromInstance(instance);
            }

            return null;
        }

        protected override object[] GetItems(object editValue)
        {
            this._instanceCount = ((dynamic)editValue).Count;
            return base.GetItems(editValue);
        }

        protected override void DestroyInstance(object instance)
        {
            //注:删除弹出集合编辑西式前已存在的项不会触发此方法
            this._instanceCount--;
            base.DestroyInstance(instance);
        }
*/
    }
}
